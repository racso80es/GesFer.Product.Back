//! Skill invoke-command: interceptor de comandos de sistema (git, dotnet, npm, pwsh).
//! Cumple skills-contract: CLI + JSON input (--input-path), salida JSON.
//! Registro en docs/diagnostics/{branch}/execution_history.json.

use std::io::Write;
use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_skills::{FeedbackEntry, SkillResult, to_skill_json};
use serde::Deserialize;

const SKILL_ID: &str = "invoke-command";

const VALID_FASES: &[&str] = &["Triaje", "Analisis", "Evaluacion", "Marcado", "Accion"];

#[derive(Parser)]
#[command(name = "invoke_command")]
struct Args {
    #[arg(long, short)]
    command: Option<String>,
    #[arg(long)]
    command_file: Option<String>,
    #[arg(long, default_value = "GesFer")]
    contexto: String,
    #[arg(long, default_value = "Accion")]
    fase: String,
    #[arg(long)]
    working_dir: Option<String>,
    #[arg(long)]
    input_path: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
struct JsonInput {
    #[serde(rename = "command")]
    command: Option<String>,
    #[serde(rename = "commandFile")]
    command_file: Option<String>,
    #[serde(rename = "contexto")]
    contexto: Option<String>,
    #[serde(rename = "fase")]
    fase: Option<String>,
    #[serde(rename = "workingDir")]
    working_dir: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    feedback.push(FeedbackEntry::info("init", "Skill invoke-command (Rust)"));

    let args = Args::parse();

    let (command, contexto, fase, working_dir) = match load_input(&args, &mut feedback) {
        Ok(p) => p,
        Err((msg, exit_code)) => {
            feedback.push(FeedbackEntry::error("input", &msg, None));
            emit_result(false, exit_code, &msg, feedback, None, start, &args);
            std::process::exit(exit_code);
        }
    };

    if !VALID_FASES.contains(&fase.as_str()) {
        let msg = format!(
            "Fase debe ser uno de: {}. Recibido: {}",
            VALID_FASES.join(", "),
            fase
        );
        feedback.push(FeedbackEntry::error("input", &msg, None));
        emit_result(false, 1, &msg, feedback, None, start, &args);
        std::process::exit(1);
    }

    feedback.push(FeedbackEntry::info(
        "params",
        &format!("Command: {} | Contexto: {} | Fase: {}", command, contexto, fase),
    ));

    let cwd = std::env::current_dir().unwrap_or_else(|_| std::path::PathBuf::from("."));
    let repo_root = cwd
        .ancestors()
        .find(|p| (p.join(".git")).exists())
        .unwrap_or(&cwd);
    let branch = get_current_branch(repo_root).unwrap_or_else(|| "unknown".to_string());
    feedback.push(FeedbackEntry::info("branch", &format!("Rama: {}", branch)));

    let mut cmd = if cfg!(target_os = "windows") {
        let mut c = Command::new("cmd");
        c.args(["/C", &command]);
        c
    } else {
        let mut c = Command::new("sh");
        c.args(["-c", &command]);
        c
    };

    if let Some(ref wd) = working_dir {
        cmd.current_dir(wd);
        feedback.push(FeedbackEntry::info("working_dir", wd));
    } else {
        cmd.current_dir(repo_root);
    }

    feedback.push(FeedbackEntry::info("exec", &format!("Ejecutando: {}", command)));
    let output = cmd.output();

    let (success, exit_code, output_str, status_str) = match output {
        Ok(o) => {
            let out = String::from_utf8_lossy(&o.stdout);
            let err = String::from_utf8_lossy(&o.stderr);
            let combined = if err.trim().is_empty() {
                out.trim().to_string()
            } else {
                format!("{}\n{}", out.trim(), err.trim())
            };
            let code = o.status.code().unwrap_or(1);
            let ok = o.status.success();
            if !ok && !combined.is_empty() {
                feedback.push(FeedbackEntry::error("exec", &combined, None));
            } else if !combined.is_empty() {
                feedback.push(FeedbackEntry::info("output", &combined));
            }
            (
                ok,
                code,
                combined,
                if ok { "Success" } else { "Failed" }.to_string(),
            )
        }
        Err(e) => {
            let msg = format!("Error ejecutando comando: {}", e);
            feedback.push(FeedbackEntry::error("exec", &msg, None));
            emit_result(false, 1, &msg, feedback, None, start, &args);
            std::process::exit(1);
        }
    };

