//! Herramienta invoke-mysql-seeds en Rust (contrato tools).
//! Comprueba MySQL, ejecuta dotnet ef database update y seeds (RUN_SEEDS_ONLY).
//! Cumple SddIA/tools/tools-contract.json.

use std::env;
use std::path::PathBuf;
use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_tools::{FeedbackEntry, ToolResult, to_contract_json};
use serde::Deserialize;

const TOOL_ID: &str = "invoke-mysql-seeds";

#[derive(Parser)]
#[command(name = "invoke_mysql_seeds")]
struct Args {
    /// No ejecutar dotnet ef database update; solo seeds.
    #[arg(long)]
    skip_migrations: bool,
    /// Solo ejecutar migraciones; no ejecutar seeds.
    #[arg(long)]
    skip_seeds: bool,
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
struct MysqlSeedsConfig {
    #[serde(default = "default_ef_project")]
    ef_project: String,
    #[serde(default = "default_startup_project")]
    startup_project: String,
    #[serde(default = "default_seeds_project")]
    seeds_project: String,
    #[serde(default = "default_mysql_container")]
    mysql_container_name: String,
    #[serde(default = "default_true")]
    run_migrations: bool,
    #[serde(default = "default_true")]
    run_seeds: bool,
}

fn default_ef_project() -> String {
    "src/GesFer.Admin.Back.Infrastructure/GesFer.Admin.Back.Infrastructure.csproj".to_string()
}
fn default_startup_project() -> String {
    "src/GesFer.Admin.Back.Infrastructure/GesFer.Admin.Back.Infrastructure.csproj".to_string()
}
fn default_seeds_project() -> String {
    "src/GesFer.Admin.Back.Api/GesFer.Admin.Back.Api.csproj".to_string()
}
fn default_mysql_container() -> String {
    "GesFer_product_db".to_string()
}
fn default_true() -> bool {
    true
}

impl Default for MysqlSeedsConfig {
    fn default() -> Self {
        Self {
            ef_project: default_ef_project(),
            startup_project: default_startup_project(),
            seeds_project: default_seeds_project(),
            mysql_container_name: default_mysql_container(),
            run_migrations: true,
            run_seeds: true,
        }
    }
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();

    let args = Args::parse();

    feedback.push(FeedbackEntry::info("init", "Iniciando invoke-mysql-seeds (Rust)"));

    let repo_root = env::var("GESFER_REPO_ROOT").unwrap_or_else(|_| {
        env::current_dir()
            .map(|p| p.display().to_string())
            .unwrap_or_else(|_| ".".to_string())
    });

    let config = load_config(&repo_root, &args, &mut feedback);

    let do_migrations = config.run_migrations && !args.skip_migrations;
    let do_seeds = config.run_seeds && !args.skip_seeds;

    // 1. Comprobar MySQL
    let container = &config.mysql_container_name;
    feedback.push(FeedbackEntry::info(
        "mysql",
        &format!("Comprobando MySQL ({})...", container),
    ));
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

    match ping {
        Ok(out) if out.status.success() => {
            feedback.push(FeedbackEntry::info("mysql", "MySQL disponible."));
        }
        _ => {
            feedback.push(FeedbackEntry::error(
                "mysql",
                "MySQL no responde. Levanta Docker (Prepare-FullEnv) o comprueba el contenedor.",
                None,
            ));
            let data = serde_json::json!({
                "mysql": { "container": container, "ready": false },
                "migrations": null,
                "seeds": null
            });
            emit_result(false, 1, "MySQL no disponible", feedback, Some(data), start, &args);
            std::process::exit(1);
        }
    }

    let mut mig_ms: u64 = 0;
    let mut seed_ms: u64 = 0;

    // 2. Migraciones
    if do_migrations {
        feedback.push(FeedbackEntry::info("migrations", "Aplicando migraciones EF..."));
        let mig_start = Instant::now();
        let ef = Command::new("dotnet")
            .args([
                "ef",
                "database",
                "update",
                "--project",
                &config.ef_project,
                "--startup-project",
                &config.startup_project,
            ])
            .current_dir(&repo_root)
            .output();
        mig_ms = mig_start.elapsed().as_millis() as u64;

        match ef {
            Ok(out) if out.status.success() => {
                feedback.push(FeedbackEntry::info("migrations", "Migraciones aplicadas."));
            }
            Ok(out) => {
                let stderr = String::from_utf8_lossy(&out.stderr);
                let stdout = String::from_utf8_lossy(&out.stdout);
                let detail = if !stderr.is_empty() {
                    stderr.to_string()
                } else {
                    stdout.to_string()
                };
                let detail_opt = if detail.is_empty() { None } else { Some(detail.as_str()) };
                feedback.push(FeedbackEntry::error(
                    "migrations",
                    "dotnet ef database update falló",
                    detail_opt,
                ));
                let data = serde_json::json!({
                    "mysql": { "container": container, "ready": true },
                    "migrations": { "exitCode": out.status.code().unwrap_or(-1), "duration_ms": mig_ms },
                    "seeds": null
                });
                emit_result(
                    false,
                    out.status.code().unwrap_or(1),
                    "Error en migraciones",
                    feedback,
                    Some(data),
                    start,
                    &args,
                );
                std::process::exit(1);
            }
            Err(e) => {
                feedback.push(FeedbackEntry::error(
                    "migrations",
                    "No se pudo ejecutar dotnet ef",
                    Some(&e.to_string()),
                ));
                emit_result(false, 1, "dotnet ef no disponible", feedback, None, start, &args);
                std::process::exit(1);
            }
        }
    } else {
        feedback.push(FeedbackEntry::info("migrations", "Omitido (--skip-migrations o runMigrations=false)."));
    }

