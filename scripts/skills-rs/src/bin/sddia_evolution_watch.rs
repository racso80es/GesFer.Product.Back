use clap::Parser;
use notify::{Config, Event, RecommendedWatcher, RecursiveMode, Watcher};
use std::path::Path;
use std::sync::mpsc::channel;
use std::time::Duration;

#[derive(Parser, Debug)]
#[command(author, version, about, long_about = None)]
struct Args {
    #[arg(short, long, default_value = "SddIA")]
    path: String,
}

fn main() -> notify::Result<()> {
    let args = Args::parse();
    let path_to_watch = Path::new(&args.path);

    println!("Watching {:?} for SddIA evolution sync...", path_to_watch);

    let (tx, rx) = channel();

    let mut watcher = RecommendedWatcher::new(
        tx,
        Config::default().with_poll_interval(Duration::from_secs(2)),
    )?;

    watcher.watch(path_to_watch, RecursiveMode::Recursive)?;

    for res in rx {
        match res {
            Ok(Event { paths, .. }) => {
                for path in paths {
                    if path.starts_with(Path::new("SddIA/evolution")) {
                        continue; // ignore changes in the evolution folder itself to prevent loop
                    }
                    eprintln!("Cambio detectado en {:?} → ejecutar sddia_evolution_register", path);
                    // Just log for now. Register should be invoked manually or via agents for full context.
                }
            }
            Err(e) => println!("watch error: {:?}", e),
        }
    }

    Ok(())
}
