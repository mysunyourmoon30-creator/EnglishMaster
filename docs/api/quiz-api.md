# Quiz API

Base path:

```text
/api/v1/quizzes
```

The API returns contract DTOs from `EnglishMaster.Contracts.Quizzes`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/quizzes` | Search, filter, sort, and paginate quizzes. |
| `GET` | `/api/v1/quizzes/{id}` | Get one quiz by id. |
| `POST` | `/api/v1/quizzes` | Create a quiz. |
| `PUT` | `/api/v1/quizzes/{id}` | Update a quiz. |
| `DELETE` | `/api/v1/quizzes/{id}` | Deactivate a quiz. |
| `POST` | `/api/v1/quizzes/{id}/publish` | Publish a quiz. |
| `POST` | `/api/v1/quizzes/{id}/unpublish` | Unpublish a quiz. |
| `POST` | `/api/v1/quizzes/{id}/activate` | Activate a quiz. |
| `POST` | `/api/v1/quizzes/{id}/deactivate` | Deactivate a quiz. |
| `GET` | `/api/v1/quizzes/{quizId}/questions` | List questions in a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions` | Add a question to a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions/reorder` | Reorder questions in a quiz. |

Quiz question and choice examples are documented in [Quiz Question API](quiz-question-api.md) and [Quiz Choice API](quiz-choice-api.md).

## Search Parameters

`GET /api/v1/quizzes` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Title`, `Slug`, and `Summary`. |
| `cefrLevel` | string | Optional CEFR filter: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `categoryId` | guid | Optional category filter. |
| `lessonId` | guid | Optional lesson filter. |
| `courseId` | guid | Optional course filter. |
| `bookId` | guid | Optional book filter. |
| `isPublished` | bool | Optional published-state filter. Omitted means both published and draft quizzes are returned. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |
| `sortBy` | string | `Title` or `CreatedAt`. Defaults to `Title`. |
| `sortDirection` | string | `Asc` or `Desc`. Defaults to `Asc`. |

Example:

```http
GET /api/v1/quizzes?search=Starter&cefrLevel=A1&isPublished=true&isActive=true&pageNumber=1&pageSize=20&sortBy=Title&sortDirection=Asc
```

Response:

```json
{
  "items": [
    {
      "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "title": "Starter Quiz",
      "slug": "starter-quiz",
      "summary": "Check beginner knowledge",
      "description": "A short admin quiz.",
      "cefrLevel": "A1",
      "categoryId": null,
      "category": null,
      "lessonId": null,
      "lesson": null,
      "courseId": null,
      "course": null,
      "bookId": null,
      "book": null,
      "timeLimitMinutes": 15,
      "passingScore": 70,
      "sortOrder": 0,
      "questions": [],
      "isPublished": true,
      "isActive": true,
      "createdAt": "2026-07-13T08:00:00+00:00",
      "updatedAt": "2026-07-13T08:00:00+00:00"
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
GET /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `200 OK` with a `QuizDto`. Unknown ids return `404 Not Found`.

## Create Quiz

```http
POST /api/v1/quizzes
Content-Type: application/json
```

Request:

```json
{
  "title": "Starter Quiz",
  "summary": "Check beginner knowledge",
  "description": "A short admin quiz.",
  "cefrLevel": "A1",
  "categoryId": null,
  "lessonId": null,
  "courseId": null,
  "bookId": null,
  "timeLimitMinutes": 15,
  "passingScore": 70,
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/quizzes/{id}` with the created `QuizDto`. A new quiz always starts as `isPublished: false`, `isActive: true`.

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

A duplicate title/slug returns `400 Bad Request` under the PascalCase field name `Title`, matching the existing validation pattern used by other modules:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "A quiz with this title already exists."
    ]
  }
}
```

## Update Quiz

```http
PUT /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Content-Type: application/json
```

Request:

```json
{
  "title": "Starter Quiz Updated",
  "summary": "Check beginner knowledge",
  "description": "A short admin quiz.",
  "cefrLevel": "A1",
  "categoryId": null,
  "lessonId": null,
  "courseId": null,
  "bookId": null,
  "timeLimitMinutes": 20,
  "passingScore": 80,
  "sortOrder": 1,
  "isPublished": false,
  "isActive": true
}
```

Success returns `200 OK` with the updated `QuizDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Quiz

```http
DELETE /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `204 No Content`. The quiz is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Publish / Unpublish Quiz

```http
POST /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/publish
POST /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/unpublish
```

Both return `200 OK` with the updated `QuizDto`, so the caller can immediately read the new `isPublished` value. Unknown ids return `404 Not Found`.

## Activate / Deactivate Quiz

```http
POST /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/activate
POST /api/v1/quizzes/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/deactivate
```

Both return `200 OK` with the updated `QuizDto`, so the caller can immediately read the new `isActive` value. Unknown ids return `404 Not Found`.

## Quiz Questions

Quiz questions are managed through nested routes:

- `GET /api/v1/quizzes/{quizId}/questions`
- `POST /api/v1/quizzes/{quizId}/questions`
- `POST /api/v1/quizzes/{quizId}/questions/reorder`

See [Quiz Question API](quiz-question-api.md) for request and response examples.

## Security Note About Correct Answers

`QuizDto` includes nested `QuizQuestionDto` and `QuizChoiceDto` rows. `QuizChoiceDto` includes `isCorrect` because these endpoints currently serve the admin authoring surface. Do not expose these DTOs directly from future public quiz-taking endpoints.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- Only active categories, lessons, courses, and books can be referenced.
- `sortOrder` defaults are handled by callers; callers that want a specific position should pass it explicitly.
- There are no learner attempts, scoring records, timers, or progress tracking yet.
