<#
.SYNOPSIS
    Ejecuta pruebas E2E en local con infra en punto de origen: prepare-full-env, invoke-mysql-seeds, API y tests.
.DESCRIPTION
    Orquesta: (1) prepare-full-env (Docker + MySQL), (2) invoke-mysql-seeds (migraciones y seeds),
    (3) arranque de la API en background si no responde en E2E_BASE_URL, (4) dotnet test --filter Category=E2E.
    Ejecutar desde la raíz del repositorio. Requiere PowerShell 7+.
.PARAMETER SkipPrepare
    No ejecutar prepare-full-env (asume Docker/MySQL ya levantados).
.PARAMETER SkipSeeds
    No ejecutar invoke-mysql-seeds (asume BD ya migrada y con seeds).
.PARAMETER SkipApiStart
    No intentar arrancar la API; falla si /health no responde.
.PARAMETER E2EBaseUrl
    URL base de la API (por defecto http://localhost:5010). Se pasa como E2E_BASE_URL a los tests.
    Si el script arranca la API, usa 5012 para no chocar con otra instancia.
.PARAMETER OnlyTests
    Solo ejecutar los tests E2E (sin prepare, seeds ni API). Útil si el entorno ya está listo.
#>
[CmdletBinding()]
param(
    [switch] $SkipPrepare,
    [switch] $SkipSeeds,
    [switch] $SkipApiStart,
    [string] $E2EBaseUrl = "http://localhost:5010",
    [switch] $OnlyTests
)

$ApiPortWhenStartedByScript = 5012

$ErrorActionPreference = "Stop"
$repoRoot = $PSScriptRoot | Split-Path -Parent
if (-not $repoRoot) { $repoRoot = (Get-Location).Path }

$toolsDir = Join-Path $repoRoot "scripts\tools"
$prepareBat = Join-Path $toolsDir "prepare-full-env\Prepare-FullEnv.bat"
$seedsBat = Join-Path $toolsDir "invoke-mysql-seeds\Invoke-MySqlSeeds.bat"
$slnPath = Join-Path $repoRoot "src\GesFer.Admin.Back.sln"
$apiProject = Join-Path $repoRoot "src\GesFer.Admin.Back.Api\GesFer.Admin.Back.Api.csproj"
$e2eProject = Join-Path $repoRoot "src\GesFer.Admin.Back.E2ETests\GesFer.Admin.Back.E2ETests.csproj"
$healthUrl = $E2EBaseUrl.TrimEnd("/") + "/health"

function Test-HealthUrl {
    param([string]$Url, [int]$TimeoutSec = 5)
    try {
        $r = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec $TimeoutSec -ErrorAction Stop
        return $r.StatusCode -eq 200
    } catch {
        return $false
    }
}

function Start-ApiBackground {
    param([string]$HealthUrlToUse)
    $port = $ApiPortWhenStartedByScript
    $url = "http://localhost:$port"
    Write-Host "Iniciando API en background en $url ..."
    $arg = "set ASPNETCORE_URLS=http://localhost:$port&& dotnet run --project `"$apiProject`" --no-build"
    $p = Start-Process -FilePath "cmd.exe" -ArgumentList "/c", $arg -WorkingDirectory $repoRoot -PassThru -WindowStyle Hidden
    $waitSec = 50
    $attempt = 0
    $targetHealth = $url + "/health"
    while ($attempt -lt $waitSec) {
        Start-Sleep -Seconds 1
        $attempt++
        if (Test-HealthUrl -Url $targetHealth -TimeoutSec 2) {
            Write-Host "API respondiendo en $targetHealth"
            return $url
        }
        if ($p.HasExited -and $p.ExitCode -ne 0) {
            Write-Warning "La API termino con codigo $($p.ExitCode)."
            return $null
        }
    }
    Write-Warning "API no respondio en $waitSec s."
    return $null
}

# --- Main ---
Push-Location $repoRoot
try {
    if (-not $OnlyTests) {
        if (-not $SkipPrepare -and (Test-Path $prepareBat)) {
            Write-Host "Paso 1/3: Prepare-FullEnv (Docker, MySQL)..."
            & $prepareBat
            if ($LASTEXITCODE -ne 0) {
                Write-Error "Prepare-FullEnv fallo con codigo $LASTEXITCODE"
                exit $LASTEXITCODE
            }
        } else {
            if ($SkipPrepare) { Write-Host "Omitiendo Prepare-FullEnv (SkipPrepare)." }
            else { Write-Host "Prepare-FullEnv no encontrado; omitiendo." }
        }

        if (-not $SkipSeeds -and (Test-Path $seedsBat)) {
            Write-Host "Paso 2/3: Invoke-MySqlSeeds (migraciones y seeds)..."
            & $seedsBat
            if ($LASTEXITCODE -ne 0) {
                Write-Error "Invoke-MySqlSeeds fallo con codigo $LASTEXITCODE"
                exit $LASTEXITCODE
            }
        } else {
            if ($SkipSeeds) { Write-Host "Omitiendo Invoke-MySqlSeeds (SkipSeeds)." }
            else { Write-Host "Invoke-MySqlSeeds no encontrado; omitiendo." }
        }
    }

    if (-not (Test-Path $e2eProject)) {
        Write-Error "Proyecto E2E no encontrado: $e2eProject"
        exit 1
    }

    Write-Host "Compilando solucion (Api + E2ETests)..."
    if (Test-Path $slnPath) {
        dotnet build $slnPath -c Debug
    } else {
        dotnet build $apiProject -c Debug
        dotnet build $e2eProject -c Debug
    }
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build fallo."
        exit $LASTEXITCODE
    }

    $baseUrlForTests = $E2EBaseUrl.TrimEnd("/")
    if (-not $OnlyTests -and -not $SkipApiStart) {
        $apiOk = Test-HealthUrl -Url $healthUrl
        if (-not $apiOk) {
            $startedUrl = Start-ApiBackground -HealthUrlToUse $healthUrl
            if ($startedUrl) { $baseUrlForTests = $startedUrl }
            else {
                Write-Warning "No se pudo arrancar la API en segundo plano. Arranquela manualmente: dotnet run --project src\GesFer.Admin.Back.Api"
                Write-Warning "Luego ejecute: .\scripts\Run-E2ELocal.ps1 -OnlyTests"
            }
        }
    }

    Write-Host "Ejecutando pruebas E2E (Category=E2E) en $baseUrlForTests ..."
    $env:E2E_BASE_URL = $baseUrlForTests
    $env:E2E_INTERNAL_SECRET = "dev-internal-secret-change-in-production"
    dotnet test $e2eProject --filter "Category=E2E" --no-build -v normal
    $testExit = $LASTEXITCODE
    if ($testExit -eq 0) {
        Write-Host "E2E completados correctamente."
    } else {
        Write-Error "E2E fallaron con codigo $testExit"
    }
    exit $testExit
} finally {
    Pop-Location
}
