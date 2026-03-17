# start-api

**toolId:** start-api  
**Cápsula:** paths.toolCapsules.start-api (Cúmulo)

## Objetivo

Levanta la API del proyecto GesFer.Admin.Back. Comprueba si el puerto está ocupado; según `--port-blocked` puede fallar o cerrar el proceso que lo usa. Tras build opcional, inicia la API y considera **éxito** solo si el endpoint **health** responde correctamente (HTTP 200).

## Uso (binario Rust)

Desde la raíz del repositorio (o con `GESFER_REPO_ROOT` apuntando a la raíz):

```powershell
# Desde scripts/tools/start-api/ o con PATH
.\start_api.exe

# Puerto ocupado: fallar (por defecto)
.\start_api.exe --port-blocked fail

# Puerto ocupado: cerrar proceso que usa el puerto y continuar
.\start_api.exe --port-blocked kill

# Opciones
.\start_api.exe --no-build
.\start_api.exe --profile Development
.\start_api.exe --port 5010
.\start_api.exe --output-json
.\start_api.exe --output-path result.json
.\start_api.exe --config-path ruta/a/start-api-config.json
```

## Salida

JSON según SddIA/tools/tools-contract.json: toolId, exitCode, success, timestamp, message, feedback[], data (url_base, port, pid, healthy), duration_ms. **success = true** solo si el health responde 200.

### Códigos de error relevantes

| exitCode | Situación |
|----------|-----------|
| 7 | Health no respondió a tiempo |
| 8 | **Base de datos (MySQL) no disponible** — ejecute prepare-full-env e invoke-mysql-seeds antes |

## Implementación

Implementación **obligatoria en Rust** (binario `start_api.exe` en la misma carpeta que el .bat). El launcher .bat invoca el .exe si existe; si no, fallback a Start-Api.ps1.
