@echo off
setlocal
REM Run-Tests-Local.bat - Ejecuta tests en condiciones locales (contrato tools, Rust .exe)
REM Capsula: scripts/tools/run-tests-local/

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\.."
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%run_tests_local.exe"
if exist "%RUST_EXE%" (
    set "GESFER_REPO_ROOT=%REPO_ROOT%"
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo ERROR: No se encontro run_tests_local.exe. Ejecute scripts/tools-rs/install.ps1
endlocal
exit /b 1
