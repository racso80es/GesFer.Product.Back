# Invoke-MySqlSeeds — MySQL, migraciones y seeds

Herramienta que comprueba MySQL, aplica migraciones EF Core y ejecuta los seeds de Admin (companies, admin-users) de forma adecuada.

## Requisitos

- **Windows 11** con **PowerShell 7+**.
- **Docker** con el contenedor MySQL (p. ej. tras `Prepare-FullEnv`).
- **.NET SDK 8**.
- Base de datos MySQL accesible (por defecto vía contenedor `gesfer_db`).

## Uso

Desde la **raíz del repositorio**:

```powershell
.\scripts\tools\invoke-mysql-seeds\Invoke-MySqlSeeds.bat
```

O desde la cápsula (compatibilidad con launcher en raíz de tools):

```powershell
.\scripts\tools\Invoke-MySqlSeeds.bat
```

El `.bat` usa `invoke_mysql_seeds.exe` dentro de la cápsula si existe (build Rust + `scripts/tools-rs/install.ps1`); si no, ejecuta el script PowerShell.

En PowerShell directo:

```powershell
.\scripts\tools\invoke-mysql-seeds\Invoke-MySqlSeeds.ps1
```

| Parámetro          | Descripción |
|--------------------|-------------|
| `-SkipMigrations`  | No ejecutar `dotnet ef database update`; solo seeds. |
| `-SkipSeeds`       | Solo ejecutar migraciones; no ejecutar seeds. |
| `-ConfigPath`      | Ruta al JSON de configuración. |
| `-OutputPath`      | Fichero donde escribir el resultado JSON (contrato tools). |
| `-OutputJson`      | Emitir el resultado JSON por stdout. |

Ejemplos:

```powershell
.\scripts\tools\invoke-mysql-seeds\Invoke-MySqlSeeds.ps1 -OutputJson
.\scripts\tools\invoke-mysql-seeds\Invoke-MySqlSeeds.ps1 -SkipMigrations -OutputPath logs\mysql-seeds-result.json
```

## Configuración: `mysql-seeds-config.json`

Ubicación: en esta cápsula, `mysql-seeds-config.json`. Ruta canónica (Cúmulo): **paths.toolCapsules['invoke-mysql-seeds']** (`SddIA/agents/cumulo.json`).

| Campo                  | Descripción |
|------------------------|-------------|
| `efProject`            | Proyecto que contiene el DbContext (Infrastructure). |
| `startupProject`       | Proyecto de arranque (Api). |
| `runMigrations`        | Si ejecutar migraciones (dotnet ef database update). |
| `runSeeds`             | Si ejecutar seeds (RUN_SEEDS_ONLY en la API). |
| `mysqlContainerName`   | Nombre del contenedor MySQL para el ping. |
| `healthCheck.*`        | Reintentos y espera para el ping a MySQL. |

## Flujo

1. **MySQL:** Comprueba que el contenedor `gesfer_db` responda a `mysqladmin ping`.
2. **Migraciones:** Ejecuta `dotnet ef database update --project ... --startup-project ...`.
3. **Seeds:** Ejecuta la API con `RUN_SEEDS_ONLY=1`; la API aplica migraciones y ejecuta `SeedCompaniesAsync` y `SeedAdminUsersAsync`, luego sale.

Los archivos de seed están en `src/Infrastructure/Data/Seeds/` (companies.json, admin-users.json).

## Salida JSON (contrato tools)

Cumple `SddIA/tools/tools-contract.json`: `toolId`, `exitCode`, `success`, `timestamp`, `message`, `feedback[]`, `data` (mysql, migrations, seeds), `duration_ms`.

## Referencia

- Contrato de herramientas: `SddIA/tools/tools-contract.json`.
- Seeds Admin: `src/Infrastructure/Data/Seeds/README.md`, `AdminJsonDataSeeder`.
- Manifest de la cápsula: `manifest.json` en esta carpeta.
