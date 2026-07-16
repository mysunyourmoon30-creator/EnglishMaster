# Motivation Security

## Authentication And Ownership

All `/api/v1/me/motivation`, `/api/v1/me/streak`, and `/api/v1/me/achievements` endpoints require authentication.

Learner motivation data is scoped by `StudentProfileId`; users can only access their own activity, streak, and achievement records.

## Admin Permissions

Achievement definition administration uses:

```text
achievements.read
achievements.manage
motivation.read
```

Read endpoints require `achievements.read`. Mutation and seed endpoints require `achievements.manage`.

## Privacy

Activity metadata must not store sensitive auth data. The implementation strips obvious password, token, secret, api_key, authorization, cookie, and credential keys.

## Known Risks

- More robust structured metadata validation should be added if metadata expands.
- Automatic event integration still needs workflow-by-workflow review.
