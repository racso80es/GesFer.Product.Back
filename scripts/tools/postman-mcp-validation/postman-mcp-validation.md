# postman-mcp-validation

Herramienta SddIA de **seguridad externa** que valida los **endpoints** del proyecto ejecutando la colección Postman mediante Newman. No invocar `newman` ni comandos de red directamente desde el agente; usar esta herramienta (norma commands-via-skills-or-tools).

## Prerequisitos

- **Node.js/npm** instalado.
- **Newman** disponible: `npm install -g newman` o uso vía `npx newman` (la cápsula intenta ambos).
- Opcional: variable de entorno **POSTMAN_INTERNAL_SECRET** para el secreto interno (si no se pasa por parámetro ni en config).

## Uso

Desde la raíz del repositorio:

```powershell
.\scripts\tools\postman-mcp-validation\Postman-Mcp-Validation.bat
```

O con PowerShell:

```powershell
.\scripts\tools\postman-mcp-validation\Postman-Mcp-Validation.ps1 -OutputJson
```

## Parámetros

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| **CollectionPath** | string | Ruta al JSON de la colección (por defecto: docs/postman/GesFer.Admin.Back.API.postman_collection.json). |
| **BaseUrl** | string | URL base de la API (por defecto: http://localhost:5010). |
| **InternalSecret** | string | Secreto para X-Internal-Secret (por defecto desde config o env POSTMAN_INTERNAL_SECRET). |
| **EnvironmentPath** | string | (Opcional) Ruta a fichero de entorno Postman .json. |
| **OutputPath** | string | Fichero donde escribir el resultado JSON. |
| **OutputJson** | switch | Emitir resultado JSON por stdout. |

## Salida

Cumple **SddIA/tools/tools-contract.json**: JSON con toolId, exitCode, success, timestamp, message, feedback[] (fases init, newman, done/error), data.run_summary (executed, passed, failed), data.duration_ms.

## Definición

- **Definición (SddIA):** paths.toolsDefinitionPath/postman-mcp-validation/ (spec.md, spec.json).
- **Implementación:** paths.toolCapsules.postman-mcp-validation (Cúmulo).

## Diseño MCP-ready

Para futura integración con Postman MCP: nombre de capacidad sugerido `run_endpoint_validation`; argumentos y formato de resultado documentados en SddIA/tools/postman-mcp-validation/spec.md.
