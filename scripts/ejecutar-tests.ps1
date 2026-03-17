#!/usr/bin/env pwsh
$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Ejecutando Tests GesFer (Dockerized)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Definir Raíz del Repositorio
$RootPath = Resolve-Path "$PSScriptRoot/.."

# Rutas de validación
$BackTestProject = Join-Path $RootPath "src/GesFer.Admin.Back.IntegrationTests/GesFer.Admin.Back.IntegrationTests.csproj"

# 1. Validación de Rutas
Write-Host "`n[1/2] Verificando rutas..." -ForegroundColor Yellow
if (-not (Test-Path $BackTestProject)) {
    Write-Error "❌ ERROR: No se encontró $BackTestProject"
}
Write-Host "✅ Rutas verificadas." -ForegroundColor Green

# 2. Tests Backend
Write-Host "`n[2/2] Ejecutando Tests Backend (Container)..." -ForegroundColor Yellow
$ComposeFile = Join-Path $RootPath "docker-compose.test.yml"

try {
    # Ejecuta dotnet test dentro del contenedor SDK
    # Nota: Se usa la ruta relativa desde la raíz del repo (working_dir: /app)
    $TestCmd = "dotnet test src/GesFer.Admin.Back.IntegrationTests/GesFer.Admin.Back.IntegrationTests.csproj --verbosity normal --logger 'console;verbosity=detailed'"

    docker compose -f $ComposeFile run --rm backend-test $TestCmd
    if ($LASTEXITCODE -ne 0) {
        throw "Fallaron los tests de Backend."
    }
    Write-Host "✅ Tests Backend Exitosos." -ForegroundColor Green
}
catch {
    Write-Error "❌ $_"
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "🎉 EJECUCIÓN FINALIZADA CON ÉXITO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
