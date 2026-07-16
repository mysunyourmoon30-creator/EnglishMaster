# Docker Foundation

## Purpose

The Docker foundation prepares EnglishMaster MVP for local container testing. It does not deploy to cloud, add Kubernetes, split the modular monolith into microservices, or introduce paid services.

## Containers

`docker-compose.yml` defines three local services:

- `englishmaster-sqlserver`: SQL Server 2022 Developer container.
- `englishmaster-api`: ASP.NET Core API container built from `src/Backend/EnglishMaster.Api/Dockerfile`.
- `englishmaster-web`: Blazor Web container built from `src/Frontend/EnglishMaster.Web/Dockerfile`.

The API and Web apps stay separate because the solution has separate runnable projects.

## Files

- `.dockerignore`
- `.env.example`
- `docker-compose.yml`
- `src/Backend/EnglishMaster.Api/Dockerfile`
- `src/Frontend/EnglishMaster.Web/Dockerfile`
- `src/Backend/EnglishMaster.Api/appsettings.Production.example.json`
- `src/Frontend/EnglishMaster.Web/appsettings.Production.example.json`

## Local Compose Ports

Default local ports:

- Web: `http://localhost:7002`
- API: `http://localhost:7001`
- SQL Server: `localhost,14333`

Override these values in `.env` when needed.

## Local Startup

1. Copy `.env.example` to `.env`.
2. Change the local SQL and SuperAdmin passwords.
3. Start the containers:

```powershell
docker compose up --build
```

4. Open the Blazor admin UI:

```text
http://localhost:7002
```

## Database And Migrations

The API startup path calls the existing security seed flow. Outside the Testing environment, that flow applies EF Core migrations to the configured relational database and then seeds roles, permissions, optional SuperAdmin, and optional development content.

For local Docker Compose, this means the API initializes the SQL Server database when it starts. No destructive migration was added as part of the Docker foundation.

Manual migration command for local development:

```powershell
dotnet ef database update --project src/Backend/EnglishMaster.Infrastructure --startup-project src/Backend/EnglishMaster.Api
```

Before production deployment, review generated migrations and apply them through the chosen operational process. Do not rely on development seed data for production.

## Storage

Compose creates named volumes for:

- SQL Server data
- Uploaded media
- Publishing artifacts

The API uses `/app/media` and `/app/publishing` inside the container. The publishing path is configured through `Publishing__LocalStoragePath`.

## Security Notes

- `.env` is ignored by Git.
- `.env.example` contains placeholders and development-only sample values.
- Production connection strings, SQL passwords, and SuperAdmin credentials must come from the deployment environment or secret manager.
- Development seed data should be disabled outside local development.
