//! Tool run-test-e2e-local: E2E GesFer.Product.Back.E2ETests con entorno local opcional.
//! Cumple tools-contract: salida JSON, feedback por fases.

use std::env;
use std::path::{Path, PathBuf};
use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_tools::{FeedbackEntry, ToolResult, to_contract_json};
use serde::Deserialize;
use serde_json::json;

const TOOL_ID: &str = "run-test-e2e-local";
const DEFAULT_INTERNAL_SECRET: &str = "dev-internal-secret-change-in-production";

#[derive(Parser)]
#[command(name = "run_test_e2e_local")]
struct Args {
    #[arg(long, default_value = "http://localhost:5010")]
    admin_api_url: String,
    #[arg(long, default_value = "http://localhost:5020")]
    product_api_url: String,
    #[arg(long, default_value_t = false)]
    skip_prepare: bool,
    #[arg(long, default_value_t = false)]
    skip_seeds: bool,
    #[arg(long, default_value_t = false)]
    only_tests: bool,
    #[arg(long, default_value_t = false)]
    skip_api_probe: bool,
    #[arg(long)]
    e2e_internal_secret: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long, default_value_t = false)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
struct CapsuleConfig {
    #[serde(rename = "e2eEnv")]
    e2e_env: Option<E2eEnv>,
}

#[derive(Debug, Deserialize)]
struct E2eEnv {
    #[serde(rename = "E2E_INTERNAL_SECRET")]
    e2e_internal_secret: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    let args = Args::parse();

    let repo_root = find_repo_root();
    let e2e_project = repo_root
        .join("src")
        .join("GesFer.Product.Back.E2ETests")
        .join("GesFer.Product.Back.E2ETests.csproj");

    let admin_base = normalize_base_url(&args.admin_api_url);
    let product_base = normalize_base_url(&args.product_api_url);

    let mut skip_prepare = args.skip_prepare;
    let mut skip_seeds = args.skip_seeds;
    if args.only_tests {
        skip_prepare = true;
        skip_seeds = true;
    }

    let internal_secret = resolve_internal_secret(&repo_root, &args.e2e_internal_secret);

    feedback.push(FeedbackEntry::info(
        "init",
        &format!(
            "Iniciando {} (Admin={} Product={})",
            TOOL_ID, admin_base, product_base
        ),
    ));

    if !e2e_project.exists() {
        feedback.push(FeedbackEntry::error(
            "init",
            &format!("Proyecto E2E no encontrado: {}", e2e_project.display()),
            None,
        ));
        emit_result(
            false,
            1,
            "Proyecto E2E no encontrado",
            feedback,
            Some(json!({ "project": e2e_project.display().to_string() })),
            start,
            &args,
        );
        std::process::exit(1);
    }

    let tools_dir = repo_root.join("scripts").join("tools");
    let prepare_bat = tools_dir.join("prepare-full-env").join("Prepare-FullEnv.bat");
    let seeds_bat = tools_dir.join("invoke-mysql-seeds").join("Invoke-MySqlSeeds.bat");

