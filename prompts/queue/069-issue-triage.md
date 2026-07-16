Create MVP issue triage workflow for EnglishMaster after v0.1.0 release.

Project:
EnglishMaster

Goal:
Prepare a workflow for handling bugs and improvements after pilot testing.

Do not add new features.
Only create process documentation and GitHub issue guidance.

Tasks:

1. Create issue triage docs
Create:
- docs/project/issue-triage-workflow.md

2. Define Issue Types
- Bug
- Security
- UX
- Performance
- Content
- Documentation
- Enhancement

3. Define Priority
- P0 Critical
- P1 High
- P2 Medium
- P3 Low

4. Define Workflow
- New
- Confirmed
- In Progress
- Review
- Ready for Test
- Done
- Deferred

5. Define Bug Fix Rules
- P0 must be fixed immediately.
- Security issues must be reviewed before merge.
- No new features during stabilization unless approved.
- Every bug fix must include test or manual verification note.

6. Create templates if useful:
- .github/ISSUE_TEMPLATE/bug_report.md
- .github/ISSUE_TEMPLATE/security_issue.md
- .github/ISSUE_TEMPLATE/feature_request.md

7. Output
Provide:
- Triage workflow summary
- Issue templates created
- Recommended GitHub Project columns
- Recommended stabilization period