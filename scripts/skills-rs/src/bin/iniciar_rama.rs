//! Skill iniciar-rama: crear rama feat/ o fix/ desde troncal actualizada.
//! Cumple skills-contract: JSON entrada (CLI o --input-path) y salida (--output-json o --output-path).

use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_skills::{FeedbackEntry, SkillResult, to_skill_json};
use serde::Deserialize;

const SKILL_ID: &str = "iniciar-rama";

#[derive(Parser)]
#[command(name = "iniciar_rama")]
struct Args {
    #[arg(long)]
    branch_type: Option<String>,
    #[arg(long)]
    branch_name: Option<String>,
    #[arg(long)]
    main_branch: Option<String>,
    #[arg(long)]
    skip_pull: bool,
    #[arg(long)]
    input_path: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
struct JsonInput {
    #[serde(rename = "BranchType")]
    branch_type: Option<String>,
    #[serde(rename = "BranchName")]
    branch_name: Option<String>,
    #[serde(rename = "MainBranch")]
    main_branch: Option<String>,
    #[serde(rename = "SkipPull")]
    skip_pull: Option<bool>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    feedback.push(FeedbackEntry::info("init", "Skill iniciar-rama (Rust)"));

    let args = Args::parse();

    let (branch_type, branch_name, main_branch, skip_pull) = match load_input(&args, &mut feedback) {
        Ok(params) => params,
        Err((msg, exit_code)) => {
            feedback.push(FeedbackEntry::error("input", &msg, None));
            emit_result(false, exit_code, &msg, feedback, None, start, &args);
            std::process::exit(exit_code);
        }
    };

    let branch_type = match branch_type.to_lowercase().as_str() {
        "feat" | "fix" => branch_type,
        _ => {
            let msg = "BranchType debe ser feat o fix";
            feedback.push(FeedbackEntry::error("input", msg, None));
            emit_result(false, 1, msg, feedback, None, start, &args);
            std::process::exit(1);
        }
    };

    let slug = normalize_slug(&branch_name);
    if slug.is_empty() {
        let msg = "BranchName no puede estar vacío";
        feedback.push(FeedbackEntry::error("input", msg, None));
        emit_result(false, 1, msg, feedback, None, start, &args);
        std::process::exit(1);
    }

    let full_branch = format!("{}/{}", branch_type, slug);
    let main = main_branch
        .as_deref()
        .unwrap_or_else(|| detect_main_branch(&mut feedback));

    feedback.push(FeedbackEntry::info(
        "params",
        &format!("Rama: {}, troncal: {}, skip_pull: {}", full_branch, main, skip_pull),
    ));

    let cwd = std::env::current_dir().unwrap_or_else(|_| std::path::PathBuf::from("."));
    feedback.push(FeedbackEntry::info("cwd", &format!("Directorio: {}", cwd.display())));

    if let Err((msg, exit_code)) = run_git_flow(&full_branch, main, skip_pull, &mut feedback) {
        feedback.push(FeedbackEntry::error("git", &msg, None));
        let data = serde_json::json!({ "branch": full_branch, "main": main });
        emit_result(false, exit_code, &msg, feedback, Some(data), start, &args);
        std::process::exit(exit_code);
    }