    if !args.only_tests {
        if !skip_prepare && prepare_bat.exists() {
            feedback.push(FeedbackEntry::info("prepare", "Invocando prepare-full-env..."));
            let t0 = Instant::now();
            let st = run_cmd_bat(&prepare_bat, &repo_root);
            let ms = t0.elapsed().as_millis() as u64;
            feedback.push(FeedbackEntry::info(
                "prepare",
                &format!("prepare-full-env finalizado duration_ms={}", ms),
            ));
            if !st.success() {
                let code = st.code().unwrap_or(1);
                feedback.push(FeedbackEntry::error(
                    "prepare",
                    "prepare-full-env fallo",
                    Some(&format!("exitCode={}", code)),
                ));
                emit_result(
                    false,
                    code,
                    "prepare-full-env fallo",
                    feedback,
                    Some(json!({ "phase": "prepare", "exitCode": code })),
                    start,
                    &args,
                );
                std::process::exit(code);
            }
        } else if skip_prepare {
            feedback.push(FeedbackEntry::info(
                "prepare",
                "Omitiendo prepare-full-env (SkipPrepare).",
            ));
        }

        if !skip_seeds && seeds_bat.exists() {
            feedback.push(FeedbackEntry::info("seeds", "Invocando invoke-mysql-seeds..."));
            let t0 = Instant::now();
            let st = run_cmd_bat(&seeds_bat, &repo_root);
            let ms = t0.elapsed().as_millis() as u64;
            feedback.push(FeedbackEntry::info(
                "seeds",
                &format!("invoke-mysql-seeds finalizado duration_ms={}", ms),
            ));
            if !st.success() {
                let code = st.code().unwrap_or(1);
                feedback.push(FeedbackEntry::error(
                    "seeds",
                    "invoke-mysql-seeds fallo",
                    Some(&format!("exitCode={}", code)),
                ));
                emit_result(
                    false,
                    code,
                    "invoke-mysql-seeds fallo",
                    feedback,
                    Some(json!({ "phase": "seeds", "exitCode": code })),
                    start,
                    &args,
                );
                std::process::exit(code);
            }
        } else if skip_seeds {
            feedback.push(FeedbackEntry::info(
                "seeds",
                "Omitiendo invoke-mysql-seeds (SkipSeeds).",
            ));
        }
    }

    if !args.skip_api_probe {
        feedback.push(FeedbackEntry::info(
            "probe",
            "Comprobando health Admin y Product...",
        ));
        let ok_admin = check_health(&admin_base);
        let ok_product = check_health(&product_base);
        if !ok_admin {
            feedback.push(FeedbackEntry::error(
                "probe",
                &format!("Admin API no responde en {}/health", admin_base),
                None,
            ));
            emit_result(
                false,
                1,
                "Health Admin fallo",
                feedback,
                Some(json!({
                    "admin_api_url": admin_base,
                    "product_api_url": product_base,
                    "phase": "probe"
                })),
                start,
                &args,
            );
            std::process::exit(1);
        }
        if !ok_product {
            feedback.push(FeedbackEntry::error(
                "probe",
                &format!("Product API no responde en {}/health", product_base),
                None,
            ));
            emit_result(
                false,
                1,
                "Health Product fallo",
                feedback,
                Some(json!({
                    "admin_api_url": admin_base,
                    "product_api_url": product_base,
                    "phase": "probe"
                })),
                start,
                &args,
            );
            std::process::exit(1);
        }
        feedback.push(FeedbackEntry::info(
            "probe",
            "Health OK (Admin y Product).",
        ));
    } else {
        feedback.push(FeedbackEntry::warning(
            "probe",
            "Omitiendo comprobacion health (SkipApiProbe).",
            None,
        ));
    }

    feedback.push(FeedbackEntry::info("build", "Compilando proyecto E2E..."));
    let build_start = Instant::now();
    let build_out = Command::new("dotnet")
        .args([
            "build",
            e2e_project.to_str().unwrap(),
            "-c",
            "Debug",
            "-v",
            "q",
        ])
        .current_dir(&repo_root)
        .output();
    let build_ms = build_start.elapsed().as_millis() as u64;
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
        &format!(
            "Build finalizado (exitCode={}) duration_ms={}",
            build_exit, build_ms
        ),
    ));
    if build_exit != 0 {
        emit_result(
            false,
            build_exit,
            "Build falló",
            feedback,
            Some(json!({ "build_exit_code": build_exit })),
            start,
            &args,
        );
        std::process::exit(build_exit);
    }

    env::set_var("E2E_BASE_URL", &product_base);
    env::set_var("E2E_INTERNAL_SECRET", &internal_secret);

    feedback.push(FeedbackEntry::info(
        "tests",
        &format!(
            "Ejecutando tests E2E (Category=E2E) con E2E_BASE_URL={}...",
            product_base
        ),
    ));
    let test_start = Instant::now();
    let test_out = Command::new("dotnet")
        .args([
            "test",
            e2e_project.to_str().unwrap(),
            "--filter",
            "Category=E2E",
            "--no-build",
            "-v",
            "minimal",
        ])
        .env("E2E_BASE_URL", &product_base)
        .env("E2E_INTERNAL_SECRET", &internal_secret)
        .current_dir(&repo_root)
        .output();
    let test_ms = test_start.elapsed().as_millis() as u64;
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
        &format!(
            "Tests E2E finalizados (exitCode={}) duration_ms={}",
            tests_exit, test_ms
        ),
    ));

    let data = json!({
        "admin_api_url": admin_base,
        "product_api_url": product_base,
        "tests_exit_code": tests_exit,
        "e2e_project": e2e_project.display().to_string()
    });

    if tests_exit != 0 {
        feedback.push(FeedbackEntry::warning(
            "done",
            "Tests E2E con fallos",
            None,
        ));
        emit_result(
            false,
            tests_exit,
            "Tests E2E con fallos",
            feedback,
            Some(data),
            start,
            &args,
        );
        std::process::exit(tests_exit);
    }

    feedback.push(FeedbackEntry::info(
        "done",
        "Tests E2E completados correctamente",
    ));
    emit_result(
        true,
        0,
        "Tests E2E completados correctamente",
        feedback,
        Some(data),
        start,
        &args,
    );
}

