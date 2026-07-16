# Achievement API

## Learner Endpoints

```text
GET  /api/v1/me/achievements
GET  /api/v1/me/achievements/earned
GET  /api/v1/me/achievements/progress
POST /api/v1/me/achievements/evaluate
```

## Admin Endpoints

```text
GET  /api/v1/admin/achievement-definitions
POST /api/v1/admin/achievement-definitions
PUT  /api/v1/admin/achievement-definitions/{id}
POST /api/v1/admin/achievement-definitions/{id}/activate
POST /api/v1/admin/achievement-definitions/{id}/deactivate
POST /api/v1/admin/achievement-definitions/seed-defaults
```

`GET` requires `achievements.read`. Mutations require `achievements.manage`.

## Default Achievements

Default achievements include first lesson, first quiz, first passed quiz, first practice session, streak achievements, ten lessons, first course, first book, daily plan completion, and practice completion progress.
