# Local Run

## Run Without Containers

Use the normal .NET workflow when developing code locally:

```powershell
dotnet restore EnglishMaster.sln
dotnet build EnglishMaster.sln
dotnet test EnglishMaster.sln
```

Start the API and Web projects from Visual Studio 2022 or with `dotnet run` from each project folder.

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
