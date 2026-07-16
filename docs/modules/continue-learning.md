# Continue Learning

## Purpose

Continue Learning helps authenticated learners resume in-progress lessons, courses, and books.

## ContinueLearningItemDto

Fields:

- `ContentType`
- `ContentId`
- `Slug`
- `Title`
- `Summary`
- `Url`
- `ProgressPercent`
- `Status`
- `LastAccessedAt`
- `RecommendationReason`
- `SortOrder`

## Rules

- Only `InProgress` progress appears.
- Completed items are excluded.
- Results are scoped to the current authenticated user.
- Results are sorted by `LastAccessedAt` descending.
- Results are limited by the API limit, capped at 20.
- Lessons, courses, and books must still be active and published.

## Empty State

If the learner has no in-progress published content, the API returns an empty collection and the UI shows a neutral empty state.

