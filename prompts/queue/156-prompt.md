---
id: 156
title: Prompt 156
status: pending
phase: release
---

Execute v0.2.0 post-release monitoring and hotfix triage.

Project:
EnglishMaster

Current Status:
- v0.2.0 production smoke test is completed or being tracked.
- Post-release monitoring document exists.
- Hotfix process exists.
- Feature freeze remains active for v0.2.0.

Goal:
Monitor the first release window and triage issues safely.

Important:
Do not add new features.
Do not start v0.3.0 feature work yet.
Do not redesign architecture.
Do not hide production issues.
Only fix hotfix-scope issues.

Read:
- docs/operations/v0.2.0-post-release-monitoring.md
- docs/testing/v0.2.0-production-smoke-test-results.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-hotfix-process.md
- docs/release/v0.2.0-rollback-plan.md
- docs/release/v0.2.0-known-limitations.md

Tasks:

1. Create or update:
- docs/operations/v0.2.0-monitoring-results.md

Include:
- Monitoring period
- App health
- Login status
- Admin status
- Public search status
- Student dashboard status
- Quiz status
- Practice status
- Study plan status
- Weekly report status
- Import job status
- Publish job status
- Email message status
- Notification status
- Error log summary
- Database health
- Disk/storage health
- Backup status

2. Triage issues:
Classify as:
- Blocker
- High
- Medium
- Low
- Deferred

3. Hotfix decision:
For each issue decide:
- Fix now
- Defer to v0.2.1
- Defer to v0.3.0
- Document as known limitation
- Rollback required

4. If hotfix is needed:
Follow hotfix process:
- Create hotfix branch.
- Fix only the issue.
- Run dotnet build.
- Run dotnet test.
- Update defect log.
- Update hotfix notes.

5. Create or update:
- docs/release/v0.2.0-hotfix-triage.md

Include:
- Issues found
- Hotfix required
- Hotfix not required
- Deferred issues
- Rollback required or not
- Release stability status

6. Final checks:
- dotnet build
- dotnet test

Output:
1. Monitoring results updated
2. Issues found
3. Triage result
4. Hotfix required or not
5. Rollback required or not
6. Build/test result
7. Release stability recommendation
