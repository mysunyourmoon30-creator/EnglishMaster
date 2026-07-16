# Changelog

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
