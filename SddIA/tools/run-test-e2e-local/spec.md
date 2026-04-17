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
implementation_path_ref: paths.toolCapsules.run-test-e2e-local
inputs:
  AdminApiUrl: string (opcional). URL base de la API Admin; por defecto http://localhost:5010. Se usa para comprobar disponibilidad y, si se arranca la API de producto desde la herramienta, como variable de entorno AdminApi__BaseUrl.
  ProductApiUrl: string (opcional). URL base de la API Product (tests E2E); por defecto http://localhost:5020. Se exporta como E2E_BASE_URL al ejecutar dotnet test.
  OnlyTests: boolean (opcional). Solo ejecutar tests E2E (sin prepare-full-env ni invoke-mysql-seeds).
  SkipPrepare: boolean (opcional). No invocar prepare-full-env.
  SkipSeeds: boolean (opcional). No invocar invoke-mysql-seeds.
  SkipApiProbe: boolean (opcional). No comprobar /health en Admin y Product antes de los tests.
  OutputJson: boolean (opcional). Emitir resultado JSON por stdout.
  OutputPath: string (opcional). Fichero donde escribir el resultado JSON.
  E2EInternalSecret: string (opcional). Secreto interno alineado con Admin/Product (tests o cabeceras); no registrar en logs.
output:
  data_fields: tests_summary (passed, failed, skipped, total), duration_ms, admin_api_url, product_api_url
  phases_feedback:
    - init
    - prepare
    - seeds
    - probe
    - build
    - tests
    - done
    - error
  schema_ref: tools-contract.md output.required_fields y optional_fields
toolId: run-test-e2e-local
version: 1.0.0
---

# Especificación: run-test-e2e-local

**toolId:** `run-test-e2e-local`  
**Definición (SddIA):** `paths.toolsDefinitionPath/run-test-e2e-local/` (este directorio).  
**Implementación (cápsula):** `paths.toolCapsules.run-test-e2e-local` → `scripts/tools/run-test-e2e-local/` (Cúmulo).

## Objetivo

Orquestar el **entorno local preparado** (Docker/MySQL y seeds cuando aplique) y ejecutar **todos los tests E2E** del proyecto **GesFer.Product.Back** contra la API de producto en ejecución, parametrizando explícitamente la **URL de la API Admin** y la **URL de la API Product**.

Los tests E2E viven en el proyecto `GesFer.Product.Back.E2ETests` (trait `Category=E2E`). La herramienta debe invocar:

```text
dotnet test <ruta>/GesFer.Product.Back.E2ETests.csproj --filter "Category=E2E" --no-build
```

(previa compilación del proyecto o de la solución según implementación), con las variables de entorno adecuadas.

## Relación Admin / Product

- Los tests HTTP llaman solo a **Product** (`E2E_BASE_URL` = `ProductApiUrl`).
- La API de producto, en runtime, debe resolver empresas y compañía vía **Admin** (`AdminApi:BaseUrl`). Quien levante el proceso de producto debe usar la misma URL que `AdminApiUrl` (p. ej. `appsettings.Development.json` o variable de entorno `AdminApi__BaseUrl`).
- La herramienta **valida** (salvo `SkipApiProbe`) que respondan `GET {AdminApiUrl}/health` y `GET {ProductApiUrl}/health` antes de lanzar los tests; si fallan, feedback `warning` o `error` según política acordada en implementación.

## Entradas

| Parámetro | Tipo | Descripción |
|-----------|------|-------------|
| AdminApiUrl | string | URL base Admin (sin barra final obligatoria; normalizar). Por defecto `http://localhost:5010`. |
| ProductApiUrl | string | URL base Product para `E2E_BASE_URL`. Por defecto `http://localhost:5020`. |
| SkipPrepare | switch | No ejecutar prepare-full-env. |
| SkipSeeds | switch | No ejecutar invoke-mysql-seeds. |
| OnlyTests | switch | Solo build + tests (sin prepare ni seeds). |
| SkipApiProbe | switch | No verificar health de Admin/Product. |
| OutputPath | string | Ruta del JSON de resultado. |
| OutputJson | switch | Escribir JSON en stdout. |
| E2EInternalSecret | string | Opcional; solo si los tests o el entorno lo requieren explícitamente. |

## Entorno y variables

| Variable | Origen | Uso |
|----------|--------|-----|
| `E2E_BASE_URL` | `ProductApiUrl` | Proyecto E2ETests (`E2EApiProbe`, clientes HTTP). |
| `E2E_INTERNAL_SECRET` | Config o parámetro | Alineado con `InternalSecret` de la API si los tests lo necesitan. |
| `AdminApi__BaseUrl` | `AdminApiUrl` | Solo si la implementación **arranca** el proceso `dotnet run` de la API Product; no sustituye la configuración de un proceso ya iniciado manualmente. |

## Fases (feedback)

1. **init** — Validar rutas de repo, proyecto E2ETests, herramientas dependientes.  
2. **prepare** — Invocar `prepare-full-env` (salvo SkipPrepare / OnlyTests).  
3. **seeds** — Invocar `invoke-mysql-seeds` (salvo SkipSeeds / OnlyTests).  
4. **probe** — Comprobar health de Admin y Product (salvo SkipApiProbe).  
5. **build** — `dotnet build` del proyecto E2ETests o solución mínima necesaria.  
6. **tests** — `dotnet test` con filtro `Category=E2E` y env `E2E_BASE_URL` (y demás acordadas).  
7. **done** / **error** — Resultado JSON según `tools-contract.md`.

## Salida

Cumple `SddIA/tools/tools-contract.md`: `toolId`, `exitCode`, `success`, `timestamp`, `message`, `feedback[]`, `data` (p. ej. `tests_summary`, `duration_ms`, URLs usadas). No incluir secretos ni tokens en `message` ni en `data`.

## Estado de implementación

**Formato objetivo:** Ejecutable Rust (`.exe`) en `scripts/tools/run-test-e2e-local/bin/`, launcher `.bat` según contrato. Fuente en `paths.toolsRustPath`.

**Estado actual:** Binario **Rust** `run_test_e2e_local.exe` en la cápsula (compilar con `scripts/tools-rs/install.ps1`). Reserva: `Run-Test-E2E-Local.ps1` si el `.exe` no existe. El launcher `.bat` prioriza el ejecutable.

**Fuente:** `paths.toolsRustPath` → `src/bin/run_test_e2e_local.rs`.

### Invocación

```powershell
.\scripts\tools\run-test-e2e-local\Run-Test-E2E-Local.bat --admin-api-url "http://localhost:5010" --product-api-url "http://localhost:5020"
```

(O el `.exe` con los mismos argumentos long de **clap**.) Parámetros: `--only-tests`, `--skip-prepare`, `--skip-seeds`, `--skip-api-probe`, `--output-json`, `--output-path`, `--e2e-internal-secret`. Ver `scripts/tools/run-test-e2e-local/run-test-e2e-local.md`.

---

*Idioma: es-ES. toolId en kebab-case.*
