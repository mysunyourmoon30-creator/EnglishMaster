Prepare final EnglishMaster v0.2.0 RC decision and tag preparation.

Project:
EnglishMaster

Current Status:
- v0.2.0 Release Candidate prepared.
- Smoke test executed or tracked.
- UAT tracker created.
- Bug fix sprint completed.
- Feature freeze is active.

Goal:
Decide whether v0.2.0 is ready for RC tag or still blocked.

Important:
Do not add new features.
Do not deploy production.
Do not push tags automatically.
Do not hide blockers.
Do not mark as ready if blockers remain.

Read:
- docs/testing/v0.2.0-smoke-test-results.md
- docs/testing/v0.2.0-uat-results.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-bug-fix-summary.md
- docs/release/v0.2.0-release-notes.md
- docs/release/v0.2.0-known-limitations.md
- docs/release/v0.2.0-go-live-checklist.md
- docs/release/v0.2.0-rollback-plan.md

Final checks:
- dotnet restore
- dotnet build
- dotnet test

Verify:
- No Blocker defects remain open.
- No High security defects remain open.
- No known quiz answer exposure.
- No known student data privacy issue.
- No known public draft content exposure.
- No secrets committed.
- Release notes exist.
- Smoke test results exist.
- UAT tracker exists.
- Rollback plan exists.
- Known limitations are documented.

Create or update:
- docs/release/v0.2.0-rc-decision.md

Include:
1. RC readiness status:
   - Ready for RC tag
   - Ready for UAT only
   - Blocked

2. Evidence:
   - Build result
   - Test result
   - Smoke test result
   - UAT status
   - Security status
   - Defect status

3. Open defect summary:
   - Blocker
   - High
   - Medium
   - Low

4. Known limitations:
   - Link or summarize from known limitations doc

5. Recommended tag:
   - v0.2.0-rc.1

6. Safe tag commands:
Do not execute. Provide commands only:
- git tag -a v0.2.0-rc.1 -m "EnglishMaster v0.2.0 Release Candidate 1"
- git push origin v0.2.0-rc.1

7. Next step:
- If ready: tag RC and deploy to staging.
- If not ready: fix blockers only.

Output:
1. Final build result
2. Final test result
3. RC decision document updated
4. Open blocker summary
5. Safe tag commands
6. Final recommendation