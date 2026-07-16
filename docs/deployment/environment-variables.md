# Environment Variables

## Docker Compose Variables

| Variable | Used By | Required | Default | Purpose |
| --- | --- | --- | --- | --- |
| `ENGLISHMASTER_SQL_PASSWORD` | SQL Server, API | Recommended | `EnglishMaster_dev_12345!` | Local SQL Server `sa` password. Override in `.env`. |
| `ENGLISHMASTER_SQL_PORT` | SQL Server | No | `14333` | Host port mapped to container SQL port `1433`. |
| `ENGLISHMASTER_API_PORT` | API | No | `7001` | Host port mapped to API container port `8080`. |
| `ENGLISHMASTER_WEB_PORT` | Web | No | `7002` | Host port mapped to Web container port `8080`. |
| `ENGLISHMASTER_SUPERADMIN_EMAIL` | API seed | Optional | Empty | Creates a local SuperAdmin only when email and password are both set. |
| `ENGLISHMASTER_SUPERADMIN_PASSWORD` | API seed | Optional | Empty | Local SuperAdmin password. Never use a production password here. |
| `ENGLISHMASTER_DEVELOPMENT_SEED_ENABLED` | API seed | No | `true` | Enables small demo content for local development. |

## ASP.NET Core Configuration Variables

These variables are passed to containers by `docker-compose.yml`:

| Variable | Service | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | API, Web | Uses `Development` for local Compose. |
| `ASPNETCORE_URLS` | API, Web | Binds each app to `http://+:8080` inside the container. |
| `ConnectionStrings__DefaultConnection` | API | SQL Server connection string. |
| `DevelopmentSeed__Enabled` | API | Toggles demo content seed. |
| `Auth__InitialSuperAdmin__Email` | API | Optional initial SuperAdmin email. |
| `Auth__InitialSuperAdmin__Password` | API | Optional initial SuperAdmin password. |
| `Publishing__LocalStoragePath` | API | Container path for generated publishing artifacts. |
| `Media__LocalStoragePath` | API | Container path for uploaded media files. |
| `Email__Provider` | API | Email provider selector. Use `Development` until a real adapter exists. |
| `Email__FromAddress` | API | Placeholder from-address for future email provider integration. |
| `ApiBaseUrl` | Web | Internal API base URL used by Blazor server-side API clients. |

## Production Configuration

Use environment variables or deployment secrets for production. Do not commit real values.

Production examples are provided as templates only:

- `src/Backend/EnglishMaster.Api/appsettings.Production.example.json`
- `src/Frontend/EnglishMaster.Web/appsettings.Production.example.json`

Production should set:

- A real SQL Server connection string.
- `DevelopmentSeed__Enabled=false`.
- Durable `Media__LocalStoragePath` and `Publishing__LocalStoragePath` values.
- Secure SuperAdmin bootstrap values only during controlled setup, then rotate or remove them.
- A public HTTPS API URL for the Web app's `ApiBaseUrl`.
- Production host names in `AllowedHosts`.

## Staging Configuration

Staging examples are provided as templates only:

- `src/Backend/EnglishMaster.Api/appsettings.Staging.example.json`
- `src/Frontend/EnglishMaster.Web/appsettings.Staging.example.json`

Required staging variables:

| Variable | Service | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | API, Web | Set to `Staging`. |
| `ConnectionStrings__DefaultConnection` | API | Staging SQL Server connection string. |
| `DevelopmentSeed__Enabled` | API | Set to `false` unless the environment is disposable demo staging. |
| `Auth__InitialSuperAdmin__Email` | API | Optional one-time bootstrap admin email. |
| `Auth__InitialSuperAdmin__Password` | API | Optional one-time bootstrap admin password. Remove after setup. |
| `Publishing__LocalStoragePath` | API | Durable staging path for publishing artifacts. |
| `Media__LocalStoragePath` | API | Durable staging path for uploaded media. |
| `Email__Provider` | API | Staging email provider selector. Prefer `Development` until real delivery is configured. |
| `Email__FromAddress` | API | Placeholder sender address for future provider integration. |
| `ApiBaseUrl` | Web | Staging API base URL. |
| `AllowedHosts` | API, Web | Staging host names. |

Local-staging Compose variables:

| Variable | Purpose |
| --- | --- |
| `ENGLISHMASTER_STAGING_SQL_PASSWORD` | SQL Server `sa` password for local-staging Compose. |
| `ENGLISHMASTER_STAGING_SQL_PORT` | Host SQL port. |
| `ENGLISHMASTER_STAGING_DATABASE` | Staging database name. |
| `ENGLISHMASTER_STAGING_API_PORT` | Host API port. |
| `ENGLISHMASTER_STAGING_WEB_PORT` | Host Web port. |
| `ENGLISHMASTER_STAGING_ALLOWED_HOSTS` | Semicolon-separated host names. |
| `ENGLISHMASTER_STAGING_API_BASE_URL` | API URL used by the Web app. |
| `ENGLISHMASTER_STAGING_DEVELOPMENT_SEED_ENABLED` | Demo seed toggle. Prefer `false`. |
| `ENGLISHMASTER_STAGING_SUPERADMIN_EMAIL` | Optional bootstrap admin email. |
| `ENGLISHMASTER_STAGING_SUPERADMIN_PASSWORD` | Optional bootstrap admin password. |
