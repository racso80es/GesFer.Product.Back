---
type: execution
status: completed
feature_id: create-tool-run-test-e2e-local
process_id: create-tool
---

# Execution — run-test-e2e-local

## Comandos ejecutados (validación)

- `Run-Test-E2E-Local.ps1 -OnlyTests -SkipApiProbe -ProductApiUrl http://localhost:59999` — Build OK; tests E2E omitidos (API inexistente); **exitCode 0** (coherente con xUnit omitidos).

## Próxima ejecución recomendada (local completo)

Con Docker/MySQL/seeds y APIs Admin (5010) + Product (5020) levantadas:

```powershell
.\scripts\tools\run-test-e2e-local\Run-Test-E2E-Local.ps1 -OnlyTests -AdminApiUrl "http://localhost:5010" -ProductApiUrl "http://localhost:5020"
```
