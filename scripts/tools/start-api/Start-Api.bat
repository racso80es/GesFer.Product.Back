@echo off
setlocal
set "SCRIPT_DIR=%~dp0"
REM Repo root para el binario Rust (scripts/tools/start-api -> ../../../)
if not defined GESFER_REPO_ROOT (
    for %%I in ("%~dp0..\..\..") do set "GESFER_REPO_ROOT=%%~fI"
)
set "BIN_EXE=%SCRIPT_DIR%start_api.exe"
if exist "%BIN_EXE%" (
    "%BIN_EXE%" %*
) else (
    powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0Start-Api.ps1" %*
)
