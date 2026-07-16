Prepare EnglishMaster v0.2.0 production go-live execution.

Project:
EnglishMaster

Current Status:
- v0.2.0 final release package completed.
- Final release notes completed.
- Final decision completed.
- Go-live checklist completed.
- Rollback plan completed.
- Post-release monitoring plan completed.
- Feature freeze is active.

Goal:
Prepare production go-live safely.

Important:
Do not add new features.
Do not redesign architecture.
Do not deploy automatically unless explicitly approved.
Do not push final tag automatically unless explicitly approved.
Do not hide blockers.

Tasks:

1. Read:
- docs/release/v0.2.0-final-release-decision.md
- docs/release/v0.2.0-go-live-checklist.md
- docs/release/v0.2.0-rollback-plan.md
- docs/testing/v0.2.0-production-smoke-test.md
- docs/operations/v0.2.0-post-release-monitoring.md
- docs/release/v0.2.0-hotfix-process.md
- docs/release/v0.2.0-final-summary.md

2. Run final local checks:
- dotnet restore
- dotnet build
- dotnet test

3. Verify release readiness:
- No Blocker defects open.
- No High security defects open.
- No High privacy defects open.
- No public draft content exposure.
- No quiz answer exposure.
- No secrets committed.
- Production environment variables are documented.
- Backup plan exists.
- Rollback plan exists.
- Production smoke test exists.
- Post-release monitoring plan exists.

4. Prepare final tag command only:
Do not execute unless explicitly approved.

Commands:
- git tag -a v0.2.0 -m "EnglishMaster v0.2.0"
- git push origin v0.2.0

5. Create or update:
- docs/release/v0.2.0-go-live-execution-record.md

Include:
- Release version
- Release owner
- Planned deployment date
- Pre-deploy checklist
- Backup status
- Migration status
- Final build result
- Final test result
- Final tag status
- Deployment approval status
- Rollback owner
- Go / No-Go decision

Output:
1. Final build result
2. Final test result
3. Go-live execution record updated
4. Safe final tag commands
5. Remaining blockers
6. Go / No-Go recommendation