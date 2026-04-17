# run-test-e2e-local (cápsula)

**toolId:** `run-test-e2e-local`  
**Definición:** `SddIA/tools/run-test-e2e-local/spec.md`

## Estado

Implementación en **PowerShell** (`Run-Test-E2E-Local.ps1`), transición hasta binario Rust (`run_test_e2e_local.exe`) según contrato tools.

## Uso (desde la raíz del repo)

```powershell
.\scripts\tools\run-test-e2e-local\Run-Test-E2E-Local.bat
```

Parámetros (PowerShell):

| Parámetro | Descripción |
|-----------|-------------|
| `-AdminApiUrl` | URL base Admin (default `http://localhost:5010`) |
| `-ProductApiUrl` | URL base Product → `E2E_BASE_URL` (default `http://localhost:5020`) |
| `-OnlyTests` | Solo build + tests (sin Docker/seeds) |
| `-SkipPrepare` / `-SkipSeeds` | Omitir herramientas dependientes |
| `-SkipApiProbe` | No comprobar `/health` antes de tests |
| `-OutputJson` | JSON resultado por stdout |
| `-OutputPath` | Archivo JSON de resultado |

Ejemplo solo tests con APIs ya levantadas:

```powershell
.\scripts\tools\run-test-e2e-local\Run-Test-E2E-Local.ps1 -OnlyTests -AdminApiUrl "http://localhost:5010" -ProductApiUrl "http://localhost:5020"
```

## Referencia manual (equivalente)

```powershell
$env:E2E_BASE_URL = "http://localhost:5020"
$env:E2E_INTERNAL_SECRET = "dev-internal-secret-change-in-production"
dotnet test src\GesFer.Product.Back.E2ETests\GesFer.Product.Back.E2ETests.csproj --filter "Category=E2E"
```
