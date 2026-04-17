<#
.SYNOPSIS
    Ejecuta todos los tests E2E (GesFer.Product.Back.E2ETests) con entorno local opcional. Contrato SddIA/tools.
.DESCRIPTION
    Orquesta prepare-full-env, invoke-mysql-seeds, comprueba health de Admin y Product, compila y ejecuta dotnet test --filter Category=E2E.
.PARAMETER AdminApiUrl
    URL base API Admin (por defecto http://localhost:5010).
.PARAMETER ProductApiUrl
    URL base API Product; se exporta como E2E_BASE_URL (por defecto http://localhost:5020).
.PARAMETER SkipPrepare
    No invocar prepare-full-env.
.PARAMETER SkipSeeds
    No invocar invoke-mysql-seeds.
.PARAMETER OnlyTests
    Solo build + tests (sin prepare ni seeds).
.PARAMETER SkipApiProbe
    No verificar GET /health en Admin ni Product.
.PARAMETER E2EInternalSecret
    Valor para E2E_INTERNAL_SECRET (por defecto desde config o dev-internal-secret-change-in-production).
.PARAMETER OutputPath
    Fichero JSON de resultado.
.PARAMETER OutputJson
    Emitir JSON por stdout.
#>
[CmdletBinding()]
param(
    [string] $AdminApiUrl = "http://localhost:5010",
    [string] $ProductApiUrl = "http://localhost:5020",
    [switch] $SkipPrepare,
    [switch] $SkipSeeds,
    [switch] $OnlyTests,
    [switch] $SkipApiProbe,
    [string] $E2EInternalSecret,
    [string] $OutputPath,
    [switch] $OutputJson
)

$ErrorActionPreference = "Stop"
$scriptDir = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $scriptDir "..\..\..")).Path
$startTime = Get-Date
$toolId = "run-test-e2e-local"
$feedbackList = [System.Collections.Generic.List[object]]::new()

function Normalize-BaseUrl([string]$Url) {
    if ([string]::IsNullOrWhiteSpace($Url)) { return $Url }
    return $Url.Trim().TrimEnd('/')
}

function Add-Feedback {
    param([string]$Phase, [string]$Level, [string]$Message, [string]$Detail = $null, [int]$DurationMs = $null)
    $entry = @{ phase = $Phase; level = $Level; message = $Message; timestamp = (Get-Date -Format "o") }
    if ($Detail) { $entry.detail = $Detail }
    if ($null -ne $DurationMs) { $entry.duration_ms = $DurationMs }
    $feedbackList.Add($entry) | Out-Null
    $color = switch ($Level) { "error" { "Red" } "warning" { "Yellow" } default { "White" } }
    Write-Host $Message -ForegroundColor $color
}

function Write-Result {
    param([bool]$Success, [int]$ExitCode, [string]$Message, [object]$Data = @{})
    $endTime = Get-Date
    $durationMs = [int](($endTime - $startTime).TotalMilliseconds)
    $result = @{
        toolId      = $toolId
        exitCode    = $ExitCode
        success     = $Success
        timestamp   = $endTime.ToUniversalTime().ToString("o")
        message     = $Message
        feedback    = @($feedbackList)
        data        = $Data
        duration_ms = $durationMs
    }
    $json = $result | ConvertTo-Json -Depth 10 -Compress
    if ($OutputPath) {
        $dir = Split-Path -Parent $OutputPath
        if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Path $dir -Force | Out-Null }
        $json | Set-Content -Path $OutputPath -Encoding UTF8 -NoNewline
    }
    if ($OutputJson) { Write-Output $json }
    exit $ExitCode
}

$configPath = Join-Path $scriptDir "run-test-e2e-local-config.json"
$defaultSecret = "dev-internal-secret-change-in-production"
if (Test-Path $configPath) {
    try {
        $cfg = Get-Content $configPath -Raw -Encoding UTF8 | ConvertFrom-Json
        if ($cfg.e2eEnv -and $cfg.e2eEnv.E2E_INTERNAL_SECRET) {
            $defaultSecret = $cfg.e2eEnv.E2E_INTERNAL_SECRET
        }
    } catch { }
}
if (-not $E2EInternalSecret) { $E2EInternalSecret = $defaultSecret }

$adminBase = Normalize-BaseUrl $AdminApiUrl
$productBase = Normalize-BaseUrl $ProductApiUrl

$toolsDir = Join-Path $repoRoot "scripts\tools"
$prepareBat = Join-Path $toolsDir "prepare-full-env\Prepare-FullEnv.bat"
$seedsBat = Join-Path $toolsDir "invoke-mysql-seeds\Invoke-MySqlSeeds.bat"
$e2eProject = Join-Path $repoRoot "src\GesFer.Product.Back.E2ETests\GesFer.Product.Back.E2ETests.csproj"

if ($OnlyTests) { $SkipPrepare = $true; $SkipSeeds = $true }

Add-Feedback -Phase "init" -Level "info" -Message "Iniciando $toolId (Admin=$adminBase Product=$productBase)"

function Test-Health([string]$BaseUrl) {
    $u = "$BaseUrl/health"
    try {
        $r = Invoke-WebRequest -Uri $u -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
        return $r.StatusCode -eq 200
    } catch {
        return $false
    }
}

