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
implementation_path_ref: paths.toolCapsules.start-api
inputs:
  ConfigPath: string (opcional). Ruta al JSON de configuración; por defecto en implementación.
  NoBuild: boolean (opcional). No compilar; solo ejecutar si ya hay build.
  OutputJson: boolean (opcional). Emitir resultado JSON por stdout.
  OutputPath: string (opcional). Fichero donde escribir el resultado JSON.
  Port: number (opcional). Puerto del host (override).
  PortBlocked: 'fail | kill (opcional). Si el puerto está ocupado: fail = error y salir; kill = cerrar proceso que usa el puerto y continuar. Por defecto: fail.'
  Profile: string (opcional). Perfil de ejecución (ej. Development). Por defecto según config.
output:
  exit_codes:
    '0': 'Éxito: health responde 200'
    '1': Config no encontrado o inválido
    '2': Puerto ocupado (PortBlocked=fail)
    '3': 'Puerto ocupado: no se pudo liberar o sigue ocupado'
    '4': Directorio API no encontrado
    '5': Build fallido
    '6': Error al lanzar dotnet run
    '7': Health no respondió a tiempo
    '8': Base de datos no disponible (MySQL). Ejecutar prepare-full-env e invoke-mysql-seeds antes.
  phases_feedback:
  - init
  - port-check
  - port-kill
  - build
  - launch
  - healthcheck
  - done
  - error
  schema_ref: tools-contract.md output.required_fields y optional_fields
  success_criterion: El endpoint health responde adecuadamente (HTTP 200).
port_check: Validar si el puerto está ocupado antes de arrancar; comportamiento según PortBlocked.
toolId: start-api
version: 1.0.0
---

# Especificación: start-api

**toolId:** `start-api`  
**Definición (SddIA):** Este directorio.  
**Implementación (scripts):** Ruta canónica en Cúmulo → **implementation_path_ref:** `paths.toolCapsules.start-api` (consultar `SddIA/agents/cumulo.json`). La raíz del path de implementación la indica Cúmulo.

## Objetivo

Herramienta que **levanta la API** del proyecto (GesFer.Admin.Back): compila si es necesario, inicia el host (dotnet run / Kestrel) y opcionalmente comprueba salud. Completa el ciclo de vida junto con **prepare-full-env** (infraestructura) e **invoke-mysql-seeds** (datos), permitiendo la gestión de la solución por terceros (agentes, CI/CD, orquestadores).

## Entradas

| Parámetro     | Tipo   | Descripción |
|---------------|--------|-------------|
| NoBuild       | switch | No compilar; solo ejecutar si ya hay build. |
| Profile       | string | Perfil de ejecución (ej. Development). Por defecto según config. |
| Port          | number | Puerto del host (override). |
| PortBlocked   | enum   | Comportamiento si el puerto está ocupado: **fail** (valor por defecto) — fallar con error; **kill** — intentar cerrar el proceso que usa el puerto y continuar. |
| ConfigPath    | string | Ruta al JSON de configuración (por defecto en la implementación). |
| OutputPath    | string | Fichero donde escribir el resultado JSON (contrato). |
| OutputJson    | switch | Emitir el resultado JSON por stdout. |

## Validación de éxito

La herramienta considera la ejecución **correcta** si y solo si el endpoint **health** responde adecuadamente (HTTP 200 en la URL configurada, p. ej. `http://localhost:<port>/health`). Si el healthcheck no responde en el tiempo configurado, la herramienta devuelve error.

## Puerto ocupado

Antes de levantar la API se comprueba si el puerto está en uso. Si está ocupado:

- **PortBlocked=fail:** se emite error y se termina con exitCode distinto de 0.
- **PortBlocked=kill:** se intenta identificar y cerrar el proceso que usa el puerto (en Windows: netstat + taskkill); tras liberar el puerto se continúa con el arranque.

## Códigos de salida (exitCode)

| exitCode | Situación | Acción recomendada |
|----------|-----------|---------------------|
| 0 | Éxito: health responde 200 | — |
| 1 | Config no encontrado o inválido | Verificar ruta de start-api-config.json |
| 2 | Puerto ocupado (PortBlocked=fail) | Usar --port-blocked kill o cambiar puerto |
| 3 | Puerto ocupado: no se pudo liberar o sigue ocupado | Liberar puerto manualmente |
| 4 | Directorio API no encontrado | Verificar apiWorkingDir en config |
| 5 | Build fallido | Revisar compilación dotnet |
| 6 | Error al lanzar dotnet run | Verificar .NET SDK instalado |
| 7 | Health no respondió a tiempo | Revisar logs de la API; puede haber otro problema |
| 8 | **Base de datos no disponible (MySQL)** | Ejecutar **prepare-full-env** e **invoke-mysql-seeds** antes de start-api |

La herramienta detecta en la salida de la API errores de conexión a MySQL (p. ej. `Unable to connect to any of the specified MySQL hosts`, `MySqlConnector.MySqlException`) y devuelve exitCode 8 con mensaje explícito y sugerencia de dependencias.

## Salida

Cumple `SddIA/tools/tools-contract.md`: objeto JSON con toolId, exitCode, success, timestamp, message, feedback[], data (url_base, pid, port, healthy), duration_ms.

## Fases (feedback)

init → port-check → [port-kill si aplica] → build (opcional) → launch → healthcheck → done | error.

## Dependencias lógicas

Recomendado tener ejecutadas antes **prepare-full-env** e **invoke-mysql-seeds** (infra y datos listos). No bloqueante si el invocador ya las ejecutó.

## Implementación

**Formato:** Ejecutable Rust (`.exe`)  
**Ubicación:** `scripts/tools/start-api/start_api.exe`  
**Fuente Rust:** `scripts/tools-rs/src/start_api.rs`

**Estándar:** Solo se generan ejecutables `.exe`. No se deben crear archivos `.ps1`.

### Invocación

```powershell
# Invocación directa
& "scripts/tools/start-api/start_api.exe" [opciones]

# Opciones disponibles
--no-build              # No compilar; solo ejecutar si ya hay build
--profile <perfil>      # Perfil de ejecución (ej. Development)
--port <número>         # Puerto del host (override)
--port-blocked <acción> # Comportamiento si puerto ocupado: fail | kill
--config-path <path>    # Ruta al JSON de configuración
--output-path <path>    # Fichero donde escribir resultado JSON
--output-json           # Emitir resultado JSON por stdout
```

**Implementación (scripts):** Ruta canónica en Cúmulo → **implementation_path_ref:** `paths.toolCapsules.start-api` (consultar `SddIA/agents/cumulo.json`). La raíz del path de implementación la indica Cúmulo.
