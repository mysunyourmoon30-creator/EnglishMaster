# MVP Release Checklist

## Status

EnglishMaster MVP release automation has passed against disposable CI staging. Deployment to a persistent staging environment and live browser smoke testing remain required before the next product feature begins.

Review date: 2026-08-03

Suggested release tag: `v0.1.0-mvp-rc1`

## Phase A Release Automation Update

Update date: 2026-08-03

| Check | Status | Evidence |
| --- | --- | --- |
| Migration ownership is explicit | Passed | Development defaults to startup migration; Staging and Production set `Database__ApplyMigrationsOnStartup=false`. |
| Reviewed migration artifact exists | Passed | `.github/workflows/release-build.yml` packages a self-contained Linux EF migration bundle. Local bundle generation completed successfully. |
| Release smoke automation exists | Passed | `scripts/Invoke-EnglishMasterReleaseSmoke.ps1` passed health, anonymous grammar, redirect, `401`, login, dashboard, and logout checks against local API/Web hosts. |
| Compose and disposable CI staging | Passed | Release Build run [30828858339](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828858339) built the custom SQL Server Full-Text image, applied the reviewed migration bundle to a fresh database, started API/Web containers, passed readiness, and completed authenticated release smoke testing. |
| Release build | Passed | Build run [30828846655](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828846655) and Release Build run [30828858339](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828858339) passed for revision `2784568`. |
| Automated tests | Passed | Release configuration passed 220 unit, 7 architecture, and 208 integration tests on the Windows job. The Full-Text fresh-database migration is intentionally executed by the Linux container job, where SQL Server Full-Text is installed. |
| Persistent staging deployment | Blocked by environment configuration | No staging URL, host, deployment credentials, GitHub Environment, repository secret, or deployment workflow is configured. Do not begin the next product feature until deployment and browser smoke testing are complete. |

## Build And Test

| Check | Status | Evidence |
| --- | --- | --- |
| Solution builds | Passed | `dotnet build EnglishMaster.sln --no-restore` completed with 0 errors and 0 warnings. |
| Tests pass | Passed | `dotnet test EnglishMaster.sln --no-build --no-restore` passed 187 unit, 44 integration, and 7 architecture tests. |
| No failing tests | Passed | Test run reported 0 failed tests. |
| No obvious temporary files committed | Passed with note | `.env` files are ignored. Build outputs are ignored. `.env.example` is intentionally tracked. |

## Security

| Check | Status | Evidence |
| --- | --- | --- |
| No committed production secrets | Passed | No real secrets found in source scan. Example files use placeholders and documented development-only values. |
| No hardcoded production credentials | Passed | Initial SuperAdmin values come from configuration/environment variables. |
| Admin routes require authentication | Passed | Web middleware redirects unauthenticated `/admin/*` requests to `/login`. |
| Admin APIs require authentication | Passed | API middleware requires authentication for `/api/v1` except login. Endpoint groups use authorization policies. |
| Sensitive permissions are applied | Passed | Users, roles, permissions, publishing, import/export, and content mutations use permission policies. |
| File upload validation exists | Passed | Media upload validates size, content type, signatures, and safe storage paths. |
| Import validation exists | Passed | Import validates file type, size, required fields, and row-level errors. |
| Internal errors are not leaked outside Development/Testing | Passed | API and Web use exception handlers outside Development/Testing. |

## Data

| Check | Status | Evidence |
| --- | --- | --- |
| Migrations exist | Passed | Migrations cover Word, Category/Tag, Media, Pronunciation, Grammar, Lesson, Course, Book, Quiz, Publishing, and Security. |
| Migrations are build-valid | Passed | Build and integration tests compile against the current DbContext and migration snapshot. |
| Seed data is development-safe | Passed | Demo content is gated behind `DevelopmentSeed:Enabled`. |
| SuperAdmin setup is documented | Passed | See `docs/development-seed-data.md` and deployment environment variable docs. |
| Import/export limitations are documented | Passed | See `docs/modules/import-export.md` and `docs/api/import-export-api.md`. |

## MVP Admin Areas

The following admin areas have documented routes and Blazor pages:

- Login / Logout
- Dashboard
- Words
- Categories
- Tags
- Media
- Pronunciations
- Grammar Topics, Rules, and Examples
- Lessons
- Courses
- Books
- Quizzes
- Publishing
- Users
- Roles
- Permissions
- Import / Export

Live browser smoke testing should be completed in staging after deployment.

## Routes

| Check | Status | Evidence |
| --- | --- | --- |
| Admin routes documented | Passed | `docs/routes/admin-routes.md` |
| API routes documented | Passed | `docs/api/api-route-overview.md` |
| Navigation routes centralized | Passed | `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs` |
| Navigation links present | Passed with note | Admin navigation includes MVP areas. Navigation visibility is not yet permission-aware. |

## Performance

| Check | Status | Evidence |
| --- | --- | --- |
| Main list pages use pagination | Passed | Paginated list/search APIs exist for the main content, security, publishing, and media areas. |
| Search endpoints have safe defaults | Passed | Main paginated endpoints default to small page sizes and cap page size. |
| No obvious large graph loading in list queries | Passed | Recent hardening removed unnecessary list-query child collection includes. |
| Known unbounded lookups documented | Passed with note | Category and Tag search APIs remain small lookup-style responses. |

## Operations

| Check | Status | Evidence |
| --- | --- | --- |
| Logging documented | Passed with note | Standard ASP.NET Core logging is configured in appsettings; production log sinks are future deployment work. |
| Health checks documented | Passed | API and Web expose `/health`, `/health/live`, and `/health/ready`. Docker Compose includes SQL Server and app health checks. |
| Docker/local run documented | Passed | `docs/deployment/docker.md` and `docs/deployment/local-run.md` |
| Environment variables documented | Passed | `docs/deployment/environment-variables.md` |

## Critical Blockers

None found during build, test, security, route, documentation, and static release review.

## Required Staging Smoke Test

After deploying to staging, verify:

1. API starts and can connect to SQL Server.
2. Migrations apply successfully or are applied by the staging release process.
3. Web starts and reaches the API through `ApiBaseUrl`.
4. Initial SuperAdmin setup works from staging-safe configuration.
5. Login and logout work.
6. `/admin` redirects unauthenticated users and loads for an authenticated admin.
7. Each MVP admin area list page opens.
8. One create/edit/detail workflow works for representative content.
9. Media upload rejects invalid files and accepts a valid small image.
10. Import rejects invalid files and reports row errors.

## Related Docs

- `docs/release/known-limitations.md`
- `docs/release/next-roadmap.md`
- `docs/security/authorization.md`
- `docs/deployment/docker.md`
- `docs/deployment/environment-variables.md`
- `docs/performance/mvp-performance-hardening.md`
