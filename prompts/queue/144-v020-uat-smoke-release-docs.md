Create v0.2.0 UAT, Smoke Test, and Release Documentation for EnglishMaster.

Project:
EnglishMaster

Goal:
Prepare final manual testing and release documentation for v0.2.0.

Important:
Do not add new features.
Do not change application code unless documentation references are broken.
Do not deploy production.

Create or update docs:

1. docs/release/v0.2.0-release-notes.md
Include:
- Release summary
- New modules
- Improved modules
- Security improvements
- Operations improvements
- Known limitations
- Upgrade notes
- Rollback notes

2. docs/release/v0.2.0-known-limitations.md
Include:
- No AI tutor yet
- No mobile app
- No payment
- No marketplace
- No external email provider yet
- No external search engine
- No advanced analytics
- No leaderboard/social features
- Any technical limitations found

3. docs/testing/v0.2.0-smoke-test.md
Include manual smoke tests for:
- Login
- Admin dashboard
- Public home
- Public search
- Dictionary
- Course detail
- Lesson detail
- Quiz practice
- Student dashboard
- Practice session
- Study plan
- Recommendations
- Achievements
- Weekly report
- Admin content workflow
- Publishing preview
- Import validation
- Content quality check
- Bulk operation
- Reports
- Notifications
- Health checks

4. docs/testing/v0.2.0-uat-plan.md
Include:
- UAT objective
- Test roles
- Test data required
- Admin test scenarios
- Reviewer test scenarios
- ContentEditor test scenarios
- Student test scenarios
- Public visitor test scenarios
- Acceptance criteria
- Defect severity levels
- Sign-off checklist

5. docs/testing/v0.2.0-regression-checklist.md
Include:
- v0.1.0 features that must still work
- Admin modules that must still work
- Public modules that must still work
- Student modules that must still work
- Security regression checks
- Performance regression checks

6. docs/release/v0.2.0-go-live-checklist.md
Include:
- Build/test passed
- Migration backup ready
- Database backup ready
- File backup ready
- Environment variables ready
- Admin account ready
- Health check ready
- Smoke test passed
- UAT sign-off
- Rollback plan ready

7. docs/release/v0.2.0-rollback-plan.md
Include:
- Database rollback concept
- File storage rollback concept
- App version rollback
- Migration rollback caution
- Restore from backup
- Post-rollback smoke test

Also update:
- docs/roadmap/v0.2.0-roadmap.md
- README.md if release summary is outdated

Run:
- dotnet build
- dotnet test

Output:
1. Release docs created or updated
2. Smoke test summary
3. UAT plan summary
4. Regression checklist summary
5. Go-live checklist summary
6. Rollback plan summary
7. Build/test result
8. Remaining release documentation gaps