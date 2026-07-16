Prepare final EnglishMaster v0.2.0 release package and post-release monitoring checklist.

Project:
EnglishMaster

Current Status:
- v0.2.0 final release decision prepared.
- Feature freeze is active.
- Release is ready or nearly ready.

Goal:
Prepare final release package documentation and post-release monitoring plan.

Important:
Do not add new features.
Do not deploy production.
Do not push tags automatically.
Do not change code unless fixing release documentation errors.
Do not remove known limitations.

Tasks:

1. Final release notes:
Update:
- docs/release/v0.2.0-release-notes.md

Include final sections:
- Release version: v0.2.0
- Release date placeholder
- Summary
- Major modules
- Student features
- Admin features
- Content operations features
- Security improvements
- Operations improvements
- Known limitations
- Upgrade notes
- Rollback notes

2. Final changelog:
Create or update:
- CHANGELOG.md

Add:
- v0.2.0
- Added
- Changed
- Fixed
- Security
- Documentation
- Known limitations

3. Final go-live checklist:
Update:
- docs/release/v0.2.0-go-live-checklist.md

Ensure checklist includes:
- Final tag ready
- Build/test passed
- Staging validation passed
- Database backup ready
- File backup ready
- Environment variables ready
- Migration plan ready
- Admin account ready
- Health check ready
- Smoke test ready after deploy
- Rollback plan ready
- Monitoring owner assigned

4. Post-release monitoring:
Create:
- docs/operations/v0.2.0-post-release-monitoring.md

Include first 24 hours checks:
- App health
- Login success/failure
- Admin dashboard
- Public search
- Student dashboard
- Quiz attempts
- Practice sessions
- Study plans
- Import jobs
- Publish jobs
- Email messages
- Notifications
- Error logs
- Database health
- Disk space
- Backup status

Include first 7 days checks:
- Student activity
- Failed jobs
- Performance issues
- Security issues
- Data privacy reports
- Content review issues
- Import/export issues
- Public page errors

5. Hotfix process:
Create or update:
- docs/release/v0.2.0-hotfix-process.md

Include:
- Hotfix branch naming
- Allowed hotfix scope
- Build/test requirement
- Security review requirement
- Tag format:
  - v0.2.1
  - v0.2.2
- Rollback condition
- Communication note

6. Production smoke test after deployment:
Create:
- docs/testing/v0.2.0-production-smoke-test.md

Include:
- Home page
- Login
- Admin dashboard
- Public search
- Student dashboard
- Practice
- Study plan
- Weekly report
- Content review
- Publishing
- Import validation
- Health endpoint

7. Final release summary:
Create:
- docs/release/v0.2.0-final-summary.md

Include:
- What was delivered
- What was deferred
- Release readiness
- Test evidence
- Security evidence
- Operations evidence
- Recommended next roadmap phase

8. Run:
- dotnet build
- dotnet test

Output:
1. Release notes updated
2. Changelog updated
3. Go-live checklist updated
4. Post-release monitoring doc created
5. Hotfix process doc created
6. Production smoke test doc created
7. Final summary created
8. Build/test result
9. Final v0.2.0 release package status