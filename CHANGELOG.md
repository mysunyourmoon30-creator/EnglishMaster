# Changelog

## v0.3.0 - 2026-07-18

### Added

- External email provider boundary (MailKit-based SMTP adapter, Gmail-compatible) with an automatic notification delivery queue (`EmailDeliveryWorker`), retry handling, and failure/admin review documentation.
- Certificate template management with a full admin CRUD UI, learner certificate generation, and public certificate verification.
- Admin advanced analytics and student analytics dashboards, with learning analytics aggregation.
- Self-contained system health monitoring dashboard and threshold-based email alerting (`SystemHealthWorker`), built without dependency on any specific hosting platform.

### Fixed

- Fixed release-blocking database migration and authentication/authorization defects found during the first staging validation pass (10 defects total: 8 Blocker, 2 Low/High — all fixed and verified).
- Added a `FreshDatabaseMigrationTests` regression test to close the gap that let those migration defects through undetected.

### Security

- Public certificate verification is now rate-limited (10 requests/minute per IP), verified against real HTTP 429 responses.

### Documentation

- Added v0.3.0 release candidate, UAT, staging validation, defect log, production go-live checklist, release closure, final tag execution, monitoring/alerting setup, and backup/restore verification documentation.

### Known Limitations

- Production **deployment** is not yet approved (the tag is not the same as deployment) — no hosting platform has been chosen, and no named operations owner or on-call rotation exists yet.
- Docker Compose staging re-validation was attempted but not completed, due to a hardware/firmware boot-stability issue on the development machine.
- File storage backup was verified with a single small test file, not against a production-scale media library.
- No admin-wide "list all issued certificates" browser exists yet — only the self-scoped `/me/certificates` view.
- Analytics rollups/warehouse integration are not included (not needed at current data volume).
- No production-scale email provider (e.g. SendGrid/SES/Mailgun with SPF/DKIM) is set up; Gmail SMTP is used for low volume.
- v0.4.0 candidate scope was approved by the release owner, but no v0.4.0 implementation has started.

## v0.2.0 - TBD

### Added

- Public search and public route entry points.
- Student dashboard, recommendations, practice sessions, learning goals, study plans, motivation, achievements, activity, and weekly reports.
- Admin reporting, notifications, email message foundation, publishing jobs/templates/artifacts, content quality, content revisions, restore requests, bulk operations, and advanced import validation.
- Release, smoke, UAT, staging validation, rollback, hotfix, and operations documentation.

### Changed

- Expanded EnglishMaster from MVP admin foundations into a broader learner and content operations release candidate.
- Publishing and reporting modules now include more operational surfaces and documentation.
- Public discovery routes are stabilized as entry points while dedicated detail page work remains deferred.

### Fixed

- Import upload validation rejects path-like original filenames.
- Weekly report generation avoids exposing quiz answer keys and raw activity metadata.

### Security

- Admin APIs require permission policies.
- Learner APIs scope data to the authenticated user or student profile.
- Public search uses learner-safe DTOs and published/active content filters.
- Media upload validation checks size, content type, file signature, and file names.
- Content revision and activity metadata sanitization avoid obvious sensitive fields.

### Documentation

- Added v0.2.0 release notes, known limitations, smoke/UAT plans, staging validation docs, final release decision, go-live checklist, rollback plan, hotfix process, production smoke test, and post-release monitoring plan.

### Known Limitations

- No AI tutor, mobile app, payment flow, marketplace, external email provider, external search engine, advanced analytics, leaderboard, or social features.
- Manual staging validation, manual UAT, role/permission spot checks, deployed health checks, backup readiness, and operations sign-off are required before production go-live.
