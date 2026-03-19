<#
.SYNOPSIS
    Post-PR: checkout a main/master, pull, eliminar rama local y opcionalmente remota.
.DESCRIPTION
    Fase post_pr de skill finalizar-git. Tras merge del PR en remoto.
.PARAMETER BranchName
    Rama a eliminar (ej. feature/fix-namespaces-remove-shared-14188559524536972040).
.PARAMETER DeleteRemote
    Eliminar también la rama remota.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string] $BranchName,

    [Parameter(Mandatory = $false)]
    [switch] $DeleteRemote
)

$ErrorActionPreference = "Stop"
$scriptDir = $PSScriptRoot
$repoRoot = (Resolve-Path (Join-Path $scriptDir "..\..\")).Path
Push-Location $repoRoot
try {
    $branch = if ($BranchName) { $BranchName } else { (git branch --show-current).Trim() }
    if ([string]::IsNullOrWhiteSpace($branch)) {
        Write-Error "No se pudo obtener la rama. Especifique -BranchName."
        exit 1
    }
    if ($branch -eq "main" -or $branch -eq "master") {
        Write-Host "Ya estás en la rama troncal." -ForegroundColor Green
        exit 0
    }

    $base = "main"
    $remoteMain = git rev-parse --verify "origin/main" 2>$null
    if (-not $remoteMain) { $base = "master" }

    Write-Host "[Merge-To-Master-Cleanup] Checkout $base..." -ForegroundColor Cyan
    git checkout $base
    if ($LASTEXITCODE -ne 0) { exit 1 }

    Write-Host "[Merge-To-Master-Cleanup] Pull origin $base..." -ForegroundColor Cyan
    git pull origin $base
    if ($LASTEXITCODE -ne 0) { exit 1 }

    Write-Host "[Merge-To-Master-Cleanup] Eliminando rama local $branch..." -ForegroundColor Cyan
    git branch -d $branch 2>$null
    if ($LASTEXITCODE -ne 0) { git branch -D $branch }

    if ($DeleteRemote) {
        Write-Host "[Merge-To-Master-Cleanup] Eliminando rama remota $branch..." -ForegroundColor Cyan
        git push origin --delete $branch 2>$null
    }

    Write-Host "[Merge-To-Master-Cleanup] Listo. Rama actual: $base" -ForegroundColor Green
} finally {
    Pop-Location
}
