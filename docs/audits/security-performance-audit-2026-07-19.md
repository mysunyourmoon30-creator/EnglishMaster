# EnglishMaster Security and Performance Audit - 2026-07-19

## Audit Roles

- Security architect: authentication, authorization, cookies, CSRF, headers, secrets, upload surface, dependency risk.
- Performance engineer: SQL Server query shape, pagination, search, export, dashboard/report hot paths.
- .NET architecture reviewer: Clean Architecture boundaries, production configuration, operational readiness.

## Executive Summary

EnglishMaster is usable for local/internal testing, and the recent SQL Server word-search optimization is a strong improvement. It is not yet at "maximum security" or hardened production readiness.

The main blockers are:

- High-severity vulnerable transitive SQLite package.
- Login has no dedicated rate limit, lockout, or MFA path.
- Web login/logout endpoints disable antiforgery.
- Web stores the API session cookie inside the web auth claims.
- Security headers are incomplete on the running API/Web.
- Public search and export paths still have large-data performance risk.
- Local scripts intentionally use insecure SQL Server options and default load-test password; keep them local only.

## Findings

### P0 - Vulnerable Transitive SQLite Native Package

Reference:

- `src/Backend/EnglishMaster.Infrastructure/EnglishMaster.Infrastructure.csproj:9`
- Command: `dotnet list EnglishMaster.sln package --vulnerable --include-transitive`

Evidence:

- `SQLitePCLRaw.lib.e_sqlite3 2.1.10`
- Severity: High
- Advisory: `GHSA-2m69-gcr7-jv3q`

Risk:

Even if production uses SQL Server, the vulnerable native package is still part of the dependency graph for API/Infrastructure/test projects. This fails a serious production security gate.

Fix:

- Prefer removing `Microsoft.EntityFrameworkCore.Sqlite` from production Infrastructure if SQLite is only a local fallback.
- If SQLite support must remain, upgrade/pin SQLitePCLRaw packages to a non-vulnerable version.
- Re-run `dotnet list EnglishMaster.sln package --vulnerable --include-transitive`.

Verification:

- Vulnerability scan returns no vulnerable packages.
- Build and focused integration tests pass.

### P0 - Login Lacks Brute-Force Protection

Reference:

- `src/Backend/EnglishMaster.Api/Endpoints/SecurityEndpoints.cs:17`
- `src/Backend/EnglishMaster.Api/Program.cs:348`
- `src/Backend/EnglishMaster.Infrastructure/Security/EfSecurityService.cs:17`

Evidence:

- Login is anonymous.
- Rate limiter currently defines only `certificate-verification`.
- No observed account lockout, IP throttling, failed-attempt tracking, CAPTCHA, or MFA.

Risk:

Attackers can automate password guessing against admin accounts. Password hashing helps after compromise, but it does not stop online guessing.

Fix:

- Add a named rate-limit policy for `/api/v1/auth/login`.
- Add failed-login tracking and temporary lockout per account and per IP.
- Add optional TOTP/WebAuthn MFA for admin roles.
- Log failed login attempts without logging passwords.

Verification:

- Repeated failed login attempts return 429 or account lockout.
- Legitimate login still succeeds after the lockout window.

### P1 - Web Login and Logout Disable Antiforgery

Reference:

- `src/Frontend/EnglishMaster.Web/Program.cs:156`
- `src/Frontend/EnglishMaster.Web/Program.cs:198`
- `src/Frontend/EnglishMaster.Web/Program.cs:209`

Evidence:

- `/account/login` and `/logout` both call `.DisableAntiforgery()`.

Risk:

Login CSRF can force a browser into an attacker-controlled session. Logout CSRF can force sign-out. The practical impact is lower than credential theft, but this is still not maximum-security posture.

Fix:

- Add antiforgery token to the login form.
- Require antiforgery on logout.
- Keep API endpoints cookie-safe with explicit CSRF policy where browser cookies are accepted.

Verification:

- POST without antiforgery token fails.
- Normal login/logout from UI succeeds.