fn find_repo_root() -> PathBuf {
    if let Ok(cwd) = env::current_dir() {
        if cwd.join("src").join("GesFer.Product.sln").exists() {
            return cwd;
        }
    }
    if let Ok(exe) = env::current_exe() {
        let mut p = exe.parent().map(Path::to_path_buf).unwrap_or_default();
        for _ in 0..12 {
            if p.join("src").join("GesFer.Product.sln").exists() {
                return p;
            }
            if !p.pop() {
                break;
            }
        }
    }
    env::current_dir().unwrap_or_else(|_| PathBuf::from("."))
}

fn normalize_base_url(url: &str) -> String {
    url.trim().trim_end_matches('/').to_string()
}

fn resolve_internal_secret(repo_root: &Path, arg: &Option<String>) -> String {
    if let Some(ref s) = arg {
        if !s.is_empty() {
            return s.clone();
        }
    }
    let config_path = repo_root
        .join("scripts")
        .join("tools")
        .join("run-test-e2e-local")
        .join("run-test-e2e-local-config.json");
    if let Ok(raw) = std::fs::read_to_string(&config_path) {
        if let Ok(cfg) = serde_json::from_str::<CapsuleConfig>(&raw) {
            if let Some(env) = cfg.e2e_env {
                if let Some(sec) = env.e2e_internal_secret {
                    if !sec.is_empty() {
                        return sec;
                    }
                }
            }
        }
    }
    DEFAULT_INTERNAL_SECRET.to_string()
}

fn check_health(base: &str) -> bool {
    let client = match reqwest::blocking::Client::builder()
        .timeout(std::time::Duration::from_secs(10))
        .build()
    {
        Ok(c) => c,
        Err(_) => return false,
    };
    let url = format!("{}/health", base);
    client.get(url).send().map(|r| r.status().is_success()).unwrap_or(false)
}

#[cfg(windows)]
fn run_cmd_bat(bat: &Path, cwd: &Path) -> std::process::ExitStatus {
    Command::new("cmd")
        .args(["/C", bat.to_str().unwrap_or("")])
        .current_dir(cwd)
        .status()
        .unwrap_or_else(|_| {
            Command::new("cmd")
                .args(["/c", "exit", "1"])
                .status()
                .expect("cmd exit 1")
        })
}

/// La orquestación de .bat es solo Windows; en otros SO devuelve fallo (no-op).
#[cfg(not(windows))]
fn run_cmd_bat(_bat: &Path, _cwd: &Path) -> std::process::ExitStatus {
    std::process::Command::new("false")
        .status()
        .unwrap_or_else(|_| std::process::Command::new("/bin/false").status().expect("false"))
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
        if let Some(parent) = Path::new(path).parent() {
            let _ = std::fs::create_dir_all(parent);
        }
        let _ = std::fs::write(path, &json);
    }
    if args.output_json {
        println!("{}", json);
    }
}
