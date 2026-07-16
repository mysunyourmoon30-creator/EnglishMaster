# Course API

Base path:

```text
/api/v1/courses
```

The API returns contract DTOs from `EnglishMaster.Contracts.Courses`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/courses` | Search, filter, sort, and paginate courses. |
| `GET` | `/api/v1/courses/{id}` | Get one course by id. |
| `POST` | `/api/v1/courses` | Create a course. |
| `PUT` | `/api/v1/courses/{id}` | Update a course. |
| `DELETE` | `/api/v1/courses/{id}` | Deactivate a course. |
| `POST` | `/api/v1/courses/{id}/publish` | Publish a course. |
| `POST` | `/api/v1/courses/{id}/unpublish` | Unpublish a course. |
| `GET` | `/api/v1/courses/{courseId}/lessons` | List lessons in a course. |
| `POST` | `/api/v1/courses/{courseId}/lessons/{lessonId}` | Add a lesson to a course. |
| `DELETE` | `/api/v1/courses/{courseId}/lessons/{lessonId}` | Remove a lesson from a course. |
| `POST` | `/api/v1/courses/{courseId}/lessons/reorder` | Reorder lessons in a course. |

## Search Parameters

`GET /api/v1/courses` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Title`, `Slug`, and `Summary`. |
| `cefrLevel` | string | Optional CEFR filter: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `categoryId` | guid | Optional category filter. |
| `isPublished` | bool | Optional published-state filter. Omitted means both published and draft courses are returned. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |
| `sortBy` | string | `Title` or `CreatedAt`. Defaults to `Title`. |
| `sortDirection` | string | `Asc` or `Desc`. Defaults to `Asc`. |

Example:

```http
GET /api/v1/courses?search=Beginner&cefrLevel=A1&isPublished=true&isActive=true&pageNumber=1&pageSize=20&sortBy=Title&sortDirection=Asc
```

Response:

```json
{
  "items": [
    {
      "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "title": "Beginner English Path",
      "slug": "beginner-english-path",
      "summary": "A structured beginner path",
      "description": "A longer course description.",
      "cefrLevel": "A1",
      "categoryId": null,
      "category": null,
      "thumbnailMediaId": null,
      "thumbnailMedia": null,
      "estimatedMinutes": 120,
      "sortOrder": 0,
      "lessons": [],
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
GET /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `200 OK` with a `CourseDto`. Unknown ids return `404 Not Found`.

## Create Course

```http
POST /api/v1/courses
Content-Type: application/json
```

Request:

```json
{
  "title": "Beginner English Path",
  "summary": "A structured beginner path",
  "description": "A longer course description.",
  "cefrLevel": "A1",
  "categoryId": null,
  "thumbnailMediaId": null,
  "estimatedMinutes": 120,
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/courses/{id}` with the created `CourseDto`. A new course always starts as `isPublished: false`, `isActive: true`.

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
      "A course with this title already exists."
    ]
  }
}
```

## Update Course

```http
PUT /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Content-Type: application/json
```

Request:

```json
{
  "title": "Beginner English Path",
  "summary": "A structured beginner path",
  "description": "A longer course description.",
  "cefrLevel": "A1",
  "categoryId": null,
  "thumbnailMediaId": null,
  "estimatedMinutes": 150,
  "sortOrder": 1,
  "isPublished": false,
  "isActive": true
}
```

Success returns `200 OK` with the updated `CourseDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Course

```http
DELETE /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `204 No Content`. The course is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Publish / Unpublish Course

```http
POST /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/publish
POST /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/unpublish
```

Both return `200 OK` with the updated `CourseDto`, so the caller can immediately read the new `isPublished` value. Unknown ids return `404 Not Found`.

## Course Lessons

### List Course Lessons

```http
GET /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons
```

Success returns `200 OK` with a collection of `CourseLessonDto` rows ordered by `SortOrder`. Unknown course ids return `404 Not Found`.

Response:

```json
[
  {
    "id": "cccccccc-cccc-cccc-cccc-cccccccccccc",
    "courseId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "lessonId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "lessonTitle": "Daily Routines",
    "lessonSlug": "daily-routines",
    "lessonSummary": "Talk about everyday activities",
    "lessonCefrLevel": "A1",
    "sortOrder": 0,
    "isRequired": true,
    "createdAt": "2026-07-13T08:00:00+00:00",
    "updatedAt": "2026-07-13T08:00:00+00:00"
  }
]
```

### Add Lesson To Course

```http
POST /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb?sortOrder=0&isRequired=true
```

No request body. `sortOrder` is optional and defaults to `0`. `isRequired` is optional and defaults to `true`.

Success returns `200 OK` with the updated `CourseDto`. Adding the same lesson twice is idempotent for the relation count and updates the existing relation's `IsRequired` value. Returns `404 Not Found` if the course does not exist, and `400 Bad Request` if the lesson does not exist, is inactive, or the sort order is invalid.

### Remove Lesson From Course

```http
DELETE /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb
```

Success returns `204 No Content`. Returns `404 Not Found` if the course does not exist or the lesson is not currently related to the course.

### Reorder Course Lessons

```http
POST /api/v1/courses/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons/reorder
Content-Type: application/json
```

Request:

```json
{
  "orderedCourseLessonIds": [
    "dddddddd-dddd-dddd-dddd-dddddddddddd",
    "cccccccc-cccc-cccc-cccc-cccccccccccc"
  ]
}
```

Success returns `200 OK` with the reordered `CourseLessonDto` collection. The submitted list must include the same relation ids currently attached to the course, without duplicates.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- `ActivateCourse` and `DeactivateCourse` use cases exist in the Application layer but have no dedicated API route.
- Only active lessons can be added to a course.
- `sortOrder` on the add-lesson endpoint defaults to `0`; callers that want a specific position should pass it explicitly.
- Course thumbnails must reference active image media; upload still belongs to the Media API.
