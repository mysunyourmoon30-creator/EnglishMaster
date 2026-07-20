@echo off
setlocal

cd /d "%~dp0.."

if "%ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD%"=="" (
  set /p ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD=Temporary SuperAdmin password: 
)

set "ASPNETCORE_ENVIRONMENT=Development"
set "ASPNETCORE_URLS=http://127.0.0.1:7101"
if "%ENGLISHMASTER_INTERNAL_SQLSERVER%"=="" (
  set "ENGLISHMASTER_INTERNAL_SQLSERVER=."
)

set "Database__Provider=SqlServer"
set "Database__Name=EnglishMasterInternal"
if "%ENGLISHMASTER_INTERNAL_SQLUSER%"=="" (
  set "ConnectionStrings__DefaultConnection=Server=%ENGLISHMASTER_INTERNAL_SQLSERVER%;Database=EnglishMasterInternal;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
) else (
  set "ConnectionStrings__DefaultConnection=Server=%ENGLISHMASTER_INTERNAL_SQLSERVER%;Database=EnglishMasterInternal;User Id=%ENGLISHMASTER_INTERNAL_SQLUSER%;Password=%ENGLISHMASTER_INTERNAL_SQLPASSWORD%;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
)
set "DevelopmentSeed__Enabled=false"
set "Auth__InitialSuperAdmin__Email=internal.admin@englishmaster.local"
set "Auth__InitialSuperAdmin__Password=%ENGLISHMASTER_INTERNAL_ADMIN_PASSWORD%"
set "Media__LocalStoragePath=%CD%\.local-internal\media"
set "Publishing__LocalStoragePath=%CD%\.local-internal\publishing"
set "Logging__FilePath=%CD%\.local-internal\logs"
set "DataProtection__KeysPath=%CD%\.local-internal\keys\api"
set "ENGLISHMASTER_INTERNAL_API_BUILD=%CD%\.local-internal\build\api-%RANDOM%%RANDOM%"

if not exist "%ENGLISHMASTER_INTERNAL_API_BUILD%" mkdir "%ENGLISHMASTER_INTERNAL_API_BUILD%"

echo Starting EnglishMaster API on http://127.0.0.1:7101
dotnet build ".\src\Backend\EnglishMaster.Api\EnglishMaster.Api.csproj" --no-restore -p:UseAppHost=false -p:OutputPath="%ENGLISHMASTER_INTERNAL_API_BUILD%\"
if errorlevel 1 goto :failed
dotnet "%ENGLISHMASTER_INTERNAL_API_BUILD%\EnglishMaster.Api.dll"
goto :done

:failed
echo.
echo API build failed. Check the messages above.

:done

pause
