Prepare staging deployment for the EnglishMaster MVP.

Project:
EnglishMaster

Current Status:
- MVP modules completed.
- Auth, roles, permissions completed.
- Seed data completed.
- Import/export completed.
- Security hardening completed.
- Performance hardening completed.
- Logging and health checks completed.
- Docker and release checklist completed.

Goal:
Prepare the project for staging deployment.

Do not add new business modules.
Do not add AI, Mobile, Marketplace, Payment, Plugin System, or Microservices.
Do not deploy to production yet.

Tasks:

1. Review Deployment Readiness
Check:
- dotnet build passes
- dotnet test passes
- Docker files exist
- docker-compose.yml exists
- health checks exist
- environment variables are documented
- appsettings production example exists
- no secrets are committed

2. Staging Environment Configuration
Create or update:
- appsettings.Staging.example.json
- .env.example
- docs/deployment/staging.md

Document required variables:
- ASPNETCORE_ENVIRONMENT
- ConnectionStrings__DefaultConnection
- Auth / Identity settings if any
- Seed data flags
- Storage paths
- Public URL / base URL if used

3. Database Preparation
Document:
- How to create staging database
- How to apply migrations
- How to seed development/staging data safely
- How to create initial SuperAdmin safely

4. Docker Staging
Create or update:
- docker-compose.staging.yml

Requirements:
- App container
- SQL Server container if using local staging
- Environment variables only
- No real secrets
- Persistent volumes for SQL Server
- Persistent volume for media/publishing files if needed

5. Health Check
Ensure staging can verify:
- /health
- /health/live
- /health/ready

6. Documentation
Update:
- docs/deployment/staging.md
- docs/deployment/environment-variables.md
- docs/release/mvp-release-checklist.md

7. Quality
Run:
- dotnet build
- dotnet test
- docker compose config if available

Fix only deployment-related issues.

Output:
1. Staging files created or updated
2. Environment variables documented
3. Database migration instructions
4. Docker staging summary
5. Health check summary
6. Build/test result
7. Remaining staging risks