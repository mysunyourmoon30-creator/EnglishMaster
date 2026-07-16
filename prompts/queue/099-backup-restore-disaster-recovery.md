Create Backup, Restore, and Disaster Recovery documentation and checks for EnglishMaster.

Project:
EnglishMaster

Goal:
Prepare basic operational safety for production.

Do not add paid services.
Do not add cloud-specific implementation unless already present.
Do not add complex enterprise DR tooling.
Do not redesign architecture.

Tasks:

1. Database Backup Plan
Create documentation for:
- SQL Server backup strategy.
- Daily backup recommendation.
- Weekly full backup recommendation.
- Backup retention recommendation.
- How to store backups safely.
- How to verify backup files.
- How to restore to staging first.

2. File Storage Backup Plan
Document backup strategy for:
- Media files.
- Published artifacts.
- Import/export files if retained.
- Local storage folders.
- Future cloud storage note.

3. Restore Procedure
Create step-by-step restore docs:
- Restore database.
- Restore media files.
- Restore published artifacts.
- Apply migrations if needed.
- Verify app health.
- Verify login.
- Verify admin pages.
- Verify public pages.

4. Disaster Recovery Runbook
Create:
- docs/operations/disaster-recovery-runbook.md

Include:
- Database failure
- File storage failure
- App container failure
- Migration failure
- Bad deployment rollback
- Lost admin access
- Secret/key rotation note

5. Operational Checklist
Create:
- docs/operations/backup-restore-checklist.md

6. Optional Scripts
If safe and simple, create placeholder scripts:
- scripts/backup-database.example.ps1
- scripts/restore-database.example.ps1

Scripts must:
- use placeholders
- not contain real secrets
- be clearly marked as examples

7. Quality
Run:
- dotnet build
- dotnet test

Do not change application code unless needed for documentation references.

Output:
1. Backup docs created
2. Restore docs created
3. DR runbook created
4. Example scripts created if applicable
5. Build/test result
6. Remaining operational risks