//! Skill finalizar-git (pre-pr): push de la rama y creación del PR (gh pr create o URL manual).
//! Cumple skills-contract: CLI + JSON input (--input-path), salida JSON.

use std::process::Command;
use std::time::Instant;

use clap::Parser;
use gesfer_skills::{FeedbackEntry, SkillResult, to_skill_json};
use serde::Deserialize;

const SKILL_ID: &str = "push-and-create-pr";

#[derive(Parser)]
#[command(name = "push_and_create_pr")]
struct Args {
    #[arg(long)]
    persist: Option<String>,
    #[arg(long)]
    branch_name: Option<String>,
    #[arg(long)]
    title: Option<String>,
    #[arg(long)]
    input_path: Option<String>,
    #[arg(long)]
    output_path: Option<String>,
    #[arg(long)]
    output_json: bool,
}

#[derive(Debug, Deserialize)]
struct JsonInput {
    #[serde(rename = "persist")]
    persist: Option<String>,
    #[serde(rename = "branchName")]
    branch_name: Option<String>,
    #[serde(rename = "title")]
    title: Option<String>,
}

fn main() {
    let start = Instant::now();
    let mut feedback = Vec::new();
    feedback.push(FeedbackEntry::info("init", "Skill push-and-create-pr (Rust)"));

    let args = Args::parse();

    let (persist, branch_name, title) = match load_input(&args, &mut feedback) {
        Ok(p) => p,
        Err((msg, exit_code)) => {
            feedback.push(FeedbackEntry::error("input", &msg, None));
            emit_result(false, exit_code, &msg, feedback, None, start, &args);
            std::process::exit(exit_code);
        }
    };

    let cwd = std::env::current_dir().unwrap_or_else(|_| std::path::PathBuf::from("."));
    let repo_root = cwd
        .ancestors()
        .find(|p| (p.join(".git")).exists())
        .unwrap_or(&cwd);

    let branch = if branch_name.is_empty() {
        get_current_branch(repo_root).unwrap_or_default()
    } else {
        branch_name
    };

    if branch.is_empty() {
        let msg = "No se pudo obtener la rama. Especifique --branch-name.";
        feedback.push(FeedbackEntry::error("input", msg, None));
        emit_result(false, 1, msg, feedback, None, start, &args);
        std::process::exit(1);
    }

    feedback.push(FeedbackEntry::info("push", &format!("git push origin {}", branch)));
    if let Err((msg, code)) = run_git(
        Command::new("git")
            .args(["push", "origin", &branch])
            .current_dir(repo_root),
        &mut feedback,
    ) {
        emit_result(false, code, &msg, feedback, None, start, &args);
        std::process::exit(code);
    }

    let main = detect_main_branch(repo_root, &mut feedback);
    let pr_title = if title.is_empty() {
        format!("feat: {}", branch)
    } else {
        title
    };
    let body = if persist.is_empty() {
        "PR desde skill push-and-create-pr.".to_string()
    } else {
        format!("Documentación: {}", persist)
    };

    let gh_available = Command::new("gh")
        .args(["auth", "status"])
        .current_dir(repo_root)
        .output()
        .map(|o| o.status.success())
        .unwrap_or(false);

    let pr_created = if gh_available {
        feedback.push(FeedbackEntry::info("gh", "Creando PR con gh..."));
        let out = Command::new("gh")
            .args([
                "pr",
                "create",
                "--base",
                &main,
                "--head",
                &branch,
                "--title",
                &pr_title,
                "--body",
                &body,
            ])
            .current_dir(repo_root)
            .output();
        match out {
            Ok(o) if o.status.success() => {
                feedback.push(FeedbackEntry::info("done", "PR creado correctamente."));
                true
            }
            _ => false,
        }
    } else {
        let pr_url = get_pr_compare_url(repo_root, &main, &branch);
        feedback.push(FeedbackEntry::warning(
            "gh",
            "gh no disponible. Crear PR manualmente",
            Some(&pr_url),
        ));
        feedback.push(FeedbackEntry::info("url", &pr_url));
        false
    };

    let data = serde_json::json!({
        "branch": branch,
        "main": main,
        "pr_created": pr_created,
        "persist": persist
    });
    emit_result(
        true,
        0,
        if pr_created {
            "Push y PR creado correctamente"
        } else {
            "Push realizado. Crear PR manualmente si gh no está disponible."
        },
        feedback,
        Some(data),
        start,
        &args,
    );
}

fn load_input(
    args: &Args,
    feedback: &mut Vec<FeedbackEntry>,
) -> Result<(String, String, String), (String, i32)> {
    if let Some(ref path) = args.input_path {
        feedback.push(FeedbackEntry::info("input", &format!("Cargando JSON desde {}", path)));
        let content = std::fs::read_to_string(path)
            .map_err(|e| (format!("No se pudo leer --input-path: {}", e), 1))?;
        let input: JsonInput = serde_json::from_str(&content)
            .map_err(|e| (format!("JSON inválido en --input-path: {}", e), 1))?;
        let persist = input.persist.unwrap_or_default();
        let bn = input.branch_name.unwrap_or_default();
        let title = input.title.unwrap_or_default();
        return Ok((persist, bn, title));
    }
    Ok((
        args.persist.clone().unwrap_or_default(),
        args.branch_name.clone().unwrap_or_default(),
        args.title.clone().unwrap_or_default(),
    ))
}

fn detect_main_branch(repo_root: &std::path::Path, _feedback: &mut Vec<FeedbackEntry>) -> String {
    for candidate in ["main", "master"] {
        let out = Command::new("git")
            .args(["rev-parse", "--verify", &format!("origin/{}", candidate)])
            .current_dir(repo_root)
            .output();
        if let Ok(o) = out {
            if o.status.success() {
                return candidate.to_string();
            }
        }
    }
    "main".to_string()
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

fn get_pr_compare_url(repo_root: &std::path::Path, base: &str, head: &str) -> String {
    let out = Command::new("git")
        .args(["config", "--get", "remote.origin.url"])
        .current_dir(repo_root)
        .output();
    let url = match out {
        Ok(o) if o.status.success() => {
            let s = String::from_utf8_lossy(&o.stdout);
            let s = s.trim().trim_end_matches(".git");
            let s = s.replace("git@github.com:", "https://github.com/");
            format!("{}/compare/{}...{}?expand=1", s, base, head)
        }
        _ => format!("https://github.com/.../compare/{}...{}?expand=1", base, head),
    };
    url
}

fn run_git(cmd: &mut Command, _feedback: &mut Vec<FeedbackEntry>) -> Result<(), (String, i32)> {
    let output = cmd.output().map_err(|e| (format!("Error ejecutando comando: {}", e), 1))?;
    let stderr = String::from_utf8_lossy(&output.stderr);
    if !output.status.success() {
        let msg = if stderr.trim().is_empty() {
            "git push falló".to_string()
        } else {
            stderr.trim().to_string()
        };
        return Err((msg, output.status.code().unwrap_or(1)));
    }
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
