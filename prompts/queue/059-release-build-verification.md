---
id: 059
title: Prompt 059
status: pending
phase: release
---

Create GitHub Actions workflow for EnglishMaster MVP release build.

Goal:
Create a safe CI workflow that validates the MVP before staging deployment.

Do not deploy to production.
Do not add cloud secrets.
Do not add new features.

Tasks:

1. Review existing workflows
Check .github/workflows.

2. Create or update release workflow
Create:
- .github/workflows/release-build.yml

Workflow should:
- run on pull_request to main
- run on push to main
- restore dependencies
- build solution
- run tests
- publish app artifact if safe
- upload build artifact

3. Security
- Do not print secrets.
- Do not hardcode credentials.
- Use GitHub secrets placeholders only if needed.
- Document required secrets but do not create real values.

4. Docker Validation
If Docker exists, add optional docker build validation if simple.

5. Documentation
Update:
- docs/deployment/github-actions.md
- docs/release/mvp-release-checklist.md

6. Quality
Run local checks if possible:
- dotnet build
- dotnet test

Output:
1. Workflow created or updated
2. Build steps included
3. Test steps included
4. Artifact publishing summary
5. Required secrets or variables
6. Docs updated
7. Build/test result
