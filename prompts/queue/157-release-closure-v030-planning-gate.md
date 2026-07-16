Close EnglishMaster v0.2.0 release and prepare v0.3.0 planning gate.

Project:
EnglishMaster

Current Status:
- v0.2.0 final release package completed.
- Production go-live prepared or completed.
- Production smoke test tracked.
- Post-release monitoring tracked.
- Hotfix triage completed.

Goal:
Close v0.2.0 cleanly and prepare for v0.3.0 planning.

Important:
Do not start v0.3.0 implementation yet.
Do not add new features.
Do not hide unresolved issues.
Do not delete known limitations.

Tasks:

1. Read:
- docs/release/v0.2.0-final-summary.md
- docs/release/v0.2.0-go-live-execution-record.md
- docs/testing/v0.2.0-production-smoke-test-results.md
- docs/operations/v0.2.0-monitoring-results.md
- docs/release/v0.2.0-hotfix-triage.md
- docs/testing/v0.2.0-defect-log.md
- docs/release/v0.2.0-known-limitations.md

2. Create:
- docs/release/v0.2.0-release-closure.md

Include:
- Release version
- Release status
- What was delivered
- What was deferred
- Production smoke status
- Monitoring status
- Open defects
- Deferred defects
- Known limitations
- Hotfix status
- Lessons learned
- Final release recommendation

3. Create:
- docs/roadmap/v0.3.0-planning-gate.md

Include candidate themes only:
- AI Tutor
- External Email Provider
- External Search Engine
- Advanced Analytics
- Mobile/PWA
- Payment
- Marketplace
- Certificate System
- Teacher/Admin Collaboration
- Content Authoring Improvement

For each candidate theme include:
- Business value
- User value
- Risk
- Complexity
- Dependency
- Recommended priority
- Why now / why later

4. Create:
- docs/roadmap/v0.3.0-backlog-candidates.md

Include backlog items:
- Must Have
- Should Have
- Could Have
- Won't Have for v0.3.0

5. Create:
- docs/roadmap/v0.3.0-decision-matrix.md

Use scoring:
- User impact 1-5
- Business impact 1-5
- Technical risk 1-5
- Development effort 1-5
- Operational risk 1-5
- Dependency risk 1-5
- Total priority score

6. Recommendation:
Provide top 3 recommended v0.3.0 directions.

Suggested default priority:
1. External Email Provider + Notification Delivery
2. Certificate System / Learning Completion Evidence
3. Advanced Analytics or AI Tutor Preparation

Do not implement any v0.3.0 feature yet.

7. Run:
- dotnet build
- dotnet test

Output:
1. v0.2.0 release closure document created
2. v0.3.0 planning gate created
3. v0.3.0 backlog candidates created
4. v0.3.0 decision matrix created
5. Top 3 v0.3.0 recommendations
6. Build/test result
7. Final recommendation