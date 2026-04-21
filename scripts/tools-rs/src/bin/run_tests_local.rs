//! Tool run-tests-local: ejecuta tests (unit, integration) en condiciones locales.
//! Cumple tools-contract: salida JSON, feedback por fases.

use std::env;
use std::path::PathBuf;
use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_tools::{FeedbackEntry, ToolResult, to_contract_json};
use serde::Deserialize;

const TOOL_ID: &str = "run-tests-local";

#[derive(Parser)]
#[command(name = "run_tests_local")]
struct Args {
    #[arg(long)]
    skip_prepare: bool,
    #[arg(long)]
    skip_seeds: bool,
    #[arg(long, default_value = "all")]
    test_scope: String,
    #[arg(long)]
    only_tests: bool,
    #[arg(long, default_value = "http://localhost:5010")]
    e2e_base_url: String,
    #[arg(long)]
    input_path: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
struct JsonInput {
    #[serde(rename = "skipPrepare")]
    skip_prepare: Option<bool>,
    #[serde(rename = "skipSeeds")]
    skip_seeds: Option<bool>,
    #[serde(rename = "testScope")]
    test_scope: Option<String>,
    #[serde(rename = "onlyTests")]
    only_tests: Option<bool>,
    #[serde(rename = "e2eBaseUrl")]
    e2e_base_url: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    feedback.push(FeedbackEntry::info("init", &format!("Iniciando {} (TestScope={})", TOOL_ID, "all")));

    let args = Args::parse();

    let (skip_prepare, skip_seeds, test_scope, only_tests, e2e_base_url) =
        match load_input(&args, &mut feedback) {
            Ok(p) => p,
            Err((msg, exit_code)) => {
                feedback.push(FeedbackEntry::error("input", &msg, None));
                emit_result(false, exit_code, &msg, feedback, None, start, &args);
                std::process::exit(exit_code);
            }
        };

    let valid_scopes = ["unit", "integration", "e2e", "all"];
    if !valid_scopes.contains(&test_scope.as_str()) {
        let msg = format!(
            "test_scope debe ser uno de: {}. Recibido: {}",
            valid_scopes.join(", "),
            test_scope
        );
        feedback.push(FeedbackEntry::error("input", &msg, None));
        emit_result(false, 1, &msg, feedback, None, start, &args);
        std::process::exit(1);
    }

    feedback.push(FeedbackEntry::info(
        "init",
        &format!("Iniciando {} (TestScope={})", TOOL_ID, test_scope),
    ));

    let repo_root = env::current_dir().unwrap_or_else(|_| PathBuf::from("."));
    let sln_path = repo_root.join("src").join("GesFer.Product.sln");
    let unit_project = repo_root
        .join("src")
        .join("tests")
        .join("GesFer.Product.UnitTests")
        .join("GesFer.Product.UnitTests.csproj");
    let integration_project = repo_root
        .join("src")
        .join("IntegrationTests")
        .join("GesFer.IntegrationTests.csproj");

    if !sln_path.exists() {
        feedback.push(FeedbackEntry::error(
        "build",
        &format!("Solución no encontrada: {}", sln_path.display()),
        None,
    ));
        let data = serde_json::json!({ "path": sln_path.display().to_string(), "scope": test_scope });
        emit_result(false, 1, "Solución no encontrada", feedback, Some(data), start, &args);
        std::process::exit(1);
    }

    feedback.push(FeedbackEntry::info("build", "Compilando solución..."));
    let build_start = Instant::now();
    let build_out = Command::new("dotnet")
        .args(["build", sln_path.to_str().unwrap(), "-c", "Debug", "-v", "q"])
        .current_dir(&repo_root)
        .output();
    let build_duration = build_start.elapsed().as_millis() as u64;
    let build_exit = match &build_out {
        Ok(o) => o.status.code().unwrap_or(1),
        Err(e) => {
            feedback.push(FeedbackEntry::error("build", &format!("Error: {}", e), None));
            emit_result(false, 1, "Build falló", feedback, None, start, &args);
            std::process::exit(1);
        }
    };
    feedback.push(FeedbackEntry::info(
        "build",
        &format!("Build finalizado (exitCode={}) duration_ms={}", build_exit, build_duration),
    ));
    if build_exit != 0 {
        let data = serde_json::json!({ "build_exit_code": build_exit, "scope": test_scope });
        emit_result(false, build_exit, "Build falló", feedback, Some(data), start, &args);
        std::process::exit(build_exit);
    }

