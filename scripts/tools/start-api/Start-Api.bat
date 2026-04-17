@echo off
setlocal enabledelayedexpansion
REM Start-Api.bat - Levanta la API (contrato tools, Rust .exe en capsula si existe)
REM Capsula: scripts/tools/start-api/
REM Ejecutar desde la raiz del repositorio o desde esta carpeta.

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\.."
cd /d "%REPO_ROOT%"

REM Ayuda rapida
if "%1"=="/?" goto :show_help
if "%1"=="-?" goto :show_help
if "%1"=="--help" goto :show_help
if "%1"=="-h" goto :show_help

REM Mostrar informacion necesaria antes de ejecutar
echo.
echo ========================================
echo   Start-Api - Levantar API (Admin Back)
echo ========================================
echo.
echo REQUISITOS (deben estar disponibles):
echo   - Windows 11
echo   - PowerShell 7+ (opcional; solo si hay .ps1 legacy)
echo   - .NET SDK 8
echo   - MySQL listo si la API lo requiere (prepare-full-env, invoke-mysql-seeds)
echo.
echo Params: --no-build ^| --config-path ^| --output-path ^| --output-json
echo Ver start-api-config.json para puerto, health, portBlocked, etc.
echo.
echo ----------------------------------------
goto :run

:show_help
echo.
echo Start-Api - Compila (opcional), dotnet run y comprobacion health
echo.
echo REQUISITOS:
echo   - .NET SDK 8
echo   - Config: start-api-config.json en esta capsula o ruta con --config-path
echo.
echo PARAMETROS:
echo   --no-build       No compilar; solo ejecutar si ya hay build
echo   --config-path    Ruta al JSON de configuracion
echo   --output-path    Fichero donde escribir el resultado JSON
echo   --output-json    Emitir resultado JSON por stdout
echo.
echo Ejemplo: Start-Api.bat --output-json
echo.
endlocal
exit /b 0

:run
set "GESFER_REPO_ROOT=%REPO_ROOT%"
set "RUST_EXE=%SCRIPT_DIR%start_api.exe"
if exist "%RUST_EXE%" goto :run_exe
set "RUST_EXE=%SCRIPT_DIR%bin\start_api.exe"
if exist "%RUST_EXE%" goto :run_exe

set "PS_SCRIPT=%SCRIPT_DIR%Start-Api.ps1"
if exist "%PS_SCRIPT%" (
    echo [Usando Start-Api.ps1]
    where pwsh >nul 2>&1
    if !ERRORLEVEL! equ 0 (
        pwsh -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
    ) else (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
    )
    exit /b !ERRORLEVEL!
) else (
    echo ERROR: No se encontro start_api.exe (ni en capsula ni en bin\) ni Start-Api.ps1
    echo Ejecute scripts\tools-rs\install.ps1 para compilar y copiar start_api.exe a esta capsula.
    exit /b 1
)

:run_exe
echo [Usando start_api.exe]
"%RUST_EXE%" %*
exit /b !ERRORLEVEL!
