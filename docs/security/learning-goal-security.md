# Learning Goal And Study Plan Security

## Authentication

All `/api/v1/me/learning-goals` and `/api/v1/me/study-plan` endpoints require authentication.

The API resolves the current user from the authenticated principal. The client never sends a student id.

## Ownership

Learning goals and daily study plans are scoped through `StudentProfileId`.

Users can only access their own:

- Goals.
- Goal summary.
- Daily plans.
- Daily plan items.
- Study plan history.
- Study summary.

Cross-user access returns not found through ownership checks.

## Content Visibility

Study plan generation uses learner-facing content filters:

- Due practice items owned by the learner.
- Active and published lessons.
- Active and published quizzes for weak quiz review.

Inactive content is excluded. Draft, in-review, changes-requested, archived, and private states should remain excluded as content lifecycle coverage becomes more uniform across modules.

## Quiz Answer Safety

Study plan quiz items link to quiz review and do not include correct answers, answer keys, or correctness flags.

## Input Safety

Supported goal types are allow-listed. Negative values are rejected or clamped safely.

## Known Risks

- EF migrations still need to be generated and reviewed.
- Automatic goal progress updates are not wired into all learning events yet.