    let (test_project, filter) = match test_scope.as_str() {
        "unit" => (unit_project, None),
        "integration" => (integration_project, None),
        "e2e" => {
            if integration_project.exists() {
                (integration_project.clone(), Some("Category=E2E"))
            } else {
                (sln_path.clone(), None)
            }
        }
        _ => (sln_path.clone(), None),
    };

    if test_scope != "all" && !test_project.exists() {
        feedback.push(FeedbackEntry::error(
            "tests",
            &format!("Proyecto no encontrado: {}", test_project.display()),
            None,
        ));
        let data = serde_json::json!({ "project": test_project.display().to_string() });
        emit_result(false, 1, "Proyecto no encontrado", feedback, Some(data), start, &args);
        std::process::exit(1);
    }

    if test_scope == "e2e" {
        env::set_var("E2E_BASE_URL", e2e_base_url.trim_end_matches('/'));
        env::set_var("E2E_INTERNAL_SECRET", "dev-internal-secret-change-in-production");
    }
    if test_scope == "all" {
        env::set_var("E2E_BASE_URL", e2e_base_url.trim_end_matches('/'));
        env::set_var("E2E_INTERNAL_SECRET", "dev-internal-secret-change-in-production");
    }

    feedback.push(FeedbackEntry::info(
        "tests",
        &format!("Ejecutando tests scope={}...", test_scope),
    ));
    let test_start = Instant::now();
    let mut test_cmd = Command::new("dotnet");
    test_cmd
        .arg("test")
        .arg(test_project.to_str().unwrap())
        .arg("--no-build")
        .args(["-v", "minimal"]);
    if let Some(f) = filter {
        test_cmd.args(["--filter", f]);
    }
    let test_out = test_cmd.current_dir(&repo_root).output();
    let test_duration = test_start.elapsed().as_millis() as u64;
    let tests_exit = match &test_out {
        Ok(o) => o.status.code().unwrap_or(1),
        Err(e) => {
            feedback.push(FeedbackEntry::error("tests", &format!("Error: {}", e), None));
            emit_result(false, 1, "Tests fallaron", feedback, None, start, &args);
            std::process::exit(1);
        }
    };
    feedback.push(FeedbackEntry::info(
        "tests",
        &format!("Tests {} finalizados (exitCode={}) duration_ms={}", test_scope, tests_exit, test_duration),
    ));

    let data = serde_json::json!({
        "scope": test_scope,
        "build_exit_code": 0,
        "tests_exit_code": tests_exit
    });

    if tests_exit != 0 {
        feedback.push(FeedbackEntry::warning(
            "done",
            &format!("Tests finalizados con fallos (exitCode={})", tests_exit),
            None,
        ));
        emit_result(false, tests_exit, "Tests con fallos", feedback, Some(data), start, &args);
        std::process::exit(tests_exit);
    }

    feedback.push(FeedbackEntry::info("done", "Tests completados correctamente"));
    emit_result(true, 0, "Tests completados correctamente", feedback, Some(data), start, &args);
}

fn load_input(
    args: &Args,
    feedback: &mut Vec<FeedbackEntry>,
) -> Result<(bool, bool, String, bool, String), (String, i32)> {
    if let Some(ref path) = args.input_path {
        feedback.push(FeedbackEntry::info("input", &format!("Cargando JSON desde {}", path)));
        let content = std::fs::read_to_string(path)
            .map_err(|e| (format!("No se pudo leer --input-path: {}", e), 1))?;
        let input: JsonInput = serde_json::from_str(&content)
            .map_err(|e| (format!("JSON inválido en --input-path: {}", e), 1))?;
        let sp = input.skip_prepare.unwrap_or(false);
        let ss = input.skip_seeds.unwrap_or(false);
        let ts = input.test_scope.unwrap_or_else(|| "all".to_string());
        let ot = input.only_tests.unwrap_or(false);
        let url = input.e2e_base_url.unwrap_or_else(|| "http://localhost:5010".to_string());
        return Ok((sp, ss, ts, ot, url));
    }
    Ok((
        args.skip_prepare,
        args.skip_seeds,
        args.test_scope.clone(),
        args.only_tests,
        args.e2e_base_url.clone(),
    ))
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
