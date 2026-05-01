//! Entrada/salida JSON y utilidades Git compartidas entre skills ejecutables (contrato skills-contract).

use std::env;
use std::fs;
use std::io::{self, Read};
use std::path::{Path, PathBuf};
use std::process::{Command, Stdio};
use std::time::Instant;

use clap::Parser;
use crate::{FeedbackEntry, SkillResult, to_skill_json};
use serde_json::Value;

#[derive(Parser, Debug)]
#[command(about = "Skill GesFer (JSON por stdin, --input-path o GESFER_CAPSULE_REQUEST)")]
pub struct SkillCli {
    #[arg(long)]
    pub input_path: Option<String>,
    #[arg(long)]
    pub output_path: Option<String>,
    /// Emitir una línea JSON en stdout (recomendado para invocación IA / cápsula).
    #[arg(long, default_value_t = true)]
    pub output_json: bool,
}

/// Lee el objeto JSON de petición (contrato skills: skillId + parámetros en camelCase).
pub fn read_request_json(cli: &SkillCli) -> Result<Value, String> {
    if let Ok(raw) = env::var("GESFER_CAPSULE_REQUEST") {
        let raw = raw.trim();
        if !raw.is_empty() {
            return serde_json::from_str(raw).map_err(|e| format!("GESFER_CAPSULE_REQUEST JSON: {e}"));
        }
    }
    let skip_stdin = env::var("GESFER_SKIP_STDIN")
        .map(|v| v == "1" || v.eq_ignore_ascii_case("true"))
        .unwrap_or(false);
    if let Some(p) = &cli.input_path {
        let s = fs::read_to_string(p).map_err(|e| format!("input-path: {e}"))?;
        return serde_json::from_str(s.trim()).map_err(|e| format!("input-path JSON: {e}"));
    }
    if skip_stdin {
        return Err(
            "GESFER_SKIP_STDIN=1 sin --input-path ni GESFER_CAPSULE_REQUEST con JSON válido".into(),
        );
    }
    let mut buf = String::new();
    io::stdin()
        .read_to_string(&mut buf)
        .map_err(|e| format!("stdin: {e}"))?;
    let buf = buf.trim();
    if buf.is_empty() {
        return Err("Entrada JSON vacía: use stdin, --input-path o GESFER_CAPSULE_REQUEST".into());
    }
    serde_json::from_str(buf).map_err(|e| format!("stdin JSON: {e}"))
}

pub fn working_dir_from_value(v: &Value) -> Result<PathBuf, String> {
    if let Some(s) = v.get("workingDirectory").and_then(|x| x.as_str()) {
        return Ok(PathBuf::from(s));
    }
    if let Ok(r) = env::var("GESFER_REPO_ROOT") {
        return Ok(PathBuf::from(r));
    }
    env::current_dir().map_err(|e| e.to_string())
}

pub fn str_field<'a>(v: &'a Value, key: &str) -> Option<&'a str> {
    v.get(key).and_then(|x| x.as_str())
}

pub fn string_field(v: &Value, key: &str) -> Option<String> {
    str_field(v, key).map(String::from)
}

pub fn bool_field(v: &Value, key: &str, default: bool) -> bool {
    v.get(key).and_then(|x| x.as_bool()).unwrap_or(default)
}

pub fn run_git_output(cwd: &Path, args: &[&str]) -> Result<std::process::Output, String> {
    Command::new("git")
        .current_dir(cwd)
        .args(args)
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .output()
        .map_err(|e| format!("git {:?}: {e}", args.first().unwrap_or(&"")))
}

pub fn run_git(cwd: &Path, args: &[&str]) -> Result<String, String> {
    let out = run_git_output(cwd, args)?;
    let stdout = String::from_utf8_lossy(&out.stdout).to_string();
    let stderr = String::from_utf8_lossy(&out.stderr).to_string();
    if !out.status.success() {
        return Err(format!(
            "git {} → {}: {}",
            args.join(" "),
            out.status,
            stderr.trim()
        ));
    }
    Ok(stdout)
}

pub fn ensure_git_repo(cwd: &Path) -> Result<(), String> {
    run_git(cwd, &["rev-parse", "--git-dir"]).map(|_| ())
}

pub fn finish(_skill_id: &str, res: SkillResult, cli: &SkillCli, _start: Instant) -> ! {
    let exit = res.exit_code;
    let json = match to_skill_json(&res) {
        Ok(j) => j,
        Err(e) => {
            eprintln!("{}", e);
            std::process::exit(1);
        }
    };
    if cli.output_json {
        println!("{}", json);
    }
    if let Some(p) = &cli.output_path {
        if let Err(e) = fs::write(p, &json) {
            eprintln!("output-path: {}", e);
            std::process::exit(1);
        }
    }
    std::process::exit(exit)
}

pub fn ok(
    skill_id: &str,
    message: &str,
    feedback: Vec<FeedbackEntry>,
    data: Option<Value>,
    start: Instant,
) -> SkillResult {
    SkillResult::ok(
        skill_id,
        message,
        feedback,
        data,
        start.elapsed().as_millis() as u64,
    )
}

pub fn err(
    skill_id: &str,
    message: &str,
    feedback: Vec<FeedbackEntry>,
    code: i32,
    data: Option<Value>,
    start: Instant,
) -> SkillResult {
    SkillResult::err(
        skill_id,
        message,
        feedback,
        code,
        data,
        start.elapsed().as_millis() as u64,
    )
}
