@echo off
setlocal
REM Wrapper: delega a la capsula scripts/tools/invoke-mysql-seeds/
set "SCRIPT_DIR=%~dp0"
call "%SCRIPT_DIR%invoke-mysql-seeds\Invoke-MySqlSeeds.bat" %*
endlocal
exit /b %ERRORLEVEL%
