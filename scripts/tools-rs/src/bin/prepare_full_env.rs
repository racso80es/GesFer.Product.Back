//! Herramienta prepare-full-env en Rust (contrato tools).
//! Prepara entorno: Docker (DB, cache, Adminer) y opcionalmente API.
//! Cumple SddIA/tools/tools-contract.json.

use std::env;
use std::path::PathBuf;
use std::process::Command;
use std::thread;
use std::time::{Duration, Instant};

use clap::Parser;
use gesfer_tools::{FeedbackEntry, ToolResult, to_contract_json};
use serde::Deserialize;

const TOOL_ID: &str = "prepare-full-env";

#[derive(Parser)]
#[command(name = "prepare_full_env")]
struct Args {
    /// Solo levanta Docker (DB, cache, Adminer).
    #[arg(long)]
    docker_only: bool,
    /// Además levanta la Admin API en local.
    #[arg(long)]
    start_api: bool,
    /// No levanta Docker (solo API/clientes si se pide).
    #[arg(long)]
    no_docker: bool,
    /// Ruta al JSON de configuración.
    #[arg(long)]
    config_path: Option<String>,
    /// Fichero donde escribir el resultado JSON.
    #[arg(long)]
    output_path: Option<String>,
    /// Emitir resultado JSON por stdout al finalizar.
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
struct PrepareEnvConfig {
    #[serde(default = "default_docker_compose_path")]
    docker_compose_path: String,
    #[serde(default = "default_mysql_container")]
    mysql_container_name: String,
    #[serde(default = "default_docker_services")]
    docker_services: Vec<String>,
    #[serde(default)]
    start_api: Option<StartApiConfig>,
    #[serde(default = "default_health_check")]
    health_check: HealthCheckConfig,
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
#[allow(dead_code)]
struct StartApiConfig {
    #[serde(default)]
    enabled: bool,
    #[serde(default = "default_working_dir")]
    working_dir: String,
    #[serde(default)]
    command: Option<String>,
    #[serde(default)]
    health_url: Option<String>,
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
#[allow(dead_code)]
struct HealthCheckConfig {
    #[serde(default = "default_mysql_max_attempts")]
    mysql_max_attempts: u32,
    #[serde(default = "default_mysql_retry_seconds")]
    mysql_retry_seconds: u64,
    #[serde(default = "default_api_wait_seconds")]
    api_wait_seconds: u64,
}

fn default_docker_compose_path() -> String {
    "docker-compose.yml".to_string()
}
fn default_mysql_container() -> String {
    "GesFer_product_db".to_string()
}
fn default_docker_services() -> Vec<String> {
    vec!["gesfer-db".to_string(), "cache".to_string(), "adminer".to_string()]
}
fn default_working_dir() -> String {
    "src/GesFer.Admin.Back.Api".to_string()
}
fn default_mysql_max_attempts() -> u32 {
    30
}
fn default_mysql_retry_seconds() -> u64 {
    2
}
fn default_api_wait_seconds() -> u64 {
    15
}
fn default_health_check() -> HealthCheckConfig {
    HealthCheckConfig {
        mysql_max_attempts: default_mysql_max_attempts(),
        mysql_retry_seconds: default_mysql_retry_seconds(),
        api_wait_seconds: default_api_wait_seconds(),
    }
}

impl Default for PrepareEnvConfig {
    fn default() -> Self {
        Self {
            docker_compose_path: default_docker_compose_path(),
            mysql_container_name: default_mysql_container(),
            docker_services: default_docker_services(),
            start_api: None,
            health_check: default_health_check(),
        }
    }
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();

    let args = Args::parse();

    feedback.push(FeedbackEntry::info("init", "Iniciando prepare-full-env (Rust)"));

    let repo_root = env::var("GESFER_REPO_ROOT").unwrap_or_else(|_| {
        env::current_dir()
            .map(|p| p.display().to_string())
            .unwrap_or_else(|_| ".".to_string())
    });
    feedback.push(FeedbackEntry::info("init", &format!("Repo root: {}", repo_root)));

    let config = load_config(&repo_root, &args, &mut feedback);

    let do_docker = !args.no_docker;
    let do_api = args.start_api
        || (!args.docker_only && config.start_api.as_ref().map(|s| s.enabled).unwrap_or(false));

    // 1. Verificar Docker
    feedback.push(FeedbackEntry::info("docker", "Comprobando Docker..."));
    let docker_info = Command::new("docker").arg("info").output();
    match docker_info {
        Ok(out) if out.status.success() => {
            feedback.push(FeedbackEntry::info("docker", "Docker OK"));
        }
        _ => {
            feedback.push(FeedbackEntry::error(
                "docker",
                "Docker no está corriendo. Inicia Docker Desktop.",
                None,
            ));
            let data = serde_json::json!({
                "docker_available": false,
                "message": "Docker no disponible"
            });
            emit_result(false, 1, "Docker no disponible", feedback, Some(data), start, &args);
            std::process::exit(1);
        }
    }