    // 3. Seeds
    if do_seeds {
        feedback.push(FeedbackEntry::info("seeds", "Ejecutando seeds (RUN_SEEDS_ONLY)..."));
        env::set_var("RUN_SEEDS_ONLY", "1");
        let seed_start = Instant::now();
        let seeds_project_abs = PathBuf::from(&repo_root).join(&config.seeds_project);
        let seeds_cwd = seeds_project_abs
            .parent()
            .filter(|p| p.is_dir())
            .map(|p| p.to_path_buf())
            .unwrap_or_else(|| PathBuf::from(&repo_root));
        let run = Command::new("dotnet")
            .args([
                "run",
                "--project",
                seeds_project_abs.to_str().unwrap_or(&config.seeds_project),
            ])
            .current_dir(&seeds_cwd)
            .output();
        let _ = env::remove_var("RUN_SEEDS_ONLY");
        seed_ms = seed_start.elapsed().as_millis() as u64;

        match run {
            Ok(out) if out.status.success() => {
                feedback.push(FeedbackEntry::info("seeds", "Seeds ejecutados correctamente."));
            }
            Ok(out) => {
                let stderr = String::from_utf8_lossy(&out.stderr);
                let stdout = String::from_utf8_lossy(&out.stdout);
                let detail = if !stderr.is_empty() {
                    stderr.to_string()
                } else {
                    stdout.to_string()
                };
                let detail_opt = if detail.is_empty() { None } else { Some(detail.as_str()) };
                feedback.push(FeedbackEntry::error(
                    "seeds",
                    "Seeds fallaron",
                    detail_opt,
                ));
                let data = serde_json::json!({
                    "mysql": { "container": container, "ready": true },
                    "migrations": { "duration_ms": mig_ms },
                    "seeds": { "exitCode": out.status.code().unwrap_or(-1), "duration_ms": seed_ms }
                });
                emit_result(
                    false,
                    out.status.code().unwrap_or(1),
                    "Error en seeds",
                    feedback,
                    Some(data),
                    start,
                    &args,
                );
                std::process::exit(1);
            }
            Err(e) => {
                feedback.push(FeedbackEntry::error(
                    "seeds",
                    "No se pudo ejecutar dotnet run",
                    Some(&e.to_string()),
                ));
                emit_result(false, 1, "dotnet run falló", feedback, None, start, &args);
                std::process::exit(1);
            }
        }
    } else {
        feedback.push(FeedbackEntry::info("seeds", "Omitido (--skip-seeds o runSeeds=false)."));
    }

    feedback.push(FeedbackEntry::info("done", "MySQL, migraciones y seeds listos."));

    let data = serde_json::json!({
        "mysql": { "container": container, "ready": true },
        "migrations": { "duration_ms": mig_ms },
        "seeds": { "duration_ms": seed_ms }
    });

    emit_result(
        true,
        0,
        "Migraciones y seeds completados",
        feedback,
        Some(data),
        start,
        &args,
    );
    std::process::exit(0);
}

fn load_config(repo_root: &str, args: &Args, feedback: &mut Vec<FeedbackEntry>) -> MysqlSeedsConfig {
    let default_config = PathBuf::from(repo_root)
        .join("scripts")
        .join("tools")
        .join("invoke-mysql-seeds")
        .join("mysql-seeds-config.json");
    let config_path = args
        .config_path
        .as_deref()
        .unwrap_or(default_config.to_str().unwrap_or("mysql-seeds-config.json"));

    let path = PathBuf::from(config_path);
    let path = if path.is_absolute() {
        path
    } else {
        PathBuf::from(repo_root).join(config_path)
    };

    if path.exists() {
        if let Ok(s) = std::fs::read_to_string(&path) {
            if let Ok(c) = serde_json::from_str::<MysqlSeedsConfig>(&s) {
                feedback.push(FeedbackEntry::info(
                    "init",
                    &format!("Config cargado: {}", path.display()),
                ));
                return c;
            }
            feedback.push(FeedbackEntry::warning(
                "init",
                "Config inválido, usando valores por defecto",
                None,
            ));
        }
    }

    feedback.push(FeedbackEntry::info(
        "init",
        "Config no encontrado, usando valores por defecto",
    ));
    MysqlSeedsConfig::default()
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
