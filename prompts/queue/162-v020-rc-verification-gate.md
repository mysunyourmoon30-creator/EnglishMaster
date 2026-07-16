Prompt 162 - v0.2.0 RC Verification Gate

Project:
EnglishMaster

Current Status:
- v0.2.0 implementation, docs, and v0.3.0 planning are complete.
- Repository is expected to be clean before this gate starts.
- v0.2.0-rc.1 has been created and pushed.
- v0.2.0 final tag may already exist, but production deployment is not complete.

Goal:
Verify whether v0.2.0 is ready to proceed from release candidate status to final release and production go-live without adding new features.

Important:
Do not implement v0.3.0 features.
Do not change product behavior.
Do not deploy production.
Do not mark manual UAT or staging checks as passed without evidence.
Do not create or move release tags without explicit approval.

Checks:
1. Verify repository state:
- git status
- git log
- local tags
- remote tags

2. Run automated verification:
- dotnet restore
- dotnet build
- dotnet test

3. Review release evidence:
- docs/release/v0.2.0-rc-decision.md
- docs/testing/v0.2.0-uat-results.md
- docs/testing/v0.2.0-staging-validation-results.md
- docs/release/v0.2.0-final-release-decision.md
- docs/release/v0.2.0-go-live-checklist.md
- docs/release/v0.2.0-rollback-plan.md
- docs/testing/v0.2.0-defect-log.md

4. Produce gate result:
- Ready for UAT only
- Ready for staging only
- Ready for final release
- Blocked

5. Document gaps:
- Missing UAT evidence
- Missing staging deployment evidence
- Missing health-check evidence
- Missing role/permission spot checks
- Missing backup/rollback readiness evidence
- Missing operations sign-off
- Any tag hygiene issue

Expected Output:
- docs/release/v0.2.0-rc-verification-gate.md
- Final recommendation
- Explicit Go / No-Go for v0.3.0 implementation

Acceptance Criteria:
- Automated verification results are current.
- Release/UAT/staging evidence is reviewed honestly.
- Production deployment remains untouched.
- v0.3.0 implementation remains blocked unless v0.2.0 release gate is accepted.
