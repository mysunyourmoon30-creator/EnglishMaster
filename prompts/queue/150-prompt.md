Prepare EnglishMaster v0.2.0 RC tag and staging deployment validation.

Project:
EnglishMaster

Current Status:
- v0.2.0 Release Candidate decision completed.
- Smoke test executed.
- UAT tracker created.
- Bug fix sprint completed.
- RC decision document exists.
- Feature freeze is active.

Goal:
Prepare and validate v0.2.0-rc.1 for staging deployment.

Important:
Do not add new features.
Do not deploy production.
Do not push tags automatically unless explicitly approved.
Do not hide blockers.
Do not introduce architecture changes.

Tasks:

1. Read release documents:
- docs/release/v0.2.0-rc-decision.md
- docs/testing/v0.2.0-smoke-test-results.md
- docs/testing/v0.2.0-uat-results.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-release-notes.md
- docs/release/v0.2.0-known-limitations.md
- docs/release/v0.2.0-go-live-checklist.md
- docs/release/v0.2.0-rollback-plan.md

2. Final pre-staging checks:
Run:
- dotnet restore
- dotnet build
- dotnet test

Verify:
- No Blocker defects remain open.
- No High security defects remain open.
- No student data privacy defect remains open.
- No public draft content exposure remains open.
- No quiz answer exposure remains open.
- No secrets are committed.
- Release notes exist.
- Rollback plan exists.

3. Prepare RC tag instructions:
Recommended tag:
- v0.2.0-rc.1

Provide commands only. Do not execute unless explicitly allowed:
- git tag -a v0.2.0-rc.1 -m "EnglishMaster v0.2.0 Release Candidate 1"
- git push origin v0.2.0-rc.1

4. Staging environment checklist:
Verify docs/config examples exist:
- appsettings.Staging.example.json
- .env.example
- docker-compose.staging.yml if available
- docs/deployment/production-environment-variables.md
- docs/deployment/production-database.md
- docs/deployment/production-file-storage.md

5. Staging deployment checklist:
Create or update:
- docs/release/v0.2.0-staging-deployment-checklist.md

Include:
- RC tag verified
- Build artifact verified
- Staging database backup ready
- Staging file storage ready
- Environment variables configured
- Migration plan ready
- Initial admin account ready
- Health check URL ready
- Smoke test owner
- Rollback owner
- Deployment start time
- Deployment end time
- Deployment result

6. Staging validation plan:
Create or update:
- docs/testing/v0.2.0-staging-validation-plan.md

Include validation for:
- App starts
- Database connects
- Login works
- Admin dashboard works
- Public home works
- Public search works
- Student dashboard works
- Practice works
- Study plan works
- Weekly report works
- Content review works
- Publishing preview works
- Import validation works
- Bulk operation works
- Health endpoints work

7. Do not perform production deployment.

Output:
1. Final build result
2. Final test result
3. RC tag command summary
4. Staging checklist created or updated
5. Staging validation plan created or updated
6. Open blockers
7. Recommendation:
   - Ready for RC tag
   - Ready for staging
   - Blocked