Perform Production Deployment Hardening for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Better Publishing completed.
- Student-facing pages completed.
- Student Progress completed.
- Basic Reporting completed.
- Notification and Email Foundation completed.

Goal:
Prepare the application for safer production deployment.

Important:
Do not add AI.
Do not add Mobile.
Do not add Marketplace.
Do not add Payment.
Do not add Microservices.
Do not redesign architecture.
Do not actually deploy to production.

Scope:
Production deployment readiness only.

Tasks:

1. Production Configuration Review
Check and fix:
- appsettings.Production.example.json exists.
- appsettings.Staging.example.json exists.
- .env.example exists.
- No real secrets are committed.
- Connection strings are environment-based.
- Storage paths are configurable.
- Public base URL is configurable.
- Email provider settings are placeholders only.
- Seed data flags are safe for production.

2. Security Configuration
Verify:
- HTTPS is documented for production.
- Secure cookie settings are documented or configured where appropriate.
- SameSite settings are documented.
- CORS is restrictive or documented.
- Authentication settings are production-ready.
- Admin routes remain protected.
- API endpoints remain protected.
- Health/diagnostic endpoints do not expose sensitive data.

3. Database Production Readiness
Document and verify:
- How to apply migrations.
- How to backup database.
- How to restore database.
- How to create initial SuperAdmin safely.
- How to disable development seed data.
- How to separate staging and production databases.

4. File Storage Readiness
Review:
- Media storage path.
- Publishing artifact storage path.
- Import upload handling.
- Path traversal protection.
- PublicUrl vs internal FilePath separation.
- Backup requirements for uploaded files.

5. Docker Production Review
Check:
- Dockerfile is production-appropriate.
- docker-compose.staging.yml does not contain real secrets.
- Production docker example is documented.
- Persistent volumes are documented.
- Container environment variables are documented.

6. GitHub Actions Release Review
Check:
- Build workflow passes.
- Test workflow passes.
- Release build workflow exists.
- No secrets are printed.
- Deployment secrets are placeholders only.
- Production deployment remains manual unless explicitly configured.

7. Health Checks
Verify:
- /health exists.
- /health/live exists if implemented.
- /health/ready exists if implemented.
- Database health check is safe.
- Health check docs exist.

8. Documentation
Create or update:
- docs/deployment/production.md
- docs/deployment/production-environment-variables.md
- docs/deployment/production-database.md
- docs/deployment/production-file-storage.md
- docs/deployment/production-security.md
- docs/release/production-go-live-checklist.md

9. Quality
Run:
- dotnet build
- dotnet test

Fix only production-readiness issues.
Do not add new business features.

Output:
1. Production readiness changes
2. Security configuration result
3. Database readiness result
4. File storage readiness result
5. Docker/GitHub Actions result
6. Docs updated
7. Build/test result
8. Remaining production blockers