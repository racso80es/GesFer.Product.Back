# GesFer Tools (Rust)

Implementación por defecto en Rust de las herramientas del contrato `SddIA/tools/tools-contract.json`.

## Requisitos

- [Rust](https://www.rust-lang.org/) (rustup, cargo) instalado.
- **Windows (target msvc):** [Visual Studio Build Tools](https://visualstudio.microsoft.com/visual-cpp-build-tools/) con la carga de trabajo **Desktop development with C++** (para `link.exe`). Si falta, la compilación falla con `linker 'link.exe' not found`.

## Compilar

Desde esta carpeta o desde la raíz del repo:

```powershell
cd scripts/tools-rs
cargo build --release
```

El script `install.ps1` compila y **copia los ejecutables a la carpeta `bin/` de cada cápsula** (rutas canónicas: Cúmulo **paths.toolCapsules**, `SddIA/agents/cumulo.json`):

- **paths.toolCapsules['prepare-full-env']** + `prepare_full_env.exe` — Prepare-FullEnv (Docker, MySQL)
- **paths.toolCapsules['invoke-mysql-seeds']** + `invoke_mysql_seeds.exe` — Invoke-MySqlSeeds (migraciones EF, seeds)
- **paths.toolCapsules['run-tests-local']** + `run_tests_local.exe` — Tests locales (unit/integration/e2e)
- **paths.toolCapsules['run-test-e2e-local']** + `run_test_e2e_local.exe` — Solo E2E Product (`GesFer.Product.Back.E2ETests`, URLs Admin/Product)

## Uso

Los launchers `.bat` en **paths.toolsPath** (Cúmulo) son wrappers que delegan a la cápsula correspondiente. Dentro de cada cápsula (**paths.toolCapsules[&lt;tool-id&gt;]**), el `.bat` invoca el `.exe` en `bin/` si existe; si no, usa el script PowerShell de la cápsula.

Variables de entorno opcionales:

- `TOOLS_OUTPUT_JSON=1` — Emite el resultado JSON por stdout.
- `GESFER_REPO_ROOT` — Raíz del repositorio (para invoke_mysql_seeds; por defecto el directorio actual).

Argumentos:

- `--output-path <ruta>` — Escribe el JSON de resultado en el fichero indicado.

## Estructura

- `src/lib.rs` — Tipos del contrato (`ToolResult`, `FeedbackEntry`, `to_contract_json`).
- `src/bin/prepare_full_env.rs` — Herramienta prepare-full-env.
- `src/bin/invoke_mysql_seeds.rs` — Herramienta mysql-seeds.
- `src/bin/run_test_e2e_local.rs` — Herramienta run-test-e2e-local.

Referencia: `SddIA/tools/tools-contract.json`, `SddIA/agents/security-engineer.json`.
