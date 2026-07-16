# Learning Recommendation Security

## Authentication

All `/api/v1/me/learning/*` endpoints require authentication.

## User Isolation

Recommendations are always scoped to the current user id claim. The API does not accept a student id parameter. If a valid user id claim is missing, endpoints return unauthorized.

## Public Content Visibility

Recommendations may point only to content safe for learners:

- Lessons, courses, books, quizzes: active and published.
- Words, grammar: active.
- Draft, inactive, archived, review-only, or admin-only content must not appear.

## Quiz Answer Safety

Recommendation DTOs contain quiz title, summary, URL, CEFR/category metadata, and reason text only. They do not include questions, choices, correct-answer flags, scoring keys, or hidden explanations.

## Operational Notes

The feature uses EF Core read queries with `AsNoTracking` where appropriate. Limits are capped to avoid large responses.

