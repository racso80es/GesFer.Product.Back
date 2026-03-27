use clap::Parser;
use serde_json::json;
use std::path::{Path, PathBuf};
use std::process::{exit, Command};

#[derive(Parser, Debug)]
#[command(author, version, about, long_about = None)]
struct Args {
    /// Base branch or commit ref
    #[arg(long)]
    base: String,

    /// Head branch or commit ref
    #[arg(long, default_value = "HEAD")]
    head: String,

    /// Output JSON to stdout
    #[arg(long)]
    output_json: bool,
}

fn main() {
    let args = Args::parse();

    // Check if there are changes in SddIA/ (excluding SddIA/evolution/)
    let git_diff = Command::new("git")
        .args(["diff", "--name-only", &format!("{}...{}", args.base, args.head)])
        .output();

    let diff_output = match git_diff {
        Ok(o) if o.status.success() => String::from_utf8_lossy(&o.stdout).to_string(),
        _ => {
            eprintln!("Failed to run git diff. Make sure you are in a git repository.");
            exit(1);
        }
    };

    let sddia_changed = diff_output.lines().any(|l| l.starts_with("SddIA/") && !l.starts_with("SddIA/evolution/"));

    if !sddia_changed {
        if args.output_json {
            println!("{}", json!({ "status": "ok", "message": "No functional SddIA changes detected." }));
        } else {
            println!("No functional SddIA changes detected.");
        }
        exit(0);
    }

    // Check if there is an evolution record created in the same PR
    let new_evolution = diff_output.lines().any(|l| l.starts_with("SddIA/evolution/") && l.ends_with(".md") && !l.ends_with("Evolution_log.md") && !l.ends_with("evolution_contract.md"));

    if new_evolution {
        if args.output_json {
            println!("{}", json!({ "status": "ok", "message": "SddIA changes detected and evolution record found." }));
        } else {
            println!("SddIA changes detected and evolution record found.");
        }
        exit(0);
    } else {
        if args.output_json {
            println!("{}", json!({ "status": "error", "message": "SddIA changes detected but NO evolution record found. Please register the change with sddia_evolution_register." }));
        } else {
            eprintln!("SddIA changes detected but NO evolution record found. Please register the change with sddia_evolution_register.");
        }
        exit(1);
    }
}
