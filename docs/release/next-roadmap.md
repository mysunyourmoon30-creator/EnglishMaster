# Next Roadmap

## Recommended Next Step

Deploy the current MVP to a staging environment and complete the staging smoke test from `docs/release/mvp-release-checklist.md`.

## Staging Hardening

1. Add a dedicated app health endpoint and document the load balancer or uptime check path.
2. Configure production-grade logging sinks, metrics, and alerting.
3. Decide whether staging and production migrations are applied by startup or by an explicit release command.
4. Run Docker Compose validation and container startup on a Docker-enabled machine.
5. Add a small set of end-to-end smoke tests for login, admin dashboard, and one content workflow.

## Security Follow-Up

1. Add audit logging for user, role, permission, publishing, and import actions.
2. Add account lockout and password reset flows.
3. Add MFA for privileged users.
4. Add permission-aware navigation and UI command visibility.
5. Add more explicit `401` and `403` integration coverage for high-risk endpoints.

## Content Operations Follow-Up

1. Add Category and Tag import.
2. Add structured import for Lessons, Courses, Books, and Quizzes.
3. Add import preview and dry-run mode.
4. Add full Quiz question and choice export where appropriate.

## Performance And Operations Follow-Up

1. Add paginated Category and Tag APIs before large catalogs.
2. Add typeahead lookup endpoints for large dropdowns.
3. Move publishing and large import/export work to background processing.
4. Add real PDF and DOCX rendering after choosing libraries deliberately.

## Product Roadmap

After staging stabilizes, the next business module should be Student Progress. Do not begin it until the MVP release candidate has passed staging smoke testing.
