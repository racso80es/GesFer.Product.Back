@echo off
setlocal
REM Invoke-Commit.bat - Skill invoke-commit (Rust en bin/ si existe)
REM Capsula: paths.skillCapsules.invoke-commit (scripts/skills/invoke-commit/)
REM Uso: Invoke-Commit.bat --message "msg" [--files "a,b"] [--all]

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\invoke_commit.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo invoke_commit.exe no encontrado. Ejecute scripts/skills-rs/install.ps1
endlocal
exit /b 1
