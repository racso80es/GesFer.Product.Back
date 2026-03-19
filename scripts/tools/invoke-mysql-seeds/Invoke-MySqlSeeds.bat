@echo off
setlocal enabledelayedexpansion
REM Invoke-MySqlSeeds.bat - Migraciones EF y seeds Admin (contrato tools, Rust .exe en capsula si existe)
REM Capsula: scripts/tools/invoke-mysql-seeds/

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
echo   Invoke-MySqlSeeds - Migraciones y seeds
echo ========================================
echo.
echo REQUISITOS (deben estar disponibles):
echo   - Windows 11
echo   - PowerShell 7+ (o PowerShell integrado)
echo   - Docker con contenedor MySQL (p. ej. tras Prepare-FullEnv)
echo   - .NET SDK 8
echo   - Base de datos MySQL accesible (contenedor GesFer_product_db)
echo.
echo ACCIONES:
echo   - Comprueba que MySQL responda (mysqladmin ping)
echo   - Aplica migraciones EF: dotnet ef database update
echo   - Ejecuta seeds Admin: companies, admin-users
echo.
echo Params: -SkipMigrations ^| -SkipSeeds ^| -ConfigPath ^| -OutputPath ^| -OutputJson
echo.
echo ----------------------------------------
goto :run

:show_help
echo.
echo Invoke-MySqlSeeds - Migraciones EF y seeds de Admin sobre MySQL
echo.
echo REQUISITOS:
echo   - Docker con MySQL (GesFer_product_db) en ejecucion
echo   - .NET SDK 8
echo   - Ejecutar desde la raiz del repositorio
echo.
echo PARAMETROS:
echo   -SkipMigrations  No ejecutar dotnet ef database update; solo seeds
echo   -SkipSeeds       Solo ejecutar migraciones; no ejecutar seeds
echo   -ConfigPath      Ruta al JSON de configuracion
echo   -OutputPath      Fichero donde escribir el resultado JSON
echo   -OutputJson      Emitir resultado JSON por stdout
echo.
echo Ejemplo: Invoke-MySqlSeeds.bat -SkipMigrations -OutputJson
echo.
endlocal
exit /b 0

:run
set "RUST_EXE=%SCRIPT_DIR%invoke_mysql_seeds.exe"
if exist "%RUST_EXE%" (
    echo [Usando invoke_mysql_seeds.exe]
    set "GESFER_REPO_ROOT=%REPO_ROOT%"
    "%RUST_EXE%" %*
    exit /b !ERRORLEVEL!
)

set "PS_SCRIPT=%SCRIPT_DIR%Invoke-MySqlSeeds.ps1"
if exist "%PS_SCRIPT%" (
    echo [Usando Invoke-MySqlSeeds.ps1]
    where pwsh >nul 2>&1
    if !ERRORLEVEL! equ 0 (
        pwsh -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
    ) else (
        powershell -NoProfile -ExecutionPolicy Bypass -File "%PS_SCRIPT%" %*
    )
    exit /b !ERRORLEVEL!
) else (
    echo ERROR: No se encontro invoke_mysql_seeds.exe ni Invoke-MySqlSeeds.ps1
    echo Ejecute scripts/tools-rs/install.ps1 para compilar el binario.
    exit /b 1
)
