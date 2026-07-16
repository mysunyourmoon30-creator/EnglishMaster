Prepare final EnglishMaster v0.2.0 release decision.

Project:
EnglishMaster

Current Status:
- v0.2.0 RC prepared.
- Staging validation executed or tracked.
- Feature freeze is active.

Goal:
Decide whether v0.2.0 is ready for final release.

Important:
Do not add new features.
Do not deploy production.
Do not push final release tag automatically.
Do not hide blockers.
Do not mark release ready if blockers remain.

Read:
- docs/testing/v0.2.0-staging-validation-results.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-bug-fix-summary.md
- docs/release/v0.2.0-release-notes.md
- docs/release/v0.2.0-known-limitations.md
- docs/release/v0.2.0-go-live-checklist.md
- docs/release/v0.2.0-rollback-plan.md
- docs/release/v0.2.0-rc-decision.md

Final checks:
Run:
- dotnet restore
- dotnet build
- dotnet test

Verify:
- No Blocker defects remain open.
- No High security defects remain open.
- No High data privacy defects remain open.
- No public draft content exposure remains open.
- No quiz correct answer exposure remains open.
- No secrets are committed.
- Staging validation result exists.
- Release notes exist.
- Known limitations are documented.
- Go-live checklist exists.
- Rollback plan exists.
- Backup/restore docs exist.

Create or update:
- docs/release/v0.2.0-final-release-decision.md

Include:

1. Release readiness status:
- Ready for final release
- Ready with accepted limitations
- Blocked

2. Evidence:
- Build result
- Test result
- Staging validation result
- Smoke test result
- UAT status
- Security status
- Defect status

3. Open defects:
Group by:
- Blocker
- High
- Medium
- Low
- Deferred

4. Accepted limitations:
Summarize:
- No AI tutor
- No mobile app
- No payment
- No marketplace
- No external email provider
- No external search engine
- No advanced analytics
- No leaderboard/social features

5. Final release tag:
Recommended:
- v0.2.0

6. Safe final tag commands:
Provide commands only. Do not execute:
- git tag -a v0.2.0 -m "EnglishMaster v0.2.0"
- git push origin v0.2.0

7. Production go-live recommendation:
- Ready for production deployment
- Ready for staging only
- Blocked

8. Next step:
- If ready: create v0.2.0 final tag and follow go-live checklist.
- If blocked: fix blockers only.

Output:
1. Final build result
2. Final test result
3. Final release decision document updated
4. Open blocker summary
5. Safe final tag commands
6. Final recommendation