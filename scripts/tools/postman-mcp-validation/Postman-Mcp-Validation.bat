@echo off
setlocal
REM Postman-Mcp-Validation.bat - Valida endpoints con colección Postman (Newman). Contrato tools. Rust .exe.
REM Capsula: scripts/tools/postman-mcp-validation/

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\.."
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%postman_mcp_validation.exe"
if exist "%RUST_EXE%" (
    set "GESFER_REPO_ROOT=%REPO_ROOT%"
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo ERROR: No se encontro postman_mcp_validation.exe. Ejecute scripts/tools-rs/install.ps1
endlocal
exit /b 1
