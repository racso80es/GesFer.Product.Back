# Pruebas E2E en local (GesFer.Admin.Back)

Las pruebas E2E se ejecutan contra la **API real** en local, con infraestructura en punto de origen (Docker, MySQL, migraciones y seeds aplicados).

## Validación del proyecto en este entorno (pasos garantizados)

Para **validar el correcto funcionamiento del proyecto** en local con infra y datos preparados:

1. **Desde la raíz del repo**, preparar infra y datos:
   ```powershell
   .\scripts\tools\prepare-full-env\Prepare-FullEnv.bat
   .\scripts\tools\invoke-mysql-seeds\Invoke-MySqlSeeds.bat
   ```

2. **En una terminal**, arrancar la API y dejarla en ejecución:
   ```powershell
   cd c:\Proyectos\GesFer.Admin.Back
   dotnet run --project src\GesFer.Admin.Back.Api\GesFer.Admin.Back.Api.csproj
   ```
   (Esperar a que muestre "Now listening on: http://localhost:5010".)

3. **En otra terminal**, ejecutar solo los E2E (el script fija `E2E_BASE_URL` y `E2E_INTERNAL_SECRET`):
   ```powershell
   cd c:\Proyectos\GesFer.Admin.Back
   .\scripts\Run-E2ELocal.ps1 -OnlyTests
   ```

Si los 6 tests E2E pasan, el proyecto está validado en este entorno (API + BD con seeds + SharedSecret de Development).

Si en el paso 1 no arranca Docker o fallan los seeds, resuelva primero la infra (Docker Desktop, MySQL en 3307, BD GesFer_Product). La API del paso 2 debe usar la misma configuración que `appsettings.Development.json` (SharedSecret y ConnectionString).

## Requisitos

- Windows 11, PowerShell 7+
- Docker Desktop (para prepare-full-env)
- .NET 8 SDK
- Ejecutar desde la **raíz del repositorio**

## Automatización completa: `Run-E2ELocal.ps1`

Desde la raíz del repo:

```powershell
.\scripts\Run-E2ELocal.ps1
```

El script hace en orden:

1. **Prepare-FullEnv** (tool) — levanta Docker (MySQL, cache, Adminer) y espera a MySQL.
2. **Invoke-MySqlSeeds** (tool) — aplica migraciones EF y ejecuta seeds.
3. **Compila** la solución (Api + E2ETests).
4. **Comprueba** si la API responde en `http://localhost:5010/health`. Si no, intenta arrancarla en background (puerto 5012); si falla, indica que la arranque manualmente.
5. **Ejecuta** `dotnet test --filter Category=E2E` con `E2E_BASE_URL` y `E2E_INTERNAL_SECRET` (valor de appsettings.Development).

### Parámetros

| Parámetro        | Descripción |
|------------------|-------------|
| `-SkipPrepare`   | No ejecutar prepare-full-env (Docker/MySQL ya levantados). |
| `-SkipSeeds`     | No ejecutar invoke-mysql-seeds (BD ya migrada y con seeds). |
| `-SkipApiStart`  | No arrancar la API; falla si /health no responde. |
| `-E2EBaseUrl`    | URL base de la API (por defecto `http://localhost:5010`). |
| `-OnlyTests`     | Solo ejecutar los tests E2E (entorno ya listo). |

Ejemplos:

```powershell
# Entorno ya listo (API corriendo en 5010)
.\scripts\Run-E2ELocal.ps1 -OnlyTests

# Sin levantar Docker; solo seeds y tests (MySQL ya up)
.\scripts\Run-E2ELocal.ps1 -SkipPrepare

# API en otro puerto
.\scripts\Run-E2ELocal.ps1 -E2EBaseUrl "http://localhost:5020" -OnlyTests
```

## Ejecutar solo los tests E2E (manual)

Si ya tienes la API en marcha y la BD preparada:

```powershell
$env:E2E_BASE_URL = "http://localhost:5010"
dotnet test src\GesFer.Admin.Back.E2ETests\GesFer.Admin.Back.E2ETests.csproj --filter "Category=E2E"
```

Variables de entorno opcionales para los tests:

- `E2E_BASE_URL` — URL base de la API (por defecto `http://localhost:5010`).
- `E2E_INTERNAL_SECRET` — Debe coincidir con `SharedSecret` de la API (p. ej. el de `appsettings.Development.json`: `dev-internal-secret-change-in-production`).
- `E2E_ADMIN_USER` / `E2E_ADMIN_PASSWORD` — Credenciales admin; por defecto `admin` / `admin123` (las mismas que deja el seed).

Para que todos los E2E pasen, la API debe estar usando la misma configuración (SharedSecret) y la BD debe tener migraciones y seeds aplicados (invoke-mysql-seeds).

## Proyecto E2ETests

- **Ubicación:** `src/GesFer.Admin.Back.E2ETests/`
- **Trait:** `Category=E2E` (permite filtrar con `--filter Category=E2E`).
- **Cobertura actual:** health, swagger, `/api/countries`, `/api/company` (con X-Internal-Secret y con JWT admin), `/api/admin/auth/login`.

Las herramientas de inicialización (prepare-full-env, invoke-mysql-seeds) están definidas en Cúmulo (`paths.toolCapsules`) y se invocan desde este script.
