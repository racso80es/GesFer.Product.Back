//! SddIA Skills Runner.
//! List y Show usan spec.md con frontmatter YAML (arquitectura post-migración).

use clap::{Parser, Subcommand};
use serde_json::Value;
use std::fs;

const SKILLS_PATH: &str = "SddIA/skills";

/// Resuelve la ruta base del repo (donde está SddIA/)
fn find_repo_root() -> Option<std::path::PathBuf> {
    let mut current = std::env::current_dir().ok()?;
    loop {
        let sddia = current.join(SKILLS_PATH);
        if sddia.exists() {
            return Some(current);
        }
        if !current.pop() {
            return None;
        }
    }
}

/// Lista skills leyendo existencia de spec.md
fn list_skills() -> Result<Vec<String>, String> {
    let root = find_repo_root().ok_or("No se encontró SddIA/skills (ejecutar desde raíz del repo)")?;
    let skills_dir = root.join(SKILLS_PATH);
    let mut skills = Vec::new();
    for entry in fs::read_dir(&skills_dir).map_err(|e| e.to_string())? {
        let entry = entry.map_err(|e| e.to_string())?;
        let path = entry.path();
        if path.is_dir() {
            let spec_md = path.join("spec.md");
            if spec_md.exists() {
                if let Some(name) = path.file_name() {
                    skills.push(name.to_string_lossy().to_string());
                }
            }
        }
    }
    skills.sort();
    Ok(skills)
}

/// Parsea frontmatter YAML de un archivo .md
fn parse_frontmatter(content: &str) -> Option<(&str, &str)> {
    let content = content.trim_start();
    if !content.starts_with("---") {
        return None;
    }
    let rest = &content[3..];
    let end = rest.find("\n---")?;
    let yaml = rest[..end].trim();
    let body = rest[end + 4..].trim_start();
    Some((yaml, body))
}

/// Carga spec.md con frontmatter: devuelve (frontmatter como Value, cuerpo)
fn load_full_context(skill_id: &str) -> Result<(Value, String), String> {
    let root = find_repo_root().ok_or("No se encontró SddIA/skills")?;
    let skill_dir = root.join(SKILLS_PATH).join(skill_id);
    let spec_md_path = skill_dir.join("spec.md");
    if !spec_md_path.exists() {
        return Err(format!("Skill '{}' no encontrado (no spec.md)", skill_id));
    }
    let content = fs::read_to_string(&spec_md_path).map_err(|e| e.to_string())?;
    let (yaml_str, body) = parse_frontmatter(&content)
        .ok_or_else(|| format!("Skill '{}': spec.md sin frontmatter YAML válido", skill_id))?;
    let value: Value = serde_yaml::from_str(yaml_str).map_err(|e| e.to_string())?;
    Ok((value, body.to_string()))
}

#[derive(Parser)]
#[command(name = "sddia-skills")]
#[command(about = "SddIA Skills Runner. Usa spec.md con frontmatter YAML.", long_about = None)]
struct Cli {
    #[command(subcommand)]
    command: Commands,
}

#[derive(Subcommand)]
enum Commands {
    /// Lista skills (lee existencia de spec.md)
    List,
    /// Muestra contexto completo de un skill (spec.md con frontmatter)
    Show {
        /// ID del skill
        skill_id: String,
    },
    /// Ejecuta un skill (placeholder)
    Run {
        /// ID del skill
        skill_id: String,
    },
}

fn main() {
    let cli = Cli::parse();

    match &cli.command {
        Commands::List => {
            match list_skills() {
                Ok(skills) => {
                    println!("Skills:");
                    for s in skills {
                        println!("  - {}", s);
                    }
                }
                Err(e) => {
                    eprintln!("Error: {}", e);
                    std::process::exit(1);
                }
            }
        }
        Commands::Show { skill_id } => {
            match load_full_context(skill_id) {
                Ok((frontmatter, body)) => {
                    println!("=== Frontmatter (YAML) ===\n{}", serde_json::to_string_pretty(&frontmatter).unwrap_or_default());
                    if !body.is_empty() {
                        println!("\n=== Cuerpo (primeras 20 líneas) ===\n{}", body.lines().take(20).collect::<Vec<_>>().join("\n"));
                    }
                }
                Err(e) => {
                    eprintln!("Error: {}", e);
                    std::process::exit(1);
                }
            }
        }
        Commands::Run { skill_id } => {
            println!("(Simulation) Skill '{}' logic would run here.", skill_id);
        }
    }
}
