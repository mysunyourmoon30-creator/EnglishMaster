# Lesson API

Base path:

```text
/api/v1/lessons
```

The API returns contract DTOs from `EnglishMaster.Contracts.Lessons`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/lessons` | Search, filter, sort, and paginate lessons. |
| `GET` | `/api/v1/lessons/{id}` | Get one lesson by id. |
| `POST` | `/api/v1/lessons` | Create a lesson. |
| `PUT` | `/api/v1/lessons/{id}` | Update a lesson. |
| `DELETE` | `/api/v1/lessons/{id}` | Deactivate a lesson. |
| `POST` | `/api/v1/lessons/{id}/publish` | Publish a lesson. |
| `POST` | `/api/v1/lessons/{id}/unpublish` | Unpublish a lesson. |
| `POST` | `/api/v1/lessons/{lessonId}/words/{wordId}` | Add a related word to a lesson. |
| `DELETE` | `/api/v1/lessons/{lessonId}/words/{wordId}` | Remove a related word from a lesson. |
| `POST` | `/api/v1/lessons/{lessonId}/grammar-rules/{grammarRuleId}` | Add a related grammar rule to a lesson. |
| `DELETE` | `/api/v1/lessons/{lessonId}/grammar-rules/{grammarRuleId}` | Remove a related grammar rule from a lesson. |
| `GET` `POST` | `/api/v1/lessons/{lessonId}/sections` | List/add sections for a lesson. Documented in [Lesson Section API](lesson-section-api.md). |
| `POST` | `/api/v1/lessons/{lessonId}/sections/reorder` | Reorder a lesson's sections. Documented in [Lesson Section API](lesson-section-api.md). |

## Search Parameters

`GET /api/v1/lessons` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Title` and `Summary`. |
| `cefrLevel` | string | Optional CEFR filter: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `categoryId` | guid | Optional category filter. |
| `isPublished` | bool | Optional published-state filter. No default — omitted means both published and draft lessons are returned. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |
| `sortBy` | string | `Title` or `CreatedAt`. Defaults to `Title`. |
| `sortDirection` | string | `Asc` or `Desc`. Defaults to `Asc`. |

Example:

```http
GET /api/v1/lessons?search=Daily&cefrLevel=A1&isPublished=true&isActive=true&pageNumber=1&pageSize=20&sortBy=Title&sortDirection=Asc
```

Response:

```json
{
  "items": [
    {
      "id": "66666666-6666-6666-6666-666666666666",
      "title": "Daily Routines",
      "slug": "daily-routines",
      "summary": "Learn to talk about your day",
      "description": "A longer description of the lesson.",
      "cefrLevel": "A1",
      "categoryId": null,
      "category": null,
      "thumbnailMediaId": null,
      "thumbnailMedia": null,
      "estimatedMinutes": 15,
      "sortOrder": 0,
      "words": [],
      "grammarRules": [],
      "isPublished": true,
      "isActive": true,
      "createdAt": "2026-07-12T08:00:00+00:00",
      "updatedAt": "2026-07-12T08:00:00+00:00"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

## Get By Id

```http
GET /api/v1/lessons/66666666-6666-6666-6666-666666666666
```

Success returns `200 OK` with a `LessonDto`. Unknown ids return `404 Not Found`.

## Create Lesson

```http
POST /api/v1/lessons
Content-Type: application/json
```

Request:

```json
{
  "title": "Daily Routines",
  "summary": "Learn to talk about your day",
  "description": "A longer description of the lesson.",
  "cefrLevel": "A1",
  "categoryId": null,
  "thumbnailMediaId": null,
  "estimatedMinutes": 15,
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/lessons/{id}` with the created `LessonDto`. A new lesson always starts as `isPublished: false`, `isActive: true`.

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

A duplicate title returns `400 Bad Request` under the PascalCase field name `Title`, the same pattern already established for Grammar Topic and Grammar Rule:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "A lesson with this title already exists."
    ]
  }
}
```

## Update Lesson

```http
PUT /api/v1/lessons/66666666-6666-6666-6666-666666666666
Content-Type: application/json
```

Request:

```json
{
  "title": "Daily Routines",
  "summary": "Learn to talk about your day",
  "description": "A longer description of the lesson.",
  "cefrLevel": "A1",
  "categoryId": null,
  "thumbnailMediaId": null,
  "estimatedMinutes": 20,
  "sortOrder": 1,
  "isPublished": false,
  "isActive": true
}
```

Success returns `200 OK` with the updated `LessonDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Lesson

```http
DELETE /api/v1/lessons/66666666-6666-6666-6666-666666666666
```

Success returns `204 No Content`. The lesson is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Publish / Unpublish Lesson

```http
POST /api/v1/lessons/66666666-6666-6666-6666-666666666666/publish
POST /api/v1/lessons/66666666-6666-6666-6666-666666666666/unpublish
```

Both return `200 OK` with the updated `LessonDto` (not `204`, since the caller typically wants to see the new `isPublished` value immediately). Unknown ids return `404 Not Found`.

## Related Words

### Add Related Word

```http
POST /api/v1/lessons/66666666-6666-6666-6666-666666666666/words/88888888-8888-8888-8888-888888888888?sortOrder=0
```

No request body. `sortOrder` is an optional query parameter defaulting to `0` (it is not part of the route). Success returns `200 OK` with the updated `LessonDto`, including the new entry in `words`. Adding the same word twice is idempotent and still returns `200 OK`. Returns `404 Not Found` if the lesson does not exist, and `400 Bad Request` if the lesson is inactive, the word does not exist or is inactive, or `sortOrder` is negative.

### Remove Related Word

```http
DELETE /api/v1/lessons/66666666-6666-6666-6666-666666666666/words/88888888-8888-8888-8888-888888888888
```

Success returns `204 No Content`. Returns `404 Not Found` if the lesson does not exist or the word is not currently related to the lesson.

## Related Grammar Rules

### Add Related Grammar Rule

```http
POST /api/v1/lessons/66666666-6666-6666-6666-666666666666/grammar-rules/99999999-9999-9999-9999-999999999999?sortOrder=0
```

Same shape as Related Words: no body, optional `sortOrder` query parameter (default `0`), `200 OK` with the updated `LessonDto` (idempotent add), `404` if the lesson does not exist, `400` if the lesson is inactive, the grammar rule does not exist or is inactive, or `sortOrder` is negative.

### Remove Related Grammar Rule

```http
DELETE /api/v1/lessons/66666666-6666-6666-6666-666666666666/grammar-rules/99999999-9999-9999-9999-999999999999
```

Success returns `204 No Content`. Returns `404 Not Found` if the lesson does not exist or the grammar rule is not currently related to the lesson.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- `ActivateLesson`/`DeactivateLesson` use cases exist in the Application layer but have no API route — only `publish`/`unpublish` are routed.
- Only active words and active grammar rules can be added as related content.
- `sortOrder` on the related-content endpoints defaults to `0` rather than auto-incrementing to the end of the list; callers that want a specific order must pass it explicitly.
