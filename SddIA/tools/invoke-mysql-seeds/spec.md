---
contract_ref: SddIA/tools/tools-contract.md
cumulo_ref: SddIA/agents/cumulo.json
env:
- Windows 11
- PowerShell 7+
- Docker
- .NET SDK 8
implementation_path_ref: paths.toolCapsules.invoke-mysql-seeds
inputs:
  ConfigPath: string (opcional). Ruta al JSON de configuración; por defecto en implementación.
  OutputJson: boolean (opcional). Emitir resultado JSON por stdout.
  OutputPath: string (opcional). Fichero donde escribir el resultado JSON.
  SkipMigrations: boolean (opcional). No ejecutar dotnet ef database update.
  SkipSeeds: boolean (opcional). Solo migraciones, no seeds.
output:
  phases_feedback:
  - init
  - mysql
  - migrations
  - seeds
  - done
  - error
  schema_ref: tools-contract.md output.required_fields y optional_fields
toolId: invoke-mysql-seeds
version: 1.0.0
---

# Especificación: invoke-mysql-seeds

**toolId:** `invoke-mysql-seeds`  
**Definición (SddIA):** Este directorio.  
**Implementación (scripts):** Ruta canónica en Cúmulo → **implementation_path_ref:** `paths.toolCapsules.invoke-mysql-seeds` (consultar `SddIA/agents/cumulo.json`). La raíz del path de implementación la indica Cúmulo.

## Objetivo

Herramienta que comprueba la disponibilidad de MySQL, aplica migraciones EF Core y ejecuta los seeds de Admin (companies, admin-users) mediante la variable de entorno RUN_SEEDS_ONLY=1 en la API.

## Entradas

| Parámetro | Tipo | Descripción |
|----------|------|-------------|
| SkipMigrations | switch | No ejecutar `dotnet ef database update`; solo seeds. |
| SkipSeeds | switch | Solo ejecutar migraciones; no ejecutar seeds. |
| ConfigPath | string | Ruta al JSON de configuración (por defecto en la implementación). |
| OutputPath | string | Fichero donde escribir el resultado JSON (contrato). |
| OutputJson | switch | Emitir el resultado JSON por stdout. |

## Salida

Cumple `SddIA/tools/tools-contract.md`: objeto JSON con toolId, exitCode, success, timestamp, message, feedback[], data (mysql, migrations, seeds), duration_ms.

## Fases (feedback)

init → mysql → migrations → seeds → done (o error).

## Implementación

**Formato:** Ejecutable Rust (`.exe`)  
**Ubicación:** `scripts/tools/invoke-mysql-seeds/bin/invoke_mysql_seeds.exe`  
**Fuente Rust:** `scripts/tools-rs/src/invoke_mysql_seeds.rs`

**Estándar:** Solo se generan ejecutables `.exe`. No se deben crear archivos `.ps1`.

### Invocación

```powershell
# Invocación directa
& "scripts/tools/invoke-mysql-seeds/bin/invoke_mysql_seeds.exe" [opciones]

# Opciones disponibles
--skip-migrations       # No ejecutar migraciones EF Core
--skip-seeds           # No ejecutar seeds
--config-path <path>   # Ruta al JSON de configuración
--output-path <path>   # Fichero donde escribir resultado JSON
--output-json          # Emitir resultado JSON por stdout
```

**Implementación (scripts):** Ruta canónica en Cúmulo → **implementation_path_ref:** `paths.toolCapsules.invoke-mysql-seeds` (consultar `SddIA/agents/cumulo.json`). La raíz del path de implementación la indica Cúmulo.
