Create Production Monitoring and Operations Checklist for EnglishMaster.

Project:
EnglishMaster

Goal:
Prepare basic operational monitoring for production usage.

Do not add paid monitoring services.
Do not add complex observability stack.
Do not add microservices.
Keep it simple and practical.

Tasks:

1. Logging Review
Verify or document:
- Application logs exist.
- Error logs are safe.
- Sensitive data is not logged.
- Login failures are logged without passwords.
- Import/export failures are logged.
- Publish job failures are logged.
- Email failures are logged.
- Critical exceptions are logged.

2. Health Monitoring
Document:
- /health usage.
- /health/live usage.
- /health/ready usage.
- How to check database connectivity.
- How to detect app startup issues.

3. Production Operations Checklist
Create:
- docs/operations/production-operations-checklist.md

Include daily checks:
- App health
- Database health
- Disk space
- Backup status
- Error logs
- Failed publish jobs
- Failed email messages
- Failed imports
- Login/auth issues

4. Incident Response
Create:
- docs/operations/incident-response.md

Include:
- Severity levels P0/P1/P2/P3
- Who responds
- What to check first
- How to rollback
- How to communicate issue
- How to document root cause

5. Admin Operations Page Review
If system health admin page exists, verify docs.
If not, document TODO rather than building large dashboard.

6. Alerting TODO
Document future alerting options:
- Email alerts
- Slack/Teams alerts
- Cloud monitoring
- Uptime monitor

Do not implement unless already simple.

7. Quality
Run:
- dotnet build
- dotnet test

Output:
1. Monitoring docs created
2. Operations checklist created
3. Incident response docs created
4. Logging/health review result
5. Build/test result
6. Remaining monitoring limitations