# run-test-e2e-local (cápsula)

**toolId:** `run-test-e2e-local`  
**Definición:** `SddIA/tools/run-test-e2e-local/spec.md`

## Estado

- **Preferido:** `run_test_e2e_local.exe` (Rust) en esta carpeta; compilar con `scripts/tools-rs/install.ps1`.
- **Reserva:** `Run-Test-E2E-Local.ps1` si el `.exe` no está presente (el `.bat` elige automáticamente).

## Uso (desde la raíz del repo)

```powershell
.\scripts\tools\run-test-e2e-local\Run-Test-E2E-Local.bat
```

Parámetros **Rust** (`run_test_e2e_local.exe`, flags con `--kebab-case`):

| Flag | Descripción |
|------|-------------|
| `--admin-api-url` | URL base Admin (default `http://localhost:5010`) |
| `--product-api-url` | URL base Product → `E2E_BASE_URL` (default `http://localhost:5020`) |
| `--only-tests` | Solo build + tests (sin Docker/seeds) |
| `--skip-prepare` / `--skip-seeds` | Omitir herramientas dependientes |
| `--skip-api-probe` | No comprobar `/health` antes de tests |
| `--output-json` | JSON resultado por stdout |
| `--output-path` | Archivo JSON de resultado |
| `--e2e-internal-secret` | Secreto interno (opcional; por defecto config o Development) |

Parámetros **PowerShell** (reserva): `-AdminApiUrl`, `-ProductApiUrl`, etc.

Ejemplo solo tests con APIs ya levantadas:

```powershell
.\scripts\tools\run-test-e2e-local\run_test_e2e_local.exe --only-tests --admin-api-url "http://localhost:5010" --product-api-url "http://localhost:5020"
```

## Referencia manual (equivalente)

```powershell
$env:E2E_BASE_URL = "http://localhost:5020"
$env:E2E_INTERNAL_SECRET = "dev-internal-secret-change-in-production"
dotnet test src\GesFer.Product.Back.E2ETests\GesFer.Product.Back.E2ETests.csproj --filter "Category=E2E"
```
