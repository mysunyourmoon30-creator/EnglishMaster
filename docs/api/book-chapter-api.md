# Book Chapter API

Book chapters are managed from the Book endpoint file. Chapter collection routes are nested under `/api/v1/books/{bookId}/chapters`; item and lesson routes use `/api/v1/book-chapters`.

The API returns contract DTOs from `EnglishMaster.Contracts.BookChapters`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/books/{bookId}/chapters` | List chapters in a book. |
| `POST` | `/api/v1/books/{bookId}/chapters` | Add a chapter to a book. |
| `POST` | `/api/v1/books/{bookId}/chapters/reorder` | Reorder chapters in a book. |
| `GET` | `/api/v1/book-chapters/{id}` | Get one chapter by id. |
| `PUT` | `/api/v1/book-chapters/{id}` | Update a chapter. |
| `DELETE` | `/api/v1/book-chapters/{id}` | Deactivate a chapter. |
| `GET` | `/api/v1/book-chapters/{chapterId}/lessons` | List lessons in a chapter. |
| `POST` | `/api/v1/book-chapters/{chapterId}/lessons/{lessonId}` | Add a lesson to a chapter. |
| `DELETE` | `/api/v1/book-chapters/{chapterId}/lessons/{lessonId}` | Remove a lesson from a chapter. |
| `POST` | `/api/v1/book-chapters/{chapterId}/lessons/reorder` | Reorder lessons in a chapter. |

## List Book Chapters

```http
GET /api/v1/books/11111111-1111-1111-1111-111111111111/chapters
```

Success returns `200 OK` with a collection of `BookChapterDto` rows ordered by `SortOrder`. Unknown book ids return `404 Not Found`.

Response:

```json
[
  {
    "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "bookId": "11111111-1111-1111-1111-111111111111",
    "title": "Getting Started",
    "slug": "getting-started",
    "summary": "Set up the topic",
    "contentMarkdown": "## Getting Started",
    "sortOrder": 0,
    "lessons": [],
    "isActive": true,
    "createdAt": "2026-07-13T08:00:00+00:00",
    "updatedAt": "2026-07-13T08:00:00+00:00"
  }
]
```

## Add Book Chapter

```http
POST /api/v1/books/11111111-1111-1111-1111-111111111111/chapters
Content-Type: application/json
```

Request:

```json
{
  "title": "Getting Started",
  "summary": "Set up the topic",
  "contentMarkdown": "## Getting Started",
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/book-chapters/{id}` with the created `BookChapterDto`. Unknown book ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Get Book Chapter

```http
GET /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `200 OK` with a `BookChapterDto`. Unknown ids return `404 Not Found`.

## Update Book Chapter

```http
PUT /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Content-Type: application/json
```

Request:

```json
{
  "title": "Getting Started Updated",
  "summary": "Set up the topic",
  "contentMarkdown": "## Updated",
  "sortOrder": 1,
  "isActive": true
}
```

Success returns `200 OK` with the updated `BookChapterDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Book Chapter

```http
DELETE /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `204 No Content`. The chapter is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Reorder Book Chapters

```http
POST /api/v1/books/11111111-1111-1111-1111-111111111111/chapters/reorder
Content-Type: application/json
```

Request:

```json
{
  "orderedChapterIds": [
    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
  ]
}
```

Success returns `200 OK` with the reordered `BookChapterDto` collection. The submitted list must include the same chapter ids currently attached to the book, without duplicates.

## Chapter Lessons

### List Chapter Lessons

```http
GET /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons
```

Success returns `200 OK` with a collection of `BookChapterLessonDto` rows ordered by `SortOrder`. Unknown chapter ids return `404 Not Found`.

Response:

```json
[
  {
    "id": "cccccccc-cccc-cccc-cccc-cccccccccccc",
    "bookChapterId": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
    "lessonId": "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "lessonTitle": "Daily Routines",
    "lessonSlug": "daily-routines",
    "lessonSummary": "Talk about everyday activities",
    "lessonCefrLevel": "A1",
    "sortOrder": 0,
    "createdAt": "2026-07-13T08:00:00+00:00",
    "updatedAt": "2026-07-13T08:00:00+00:00"
  }
]
```

### Add Lesson To Chapter

```http
POST /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb?sortOrder=0
```

No request body. `sortOrder` is optional and defaults to `0`.

Success returns `200 OK` with the updated `BookChapterDto`. Adding the same lesson twice is idempotent for the relation count. Returns `404 Not Found` if the chapter does not exist, and `400 Bad Request` if the lesson does not exist, is inactive, or the sort order is invalid.

### Remove Lesson From Chapter

```http
DELETE /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons/bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb
```

Success returns `204 No Content`. Returns `404 Not Found` if the chapter does not exist or the lesson is not currently related to the chapter.

### Reorder Chapter Lessons

```http
POST /api/v1/book-chapters/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/lessons/reorder
Content-Type: application/json
```

Request:

```json
{
  "orderedBookChapterLessonIds": [
    "dddddddd-dddd-dddd-dddd-dddddddddddd",
    "cccccccc-cccc-cccc-cccc-cccccccccccc"
  ]
}
```

Success returns `200 OK` with the reordered `BookChapterLessonDto` collection. The submitted list must include the same relation ids currently attached to the chapter, without duplicates.

## Known Limitations

- Endpoints are not authenticated yet.
- Chapter delete is a soft delete through `IsActive = false`.
- Chapter lesson relation rows are removed instead of soft-deactivated.
- Only active lessons can be added to a chapter.
- Reorder endpoints require a complete ordered id list.
- There is no standalone chapter lesson lookup by relation id.
