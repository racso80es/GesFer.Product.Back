//! Tool postman-mcp-validation: valida endpoints con colección Postman (Newman).
//! Cumple tools-contract: salida JSON, feedback por fases.

use std::env;
use std::path::PathBuf;
use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_tools::{FeedbackEntry, ToolResult, to_contract_json};
use serde::Deserialize;

const TOOL_ID: &str = "postman-mcp-validation";

#[derive(Parser)]
#[command(name = "postman_mcp_validation")]
struct Args {
    #[arg(long)]
    collection_path: Option<String>,
    #[arg(long)]
    base_url: Option<String>,
    #[arg(long)]
    internal_secret: Option<String>,
    #[arg(long)]
    environment_path: Option<String>,
    #[arg(long)]
    input_path: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
struct JsonInput {
    #[serde(rename = "collectionPath")]
    collection_path: Option<String>,
    #[serde(rename = "baseUrl")]
    base_url: Option<String>,
    #[serde(rename = "internalSecret")]
    internal_secret: Option<String>,
    #[serde(rename = "environmentPath")]
    environment_path: Option<String>,
}

#[derive(Debug, Deserialize)]
struct Config {
    #[serde(rename = "collectionPath")]
    collection_path: Option<String>,
    #[serde(rename = "baseUrl")]
    base_url: Option<String>,
    #[serde(rename = "internalSecret")]
    internal_secret: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();

    let args = Args::parse();

    let (collection_path, base_url, internal_secret, environment_path) =
        match load_input(&args, &mut feedback) {
            Ok(p) => p,
            Err((msg, exit_code)) => {
                feedback.push(FeedbackEntry::error("input", &msg, None));
                emit_result(false, exit_code, &msg, feedback, None, start, &args);
                std::process::exit(exit_code);
            }
        };

    feedback.push(FeedbackEntry::info(
        "init",
        &format!("Iniciando postman-mcp-validation; colección: {}", collection_path),
    ));

    let repo_root = env::current_dir().unwrap_or_else(|_| PathBuf::from("."));
    let collection_full = repo_root.join(&collection_path);

    if !collection_full.exists() {
        feedback.push(FeedbackEntry::error(
            "init",
            &format!("Colección no encontrada: {}", collection_full.display()),
            None,
        ));
        let data = serde_json::json!({ "run_summary": { "executed": 0, "passed": 0, "failed": 1 } });
        emit_result(false, 1, "Colección no encontrada", feedback, Some(data), start, &args);
        std::process::exit(1);
    }

    let use_npx = Command::new("newman").arg("--version").output().is_err()
        && Command::new("npx").arg("newman").arg("--version").output().is_ok();

    if !use_npx && Command::new("newman").arg("--version").output().is_err() {
        feedback.push(FeedbackEntry::error(
            "init",
            "Newman no encontrado. Instale con: npm install -g newman (o use npx).",
            None,
        ));
        let data = serde_json::json!({ "run_summary": { "executed": 0, "passed": 0, "failed": 1 } });
        emit_result(false, 1, "Newman no disponible", feedback, Some(data), start, &args);
        std::process::exit(1);
    }

    let temp_report = env::temp_dir().join(format!("newman-report-{}.json", std::process::id()));

    feedback.push(FeedbackEntry::info("newman", "Ejecutando Newman..."));
    let newman_exit = if use_npx {
        let mut cmd = Command::new("npx");
        cmd.arg("newman")
            .arg("run")
            .arg(collection_full.to_str().unwrap())
            .args(["--global-var", &format!("baseUrl={}", base_url)])
            .args(["--global-var", &format!("internalSecret={}", internal_secret)])
            .args(["--reporters", "json"])
            .args(["--reporter-json-export", temp_report.to_str().unwrap()])
            .arg("--disable-unicode")
            .current_dir(&repo_root);
        if let Some(ref ep) = environment_path {
            if !ep.is_empty() {
                let env_full = repo_root.join(ep);
                if env_full.exists() {
                    cmd.args(["--environment", env_full.to_str().unwrap()]);
                }
            }
        }
        cmd.output()
    } else {
        Command::new("newman")
            .arg("run")
            .arg(collection_full.to_str().unwrap())
            .args(["--global-var", &format!("baseUrl={}", base_url)])
            .args(["--global-var", &format!("internalSecret={}", internal_secret)])
            .args(["--reporters", "json"])
            .args(["--reporter-json-export", temp_report.to_str().unwrap()])
            .arg("--disable-unicode")
            .current_dir(&repo_root)
            .output()
    };

    let newman_exit_code = match &newman_exit {
        Ok(o) => o.status.code().unwrap_or(1),
        Err(e) => {
            feedback.push(FeedbackEntry::error(
                "newman",
                &format!("Error al ejecutar Newman: {}", e),
                None,
            ));
            let data = serde_json::json!({ "run_summary": { "executed": 0, "passed": 0, "failed": 1 } });
            emit_result(false, 1, "Error ejecutando Newman", feedback, Some(data), start, &args);
            std::process::exit(1);
        }
    };