Push-Location $repoRoot
try {
    if (-not (Test-Path $e2eProject)) {
        Add-Feedback -Phase "init" -Level "error" -Message "Proyecto E2E no encontrado" -Detail $e2eProject
        Write-Result -Success $false -ExitCode 1 -Message "Proyecto E2E no encontrado" -Data @{ project = $e2eProject }
    }

    if (-not $OnlyTests) {
        if (-not $SkipPrepare -and (Test-Path $prepareBat)) {
            Add-Feedback -Phase "prepare" -Level "info" -Message "Invocando prepare-full-env..."
            $t = Get-Date
            & $prepareBat 2>&1 | Out-Null
            Add-Feedback -Phase "prepare" -Level "info" -Message "prepare-full-env finalizado" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
            if ($LASTEXITCODE -ne 0) {
                Add-Feedback -Phase "prepare" -Level "error" -Message "prepare-full-env fallo" -Detail "exitCode=$LASTEXITCODE"
                Write-Result -Success $false -ExitCode 1 -Message "prepare-full-env fallo" -Data @{ phase = "prepare"; exitCode = $LASTEXITCODE }
            }
        } elseif ($SkipPrepare) { Add-Feedback -Phase "prepare" -Level "info" -Message "Omitiendo prepare-full-env (SkipPrepare)." }

        if (-not $SkipSeeds -and (Test-Path $seedsBat)) {
            Add-Feedback -Phase "seeds" -Level "info" -Message "Invocando invoke-mysql-seeds..."
            $t = Get-Date
            & $seedsBat 2>&1 | Out-Null
            Add-Feedback -Phase "seeds" -Level "info" -Message "invoke-mysql-seeds finalizado" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
            if ($LASTEXITCODE -ne 0) {
                Add-Feedback -Phase "seeds" -Level "error" -Message "invoke-mysql-seeds fallo" -Detail "exitCode=$LASTEXITCODE"
                Write-Result -Success $false -ExitCode 1 -Message "invoke-mysql-seeds fallo" -Data @{ phase = "seeds"; exitCode = $LASTEXITCODE }
            }
        } elseif ($SkipSeeds) { Add-Feedback -Phase "seeds" -Level "info" -Message "Omitiendo invoke-mysql-seeds (SkipSeeds)." }
    }

    if (-not $SkipApiProbe) {
        Add-Feedback -Phase "probe" -Level "info" -Message "Comprobando health Admin y Product..."
        $okAdmin = Test-Health -BaseUrl $adminBase
        $okProduct = Test-Health -BaseUrl $productBase
        if (-not $okAdmin) {
            Add-Feedback -Phase "probe" -Level "error" -Message "Admin API no responde en $adminBase/health"
            Write-Result -Success $false -ExitCode 1 -Message "Health Admin fallo" -Data @{ admin_api_url = $adminBase; product_api_url = $productBase; phase = "probe" }
        }
        if (-not $okProduct) {
            Add-Feedback -Phase "probe" -Level "error" -Message "Product API no responde en $productBase/health"
            Write-Result -Success $false -ExitCode 1 -Message "Health Product fallo" -Data @{ admin_api_url = $adminBase; product_api_url = $productBase; phase = "probe" }
        }
        Add-Feedback -Phase "probe" -Level "info" -Message "Health OK (Admin y Product)."
    } else {
        Add-Feedback -Phase "probe" -Level "warning" -Message "Omitiendo comprobacion health (SkipApiProbe)."
    }

    Add-Feedback -Phase "build" -Level "info" -Message "Compilando proyecto E2E..."
    $t = Get-Date
    dotnet build $e2eProject -c Debug -v q
    $buildExit = $LASTEXITCODE
    Add-Feedback -Phase "build" -Level "info" -Message "Build finalizado (exitCode=$buildExit)" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)
    if ($buildExit -ne 0) {
        Add-Feedback -Phase "build" -Level "error" -Message "Build fallo"
        Write-Result -Success $false -ExitCode $buildExit -Message "Build fallo" -Data @{ build_exit_code = $buildExit }
    }

    $env:E2E_BASE_URL = $productBase
    $env:E2E_INTERNAL_SECRET = $E2EInternalSecret

    Add-Feedback -Phase "tests" -Level "info" -Message "Ejecutando tests E2E (Category=E2E) con E2E_BASE_URL=$productBase..."
    $t = Get-Date
    dotnet test $e2eProject --filter "Category=E2E" --no-build -v minimal
    $testsExit = $LASTEXITCODE
    Add-Feedback -Phase "tests" -Level "info" -Message "Tests E2E finalizados (exitCode=$testsExit)" -DurationMs ([int]((Get-Date) - $t).TotalMilliseconds)

    $data = @{
        admin_api_url    = $adminBase
        product_api_url    = $productBase
        tests_exit_code    = $testsExit
        e2e_project        = $e2eProject
    }

    if ($testsExit -ne 0) {
        Add-Feedback -Phase "done" -Level "warning" -Message "Tests E2E con fallos"
        Write-Result -Success $false -ExitCode $testsExit -Message "Tests E2E con fallos" -Data $data
    }
    Add-Feedback -Phase "done" -Level "info" -Message "Tests E2E completados correctamente"
    Write-Result -Success $true -ExitCode 0 -Message "Tests E2E completados correctamente" -Data $data
} catch {
    Add-Feedback -Phase "error" -Level "error" -Message "Excepcion no controlada" -Detail $_.Exception.Message
    Write-Result -Success $false -ExitCode 1 -Message "Error: $($_.Exception.Message)"
} finally {
    Pop-Location
}
