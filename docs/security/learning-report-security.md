# Learning Report Security

## Authentication

All `/api/v1/me/learning-reports` endpoints require an authenticated user.

The API resolves the current user from the authenticated principal. It does not accept a student id in request bodies, routes, or query strings.

## Ownership

Reports are scoped through `StudentProfileId`.

Learners can only access:

- Their own current week report.
- Their own report history.
- Their own report details.
- Their own report insights.

Cross-user access returns not found.

## Quiz Answer Safety

Weekly reports aggregate quiz attempt counts, pass counts, and average score only.

The API does not return:

- Correct answers.
- Answer text.
- Correctness metadata for individual quiz choices.
- Raw quiz question payloads.

## Privacy Rules

Reports do not expose raw activity metadata, authentication data, cookies, tokens, secrets, or admin-only fields.

Summary text and insights are deterministic rule text. No external service receives learner data.

## Performance Safety

History queries are paginated. Report generation uses bounded week windows and read-only queries where possible.

## Known Risks

- Testing mode auto-authenticates non-security API routes, so unauthenticated behavior is primarily enforced by endpoint configuration in integration tests.
- EF migrations still need review before deployment.
