---
type: implementation
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
---

# Implementation — run-test-e2e-local

## Entregado

- **`scripts/tools/run-test-e2e-local/Run-Test-E2E-Local.ps1`** — Orquestación completa: opcionalmente `Prepare-FullEnv.bat` e `Invoke-MySqlSeeds.bat`; `GET {Admin}/health` y `{Product}/health` salvo `-SkipApiProbe`; `dotnet build` del proyecto E2ETests; `dotnet test --filter Category=E2E --no-build` con `E2E_BASE_URL` = `ProductApiUrl` y `E2E_INTERNAL_SECRET` (parámetro o `run-test-e2e-local-config.json`). Salida JSON (`-OutputJson` / `-OutputPath`) y `feedback[]` por fases.
- **`Run-Test-E2E-Local.bat`** — Launcher que ejecuta el `.ps1` desde la raíz del repositorio.
- **`manifest.json`** — `implementation_status`: `powershell_transition`; componente `script_ps1` documentado.
- **`run-test-e2e-local.md`** — Uso y parámetros.
- **`SddIA/tools/run-test-e2e-local/spec.md`** — Sección estado de implementación actualizada.

## Contrato

Cumple objetivos de `tools-contract.md`: `toolId`, `exitCode`, `success`, `timestamp`, `message`, `feedback`, `data` (incl. `admin_api_url`, `product_api_url`, `tests_exit_code`).

## Pendiente (Rust)

Migrar a `scripts/tools-rs` y copiar `run_test_e2e_local.exe` a `bin/` según norma de herramientas nuevas.
