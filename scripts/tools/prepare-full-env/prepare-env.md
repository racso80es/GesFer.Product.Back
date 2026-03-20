# Prepare-FullEnv — Preparar entorno completo

Herramienta para dejar listo todo el entorno de desarrollo: Docker (MySQL, Memcached, Adminer) y opcionalmente la API Admin y clientes indicados.

## Requisitos

- **Windows 11** con **PowerShell 7+**.
- **Docker Desktop** instalado y en ejecución.
- **.NET SDK** (para levantar la API en local).
- Opcional: **Node/npm** si se configuran clientes en `prepare-env.json`.

## Uso

### Ejecutable (recomendado)

Desde la **raíz del repositorio** o mediante el launcher en **paths.toolsPath** (Cúmulo):

```powershell
.\scripts\tools\prepare-full-env\Prepare-FullEnv.bat
```

o

```powershell
.\scripts\tools\Prepare-FullEnv.bat
```

El `.bat` usa `prepare_full_env.exe` dentro de la cápsula si existe (build Rust + `scripts/tools-rs/install.ps1`); si no, ejecuta el script PowerShell.

### PowerShell directo

Desde la raíz del repo:

```powershell
& .\scripts\tools\prepare-full-env\Prepare-FullEnv.ps1
```

Parámetros opcionales:

| Parámetro        | Descripción                                      |
|------------------|--------------------------------------------------|
| `-DockerOnly`    | Solo levanta Docker (DB, cache, Adminer).        |
| `-StartApi`      | Además levanta la Admin API en local.            |
| `-NoDocker`      | No levanta Docker (solo API/clientes si se pide). |
| `-ConfigPath`    | Ruta al JSON de configuración.                   |
| `-OutputPath`    | Fichero donde escribir el resultado JSON (contrato tools). |
| `-OutputJson`    | Emitir el resultado JSON por stdout al finalizar. |

Ejemplos:

```powershell
.\scripts\tools\prepare-full-env\Prepare-FullEnv.ps1 -DockerOnly
.\scripts\tools\prepare-full-env\Prepare-FullEnv.ps1 -StartApi
```

## Configuración: `prepare-env.json`

Ubicación: en esta cápsula, `prepare-env.json`. Ruta canónica (Cúmulo): **paths.toolCapsules['prepare-full-env']** (`SddIA/agents/cumulo.json`).

| Campo                  | Descripción |
|------------------------|-------------|
| `dockerComposePath`    | Ruta al `docker-compose.yml` respecto a la raíz del repo. |
| `mysqlContainerName`   | Nombre del contenedor MySQL para el healthcheck. |
| `dockerServices`       | Lista de servicios a levantar con `docker-compose up -d`. |
| `startApi.enabled`     | Si se debe levantar la API en local. |
| `startApi.workingDir`  | Directorio del proyecto API (ej. `src/Api`). |
| `startApi.command`     | Comando (ej. `dotnet run`). |
| `startApi.healthUrl`   | URL de health para comprobar que la API responde. |
| `startClients`         | Array de `{ "name", "workingDir", "command" }` para frontends u otros clientes. |
| `healthCheck.*`        | Reintentos y tiempos de espera para MySQL y API. |

Si el fichero no existe, se usan valores por defecto (solo Docker: db, cache, adminer).

## Estructura esperada

- **Raíz del repo:** contiene `docker-compose.yml` y la carpeta `src/`.
- **API:** proyecto en `src/Api` con `dotnet run` (puerto según `launchSettings.json`, típicamente 5010 para Admin).
- **Logs:** si se usa `run-service-with-log.ps1`, los logs se escriben en `logs/services/<ServiceName>.log`.

## Troubleshooting

- **Docker no está corriendo:** iniciar Docker Desktop y volver a ejecutar el script.
- **Puerto 3307 en uso:** detener el proceso que lo use o cambiar el mapeo en `docker-compose.yml` (p. ej. `3308:3306`).
- **MySQL tarda en estar listo:** el script espera hasta `mysqlMaxAttempts * mysqlRetrySeconds` segundos; si no basta, revisar `docker-compose logs gesfer-db`.
- **La API no arranca:** comprobar que la DB está accesible y que `startApi.workingDir` apunta a `src/Api` (o la ruta correcta del proyecto).

## Salida JSON (contrato tools)

La herramienta cumple `SddIA/tools/tools-contract.json`. Al finalizar produce un JSON con:

- `toolId`, `exitCode`, `success`, `timestamp`, `message`, `feedback[]`, `data`, `duration_ms`.
- `feedback`: array de eventos por fase (`init`, `docker`, `mysql`, `api`, `clients`, `done`) con `phase`, `level` (info|warning|error), `message`, `timestamp`.
- `data`: servicios Docker levantados, URLs, API y clientes iniciados.

Ejemplo de uso con salida a fichero y por stdout:

```powershell
.\scripts\tools\prepare-full-env\Prepare-FullEnv.ps1 -OutputPath "logs\prepare-env-result.json" -OutputJson
```

## Referencia

- Contrato de herramientas: `SddIA/tools/tools-contract.json`, `SddIA/tools/tools-contract.md`.
- Manifest de la cápsula: `manifest.json` en esta carpeta.