    let (executed, passed, failed) = if temp_report.exists() {
        match std::fs::read_to_string(&temp_report) {
            Ok(content) => {
                if let Ok(report) = serde_json::from_str::<serde_json::Value>(&content) {
                    let run = report.get("run").and_then(|r| r.get("stats"));
                    let (e, p, f) = if let Some(stats) = run {
                        let requests = stats.get("requests").and_then(|r| r.as_u64()).unwrap_or(0);
                        let assertions = stats.get("assertions");
                        let total = assertions.and_then(|a| a.get("total")).and_then(|t| t.as_u64()).unwrap_or(0);
                        let failed_a = assertions.and_then(|a| a.get("failed")).and_then(|f| f.as_u64()).unwrap_or(0);
                        (requests, total.saturating_sub(failed_a), failed_a)
                    } else {
                        (0, 0, 0)
                    };
                    let _ = std::fs::remove_file(&temp_report);
                    (e, p, f)
                } else {
                    (1, if newman_exit_code == 0 { 1 } else { 0 }, if newman_exit_code == 0 { 0 } else { 1 })
                }
            }
            _ => (1, if newman_exit_code == 0 { 1 } else { 0 }, if newman_exit_code == 0 { 0 } else { 1 }),
        }
    } else {
        (
            1,
            if newman_exit_code == 0 { 1 } else { 0 },
            if newman_exit_code == 0 { 0 } else { 1 },
        )
    };

    let run_summary = serde_json::json!({
        "executed": executed,
        "passed": passed,
        "failed": failed
    });
    let success = newman_exit_code == 0 && failed == 0;
    let exit_code = if success { 0 } else { 1 };

    if success {
        feedback.push(FeedbackEntry::info(
            "done",
            &format!("Validación completada: {} passed, {} failed (executed: {}).", passed, failed, executed),
        ));
    } else {
        feedback.push(FeedbackEntry::error(
            "error",
            &format!("Validación con fallos: exitCode={}, failed={}.", newman_exit_code, failed),
            None,
        ));
    }

    let data = serde_json::json!({ "run_summary": run_summary });
    let message = if success {
        "Validación de endpoints correcta."
    } else {
        "Validación con fallos o API no disponible."
    };
    emit_result(success, exit_code, message, feedback, Some(data), start, &args);
    std::process::exit(exit_code);
}

fn load_input(
    args: &Args,
    feedback: &mut Vec<FeedbackEntry>,
) -> Result<(String, String, String, Option<String>), (String, i32)> {
    let config_path = env::current_dir()
        .unwrap_or_else(|_| PathBuf::from("."))
        .join("scripts")
        .join("tools")
        .join("postman-mcp-validation")
        .join("postman-mcp-validation-config.json");

    let mut collection_path = args.collection_path.clone().unwrap_or_default();
    let mut base_url = args.base_url.clone().unwrap_or_else(|| "http://localhost:5010".to_string());
    let mut internal_secret = args.internal_secret.clone().unwrap_or_default();
    let mut environment_path = args.environment_path.clone();

    if let Some(ref path) = args.input_path {
        feedback.push(FeedbackEntry::info("input", &format!("Cargando JSON desde {}", path)));
        let content = std::fs::read_to_string(path)
            .map_err(|e| (format!("No se pudo leer --input-path: {}", e), 1))?;
        let input: JsonInput = serde_json::from_str(&content)
            .map_err(|e| (format!("JSON inválido en --input-path: {}", e), 1))?;
        if let Some(cp) = input.collection_path {
            collection_path = cp;
        }
        if let Some(bu) = input.base_url {
            base_url = bu;
        }
        if let Some(is) = input.internal_secret {
            internal_secret = is;
        }
        if let Some(ep) = input.environment_path {
            environment_path = Some(ep);
        }
    }

    if collection_path.is_empty() && config_path.exists() {
        if let Ok(content) = std::fs::read_to_string(&config_path) {
            if let Ok(config) = serde_json::from_str::<Config>(&content) {
                collection_path = config.collection_path.unwrap_or_else(|| {
                    "docs/postman/GesFer.Admin.Back.API.postman_collection.json".to_string()
                });
                if base_url == "http://localhost:5010" {
                    base_url = config.base_url.unwrap_or(base_url);
                }
                if internal_secret.is_empty() {
                    internal_secret = config.internal_secret.unwrap_or_default();
                }
            }
        }
    }
    if internal_secret.is_empty() {
        if let Ok(env_secret) = env::var("POSTMAN_INTERNAL_SECRET") {
            internal_secret = env_secret;
        }
    }
    if collection_path.is_empty() {
        collection_path = "docs/postman/GesFer.Admin.Back.API.postman_collection.json".to_string();
    }

    Ok((collection_path, base_url, internal_secret, environment_path))
}

fn emit_result(
    success: bool,
    exit_code: i32,
    message: &str,
    feedback: Vec<FeedbackEntry>,
    data: Option<serde_json::Value>,
    start: Instant,
    args: &Args,
) {
    let duration_ms = start.elapsed().as_millis() as u64;
    let result = if success {
        ToolResult::ok(TOOL_ID, message, feedback, data, duration_ms)
    } else {
        ToolResult::err(TOOL_ID, message, feedback, exit_code, data, duration_ms)
    };
    let json = to_contract_json(&result).expect("serialize");
    if let Some(ref path) = args.output_path {
        if let Some(parent) = std::path::Path::new(path).parent() {
            let _ = std::fs::create_dir_all(parent);
        }
        let _ = std::fs::write(path, &json);
    }
    if args.output_json {
        println!("{}", json);
    }
}
