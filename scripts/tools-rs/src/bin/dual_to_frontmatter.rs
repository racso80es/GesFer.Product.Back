//! Herramienta dual_to_frontmatter: convierte spec.json + spec.md → .md con frontmatter YAML.
//! Uso: dual_to_frontmatter --input-dir <ruta> [--output-path <ruta>]
//! Si --output-path no se indica, escribe en el mismo directorio (sobrescribe spec.md).

use std::fs;
use std::path::Path;

use anyhow::{Context, Result};
use clap::Parser;
use serde_json::Value;

#[derive(Parser)]
#[command(name = "dual_to_frontmatter")]
struct Args {
    /// Directorio que contiene spec.json y spec.md
    #[arg(long)]
    input_dir: String,

    /// Ruta de salida del .md con frontmatter. Por defecto: input_dir/spec.md
    #[arg(long)]
    output_path: Option<String>,
}

fn json_to_yaml(value: &Value) -> Result<String> {
    serde_yaml::to_string(value).context("Convertir JSON a YAML")
}

fn main() -> Result<()> {
    let args = Args::parse();
    let input_dir = Path::new(&args.input_dir);

    let spec_json_path = input_dir.join("spec.json");
    let spec_md_path = input_dir.join("spec.md");

    if !spec_json_path.exists() {
        anyhow::bail!("No existe spec.json en {}", input_dir.display());
    }
    if !spec_md_path.exists() {
        anyhow::bail!("No existe spec.md en {}", input_dir.display());
    }

    let json_content = fs::read_to_string(&spec_json_path)
        .context("Leer spec.json")?;
    let json_value: Value = serde_json::from_str(&json_content)
        .context("Parsear spec.json")?;

    let md_content = fs::read_to_string(&spec_md_path)
        .context("Leer spec.md")?;

    let yaml_frontmatter = json_to_yaml(&json_value)?;
    let output = format!("---\n{}\n---\n\n{}", yaml_frontmatter.trim_end(), md_content.trim_start());

    let output_path = args
        .output_path
        .map(|p| p.into())
        .unwrap_or_else(|| input_dir.join("spec.md"));

    fs::write(&output_path, output)
        .context("Escribir archivo de salida")?;

    println!("Convertido: {} -> {}", input_dir.display(), output_path.display());
    Ok(())
}
