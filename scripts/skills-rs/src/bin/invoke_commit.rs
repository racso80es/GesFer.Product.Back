//! Skill invoke-commit: git add + git commit con parámetros directos.
//! Cumple skills-contract: CLI + JSON input (--input-path), salida JSON.

use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_skills::{FeedbackEntry, SkillResult, to_skill_json};
use serde::Deserialize;

const SKILL_ID: &str = "invoke-commit";

#[derive(Parser)]
#[command(name = "invoke_commit")]
struct Args {
    #[arg(long, short)]
    message: Option<String>,
    #[arg(long)]
    files: Option<String>,
    #[arg(long, short)]
    all: bool,
    #[arg(long, default_value = "feat")]
    r#type: String,
    #[arg(long)]
    scope: Option<String>,
    #[arg(long)]
    input_path: Option<String>,
    #[arg(long)]
    output_json: bool,
    #[arg(long)]
    output_path: Option<String>,
}

#[derive(Debug, Deserialize)]
struct JsonInput {
    #[serde(rename = "message")]
    message: Option<String>,
    #[serde(rename = "files")]
    files: Option<String>,
    #[serde(rename = "all")]
    all: Option<bool>,
    #[serde(rename = "type")]
    r#type: Option<String>,
    #[serde(rename = "scope")]
    scope: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    feedback.push(FeedbackEntry::info("init", "Skill invoke-commit (Rust)"));

    let args = Args::parse();

    let (message, files, all, commit_type, scope) = match load_input(&args, &mut feedback) {
        Ok(p) => p,
        Err((msg, exit_code)) => {
            feedback.push(FeedbackEntry::error("input", &msg, None));
            emit_result(false, exit_code, &msg, feedback, None, start, &args);
            std::process::exit(exit_code);
        }
    };

    if files.is_some() && all {
        feedback.push(FeedbackEntry::error("input", "--files y --all son excluyentes", None));
        emit_result(false, 1, "Usar --files o --all, no ambos", feedback, None, start, &args);
        std::process::exit(1);
    }

    if files.is_none() && !all {
        feedback.push(FeedbackEntry::error("input", "Se requiere --files o --all", None));
        emit_result(false, 1, "Indicar --files \"ruta1,ruta2\" o --all", feedback, None, start, &args);
        std::process::exit(1);
    }

    if message.is_empty() {
        feedback.push(FeedbackEntry::error("input", "message requerido", None));
        emit_result(false, 1, "Mensaje de commit obligatorio", feedback, None, start, &args);
        std::process::exit(1);
    }

    let commit_msg = format_commit_message(&message, &commit_type, scope.as_deref());
    feedback.push(FeedbackEntry::info("message", &commit_msg));

    if all {
        feedback.push(FeedbackEntry::info("add", "git add -A"));
        let mut add_cmd = Command::new("git");
        add_cmd.args(["add", "-A"]);
        if let Err((msg, code)) = run_git(&mut add_cmd) {
            emit_result(false, code, &msg, feedback, None, start, &args);
            std::process::exit(code);
        }
    } else {
        let files: Vec<&str> = files.as_ref().unwrap().split(',').map(|s| s.trim()).filter(|s| !s.is_empty()).collect();
        for f in &files {
            feedback.push(FeedbackEntry::info("add", &format!("git add {}", f)));
        }
        if files.is_empty() {
            feedback.push(FeedbackEntry::error("add", "--files vacío o inválido", None));
            emit_result(false, 1, "Ningún archivo válido en --files", feedback, None, start, &args);
            std::process::exit(1);
        }
        let mut add_cmd = Command::new("git");
        add_cmd.arg("add").args(&files);
        if let Err((msg, code)) = run_git(&mut add_cmd) {
            emit_result(false, code, &msg, feedback, None, start, &args);
            std::process::exit(code);
        }
    }

    feedback.push(FeedbackEntry::info("commit", "git commit"));
    let mut commit_cmd = Command::new("git");
    commit_cmd.args(["commit", "-m", &commit_msg]);
    if let Err((msg, code)) = run_git(&mut commit_cmd) {
        emit_result(false, code, &msg, feedback, None, start, &args);
        std::process::exit(code);
    }

    let data = serde_json::json!({ "message": commit_msg });
    emit_result(true, 0, "Commit realizado correctamente", feedback, Some(data), start, &args);
}

fn load_input(
    args: &Args,
    feedback: &mut Vec<FeedbackEntry>,
) -> Result<(String, Option<String>, bool, String, Option<String>), (String, i32)> {
    if let Some(ref path) = args.input_path {
        feedback.push(FeedbackEntry::info("input", &format!("Cargando JSON desde {}", path)));
        let content = std::fs::read_to_string(path)
            .map_err(|e| (format!("No se pudo leer --input-path: {}", e), 1))?;
        let input: JsonInput = serde_json::from_str(&content)
            .map_err(|e| (format!("JSON inválido en --input-path: {}", e), 1))?;
        let msg = input.message.ok_or(("message requerido en JSON".to_string(), 1))?;
        let all = input.all.unwrap_or(false);
        let files = if all {
            None
        } else {
            Some(input.files.ok_or(("files requerido en JSON cuando all es false".to_string(), 1))?)
        };
        let commit_type = input.r#type.unwrap_or_else(|| "feat".to_string());
        return Ok((msg, files, all, commit_type, input.scope));
    }

    let msg = args.message.clone().ok_or(("--message requerido (o --input-path)".to_string(), 1))?;
    Ok((
        msg,
        args.files.clone(),
        args.all,
        args.r#type.clone(),
        args.scope.clone(),
    ))
}

fn format_commit_message(message: &str, commit_type: &str, scope: Option<&str>) -> String {
    let scope = scope.unwrap_or("");
    let prefix = if scope.is_empty() {
        format!("{}: ", commit_type)
    } else {
        format!("{}({}): ", commit_type, scope)
    };
    format!("{}{}", prefix, message)
}

fn run_git(cmd: &mut Command) -> Result<(), (String, i32)> {
    let output = cmd.output().map_err(|e| (format!("Error ejecutando comando: {}", e), 1))?;
    let stderr = String::from_utf8_lossy(&output.stderr);
    if !output.status.success() {
        let msg = if stderr.trim().is_empty() {
            format!("Comando falló con código {}", output.status.code().unwrap_or(-1))
        } else {
            stderr.trim().to_string()
        };
        return Err((msg, output.status.code().unwrap_or(1)));
    }
    Ok(())
}

fn emit_result(success: bool, exit_code: i32, message: &str, feedback: Vec<FeedbackEntry>, data: Option<serde_json::Value>, start: Instant, args: &Args) {
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
