---
contract_ref: SddIA/tools/tools-contract.md
cumulo_ref: SddIA/agents/cumulo.json
depends_on_tools: []
env:
- Windows 11
- PowerShell 7+
- Node.js/npm (Newman) donde se ejecute la cápsula
implementation_path_ref: paths.toolCapsules.postman-mcp-validation
inputs:
  BaseUrl: string (opcional). URL base API; por defecto http://localhost:5010.
  CollectionPath: string (opcional). Ruta al JSON de la colección. Por defecto docs/postman/GesFer.Admin.Back.API.postman_collection.json.
  EnvironmentPath: string (opcional). Ruta a fichero de entorno Postman .json.
  InternalSecret: string (opcional). Secreto para X-Internal-Secret; por defecto desde config o variable.
  OutputJson: boolean (opcional). Emitir resultado JSON por stdout.
  OutputPath: string (opcional). Fichero donde escribir el resultado JSON.
mcp_ready:
  description: Diseño para futura exposición como capacidad MCP.
  result_format: Mismo que salida JSON de esta tool (tools-contract).
  suggested_args:
  - collection_path
  - base_url
  - internal_secret
  - environment_path
  suggested_tool_name: run_endpoint_validation
output:
  data_fields: run_summary (executed, passed, failed, assertions si aplica), duration_ms
  phases_feedback:
  - init
  - newman
  - done
  - error
  schema_ref: tools-contract.md output.required_fields y optional_fields
toolId: postman-mcp-validation
version: 1.0.0
---

# Especificación: postman-mcp-validation

**toolId:** `postman-mcp-validation`  
**Definición (SddIA):** Este directorio (paths.toolsDefinitionPath/postman-mcp-validation/).  
**Implementación (scripts):** Ruta canónica en Cúmulo → **implementation_path_ref:** `paths.toolCapsules.postman-mcp-validation` (consultar `SddIA/agents/cumulo.paths.json`).

## Objetivo

Herramienta de **seguridad externa** que realiza **validaciones sobre los endpoints** del proyecto (GesFer.Admin.Back) ejecutando la colección Postman existente mediante Newman (CLI). Salida JSON y feedback conforme a tools-contract. Diseño **MCP-ready** para futura interacción con Postman MCP (Model Context Protocol).

## Entradas

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| CollectionPath | string | Ruta al JSON de la colección Postman. Por defecto: docs/postman/GesFer.Admin.Back.API.postman_collection.json (relativa al repo). |
| BaseUrl | string | URL base de la API (p. ej. http://localhost:5010). Sustituye la variable {{baseUrl}} de la colección. |
| InternalSecret | string | Secreto interno para endpoints que usan X-Internal-Secret. Sustituye {{internalSecret}}. |
| EnvironmentPath | string | (Opcional) Ruta a fichero de entorno Postman (.json). Si no se indica, se usan variables inline. |
| OutputPath | string | (Opcional) Fichero donde escribir el resultado JSON. |
| OutputJson | switch | (Opcional) Emitir resultado JSON por stdout. |

## Salida

Cumple `SddIA/tools/tools-contract.md`: toolId, exitCode, success, timestamp, message, feedback[], data (run_summary con executed/passed/failed, assertions si aplica, duration_ms).

## Fases (feedback)

init → newman (ejecución de la colección) → done (o error).

## Estado de Implementación

**Formato actual:** Script PowerShell (`.ps1`)  
**Ubicación:** `scripts/tools/postman-mcp-validation/Postman-Mcp-Validation.ps1`

**Migración pendiente a Rust:**
- Estado: Pendiente
- Prioridad: Media
- Notas: Esta herramienta aún no ha sido migrada a Rust. Se mantiene temporalmente el script `.ps1`.

**Formato objetivo:** Ejecutable Rust (`.exe`)  
**Ubicación objetivo:** `scripts/tools/postman-mcp-validation/bin/postman_mcp_validation.exe`

**Estándar futuro:** Una vez migrado, solo existirá el ejecutable `.exe`. No se deberá mantener el `.ps1`.

### Invocación actual (PowerShell)

```powershell
& "scripts/tools/postman-mcp-validation/Postman-Mcp-Validation.ps1" [opciones]
```

## Implementación

La implementación (manifest, config, .bat, .ps1) reside en la carpeta indicada por Cúmulo en **implementation_path_ref**.

## Diseño MCP-ready (futura integración con Postman MCP)

Para una fase posterior en la que esta validación se exponga como capacidad invocable por agentes vía MCP:

- **Nombre de capacidad sugerido (MCP Tool):** `run_endpoint_validation` o `postman_validate_endpoints`.
- **Argumentos sugeridos:** `collection_path` (string), `base_url` (string), `internal_secret` (string, opcional), `environment_path` (string, opcional).
- **Formato de resultado:** El mismo que la salida JSON de esta tool (toolId, exitCode, success, timestamp, message, feedback, data con run_summary y duration_ms). Así un servidor MCP puede delegar en la ejecución de esta herramienta y devolver el mismo contrato a los clientes MCP.

Referencia: Postman MCP (Model Context Protocol) — Tools, Resources, Prompts; transporte STDIO o HTTP.
