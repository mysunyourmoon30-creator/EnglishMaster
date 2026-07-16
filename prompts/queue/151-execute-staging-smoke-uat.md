Execute EnglishMaster v0.2.0 staging smoke test and UAT validation.

Project:
EnglishMaster

Current Status:
- v0.2.0-rc.1 prepared or tagged.
- Staging deployment checklist exists.
- Staging validation plan exists.
- Feature freeze is active.

Goal:
Validate v0.2.0 RC on staging or staging-like environment.

Important:
Do not add new features.
Do not deploy production.
Do not mark manual staging tests as passed without evidence.
Do not change business behavior unless fixing a release blocker.
Fix only Blocker or High defects.

Read:
- docs/release/v0.2.0-staging-deployment-checklist.md
- docs/testing/v0.2.0-staging-validation-plan.md
- docs/testing/v0.2.0-smoke-test-results.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-known-limitations.md

Tasks:

1. Run automated checks:
- dotnet restore
- dotnet build
- dotnet test

2. Run staging validation scripts if available:
- health check scripts
- smoke test scripts
- release validation scripts

3. Validate critical staging flows:
Public:
- Home page loads
- Public search works
- Dictionary works
- Course detail works
- Lesson detail works
- Grammar page works
- Book detail works
- Quiz preview does not expose correct answers

Student:
- Login works
- Student dashboard works
- Continue learning works
- Recommendations work
- Practice session works
- Study plan works
- Goals work
- Achievements work
- Weekly report works

Admin:
- Admin dashboard works
- Word management works
- Lesson management works
- Course management works
- Quiz management works
- Content review works
- Publishing preview works
- Content quality check works
- Import validation works
- Bulk operation works
- Reports work
- Notifications work

Security:
- Admin pages require login
- /me endpoints require login
- Users cannot access another user's data
- Draft content is hidden publicly
- Quiz correct answers are hidden before submit
- No internal file paths exposed
- No sensitive auth fields exposed

Operations:
- Health endpoint works
- Database connectivity works
- Logs do not expose secrets
- Backup/rollback docs are available

4. Create staging validation result:
- docs/testing/v0.2.0-staging-validation-results.md

Include:
- Environment
- RC tag/version
- Test date
- Commands executed
- Build result
- Test result
- Manual validation status
- Passed checks
- Failed checks
- Open defects
- Blockers
- High issues
- Medium issues
- Low issues
- Evidence notes
- Final staging status:
  - Passed
  - Passed with issues
  - Failed

5. Update defect log:
- docs/testing/v0.2.0-defect-log.md

Add staging defects with:
- Defect ID
- Area
- Severity
- Steps
- Expected
- Actual
- Status
- Notes

6. Fix only:
- Blocker defects
- High security defects
- Student data privacy defects
- Public content exposure defects
- Quiz answer exposure defects
- Build/test failures

After any fix, rerun:
- dotnet build
- dotnet test

Output:
1. Commands executed
2. Build result
3. Test result
4. Staging validation result file created
5. Defects found
6. Fixes applied, if any
7. Updated defect log summary
8. Remaining blockers
9. Staging readiness recommendation