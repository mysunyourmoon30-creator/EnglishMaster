Prepare final v0.2.0 Release Candidate commit and tag instructions.

Project:
EnglishMaster

Goal:
Prepare clean release candidate summary and safe tag instructions.

Important:
Do not create new features.
Do not deploy production.
Do not push tags automatically unless explicitly allowed by the repository workflow.
Do not include secrets.

Final checks:
- dotnet restore passes
- dotnet build passes
- dotnet test passes
- no temporary files
- no secrets
- no real production credentials
- no unsafe upload paths
- no unnecessary generated files
- documentation updated
- release notes updated
- UAT plan created
- smoke test docs created
- rollback plan created
- known limitations documented
- admin routes protected
- /me endpoints protected
- public endpoints safe
- quiz answers protected
- student data private
- existing modules still work

Provide:

1. Git status summary

2. Suggested final commit message:
Use format:
release: prepare EnglishMaster v0.2.0 release candidate

3. Pull Request title:
Release Candidate: EnglishMaster v0.2.0

4. Pull Request description:
Include:
- Summary
- Modules included
- Security checklist
- Testing evidence
- Documentation evidence
- Known limitations
- Rollback plan
- UAT status
- Release blockers if any

5. Suggested tag:
v0.2.0-rc.1

6. Safe tag commands:
Provide commands only, do not execute:
- git tag -a v0.2.0-rc.1 -m "EnglishMaster v0.2.0 Release Candidate 1"
- git push origin v0.2.0-rc.1

7. Release blocker list:
Classify:
- Blocker
- High
- Medium
- Low

8. Final recommendation:
Say whether the project is:
- Ready for UAT
- Ready for RC tag
- Blocked

Do not create new code unless needed to fix release blockers.