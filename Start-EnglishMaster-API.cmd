@echo off
setlocal

cd /d "%~dp0"

if "%ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD%"=="" (
  set /p ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD=Temporary SuperAdmin password: 
)

call "%~dp0scripts\start-local-internal-api.cmd"
