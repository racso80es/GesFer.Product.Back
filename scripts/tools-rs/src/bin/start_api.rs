//! Herramienta start-api en Rust (contrato tools).
//! Valida si el puerto está ocupado; opción fail/kill (desde config). Levanta la API, comprueba health (éxito = health 200).
//! Detecta errores de base de datos (MySQL no disponible) en la salida de la API.
//! Parámetros de arranque (puerto, perfil, health, timeouts, portBlocked, configuración dotnet): solo desde JSON; sin valores por defecto en código.

use std::io::{BufRead, BufReader};
use std::net::TcpListener;
use std::path::PathBuf;
use std::process::{Command, Stdio};
use std::sync::{Arc, Mutex};
use std::thread;
use std::time::{Duration, Instant};

use clap::Parser;
use gesfer_tools::{FeedbackEntry, ToolResult, to_contract_json};
use serde::Deserialize;

const TOOL_ID: &str = "start-api";

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
enum PortBlocked {
    Fail,
    Kill,
}

#[derive(Parser)]
#[command(name = "start_api")]
struct Args {
    #[arg(long)]
    no_build: bool,
    #[arg(long)]
    config_path: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
#[serde(rename_all = "camelCase")]
struct StartApiConfigFile {
    api_working_dir: String,
    default_profile: String,
    default_port: u16,
    health_url: String,
    health_check_timeout_seconds: u64,
    port_blocked: String,
    dotnet_configuration: String,
}

struct RuntimeConfig {
    api_working_dir: String,
    profile: String,
    port: u16,
    health_url: String,
    health_check_timeout_seconds: u64,
    port_blocked: PortBlocked,
    dotnet_configuration: String,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();

    let args = Args::parse();

    feedback.push(FeedbackEntry::info("init", "Iniciando start-api (Rust)"));

    let repo_root = std::env::var("GESFER_REPO_ROOT")
        .unwrap_or_else(|_| std::env::current_dir().unwrap().display().to_string());
    let config_path = args.config_path.as_deref().unwrap_or("start-api-config.json");
    let config = match load_runtime_config(&repo_root, config_path) {
        Ok(c) => c,
        Err(msg) => {
            feedback.push(FeedbackEntry::error("init", &msg, None));
            emit_result(false, 1, &msg, feedback, None, start, &args);
            std::process::exit(1);
        }
    };

    let port = config.port;
    let health_url = config.health_url.clone();
    let profile = config.profile.clone();
    let port_blocked = config.port_blocked;

    // 1. Comprobar si el puerto está ocupado
    feedback.push(FeedbackEntry::info(
        "port-check",
        &format!("Comprobando puerto {}...", port),
    ));
    if port_in_use(port) {
        feedback.push(FeedbackEntry::warning(
            "port-check",
            &format!("El puerto {} está ocupado.", port),
            None,
        ));
        match port_blocked {
            PortBlocked::Fail => {
                feedback.push(FeedbackEntry::error(
                    "port-check",
                    "Puerto ocupado. Cambie portBlocked a kill en start-api-config.json o libere el puerto.",
                    None,
                ));
                let data = serde_json::json!({ "port": port, "port_in_use": true });
                emit_result(false, 2, "Puerto ocupado", feedback, Some(data), start, &args);
                std::process::exit(2);
            }
            PortBlocked::Kill => {
                feedback.push(FeedbackEntry::info("port-kill", "Intentando cerrar proceso que usa el puerto..."));
                if let Err(e) = kill_process_on_port(port) {
                    feedback.push(FeedbackEntry::error(
                        "port-kill",
                        "No se pudo liberar el puerto",
                        Some(&e),
                    ));
                    let data = serde_json::json!({ "port": port, "port_in_use": true });
                    emit_result(false, 3, "No se pudo liberar el puerto", feedback, Some(data), start, &args);
                    std::process::exit(3);
                }
                feedback.push(FeedbackEntry::info("port-kill", "Puerto liberado."));
                thread::sleep(Duration::from_secs(1));
                if port_in_use(port) {
                    feedback.push(FeedbackEntry::error(
                        "port-check",
                        "El puerto sigue ocupado tras intentar cerrar el proceso.",
                        None,
                    ));
                    emit_result(false, 3, "Puerto aún ocupado", feedback, None, start, &args);
                    std::process::exit(3);
                }
            }
        }
    } else {
        feedback.push(FeedbackEntry::info("port-check", "Puerto libre."));
    }

    let api_dir = std::path::Path::new(&repo_root).join(&config.api_working_dir);
    if !api_dir.exists() {
        feedback.push(FeedbackEntry::error(
            "init",
            &format!("Directorio API no encontrado: {}", api_dir.display()),
            None,
        ));
        emit_result(false, 4, "Directorio API no encontrado", feedback, None, start, &args);
        std::process::exit(4);
    }