### P1 - API Session Cookie Is Stored Inside Web Auth Claims

Reference:

- `src/Frontend/EnglishMaster.Web/Program.cs:178`
- `src/Frontend/EnglishMaster.Web/Services/Security/AuthCookieHandler.cs:8`

Evidence:

- Web login stores the raw API `Set-Cookie` value as an `api_cookie` claim.
- API calls replay that claim into the `Cookie` header.

Risk:

The web auth ticket becomes a container for the API session. If the web cookie is stolen or decrypted, the API session is also exposed. This also increases cookie size and couples two sessions tightly.

Fix:

- Prefer a server-side session store keyed by the web auth ticket.
- Store API session material server-side only, protected by Data Protection or a distributed cache.
- Consider a BFF pattern where browser never holds the API cookie directly.

Verification:

- Browser cookie no longer contains API cookie material.
- Web-to-API calls still authenticate after login.

### P1 - Security Headers Are Incomplete

Reference:

- Runtime check on `http://127.0.0.1:7102/`
- Runtime check on `http://127.0.0.1:7101/`
- `src/Frontend/EnglishMaster.Web/Program.cs:133`
- `src/Backend/EnglishMaster.Api/Program.cs:369`

Evidence:

- Web has `Content-Security-Policy: frame-ancestors 'self'` and `X-Frame-Options: SAMEORIGIN`.
- Web lacks `X-Content-Type-Options`, `Referrer-Policy`, and `Permissions-Policy`.
- API lacks the checked hardening headers.
- HSTS is enabled only outside Development, which is correct, but production should be verified.

Risk:

Missing headers increase browser-side attack surface: content sniffing, clickjacking variance, referrer leakage, and overly broad browser feature access.

Fix:

- Add centralized security-header middleware for Web and API.
- Recommended baseline:
  - `X-Content-Type-Options: nosniff`
  - `Referrer-Policy: no-referrer`
  - `Permissions-Policy: camera=(), microphone=(), geolocation=()`
  - A stricter CSP for Web once assets are mapped.
  - API can use `default-src 'none'; frame-ancestors 'none'` for non-HTML responses.

Verification:

- Header checks show expected values on Web, API, media, and error responses.

### P1 - Production Dependency Graph Still Includes Non-Production Database Provider

Reference:

- `src/Backend/EnglishMaster.Infrastructure/DependencyInjection.cs:94`
- `src/Backend/EnglishMaster.Infrastructure/EnglishMaster.Infrastructure.csproj:9`

Evidence:

- Infrastructure supports SQLite and SQL Server from the same production project.

Risk:

Extra providers increase dependency and security surface. The SQLite vulnerability above is a direct example.

Fix:

- Move SQLite-only support into test/local tooling, or conditionally reference it outside production.
- Keep production Infrastructure SQL Server-only unless SQLite is a real production requirement.

Verification:

- Production project graph no longer includes SQLite packages.

### P1 - Export Endpoints Load Entire Datasets Into Memory

Reference:

- `src/Backend/EnglishMaster.Api/Endpoints/ContentImportExportEndpoints.cs:21`
- `src/Backend/EnglishMaster.Infrastructure/ImportExport/ContentImportExportService.cs:128`
- `src/Backend/EnglishMaster.Infrastructure/ImportExport/ContentImportExportService.cs:133`
- Similar export methods at lines `160`, `183`, `211`, `239`, `272`.

Evidence:

- Export methods materialize all rows using `ToListAsync` and then serialize to bytes.

Risk:

Large exports can cause high memory use, long request time, timeouts, and denial-of-service risk for authenticated users.

Fix:

- Add async streaming export.
- Add filters, maximum export size, and background export jobs for large datasets.
- Rate-limit export endpoints.

Verification:

- Export 1M-word dataset does not allocate all rows at once.
- Large exports complete through a job and artifact flow.

### P2 - Public Search Still Uses Contains Queries Across Many Content Types

Reference:

- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:35`
- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:72`
- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:81`
- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:95`
- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:109`
- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:123`
- `src/Backend/EnglishMaster.Infrastructure/PublicSearch/EfPublicSearchRepository.cs:137`

Evidence:

- Public search uses `.Contains(query)` for words, grammar, lessons, courses, books, and quizzes.

Risk:

At high volume, public search can scan multiple tables. Because it is anonymous, this becomes both a performance and abuse vector.

Fix:

- Apply the SQL Server full-text/prefix pattern used for word admin search.
- Add anonymous rate limiting for public search and suggestions.
- Cache filter lists and popular searches.

Verification:

- Public search remains fast under a 1M-record dataset.
- Repeated anonymous requests are throttled.

### P2 - Dashboard and Report Queries Use Many Independent Counts

Reference:

- `src/Backend/EnglishMaster.Infrastructure/Reports/EfReportRepository.cs:18`
- `src/Backend/EnglishMaster.Infrastructure/Reports/EfReportRepository.cs:31`
- `src/Backend/EnglishMaster.Infrastructure/Reports/EfReportRepository.cs:111`
- `src/Backend/EnglishMaster.Infrastructure/Reports/EfReportRepository.cs:114`

Evidence:

- Dashboard/report data performs many independent `CountAsync` calls.

Risk:

This is acceptable locally, but can become expensive under load or large datasets.

Fix:

- Combine counts into grouped SQL queries where possible.
- Cache admin dashboard snapshots for short intervals.
- Add indexes for report filters and dates.

Verification:

- Dashboard response time remains stable under concurrent users and large data.

### P2 - Local Scripts Contain Insecure Defaults

Reference:

- `scripts/start-local-internal.ps1:68`
- `scripts/start-local-internal-api.cmd:18`
- `scripts/test-load-professional.ps1:20`
- `scripts/test-load-professional.ps1:111`

Evidence:

- Local scripts use `Encrypt=False` and `TrustServerCertificate=True`.
- Load-test script defaults to `LoadTestPassword1!`.

Risk:

Acceptable for local/internal only. Dangerous if copied into production deployment.

Fix:

- Label scripts clearly as local-only.
- Add guardrails that refuse Production environment when insecure SQL flags or default passwords are present.
- Keep production examples using `Encrypt=True;TrustServerCertificate=False`.

Verification:

- Production startup fails if insecure local connection options are used.

## Positive Controls Found

- API endpoints generally use explicit permission policies.
- API cookies are `HttpOnly`; production cookie secure policy is `Always`.
- Password hashes use ASP.NET Core `PasswordHasher`.
- Media upload path checks extension, content type, signature, size, path traversal, and writes randomized file names.
- Word search has SQL Server full-text/prefix optimizations and parameterized raw SQL.
- Production example connection string uses encrypted SQL Server connection settings.

## Recommended Remediation Plan

### Phase 1 - Security Gate Blockers

1. Remove or upgrade vulnerable SQLite dependency.
2. Add login rate limiting and account lockout.
3. Re-enable antiforgery for Web login/logout.
4. Add baseline security headers.
5. Move API cookie material out of web claims.

### Phase 2 - Production Hardening

1. Add production startup guards for insecure connection strings and default passwords.
2. Restrict `/health/ready` details or protect it behind infrastructure.
3. Add centralized audit logging for login, role changes, permission changes, imports, exports, and publish jobs.
4. Add backup/restore and migration rollback runbooks.

### Phase 3 - High Performance

1. Make public search use full-text/prefix paths and anonymous rate limits.
2. Stream or background large exports.
3. Cache dashboard/report summaries.
4. Add focused load tests for public search, admin reports, export, import, login, and media upload.

## Verification Run During Audit

- `dotnet list EnglishMaster.sln package --vulnerable --include-transitive`
- Runtime header checks against:
  - `http://127.0.0.1:7102/`
  - `http://127.0.0.1:7101/`
- Static review of API/Web startup, auth, media upload, public search, export, reports, and word search.

## Current Verdict

