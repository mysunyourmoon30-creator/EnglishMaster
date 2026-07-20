# Local Run

## Run Without Containers

Use the normal .NET workflow when developing code locally:

```powershell
dotnet restore EnglishMaster.sln
dotnet build EnglishMaster.sln
dotnet test EnglishMaster.sln
```

Start the API and Web projects from Visual Studio 2022 or with `dotnet run` from each project folder.

## Private/Internal Local Run

Use this mode when you want to run EnglishMaster on this machine without renting hosting. This is suitable for private testing, local demos, or internal review. It is not a production go-live because the app stops when this machine, Windows session, or local network is unavailable.

Start the local internal stack:

```powershell
.\scripts\start-local-internal.ps1 -AdminPassword "<temporary-local-password>"
```

On Windows, the simplest option is to run:

```text
Start-EnglishMaster.cmd
```

It opens the API and Web windows for you.

If you prefer to open them yourself:

1. Run `Start-EnglishMaster-API.cmd`.
2. Enter a temporary SuperAdmin password when prompted.
3. Run `Start-EnglishMaster-Web.cmd`.
4. Open `http://127.0.0.1:7102`.

Do not start `EnglishMaster.Api.exe` directly from `bin/Debug` or `bin/Release`; the API needs the local environment variables above.

If Windows blocks background process launch from the current shell, run the apps manually in two PowerShell windows.

Terminal 1 - API:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://127.0.0.1:7101"
$env:Database__Provider="SqlServer"
$env:Database__Name="EnglishMasterInternal"
$env:ConnectionStrings__DefaultConnection="Server=localhost;Database=EnglishMasterInternal;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;Encrypt=False"
$env:DevelopmentSeed__Enabled="false"
$env:Auth__InitialSuperAdmin__Email="internal.admin@englishmaster.local"
$env:Auth__InitialSuperAdmin__Password="<temporary-local-password>"
$env:Media__LocalStoragePath="$PWD\.local-internal\media"
$env:Publishing__LocalStoragePath="$PWD\.local-internal\publishing"
$env:Logging__FilePath="$PWD\.local-internal\logs"
$env:DataProtection__KeysPath="$PWD\.local-internal\keys\api"
dotnet run --project .\src\Backend\EnglishMaster.Api\EnglishMaster.Api.csproj --no-launch-profile
```

Terminal 2 - Web:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://127.0.0.1:7102"
$env:ApiBaseUrl="http://127.0.0.1:7101/"
$env:Logging__FilePath="$PWD\.local-internal\logs"
$env:DataProtection__KeysPath="$PWD\.local-internal\keys\web"
dotnet run --project .\src\Frontend\EnglishMaster.Web\EnglishMaster.Web.csproj --no-launch-profile
```

The local internal run uses SQL Server by default:

```text
Server=localhost;Database=EnglishMasterInternal
```

That keeps local data across API/Web restarts in a real SQL Server database. To use a disposable in-memory database instead:

```powershell
.\scripts\start-local-internal.ps1 -DatabaseProvider InMemory -AdminPassword "<temporary-local-password>"
```

To choose a different SQL Server or database name:

```powershell
.\scripts\start-local-internal.ps1 -DatabaseProvider SqlServer -SqlServer "." -DatabaseName "EnglishMasterInternal" -AdminPassword "<temporary-local-password>"
```

If Windows authentication fails with `Failed to generate SSPI context`, create a local SQL login with:

```sql
-- Run in SQL Server Management Studio as a local SQL Server admin.
:r .\scripts\sqlserver-local-app-login.sql
```

Then start with SQL authentication:

```powershell
$env:ENGLISHMASTER_INTERNAL_SQLUSER="englishmaster_app"
$env:ENGLISHMASTER_INTERNAL_SQLPASSWORD="EnglishMaster_Local_123!"
.\Start-EnglishMaster.cmd
```

To use a local SQLite file instead:

```powershell
SQLite is intentionally not available in the current local/internal build. Use SQL Server for real testing, or InMemory only for temporary UI checks.
```

Open:

```text
http://127.0.0.1:7102
```

The API is available at:

```text
http://127.0.0.1:7101
```

Default admin email:

```text
internal.admin@englishmaster.local
```

Stop the local internal stack:

```powershell
.\scripts\stop-local-internal.ps1
```

By default the stack binds to `127.0.0.1`, so only this machine can open it. To allow other devices on the same LAN to connect, start with:

```powershell
.\scripts\start-local-internal.ps1 -BindAddress 0.0.0.0 -AdminPassword "<temporary-local-password>"
```

Then open `http://<this-computer-lan-ip>:7102` from another device. You may need to allow inbound Windows Firewall access for the selected ports. Do not expose this mode directly to the public internet.

## Run With Docker Compose

1. Prepare local environment values:

```powershell
Copy-Item .env.example .env
```

2. Edit `.env` and change development passwords.

3. Validate the Compose file:

```powershell
docker compose config
```

4. Start the stack:

```powershell
docker compose up --build
```

5. Open:

```text
http://localhost:7002
```

The API is available at:

```text
http://localhost:7001
```

## Development SuperAdmin

Set both values in `.env` to create a local SuperAdmin during API startup:

```text
ENGLISHMASTER_SUPERADMIN_EMAIL=dev.admin@englishmaster.local
ENGLISHMASTER_SUPERADMIN_PASSWORD=replace-with-a-local-development-password
```

If either value is empty, no SuperAdmin user is created.

## Reset Local Container Data

To stop containers:

```powershell
docker compose down
```

To remove local database, media, and publishing volumes:

```powershell
docker compose down --volumes
```

Only use `--volumes` when you intentionally want to delete local container data.

## Troubleshooting

- If the API cannot connect to SQL Server, confirm `ENGLISHMASTER_SQL_PASSWORD` matches in SQL Server and API connection settings.
- If login does not work, confirm both SuperAdmin environment variables are set before first API startup.
- If ports are busy, change `ENGLISHMASTER_API_PORT`, `ENGLISHMASTER_WEB_PORT`, or `ENGLISHMASTER_SQL_PORT` in `.env`.
- If Docker is unavailable, continue using the normal .NET local workflow.