    // 2. Build opcional
    let dotnet_cfg = &config.dotnet_configuration;
    if !args.no_build {
        feedback.push(FeedbackEntry::info("build", "Compilando proyecto..."));
        let out = Command::new("dotnet")
            .args(["build", &config.api_working_dir, "-c", dotnet_cfg])
            .current_dir(&repo_root)
            .output();
        match out {
            Ok(o) if o.status.success() => {
                feedback.push(FeedbackEntry::info("build", "Build OK"));
            }
            Ok(o) => {
                let stderr = String::from_utf8_lossy(&o.stderr);
                feedback.push(FeedbackEntry::error("build", "Build fallido", Some(stderr.as_ref())));
                emit_result(false, 5, "Build fallido", feedback, None, start, &args);
                std::process::exit(5);
            }
            Err(e) => {
                feedback.push(FeedbackEntry::error("build", "No se pudo ejecutar dotnet build", Some(&e.to_string())));
                emit_result(false, 5, "dotnet build no disponible", feedback, None, start, &args);
                std::process::exit(5);
            }
        }
    } else {
        feedback.push(FeedbackEntry::info("build", "NoBuild: omitiendo compilación"));
    }

    // 3. Lanzar API en segundo plano (capturar stderr para detectar errores de BD)
    feedback.push(FeedbackEntry::info(
        "launch",
        &format!("Levantando API en {} (Profile: {}, Port: {})", api_dir.display(), profile, port),
    ));
    let mut child = match Command::new("dotnet")
        .args([
            "run",
            "--no-build",
            "-c",
            dotnet_cfg,
            &format!("--urls=http://127.0.0.1:{}", port),
        ])
        .current_dir(&api_dir)
        .env("ASPNETCORE_ENVIRONMENT", &profile)
        .stdout(Stdio::null())
        .stderr(Stdio::piped())
        .spawn()
    {
        Ok(c) => c,
        Err(e) => {
            feedback.push(FeedbackEntry::error("launch", "No se pudo ejecutar dotnet run", Some(&e.to_string())));
            emit_result(false, 6, "Error al lanzar API", feedback, None, start, &args);
            std::process::exit(6);
        }
    };
    let pid = child.id();
    feedback.push(FeedbackEntry::info("launch", &format!("API iniciada con PID {}", pid)));

    let _ = child.stdin.take();
    let stderr_accum: Arc<Mutex<String>> = Arc::new(Mutex::new(String::new()));
    if let Some(stderr) = child.stderr.take() {
        let acc = Arc::clone(&stderr_accum);
        thread::spawn(move || {
            let reader = BufReader::new(stderr);
            for line in reader.lines().filter_map(Result::ok) {
                if let Ok(mut s) = acc.lock() {
                    s.push_str(&line);
                    s.push('\n');
                }
            }
        });
    }

    let timeout_secs = config.health_check_timeout_seconds;
    let step = Duration::from_secs(2);
    let deadline = Instant::now() + Duration::from_secs(timeout_secs);
    let mut healthy = false;
    let mut db_unavailable = false;
    let client = reqwest::blocking::Client::builder()
        .timeout(Duration::from_secs(5))
        .build()
        .unwrap_or_else(|_| reqwest::blocking::Client::new());

    while Instant::now() < deadline {
        thread::sleep(step);

        // Comprobar si la API reporta error de base de datos en stderr
        if let Ok(guard) = stderr_accum.lock() {
            let log = guard.as_str();
            if log.contains("Unable to connect to any of the specified MySQL hosts")
                || log.contains("MySqlConnector.MySqlException")
            {
                db_unavailable = true;
                break;
            }
        }

        if let Ok(resp) = client.get(&health_url).send() {
            if resp.status().as_u16() == 200 {
                healthy = true;
                feedback.push(FeedbackEntry::info("healthcheck", &format!("Health OK: {}", health_url)));
                break;
            }
        }
        feedback.push(FeedbackEntry::info(
            "healthcheck",
            &format!("Esperando salud ({}/{} s)...", Instant::now().duration_since(start).as_secs(), timeout_secs),
        ));
    }

    if db_unavailable {
        feedback.push(FeedbackEntry::error(
            "healthcheck",
            "Base de datos (MySQL) no disponible. La API no puede completar el arranque.",
            None,
        ));
        feedback.push(FeedbackEntry::info(
            "healthcheck",
            "Ejecute prepare-full-env (Docker/MySQL) e invoke-mysql-seeds antes de start-api.",
        ));
        let data = serde_json::json!({
            "url_base": health_url,
            "port": port,
            "profile": profile,
            "pid": pid,
            "healthy": false,
            "error_type": "database_unavailable"
        });
        emit_result(
            false,
            8,
            "Base de datos no disponible (MySQL). Ejecute prepare-full-env e invoke-mysql-seeds.",
            feedback,
            Some(data),
            start,
            &args,
        );
        std::process::exit(8);
    }

