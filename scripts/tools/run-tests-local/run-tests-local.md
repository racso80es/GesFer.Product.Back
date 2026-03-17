# run-tests-local

Herramienta SddIA que ejecuta tests (unitarios, integración, E2E) en condiciones de validación local. No invocar `dotnet test` directamente desde el agente; usar esta herramienta (norma commands-via-skills-or-tools).

## Uso

Desde la raíz del repositorio:

```powershell
.\scripts\tools\run-tests-local\Run-Tests-Local.bat
```

Parámetros (vía .ps1 o cuando se invoque la cápsula):

- **SkipPrepare** — No invocar prepare-full-env.
- **SkipSeeds** — No invocar invoke-mysql-seeds.
- **TestScope** — `unit`, `integration`, `e2e`, `all` (por defecto `all`).
- **OnlyTests** — Solo ejecutar tests (no prepare ni seeds).
- **E2EBaseUrl** — URL base de la API para E2E (por defecto http://localhost:5010).
- **OutputPath** — Fichero donde escribir el resultado JSON.
- **OutputJson** — Emitir resultado JSON por stdout.

## Salida

Cumple SddIA/tools/tools-contract.json: JSON con toolId, exitCode, success, timestamp, message, feedback[], data (tests_summary, duration_ms).

## Definición

paths.toolsDefinitionPath/run-tests-local/ (spec.md, spec.json). Implementación: paths.toolCapsules.run-tests-local (Cúmulo).
