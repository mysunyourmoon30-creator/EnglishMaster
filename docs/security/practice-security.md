# Practice Security

## Authentication

All `/api/v1/me/practice` endpoints require an authenticated user.

The API resolves the current user from the authenticated principal and never accepts a student id from the request body or route.

## Ownership

Practice data is scoped through `StudentProfileId`.

Users can only access:

- Their own practice items.
- Their own practice sessions.
- Their own practice history.
- Their own practice summary.

Cross-user access is rejected by repository ownership checks.

## Content Visibility

Practice generation only uses content that is safe for learner-facing practice:

- Active words.
- Active grammar rules.
- Active minimal pairs.
- Active and published quizzes with weak or failed learner attempts.

Inactive content is excluded. Draft, in-review, changes-requested, archived, or private content should remain excluded as those lifecycle states are added to each content module.

## Quiz Answer Safety

Quiz review items do not expose quiz correct answers in `AnswerText`.

Public or learner-facing quiz answer checking should be handled by a server-side submission endpoint instead of embedding answer keys in page or API payloads.

## Input Safety

Practice result input is validated against:

```text
Again
Hard
Good
Easy
```

Invalid values return a bad request.

## Mutation Safety

- A session item can only be submitted once.
- A session can only be completed once.
- Suspended items are excluded from due practice.

## Known Risks

- Practice tables still need a reviewed EF migration.
- More detailed content lifecycle filters should be added when every content module has uniform draft/review/archive states.
