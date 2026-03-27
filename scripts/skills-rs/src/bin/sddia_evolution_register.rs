use chrono::Utc;
use clap::Parser;
use serde::{Deserialize, Serialize};
use sha2::{Digest, Sha256};
use std::fs::{self, OpenOptions};
use std::io::{Read, Write};
use std::path::{Path, PathBuf};
use uuid::Uuid;

#[derive(Serialize, Deserialize, Debug)]
#[serde(rename_all = "camelCase")]
struct RegisterRequest {
    autor: String,
    descripcion_breve: String,
    tipo_operacion: String,
    contexto: Option<String>,
    proyecto_origen_cambio: Option<String>,
    impacto: Option<String>,
    #[serde(default)]
    cambios_realizados: Vec<CambioRealizado>,
}

#[derive(Serialize, Deserialize, Debug)]
struct CambioRealizado {
    anterior: Option<String>,
    nuevo: Option<String>,
}

#[derive(Parser, Debug)]
#[command(author, version, about, long_about = None)]
struct Args {
    #[arg(short, long)]
    input: String,
}

fn get_repo_root() -> PathBuf {
    // If CARGO_MANIFEST_DIR is set (e.g., during `cargo run`), use it.
    // Otherwise, assume the binary is being run from the repo root or another known good location,
    // and rely on the current directory.
    if let Ok(manifest_dir) = std::env::var("CARGO_MANIFEST_DIR") {
        Path::new(&manifest_dir)
            .parent()
            .and_then(|p| p.parent())
            .unwrap_or_else(|| Path::new("."))
            .to_path_buf()
    } else {
        std::env::current_dir().unwrap_or_else(|_| PathBuf::from("."))
    }
}

fn main() -> Result<(), Box<dyn std::error::Error>> {
    let args = Args::parse();

    // Read request
    let mut input_content = String::new();
    if args.input == "-" {
        std::io::stdin().read_to_string(&mut input_content)?;
    } else {
        input_content = fs::read_to_string(&args.input)?;
    }

    let req: RegisterRequest = serde_json::from_str(&input_content)?;

    let id_cambio = Uuid::new_v4().to_string();
    let fecha = Utc::now().to_rfc3339();

    // 1. Build YAML Frontmatter
    let frontmatter = format!(
        "document_type: evolution_record\n\
         contrato_version: \"1.1\"\n\
         id_cambio: \"{}\"\n\
         fecha: \"{}\"\n\
         autor: \"{}\"\n\
         proyecto_origen_cambio: \"{}\"\n\
         contexto: \"{}\"\n\
         descripcion_breve: \"{}\"\n\
         tipo_operacion: \"{}\"\n\
         cambios_realizados: []\n\
         impacto: \"{}\"\n\
         replicacion:\n  \
         instrucciones: \"N/A\"\n  \
         hash_integrity: \"SHA-256-PENDIENTE\"\n",
        id_cambio,
        fecha,
        req.autor,
         req.proyecto_origen_cambio.clone().unwrap_or_else(|| "GesFer".to_string()),
         req.contexto.clone().unwrap_or_else(|| "Actualización".to_string()),
        req.descripcion_breve,
        req.tipo_operacion,
         req.impacto.clone().unwrap_or_else(|| "Bajo".to_string())
    );

    let mut hasher = Sha256::new();
    hasher.update(frontmatter.as_bytes());
    let hash_result = format!("{:x}", hasher.finalize());

    let final_frontmatter = frontmatter.replace("SHA-256-PENDIENTE", &hash_result);

    // 2. Write to SddIA/evolution/{id}.md
    let repo_root = get_repo_root();
    let evo_dir = repo_root.join("SddIA/evolution");
    fs::create_dir_all(&evo_dir)?;

    let record_path = evo_dir.join(format!("{}.md", id_cambio));
    let mut file = fs::File::create(&record_path)?;
    write!(file, "---\n{}---\n\n# {}\n\n{}", final_frontmatter, req.descripcion_breve, req.contexto.unwrap_or_default())?;

    // 3. Update Evolution_log.md
    let log_path = evo_dir.join("Evolution_log.md");
    if let Ok(mut log_file) = OpenOptions::new().append(true).open(&log_path) {
        writeln!(log_file, "| {} | {} | {} |", id_cambio, fecha, req.descripcion_breve)?;
    }

    println!("Registro de evolución creado: {}", record_path.display());
    println!("{}", serde_json::json!({ "status": "ok", "id": id_cambio }));

    Ok(())
}