    // 2. Levantar servicios Docker
    if do_docker {
        let compose_path = PathBuf::from(&repo_root).join(&config.docker_compose_path);
        let services_str = config.docker_services.join(" ");
        feedback.push(FeedbackEntry::info(
            "docker",
            &format!("Levantando: {}", services_str),
        ));

        let mut docker_args = vec!["compose", "-f", compose_path.to_str().unwrap(), "up", "-d"];
        docker_args.extend(config.docker_services.iter().map(|s| s.as_str()));

        let compose_out = Command::new("docker")
            .args(&docker_args)
            .current_dir(&repo_root)
            .output();

        match compose_out {
            Ok(out) if out.status.success() => {
                feedback.push(FeedbackEntry::info("docker", "Servicios Docker levantados"));
            }
            Ok(out) => {
                let stderr = String::from_utf8_lossy(&out.stderr);
                feedback.push(FeedbackEntry::error(
                    "docker",
                    "Error al levantar docker-compose",
                    Some(stderr.as_ref()),
                ));
                let data = serde_json::json!({
                    "docker_services": config.docker_services,
                    "error": stderr.to_string()
                });
                emit_result(
                    false,
                    out.status.code().unwrap_or(1),
                    "docker-compose falló",
                    feedback,
                    Some(data),
                    start,
                    &args,
                );
                std::process::exit(1);
            }
            Err(e) => {
                feedback.push(FeedbackEntry::error(
                    "docker",
                    "No se pudo ejecutar docker compose",
                    Some(&e.to_string()),
                ));
                emit_result(false, 1, "docker compose no disponible", feedback, None, start, &args);
                std::process::exit(1);
            }
        }

        // 3. Esperar MySQL
        let container = &config.mysql_container_name;
        feedback.push(FeedbackEntry::info(
            "mysql",
            &format!("Esperando MySQL ({})...", container),
        ));

        let max_attempts = config.health_check.mysql_max_attempts;
        let retry_secs = config.health_check.mysql_retry_seconds;
        let mut mysql_ready = false;

        for attempt in 1..=max_attempts {
            thread::sleep(Duration::from_secs(retry_secs));
            let ping = Command::new("docker")
                .args([
                    "exec",
                    container,
                    "mysqladmin",
                    "ping",
                    "-h",
                    "localhost",
                    "-u",
                    "root",
                    "-prootpassword",
                ])
                .output();

            if let Ok(out) = ping {
                if out.status.success() {
                    mysql_ready = true;
                    feedback.push(FeedbackEntry::info("mysql", "MySQL listo"));
                    break;
                }
            }
            feedback.push(FeedbackEntry::info(
                "mysql",
                &format!("Intento {}/{}...", attempt, max_attempts),
            ));
        }

        if !mysql_ready {
            feedback.push(FeedbackEntry::error(
                "mysql",
                &format!("MySQL no listo después de {} intentos", max_attempts),
                None,
            ));
            let data = serde_json::json!({
                "mysql_container": container,
                "mysql_ready": false,
                "max_attempts": max_attempts
            });
            emit_result(false, 2, "MySQL no disponible", feedback, Some(data), start, &args);
            std::process::exit(2);
        }
    }

    // 4. Opcional: API (solo feedback; no lanzamos proceso en background desde Rust por simplicidad)
    if do_api {
        feedback.push(FeedbackEntry::info(
            "api",
            "Para iniciar la API, ejecute: dotnet run --project src/GesFer.Admin.Back.Api",
        ));
    }

    feedback.push(FeedbackEntry::info("done", "Infraestructura preparada"));

    let data = serde_json::json!({
        "docker_services": config.docker_services,
        "mysql_container": config.mysql_container_name,
        "mysql_ready": do_docker,
    });

    emit_result(true, 0, "Infraestructura preparada", feedback, Some(data), start, &args);
    std::process::exit(0);
}

fn load_config(repo_root: &str, args: &Args, feedback: &mut Vec<FeedbackEntry>) -> PrepareEnvConfig {
    let default_config = PathBuf::from(repo_root)
        .join("scripts")
        .join("tools")
        .join("prepare-full-env")
        .join("prepare-env.json");
    let config_path = args
        .config_path
        .as_deref()
        .unwrap_or(default_config.to_str().unwrap_or("prepare-env.json"));

    let path = PathBuf::from(config_path);
    let path = if path.is_absolute() {
        path
    } else {
        PathBuf::from(repo_root).join(config_path)
    };

    if path.exists() {
        match std::fs::read_to_string(&path) {
            Ok(s) => match serde_json::from_str::<PrepareEnvConfig>(&s) {
                Ok(c) => {
                    feedback.push(FeedbackEntry::info(
                        "init",
                        &format!("Config cargado: {}", path.display()),
                    ));
                    return c;
                }
                Err(e) => {
                    feedback.push(FeedbackEntry::warning(
                        "init",
                        &format!("Config inválido, usando valores por defecto: {}", e),
                        None,
                    ));
                }
            },
            Err(_) => {
                feedback.push(FeedbackEntry::warning(
                    "init",
                    "No se pudo leer config, usando valores por defecto",
                    None,
                ));
            }
        }
    } else {
        feedback.push(FeedbackEntry::info(
            "init",
            "Config no encontrado, usando valores por defecto",
        ));
    }

    PrepareEnvConfig::default()
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
    let result = ToolResult {
        tool_id: TOOL_ID.to_string(),
        exit_code,
        success,
        timestamp: chrono::Utc::now().to_rfc3339(),
        message: message.to_string(),
        feedback,
        data,
        duration_ms: Some(duration_ms),
    };
    let json = to_contract_json(&result).expect("serialize");
    if args.output_json || env::var("TOOLS_OUTPUT_JSON").as_deref() == Ok("1") {
        println!("{}", json);
    }
    if let Some(ref path) = args.output_path {
        let _ = std::fs::write(path, &json);
    }
}