Local/internal readiness: acceptable for continued testing.

Production readiness: not yet.

Maximum security/high performance target: achievable, but requires the Phase 1 blockers above before any real production exposure.

## Phase 1 Remediation Update - 2026-07-19

Implemented:

1. Removed the production SQLite provider reference from Infrastructure and made `Database:Provider=Sqlite` fail clearly at startup.
2. Added API login rate limiting plus in-memory failed-login lockout by email and remote address.
3. Re-enabled antiforgery protection for Web login and logout forms.
4. Added baseline security headers to both API and Web.
5. Moved API session cookie material out of Web authentication claims into a server-side in-memory API session store.

Verification:

- API project build passed with a separate output folder.
- Web project build passed with a separate output folder.
- API project build passed again after the final login-lockout and SQL-provider script updates.
- Web project build passed after the final antiforgery and API-session-store updates.
- Full solution build reached compilation but could not overwrite files while the running local API/Web processes had locked their existing binaries.
- Restart from the sandbox could not connect to SQL Server with Windows authentication because SQL Server returned `Failed to generate SSPI context`. This is an execution-session/SQL credential issue; the code build is clean.
- Vulnerability re-scan was requested but blocked by the execution approval/usage limit before it could run.

Remaining after Phase 1:

- Re-run `dotnet list EnglishMaster.sln package --vulnerable --include-transitive` once command execution approval is available.
- For multi-instance production, replace the in-memory API session store with a distributed server-side store.
- Add persistent database-backed account lockout/audit logs if production requires lockout state to survive API restarts.

## Phase 3 Remediation Update - 2026-07-19

Implemented (item 2 of Phase 3, "Stream or background large exports" — streaming half only, background jobs deferred, see below):

1. `IContentExportService` now returns a `ContentExportStream` (filename, content type, and a `Func<Stream, CancellationToken, Task>` writer) instead of a fully-materialized `ContentExportResult` with an in-memory `byte[]`.
2. `ContentImportExportService.cs`: all six export methods (words, grammar topics, lessons, courses, books, quizzes) now query via `IQueryable<T>.AsAsyncEnumerable()` and map each row to a dictionary as it streams from SQL Server, instead of `ToListAsync()` followed by an in-memory `.Select().ToList()`.
3. CSV and JSON serialization both stream directly to the HTTP response body (`StreamWriter` for CSV, `Utf8JsonWriter` for JSON) instead of building one `StringBuilder`/`byte[]` for the whole dataset before returning.
4. `ContentImportExportEndpoints.cs` now returns `Results.Stream(...)` instead of `Results.File(byte[], ...)`.

Verification:

- `dotnet build EnglishMaster.sln --configuration Release`: 0 warnings, 0 errors (this also validated the Phase 1 changes above compile together, since they were previously unbuilt in combination).
- `dotnet test`: 408/408 passing (up from 405; excludes `FreshDatabaseMigrationTests`, which fails for an unrelated, pre-existing reason — SQL Server Full-Text Search isn't installed on this LocalDB instance, needed by a separate word-search-performance migration, not by this change).
- Added `tests/EnglishMaster.IntegrationTests/ImportExport/ContentExportEndpointsTests.cs` (3 new tests, all passing): CSV export contains seeded word data with the correct header/content-type/filename, JSON export returns a valid array containing the seeded word, and an unsupported `format` value still returns 400 as before.
- Not verified: actual peak memory usage during a real 1,000,000-row export. The fix removes the two full-materialization steps that caused it (`ToListAsync()` and whole-payload serialization), but no memory-profiled load test was run against the new code — that would need the `tools/EnglishMaster.LoadTest` project pointed at `/api/v1/export/words` with the 1M-row dataset.

Remaining after this update:

- Filters, maximum export size, rate limiting, and a background-job/artifact flow for very large exports (Phase 3 item 2's second half, and the audit's own recommended fix list) are not implemented — this was scoped to the memory-materialization fix only.
- No memory-profiled confirmation against the 1M-row dataset yet (see above).