    if !healthy {
        feedback.push(FeedbackEntry::warning(
            "healthcheck",
            &format!("Timeout salud; API arrancada (PID {}). Compruebe {}", pid, health_url),
            None,
        ));
        let data = serde_json::json!({
            "url_base": health_url,
            "port": port,
            "profile": profile,
            "pid": pid,
            "healthy": false
        });
        emit_result(false, 7, "Health no respondió a tiempo", feedback, Some(data), start, &args);
        std::process::exit(7);
    }

    let data = serde_json::json!({
        "url_base": health_url,
        "port": port,
        "profile": profile,
        "pid": pid,
        "healthy": true
    });
    feedback.push(FeedbackEntry::info("done", &format!("API levantada. PID: {} URL: {}", pid, health_url)));
    emit_result(true, 0, "API levantada; health OK", feedback, Some(data), start, &args);
    std::process::exit(0);
}

fn resolve_config_path(repo_root: &str, config_file: &str) -> Result<PathBuf, String> {
    let mut path = PathBuf::from(repo_root);
    path.push(config_file);
    if !path.exists() {
        path = PathBuf::from(repo_root)
            .join("scripts")
            .join("tools")
            .join("start-api")
            .join(config_file);
    }
    if !path.exists() {
        return Err(format!(
            "Config no encontrado: {} (buscado en raíz y en scripts/tools/start-api/)",
            config_file
        ));
    }
    Ok(path)
}

fn parse_port_blocked(raw: &str) -> Result<PortBlocked, String> {
    match raw.trim().to_lowercase().as_str() {
        "fail" => Ok(PortBlocked::Fail),
        "kill" => Ok(PortBlocked::Kill),
        _ => Err(format!(
            "portBlocked en config debe ser 'fail' o 'kill' (valor recibido: {:?})",
            raw.trim()
        )),
    }
}

fn load_runtime_config(repo_root: &str, config_file: &str) -> Result<RuntimeConfig, String> {
    let path = resolve_config_path(repo_root, config_file)?;
    let s = std::fs::read_to_string(&path)
        .map_err(|e| format!("No se pudo leer {}: {}", path.display(), e))?;
    let file: StartApiConfigFile = serde_json::from_str(&s).map_err(|e| {
        format!(
            "Config JSON inválido o faltan campos obligatorios en {}: {}",
            path.display(),
            e
        )
    })?;
    let port_blocked = parse_port_blocked(&file.port_blocked)?;
    Ok(RuntimeConfig {
        api_working_dir: file.api_working_dir,
        profile: file.default_profile,
        port: file.default_port,
        health_url: file.health_url,
        health_check_timeout_seconds: file.health_check_timeout_seconds,
        port_blocked,
        dotnet_configuration: file.dotnet_configuration,
    })
}

fn port_in_use(port: u16) -> bool {
    TcpListener::bind(("127.0.0.1", port)).is_err()
}

/// En Windows: netstat -ano, parsear líneas con :port y LISTENING, obtener PID; taskkill /PID x /F.
#[cfg(target_os = "windows")]
fn kill_process_on_port(port: u16) -> Result<(), String> {
    let port_str = format!(":{}", port);
    let out = Command::new("netstat")
        .args(["-ano"])
        .output()
        .map_err(|e| e.to_string())?;
    let stdout = String::from_utf8_lossy(&out.stdout);
    let pids: Vec<u32> = stdout
        .lines()
        .filter(|line| line.contains("LISTENING") && line.contains(&port_str))
        .filter_map(|line| {
            let parts: Vec<&str> = line.split_whitespace().collect();
            parts.last().and_then(|s| s.parse::<u32>().ok())
        })
        .collect::<std::collections::HashSet<_>>()
        .into_iter()
        .collect();
    if pids.is_empty() {
        return Err("No se encontró proceso escuchando en el puerto".to_string());
    }
    for pid in pids {
        let _ = Command::new("taskkill").args(["/PID", &pid.to_string(), "/F"]).output();
    }
    Ok(())
}

#[cfg(not(target_os = "windows"))]
fn kill_process_on_port(_port: u16) -> Result<(), String> {
    Err("Liberar puerto (kill) solo soportado en Windows".to_string())
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
    if args.output_json || std::env::var("TOOLS_OUTPUT_JSON").as_deref() == Ok("1") {
        println!("{}", json);
    }
    if let Some(ref path) = args.output_path {
        let _ = std::fs::write(path, &json);
    }
}
