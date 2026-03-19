@echo off
setlocal
REM Invoke-Command.bat - Skill invoke-command (Rust en bin/)
REM Capsula: paths.skillCapsules.invoke-command (scripts/skills/invoke-command/)
REM Uso: Invoke-Command.bat --command "git status" --fase Accion

set "SCRIPT_DIR=%~dp0"
set "REPO_ROOT=%SCRIPT_DIR%..\..\..\"
cd /d "%REPO_ROOT%"

set "RUST_EXE=%SCRIPT_DIR%bin\invoke_command.exe"
if exist "%RUST_EXE%" (
    "%RUST_EXE%" %*
    endlocal
    exit /b %ERRORLEVEL%
)

echo ERROR: No se encontro invoke_command.exe. Ejecute scripts/skills-rs/install.ps1
endlocal
exit /b 1
