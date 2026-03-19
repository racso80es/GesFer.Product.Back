//! Skill invoke-commit: git add + git commit con parámetros directos.
//! Cumple skills-contract: salida JSON.

use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_skills::{FeedbackEntry, SkillResult, to_skill_json};

const SKILL_ID: &str = "invoke-commit";

#[derive(Parser)]
#[command(name = "invoke_commit")]
struct Args {
    #[arg(long, short)]
    message: String,
    #[arg(long)]
    files: Option<String>,
    #[arg(long, short)]
    all: bool,
    #[arg(long, default_value = "feat")]
    r#type: String,
    #[arg(long)]
    scope: Option<String>,
    #[arg(long)]
    output_json: bool,
    #[arg(long)]
    output_path: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    feedback.push(FeedbackEntry::info("init", "Skill invoke-commit (Rust)"));

    let args = Args::parse();

    if args.files.is_some() && args.all {
        feedback.push(FeedbackEntry::error("input", "--files y --all son excluyentes", None));
        emit_result(false, 1, "Usar --files o --all, no ambos", feedback, None, start, &args);
        std::process::exit(1);
    }

    if args.files.is_none() && !args.all {
        feedback.push(FeedbackEntry::error("input", "Se requiere --files o --all", None));
        emit_result(false, 1, "Indicar --files \"ruta1,ruta2\" o --all", feedback, None, start, &args);
        std::process::exit(1);
    }

    if args.message.is_empty() {
        feedback.push(FeedbackEntry::error("input", "--message requerido", None));
        emit_result(false, 1, "Mensaje de commit obligatorio", feedback, None, start, &args);
        std::process::exit(1);
    }

    let commit_msg = format_commit_message(&args);
    feedback.push(FeedbackEntry::info("message", &commit_msg));

    if args.all {
        feedback.push(FeedbackEntry::info("add", "git add -A"));
        let mut add_cmd = Command::new("git");
        add_cmd.args(["add", "-A"]);
        if let Err((msg, code)) = run_git(&mut add_cmd) {
            emit_result(false, code, &msg, feedback, None, start, &args);
            std::process::exit(code);
        }
    } else {
        let files: Vec<&str> = args.files.as_ref().unwrap().split(',').map(|s| s.trim()).filter(|s| !s.is_empty()).collect();
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

fn format_commit_message(args: &Args) -> String {
    let scope = args.scope.as_deref().unwrap_or("");
    let prefix = if scope.is_empty() {
        format!("{}: ", args.r#type)
    } else {
        format!("{}({}): ", args.r#type, scope)
    };
    format!("{}{}", prefix, args.message)
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
