Perform production readiness review for EnglishMaster MVP v0.1.0.

Project:
EnglishMaster

Goal:
Decide what is required before production deployment.

Do not deploy automatically.
Do not add new features.
Only review, document, and fix critical blockers.

Check:

1. Security
- No secrets committed
- Admin routes protected
- API routes protected
- Roles and permissions working
- File upload safe
- Import/export safe
- Production settings documented

2. Database
- Migrations valid
- Backup plan documented
- Restore plan documented
- Seed data safe
- SuperAdmin setup safe

3. Deployment
- Docker config valid
- Staging config valid
- Production example config exists
- Environment variables documented
- Health checks exist

4. Observability
- Logging configured
- Error handling safe
- Health checks documented
- System health page protected

5. Performance
- Pagination exists
- Search limits exist
- File upload limits exist
- Known performance risks documented

6. Documentation
Create or update:
- docs/deployment/production-readiness.md
- docs/deployment/backup-and-restore.md
- docs/release/production-go-live-checklist.md

7. Output
Provide:
- Production readiness status: Ready / Not Ready
- Critical blockers
- Non-critical risks
- Required production environment variables
- Backup/restore notes
- Recommended go-live steps