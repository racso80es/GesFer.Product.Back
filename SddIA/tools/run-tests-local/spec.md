---
contract_ref: SddIA/tools/tools-contract.md
cumulo_ref: SddIA/agents/cumulo.json
depends_on_tools:
- prepare-full-env
- invoke-mysql-seeds
env:
- Windows 11
- PowerShell 7+
- .NET SDK 8
implementation_path_ref: paths.toolCapsules.run-tests-local
inputs:
  E2EBaseUrl: string (opcional). URL base API para E2E; por defecto http://localhost:5010.
  OnlyTests: boolean (opcional). Solo ejecutar tests (no prepare, no seeds).
  OutputJson: boolean (opcional). Emitir resultado JSON por stdout.
  OutputPath: string (opcional). Fichero donde escribir el resultado JSON.
  SkipPrepare: boolean (opcional). No invocar prepare-full-env.
  SkipSeeds: boolean (opcional). No invocar invoke-mysql-seeds.
  TestScope: string (opcional). unit | integration | e2e | all. Por defecto all.
output:
  data_fields: tests_summary (passed, failed, total por scope o proyecto), duration_ms
  phases_feedback:
  - init
  - prepare
  - seeds
  - build
  - tests
  - done
  - error
  schema_ref: tools-contract.md output.required_fields y optional_fields
toolId: run-tests-local
version: 1.0.0
---

# Especificación: run-tests-local

**toolId:** `run-tests-local`  
**Definición (SddIA):** Este directorio (paths.toolsDefinitionPath/run-tests-local/).  
**Implementación (scripts):** Ruta canónica en Cúmulo → **implementation_path_ref:** `paths.toolCapsules.run-tests-local` (consultar `SddIA/agents/cumulo.json`).

## Objetivo

Ejecutar tests (unitarios, integración, E2E) en condiciones de validación local sin invocar comandos de sistema directamente desde el agente. Orquesta opcionalmente prepare-full-env e invoke-mysql-seeds, compila la solución y ejecuta dotnet test según el alcance (unit | integration | e2e | all). Salida JSON y feedback conforme a tools-contract.

## Entradas

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| SkipPrepare | switch | No invocar prepare-full-env. |
| SkipSeeds | switch | No invocar invoke-mysql-seeds. |
| TestScope | string | unit, integration, e2e, all (por defecto all). |
| OnlyTests | switch | Solo ejecutar tests (no prepare, no seeds). |
| E2EBaseUrl | string | URL base API para E2E (por defecto http://localhost:5010). |
| OutputPath | string | Fichero donde escribir el resultado JSON. |
| OutputJson | switch | Emitir resultado JSON por stdout. |

## Salida

Cumple `SddIA/tools/tools-contract.md`: toolId, exitCode, success, timestamp, message, feedback[], data (tests_summary, duration_ms).

## Fases (feedback)

init → prepare (opcional) → seeds (opcional) → build → tests → done (o error).

## Estado de Implementación

**Formato actual:** Script PowerShell (`.ps1`)  
**Ubicación:** `scripts/tools/run-tests-local/Run-Tests-Local.ps1`

**Migración pendiente a Rust:**
- Estado: Pendiente
- Prioridad: Media
- Notas: Esta herramienta aún no ha sido migrada a Rust. Se mantiene temporalmente el script `.ps1`.

**Formato objetivo:** Ejecutable Rust (`.exe`)  
**Ubicación objetivo:** `scripts/tools/run-tests-local/bin/run_tests_local.exe`

**Estándar futuro:** Una vez migrado, solo existirá el ejecutable `.exe`. No se deberá mantener el `.ps1`.

### Invocación actual (PowerShell)

```powershell
& "scripts/tools/run-tests-local/Run-Tests-Local.ps1" [opciones]
```

## Implementación

La implementación (manifest, config, .bat, .ps1) reside en la carpeta indicada por Cúmulo en **implementation_path_ref**.