    let data = serde_json::json!({
        "branch": full_branch,
        "main": main,
        "skip_pull": skip_pull
    });
    let _duration_ms = start.elapsed().as_millis() as u64;
    emit_result(
        true,
        0,
        &format!("Rama {} creada/actualizada correctamente", full_branch),
        feedback,
        Some(data),
        start,
        &args,
    );
}

fn load_input(args: &Args, feedback: &mut Vec<FeedbackEntry>) -> Result<(String, String, Option<String>, bool), (String, i32)> {
    if let Some(ref path) = args.input_path {
        feedback.push(FeedbackEntry::info("input", &format!("Cargando JSON desde {}", path)));
        let content = std::fs::read_to_string(path)
            .map_err(|e| (format!("No se pudo leer --input-path: {}", e), 1))?;
        let input: JsonInput = serde_json::from_str(&content)
            .map_err(|e| (format!("JSON inválido en --input-path: {}", e), 1))?;
        let bt = input.branch_type.ok_or(("BranchType requerido en JSON".to_string(), 1))?;
        let bn = input.branch_name.ok_or(("BranchName requerido en JSON".to_string(), 1))?;
        return Ok((bt, bn, input.main_branch, input.skip_pull.unwrap_or(false)));
    }

    let bt = args.branch_type.clone().ok_or(("--branch-type requerido (o --input-path)".to_string(), 1))?;
    let bn = args.branch_name.clone().ok_or(("--branch-name requerido (o --input-path)".to_string(), 1))?;
    Ok((bt, bn, args.main_branch.clone(), args.skip_pull))
}

fn detect_main_branch(feedback: &mut Vec<FeedbackEntry>) -> &'static str {
    for candidate in ["main", "master"] {
        let out = Command::new("git")
            .args(["rev-parse", "--verify", candidate])
            .output();
        if let Ok(o) = out {
            if o.status.success() {
                feedback.push(FeedbackEntry::info(
                    "detect-main",
                    &format!("Troncal detectada: {}", candidate),
                ));
                return candidate;
            }
        }
    }
    feedback.push(FeedbackEntry::warning(
        "detect-main",
        "No se detectó main ni master; usando main por defecto",
        None,
    ));
    "main"
}

fn normalize_slug(name: &str) -> String {
    name.trim()
        .replace(' ', "-")
        .replace('/', "-")
        .replace('\\', "-")
        .chars()
        .filter(|c| c.is_alphanumeric() || *c == '-')
        .collect::<String>()
}

fn run_git(cmd: &mut Command, phase: &str, feedback: &mut Vec<FeedbackEntry>) -> Result<String, (String, i32)> {
    let output = cmd.output().map_err(|e| (format!("Error ejecutando comando: {}", e), 1))?;
    let stdout = String::from_utf8_lossy(&output.stdout);
    let stderr = String::from_utf8_lossy(&output.stderr);
    if !stdout.trim().is_empty() {
        feedback.push(FeedbackEntry::info(phase, stdout.trim()));
    }
    if !output.status.success() {
        let msg = if stderr.trim().is_empty() {
            format!("Comando falló con código {}", output.status.code().unwrap_or(-1))
        } else {
            stderr.trim().to_string()
        };
        return Err((msg, output.status.code().unwrap_or(1)));
    }
    Ok(stdout.to_string())
}

fn run_git_flow(full_branch: &str, main: &str, skip_pull: bool, feedback: &mut Vec<FeedbackEntry>) -> Result<(), (String, i32)> {
    let branch_exists = run_git(
        Command::new("git").args(["branch", "--list", full_branch]),
        "check-branch",
        feedback,
    )
    .map(|s| !s.trim().is_empty())
    .unwrap_or(false);

    if branch_exists {
        feedback.push(FeedbackEntry::info("checkout", &format!("Rama {} ya existe, checkout", full_branch)));
        run_git(Command::new("git").args(["checkout", full_branch]), "checkout", feedback)?;
        feedback.push(FeedbackEntry::info("merge", "Merge con origin para actualizar"));
        run_git(Command::new("git").args(["fetch", "origin"]), "fetch", feedback)?;
        let _ = run_git(
            Command::new("git").args(["merge", &format!("origin/{}", main)]),
            "merge",
            feedback,
        );
        return Ok(());
    }

    feedback.push(FeedbackEntry::info("fetch", "git fetch origin"));
    run_git(Command::new("git").args(["fetch", "origin"]), "fetch", feedback)?;

    feedback.push(FeedbackEntry::info("checkout-main", &format!("git checkout {}", main)));
    run_git(Command::new("git").args(["checkout", main]), "checkout-main", feedback)?;

    if !skip_pull {
        feedback.push(FeedbackEntry::info("pull", &format!("git pull origin {}", main)));
        run_git(Command::new("git").args(["pull", "origin", main]), "pull", feedback)?;
    }

    feedback.push(FeedbackEntry::info("create-branch", &format!("git checkout -b {}", full_branch)));
    run_git(Command::new("git").args(["checkout", "-b", full_branch]), "create-branch", feedback)?;

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
