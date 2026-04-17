---
type: plan
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
---

# Plan — run-test-e2e-local

## Alcance

1. Implementar `Run-Test-E2E-Local.ps1` en `paths.toolCapsules.run-test-e2e-local` con el flujo acordado en **clarify**: init → prepare → seeds → probe → build → tests E2E.
2. Actualizar `Run-Test-E2E-Local.bat` para invocar el script desde la raíz del repo.
3. Ajustar `manifest.json`, documentación de cápsula y spec SddIA (estado transición PowerShell).
4. Documentar **implementation**, **execution**, **validacion** y cierre de tarea.
5. Rama `feat/create-tool-run-test-e2e-local`, commit(s), PR vía skill **finalizar-git** (`Push-And-CreatePR.ps1`).

## Fuera de alcance

- Binario Rust (`run_test_e2e_local.exe`): fase posterior.
- Arranque automático de APIs Product/Admin desde la tool (v1).

## Riesgos / mitigación

- **APIs no levantadas:** probe falla con error explícito; `-SkipApiProbe` solo para escenarios controlados.
- **iniciar-rama .exe ausente:** creación de rama con `git checkout -b` explícito (misma intención que la skill).
