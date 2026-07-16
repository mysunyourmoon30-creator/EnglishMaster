# MVP Known Limitations

## Release Scope

This MVP is an admin-focused content management foundation for EnglishMaster. It is not yet a learner-facing production platform.

## Security And Administration

- Admin navigation is not dynamically filtered by permission. API policies remain the security boundary.
- Permission claims refresh on login, not continuously.
- No dedicated audit log exists for security-sensitive actions.
- No account lockout, MFA, or password reset flow is implemented yet.
- Correct quiz answers are available in admin DTOs. Future public quiz-taking APIs must use separate learner-safe DTOs.

## Operations

- API and Web expose `/health`, `/health/live`, and `/health/ready`; hosted staging still needs environment-specific monitoring and alerting.
- Production logging sinks, alerting, metrics, and tracing are not configured.
- Docker Compose is for local development and staging preparation only. No cloud deployment workflow exists yet.
- API startup currently applies migrations outside Testing through the security seed flow. Staging and production should review and control migration application deliberately.

## Data And Seed

- Development seed data is for local and demo usage only.
- Initial SuperAdmin creation depends on environment/configuration values.
- Seed data should be disabled outside local development.

## Import / Export

- Only Words import is implemented.
- Import files are capped at 1 MB and processed synchronously.
- Exports are flat administrative snapshots, not full relational backups.
- Quiz export does not include full question and choice bodies in the current MVP.

## Publishing

- Publish jobs run synchronously through the API.
- PDF and DOCX outputs are placeholder text-file implementations.
- Templates are stored but not fully applied by the basic content builder.
- No background queue, retry policy, or progress event stream exists yet.

## Performance

- Category and Tag search APIs are lookup-style and not paginated.
- Large production catalogs will need typeahead endpoints for dropdowns.
- Heavy export and publishing workloads should wait for a background worker design.

## UX

- Admin pages are functional MVP pages, not a polished final content operations suite.
- Minimal Pair is managed inside Pronunciation pages and has no standalone admin area.
- Grammar Examples are mostly managed inside Grammar Rules and have only a standalone edit route.

## Deployment

- Docker CLI was not available in the current verification environment, so Compose runtime validation must be completed on a Docker-enabled machine.
- Real staging secrets, SQL connection strings, host names, HTTPS certificates, and storage locations must be supplied by the staging environment.
