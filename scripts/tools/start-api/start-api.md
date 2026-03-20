# start-api

**toolId:** start-api  
**Cápsula:** paths.toolCapsules.start-api (Cúmulo)

## Objetivo

Levanta la API del proyecto. Comprueba si el puerto está ocupado; según **`portBlocked` en `start-api-config.json`** puede fallar liberar el proceso que lo usa. Tras build opcional, inicia la API y considera **éxito** solo si el endpoint **health** responde correctamente (HTTP 200).

## Configuración (obligatoria)

Todos los parámetros de arranque salen de **`start-api-config.json`** (o la ruta indicada con `--config-path`). No hay valores por defecto en el binario: si falta un campo obligatorio o el JSON es inválido, **exitCode 1**.

| Campo (camelCase) | Descripción |
|-------------------|-------------|
| `apiWorkingDir` | Ruta relativa al repo hacia el proyecto de la API (ej. `src/Api`). |
| `defaultProfile` | Valor de `ASPNETCORE_ENVIRONMENT` (ej. `Development`). |
| `defaultPort` | Puerto HTTP en `127.0.0.1`. |
| `healthUrl` | URL completa del healthcheck (debe coincidir con el puerto). |
| `healthCheckTimeoutSeconds` | Tiempo máximo de espera al health (segundos). |
| `portBlocked` | `fail` — error si el puerto está ocupado; `kill` — intentar liberar el puerto (Windows). |
| `dotnetConfiguration` | Configuración de `dotnet build` / `dotnet run` (ej. `Release`). |

Otros campos del JSON (`version`, `description`, `command`, …) se ignoran si están presentes.

## Uso (binario Rust)

Desde la raíz del repositorio (o con `GESFER_REPO_ROOT` apuntando a la raíz):

```powershell
# Usa scripts/tools/start-api/start-api-config.json por defecto
.\start_api.exe

.\start_api.exe --no-build
.\start_api.exe --output-json
.\start_api.exe --output-path result.json
.\start_api.exe --config-path ruta/a/start-api-config.json
```

Para cambiar puerto, perfil o comportamiento ante puerto ocupado, **edita el JSON** (no hay `--port` ni `--profile` ni `--port-blocked` en CLI).

## Salida

JSON según SddIA/tools/tools-contract.json: toolId, exitCode, success, timestamp, message, feedback[], data (url_base, port, pid, healthy), duration_ms. **success = true** solo si el health responde 200.

### Códigos de error relevantes

| exitCode | Situación |
|----------|-----------|
| 7 | Health no respondió a tiempo |
| 8 | **Base de datos (MySQL) no disponible** — ejecute prepare-full-env e invoke-mysql-seeds antes |

## Implementación

Implementación **obligatoria en Rust** (binario `start_api.exe` en la misma carpeta que el .bat). El launcher .bat invoca el .exe si existe; si no, fallback a Start-Api.ps1.