    if let Err(e) = append_execution_history(
        repo_root,
        &branch,
        &fase,
        &contexto,
        &command,
        &status_str,
        exit_code,
        &output_str,
    ) {
        feedback.push(FeedbackEntry::warning(
            "history",
            &format!("No se pudo registrar en execution_history: {}", e),
            None,
        ));
    }

    let data = serde_json::json!({
        "command": command,
        "exitCode": exit_code,
        "output": output_str
    });
    emit_result(
        success,
        exit_code,
        if success {
            "Comando ejecutado correctamente"
        } else {
            "Comando falló"
        },
        feedback,
        Some(data),
        start,
        &args,
    );
    std::process::exit(exit_code);
}

fn load_input(
    args: &Args,
    feedback: &mut Vec<FeedbackEntry>,
) -> Result<(String, String, String, Option<String>), (String, i32)> {
    if let Some(ref path) = args.input_path {
        feedback.push(FeedbackEntry::info("input", &format!("Cargando JSON desde {}", path)));
        let content = std::fs::read_to_string(path)
            .map_err(|e| (format!("No se pudo leer --input-path: {}", e), 1))?;
        let input: JsonInput = serde_json::from_str(&content)
            .map_err(|e| (format!("JSON inválido en --input-path: {}", e), 1))?;
        let cmd = input
            .command
            .clone()
            .or_else(|| {
                input
                    .command_file
                    .as_ref()
                    .and_then(|f| std::fs::read_to_string(f).ok())
            })
            .ok_or(("command o commandFile requerido en JSON".to_string(), 1))?;
        let ctx = input.contexto.unwrap_or_else(|| "GesFer".to_string());
        let f = input.fase.unwrap_or_else(|| "Accion".to_string());
        return Ok((cmd, ctx, f, input.working_dir));
    }

    let cmd = if let Some(ref c) = args.command {
        c.clone()
    } else if let Some(ref cf) = args.command_file {
        std::fs::read_to_string(cf)
            .map_err(|e| (format!("No se pudo leer --command-file: {}", e), 1))?
    } else {
        return Err(("--command o --command-file o --input-path requerido".to_string(), 1));
    };

    Ok((
        cmd,
        args.contexto.clone(),
        args.fase.clone(),
        args.working_dir.clone(),
    ))
}

fn get_current_branch(repo_root: &std::path::Path) -> Option<String> {
    let out = Command::new("git")
        .args(["branch", "--show-current"])
        .current_dir(repo_root)
        .output()
        .ok()?;
    if out.status.success() {
        Some(String::from_utf8_lossy(&out.stdout).trim().to_string())
    } else {
        None
    }
}

fn append_execution_history(
    repo_root: &std::path::Path,
    branch: &str,
    fase: &str,
    contexto: &str,
    command: &str,
    status: &str,
    exit_code: i32,
    output: &str,
) -> Result<(), String> {
    use chrono::Utc;
    let branch_slug = branch.replace('/', "-");
    let diagnostics_dir = repo_root.join("docs").join("diagnostics").join(&branch_slug);
    std::fs::create_dir_all(&diagnostics_dir).map_err(|e| e.to_string())?;
    let history_path = diagnostics_dir.join("execution_history.json");

    let entry = serde_json::json!({
        "Timestamp": Utc::now().to_rfc3339(),
        "Fase": fase,
        "Contexto": contexto,
        "Command": command,
        "Status": status,
        "ExitCode": exit_code,
        "Output": output
    });
    let line = format!("{}\n", serde_json::to_string(&entry).map_err(|e| e.to_string())?);

    let mut f = std::fs::OpenOptions::new()
        .create(true)
        .append(true)
        .open(&history_path)
        .map_err(|e| e.to_string())?;
    f.write_all(line.as_bytes()).map_err(|e| e.to_string())?;
    Ok(())
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
        SkillResult::ok(SKILL_ID, message, feedback, data, duration_ms)
    } else {
        SkillResult::err(SKILL_ID, message, feedback, exit_code, data, duration_ms)
    };
    let json = to_skill_json(&result).expect("serialize");
    if let Some(ref path) = args.output_path {
        let _ = std::fs::write(path, &json);
    }
    if args.output_json {
        println!("{}", json);
    }
}
