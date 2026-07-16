# Lesson Section API

Lesson section routes are split between the parent lesson route and direct item routes.

Parent base path:

```text
/api/v1/lessons/{lessonId}/sections
```

Item base path:

```text
/api/v1/lesson-sections
```

The API returns contract DTOs from `EnglishMaster.Contracts.LessonSections`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/lessons/{lessonId}/sections` | List sections for a lesson. |
| `POST` | `/api/v1/lessons/{lessonId}/sections` | Add a section to a lesson. |
| `POST` | `/api/v1/lessons/{lessonId}/sections/reorder` | Reorder a lesson's sections. |
| `PUT` | `/api/v1/lesson-sections/{id}` | Update a section. |
| `DELETE` | `/api/v1/lesson-sections/{id}` | Deactivate a section. |

There is no `GET /api/v1/lesson-sections/{id}` route — see Known Limitations.

## List By Lesson

```http
GET /api/v1/lessons/66666666-6666-6666-6666-666666666666/sections
```

Success returns `200 OK` with a plain array of `LessonSectionDto` items, ordered by `SortOrder` then `Title` — there is no pagination envelope. Unknown lesson ids return `404 Not Found`.

Response:

```json
[
  {
    "id": "77777777-7777-7777-7777-777777777777",
    "lessonId": "66666666-6666-6666-6666-666666666666",
    "title": "Vocabulary Warm-Up",
    "contentMarkdown": "## Words to learn\n- wake up\n- brush teeth",
    "sectionType": "Vocabulary",
    "mediaId": null,
    "media": null,
    "sortOrder": 0,
    "isActive": true,
    "createdAt": "2026-07-12T08:00:00+00:00",
    "updatedAt": "2026-07-12T08:00:00+00:00"
  }
]
```

## Add Section

```http
POST /api/v1/lessons/66666666-6666-6666-6666-666666666666/sections
Content-Type: application/json
```

Request:

```json
{
  "title": "Vocabulary Warm-Up",
  "contentMarkdown": "## Words to learn\n- wake up\n- brush teeth",
  "sectionType": "Vocabulary",
  "mediaId": null,
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/lesson-sections/{id}` with the created `LessonSectionDto`.

Validation errors return `400 Bad Request`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "title": [
      "title is required."
    ]
  }
}
```

Adding a section to an unknown lesson returns `404 Not Found`. Adding a section to an inactive lesson returns `400 Bad Request` with an error under `lessonId`.

## Update Section

```http
PUT /api/v1/lesson-sections/77777777-7777-7777-7777-777777777777
Content-Type: application/json
```

Request:

```json
{
  "title": "Vocabulary Warm-Up Updated",
  "contentMarkdown": "## Words to learn\n- wake up\n- brush teeth\n- get dressed",
  "sectionType": "Vocabulary",
  "mediaId": null,
  "sortOrder": 1,
  "isActive": true
}
```

Success returns `200 OK` with the updated `LessonSectionDto`. Unknown ids return `404 Not Found`.

## Delete Section

```http
DELETE /api/v1/lesson-sections/77777777-7777-7777-7777-777777777777
```

Success returns `204 No Content`. The section is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Reorder Sections

```http
POST /api/v1/lessons/66666666-6666-6666-6666-666666666666/sections/reorder
Content-Type: application/json
```

Request — the full ordered list of the lesson's section ids:

```json
{
  "orderedSectionIds": [
    "88888888-8888-8888-8888-888888888888",
    "77777777-7777-7777-7777-777777777777"
  ]
}
```

Success returns `200 OK` with the reordered array of `LessonSectionDto` items, in the submitted order, with `SortOrder` updated to match each item's position (0-based).

Validation errors return `400 Bad Request` when:

- `orderedSectionIds` is missing.
- `orderedSectionIds` contains an empty GUID.
- `orderedSectionIds` contains a duplicate id.
- `orderedSectionIds` does not contain exactly the lesson's current set of sections (missing an id, containing an extra id, or a count mismatch) — a partial reorder is rejected rather than partially applied.

Unknown lesson ids return `404 Not Found`.

## Domain And Validation Notes

- The parent lesson must exist to add a section.
- New sections cannot be added to an inactive lesson.
- `Title` is required.
- `SectionType` must be a defined value (see [Lesson Section](../modules/lesson-section.md#sectiontype-values)).
- `SortOrder` must be greater than or equal to zero.
- `MediaId`, when provided, must reference active media (no media-type constraint).

## Known Limitations

- There is no standalone search or pagination endpoint for lesson sections.
- **There is no `GET /api/v1/lesson-sections/{id}` route.** `GetLessonSectionByIdQueryHandler` exists in the Application layer and is registered in dependency injection, but no endpoint currently calls it.
- Endpoints are not authenticated yet.
- Delete is a soft delete through `IsActive = false`.
