Run v0.2.0 Bug Fix Sprint for release blockers only.

Project:
EnglishMaster

Current Status:
- v0.2.0 Release Candidate prepared.
- Smoke test results exist.
- UAT tracker exists.
- Defect log exists.
- Feature freeze is active.

Goal:
Fix only release blockers and high-priority defects.

Important:
Do not add new features.
Do not redesign architecture.
Do not add AI.
Do not refactor unrelated code.
Do not fix low priority cosmetic issues unless they break release.
Do not change documented known limitations into new features.

Read:
- docs/testing/v0.2.0-smoke-test-results.md
- docs/testing/v0.2.0-uat-results.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-known-limitations.md

Fix priority:
1. Blocker defects
2. High defects
3. Security defects
4. Build/test failures
5. Data privacy issues
6. Public content exposure issues
7. Quiz answer exposure issues

Do not fix:
- New feature requests
- Deferred limitations
- Nice-to-have UI polish
- AI tutor
- Mobile
- Payment
- Marketplace
- External search
- Advanced analytics

Required checks after each fix:
- dotnet build
- dotnet test

Areas to verify after fixes:
- Admin authorization
- /me endpoint privacy
- Public content safety
- Quiz answer safety
- Import file safety
- Student data privacy
- Route stability
- Dashboard stability
- API error safety

Update:
- docs/testing/v0.2.0-defect-log.md

For each fixed defect:
- Set Status to Fixed or Retest.
- Add fix summary.
- Add test evidence.
- Add commit note if useful.

Create or update:
- docs/release/v0.2.0-bug-fix-summary.md

Include:
- Defects fixed
- Defects deferred
- Remaining blockers
- Test evidence
- Release risk

Output:
1. Defects reviewed
2. Defects fixed
3. Files changed
4. Build result
5. Test result
6. Defect log updated
7. Bug fix summary updated
8. Remaining blockers
9. Release readiness recommendation