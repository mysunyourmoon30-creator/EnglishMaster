Perform Pilot Bug Fix Sprint for EnglishMaster MVP v0.1.0.

Goal:
Fix only bugs found during UAT / pilot testing.

Do not add new modules.
Do not add new features unless required to unblock MVP usage.
Do not redesign architecture.

Bug priority rules:
- P0: app cannot start, login broken, database broken, admin unusable
- P1: major CRUD broken, permission broken, data loss risk
- P2: validation, UX confusion, non-critical workflow issue
- P3: cosmetic or documentation issue

Tasks:

1. Review known issues
Check:
- GitHub Issues if available
- docs/testing/uat-plan-v0.1.0.md
- docs/testing/pilot-checklist-v0.1.0.md
- docs/release/known-limitations.md

2. Fix P0 and P1 first
Focus on:
- startup
- migration
- login/logout
- admin access
- role/permission
- CRUD
- import/export
- publishing
- file/media safety

3. Fix P2 only if quick and safe

4. Do not fix P3 unless documentation-only.

5. Add verification notes
Update:
- docs/testing/pilot-bug-fix-log.md

Document:
- issue
- priority
- root cause
- fix
- test result

6. Quality
Run:
- dotnet build
- dotnet test

Output:
1. Bugs reviewed
2. P0 fixed
3. P1 fixed
4. P2 fixed if any
5. Remaining issues
6. Build/test result
7. MVP stability status