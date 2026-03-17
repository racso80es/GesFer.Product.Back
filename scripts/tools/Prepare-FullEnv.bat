@echo off
setlocal
REM Wrapper: delega a la capsula scripts/tools/prepare-full-env/
set "SCRIPT_DIR=%~dp0"
call "%SCRIPT_DIR%prepare-full-env\Prepare-FullEnv.bat" %*
endlocal
exit /b %ERRORLEVEL%
