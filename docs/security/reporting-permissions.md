# Reporting Permissions

## Purpose

Reporting permissions control access to administrative reporting data in EnglishMaster. Reporting data may include operational summaries derived from content, learning progress, quiz attempts, and recent activity, so access must be limited to authorized administrative users.

## Permission

Required permission:

- `reports.read`

## Permission Behavior

Users with `reports.read` may view basic read-only reporting data, including:

- Admin dashboard metrics.
- Content status metrics.
- Learning progress metrics.
- Quiz analytics metrics.
- Recent activity.

Users without `reports.read` must not be able to access reporting API endpoints or the Blazor admin reports page.

## Enforcement Points

The permission should be enforced at:

- API endpoint authorization.
- Blazor admin reports page authorization.
- Navigation visibility for admin reporting links.
- Any application service method that returns reporting data.

UI checks are not a substitute for server-side authorization.

## Data Privacy Rules

Reporting should follow aggregate-first privacy rules:

- Prefer counts, percentages, and summary values over individual records.
- Do not expose learner email addresses or direct personal identifiers unless required for an administrative workflow.
- Do not expose quiz answers, answer history, or sensitive assessment details in dashboard summaries.
- Bound recent activity responses to the minimum useful fields.
- Do not include secrets, tokens, internal exception details, or infrastructure identifiers in reporting output.

## Recent Activity Privacy

Recent activity should be suitable for administrative monitoring. Activity descriptions should be concise and should not include unnecessary personal data or free-form user content.

When an activity item needs to reference a user, prefer a safe display identifier and avoid exposing contact information unless explicitly required.

## Performance Security Notes

Reporting permission checks should happen before executing aggregate queries. This avoids unnecessary work for forbidden callers and reduces the risk of leaking timing or partial response details.

Large reporting queries should remain bounded. Recent activity should be capped or paginated, and reporting services should avoid loading detailed records when aggregate metrics are sufficient.

## Known Limitations

- `reports.read` grants read-only access to reporting summaries; it does not imply permission to edit content, users, quizzes, or learning data.
- The permission does not define row-level or tenant-level filtering by itself.
- Advanced audit reporting is not included in the current scope.
- Export reporting is not included and should require a separate review before implementation.

## Intentionally Not Included Yet

Reporting security does not yet cover:

- Advanced analytics authorization models.
- Charts library permission models.
- Data warehouse access controls.
- AI insights review workflows.
- Export approval workflows.
- Fine-grained report-builder permissions.
