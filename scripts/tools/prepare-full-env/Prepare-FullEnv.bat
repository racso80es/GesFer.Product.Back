@echo off
setlocal enabledelayedexpansion
REM Prepare-FullEnv.bat - Prepara entorno: Docker + API/clientes (contrato tools, Rust .exe en capsula si existe)
REM Capsula: scripts/tools/prepare-full-env/
REM Ejecutar desde la raiz del repositorio.

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
echo   Prepare-FullEnv - Entorno completo
echo ========================================
echo.
echo REQUISITOS (deben estar disponibles):
echo   - Windows 11
echo   - PowerShell 7+ (o PowerShell integrado)
echo   - Docker Desktop instalado y EN EJECUCION
echo   - .NET SDK (para levantar la API)
echo   - Opcional: Node/npm si se configuran clientes
echo.
echo ACCIONES:
echo   - Levanta Docker: MySQL, Memcached, Adminer
echo   - Espera a que MySQL este listo
echo   - Opcional: inicia la Admin API en local
echo.
echo Params: -DockerOnly ^| -StartApi ^| -NoDocker ^| -ConfigPath ^| -OutputPath ^| -OutputJson
echo.
echo ----------------------------------------
goto :run

:show_help
echo.
echo Prepare-FullEnv - Prepara entorno de desarrollo completo
echo.
echo REQUISITOS:
echo   - Docker Desktop en ejecucion
echo   - .NET SDK
echo   - Ejecutar desde la raiz del repositorio
echo.
echo PARAMETROS:
echo   -DockerOnly    Solo levanta Docker (DB, cache, Adminer)
echo   -StartApi      Ademas levanta la Admin API en local
echo   -NoDocker      No levanta Docker (solo API/clientes si se pide)
echo   -ConfigPath    Ruta al JSON de configuracion
echo   -OutputPath    Fichero donde escribir el resultado JSON
echo   -OutputJson    Emitir resultado JSON por stdout
echo.
echo Ejemplo: Prepare-FullEnv.bat -DockerOnly
echo.
endlocal
exit /b 0

:run
set "RUST_EXE=%SCRIPT_DIR%prepare_full_env.exe"
if exist "%RUST_EXE%" (
    echo [Usando prepare_full_env.exe]
    "%RUST_EXE%" %*
    exit /b !ERRORLEVEL!
)

set "PS_SCRIPT=%SCRIPT_DIR%Prepare-FullEnv.ps1"
if exist "%PS_SCRIPT%" (
    echo [Usando Prepare-FullEnv.ps1]
    where pwsh >nul 2>&1
    if !ERRORLEVEL! equ 0 (
        pwsh -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
    ) else (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
    )
    exit /b !ERRORLEVEL!
) else (
    echo ERROR: No se encontro prepare_full_env.exe ni Prepare-FullEnv.ps1
    echo Ejecute scripts/tools-rs/install.ps1 para compilar el binario.
    exit /b 1
)
