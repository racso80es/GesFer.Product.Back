<#
.SYNOPSIS
    Push de la rama y creación del PR (pre_pr de skill finalizar-git).
.DESCRIPTION
    Hace git push origin <rama> y, si gh está disponible, gh pr create.
.PARAMETER Persist
    Ruta de la carpeta de la feature (ej. docs/features/fix-namespaces-remove-shared/)
.PARAMETER BranchName
    Rama a pushear (por defecto: rama actual).
.PARAMETER Title
    Título del PR (opcional).
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $Persist,

    [Parameter(Mandatory = $false)]
    [string] $BranchName,

    [Parameter(Mandatory = $false)]
    [string] $Title
)

$ErrorActionPreference = "Stop"
$branch = if ($BranchName) { $BranchName } else { (git branch --show-current).Trim() }
if ([string]::IsNullOrWhiteSpace($branch)) {
    Write-Error "No se pudo obtener la rama. Especifique -BranchName."
    exit 1
}

Write-Host "[Push-And-CreatePR] Push origin $branch" -ForegroundColor Cyan
git push origin $branch
if ($LASTEXITCODE -ne 0) {
    Write-Error "git push falló."
    exit 1
}

$base = "main"
$remoteMain = git rev-parse --verify "origin/main" 2>$null
if (-not $remoteMain) {
    $base = "master"
}
$body = "Documentación: $Persist"
$titleArg = if ($Title) { $Title } else { "feat: $branch" }

$gh = Get-Command gh -ErrorAction SilentlyContinue
if ($gh) {
    $auth = gh auth status 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "[Push-And-CreatePR] Creando PR con gh..." -ForegroundColor Cyan
        gh pr create --base $base --head $branch --title $titleArg --body $body
        if ($LASTEXITCODE -eq 0) {
            Write-Host "[Push-And-CreatePR] PR creado correctamente." -ForegroundColor Green
            exit 0
        }
    }
}

$repo = (git config --get remote.origin.url) -replace '\.git$', ''
$repo = $repo -replace '^git@github\.com:', 'https://github.com/'
$prUrl = "$repo/compare/$base...$branch?expand=1"
Write-Host "[Push-And-CreatePR] gh no disponible. Crear PR manualmente:" -ForegroundColor Yellow
Write-Host "  $prUrl" -ForegroundColor White
exit 0
