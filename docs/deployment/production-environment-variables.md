# Production Environment Variables

## API Variables

| Variable | Required | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | Set to `Production`. |
| `ASPNETCORE_URLS` | Platform-specific | Bind address inside the host/container. |
| `AllowedHosts` | Yes | Production API host names. Avoid `*` in production. |
| `ConnectionStrings__DefaultConnection` | Yes | Production SQL Server connection string from secrets. |
| `DevelopmentSeed__Enabled` | Yes | Set to `false`. |
| `Auth__InitialSuperAdmin__Email` | Temporary | Optional one-time bootstrap email. Remove after setup. |
| `Auth__InitialSuperAdmin__Password` | Temporary | Optional one-time bootstrap password. Rotate/remove after setup. |
| `Media__LocalStoragePath` | Yes | Durable uploaded media storage path. |
| `Publishing__LocalStoragePath` | Yes | Durable published artifact storage path. |
| `Email__Provider` | No | `Development` until a real provider adapter is added. |
| `Email__FromAddress` | No | Placeholder until a real provider adapter is added. |

## Web Variables

| Variable | Required | Purpose |
| --- | --- | --- |
| `ASPNETCORE_ENVIRONMENT` | Yes | Set to `Production`. |
| `ASPNETCORE_URLS` | Platform-specific | Bind address inside the host/container. |
| `AllowedHosts` | Yes | Production Web host names. Avoid `*` in production. |
| `ApiBaseUrl` | Yes | Production HTTPS API base URL. |

## Secret Rules

- Do not commit production connection strings, SQL passwords, bootstrap credentials, or provider credentials.
- Store secrets in the hosting platform secret manager, environment variables, or a vault.
- Remove temporary SuperAdmin bootstrap credentials after first setup.
