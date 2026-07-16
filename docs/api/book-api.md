# Book API

Base path:

```text
/api/v1/books
```

The API returns contract DTOs from `EnglishMaster.Contracts.Books`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/books` | Search, filter, sort, and paginate books. |
| `GET` | `/api/v1/books/{id}` | Get one book by id. |
| `POST` | `/api/v1/books` | Create a book. |
| `PUT` | `/api/v1/books/{id}` | Update a book. |
| `DELETE` | `/api/v1/books/{id}` | Deactivate a book. |
| `POST` | `/api/v1/books/{id}/publish` | Publish a book. |
| `POST` | `/api/v1/books/{id}/unpublish` | Unpublish a book. |
| `POST` | `/api/v1/books/{id}/activate` | Activate a book. |
| `POST` | `/api/v1/books/{id}/deactivate` | Deactivate a book. |
| `GET` | `/api/v1/books/{bookId}/chapters` | List chapters in a book. |
| `POST` | `/api/v1/books/{bookId}/chapters` | Add a chapter to a book. |
| `POST` | `/api/v1/books/{bookId}/chapters/reorder` | Reorder chapters in a book. |

Book chapter and chapter lesson examples are documented in [Book Chapter API](book-chapter-api.md).

## Search Parameters

`GET /api/v1/books` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Title`, `Slug`, and `Summary`. |
| `cefrLevel` | string | Optional CEFR filter: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `categoryId` | guid | Optional category filter. |
| `courseId` | guid | Optional course filter. |
| `isPublished` | bool | Optional published-state filter. Omitted means both published and draft books are returned. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |
| `sortBy` | string | `Title` or `CreatedAt`. Defaults to `Title`. |
| `sortDirection` | string | `Asc` or `Desc`. Defaults to `Asc`. |

Example:

```http
GET /api/v1/books?search=Starter&cefrLevel=A1&isPublished=true&isActive=true&pageNumber=1&pageSize=20&sortBy=Title&sortDirection=Asc
```

Response:

```json
{
  "items": [
    {
      "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
      "title": "Starter Book",
      "slug": "starter-book",
      "subtitle": "First steps",
      "summary": "A compact beginner book",
      "description": "A longer book description.",
      "cefrLevel": "A1",
      "categoryId": null,
      "category": null,
      "coverMediaId": null,
      "coverMedia": null,
      "courseId": null,
      "course": null,
      "authorName": "EnglishMaster Team",
      "edition": "First",
      "version": "1.0",
      "estimatedPages": 80,
      "sortOrder": 0,
      "chapters": [],
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
GET /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `200 OK` with a `BookDto`. Unknown ids return `404 Not Found`.

## Create Book

```http
POST /api/v1/books
Content-Type: application/json
```

Request:

```json
{
  "title": "Starter Book",
  "subtitle": "First steps",
  "summary": "A compact beginner book",
  "description": "A longer book description.",
  "cefrLevel": "A1",
  "categoryId": null,
  "coverMediaId": null,
  "courseId": null,
  "authorName": "EnglishMaster Team",
  "edition": "First",
  "version": "1.0",
  "estimatedPages": 80,
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/books/{id}` with the created `BookDto`. A new book always starts as `isPublished: false`, `isActive: true`.

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
      "A book with this title already exists."
    ]
  }
}
```

## Update Book

```http
PUT /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Content-Type: application/json
```

Request:

```json
{
  "title": "Starter Book Updated",
  "subtitle": "First steps",
  "summary": "A compact beginner book",
  "description": "A longer book description.",
  "cefrLevel": "A1",
  "categoryId": null,
  "coverMediaId": null,
  "courseId": null,
  "authorName": "EnglishMaster Team",
  "edition": "First",
  "version": "1.1",
  "estimatedPages": 90,
  "sortOrder": 1,
  "isPublished": false,
  "isActive": true
}
```

Success returns `200 OK` with the updated `BookDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Book

```http
DELETE /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `204 No Content`. The book is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Publish / Unpublish Book

```http
POST /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/publish
POST /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/unpublish
```

Both return `200 OK` with the updated `BookDto`, so the caller can immediately read the new `isPublished` value. Unknown ids return `404 Not Found`.

## Activate / Deactivate Book

```http
POST /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/activate
POST /api/v1/books/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/deactivate
```

Both return `200 OK` with the updated `BookDto`, so the caller can immediately read the new `isActive` value. Unknown ids return `404 Not Found`.

## Book Chapters

Book chapters are managed through nested routes:

- `GET /api/v1/books/{bookId}/chapters`
- `POST /api/v1/books/{bookId}/chapters`
- `POST /api/v1/books/{bookId}/chapters/reorder`

See [Book Chapter API](book-chapter-api.md) for request and response examples.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- Only active categories, active image media, and active courses can be referenced.
- `sortOrder` defaults are handled by callers; callers that want a specific position should pass it explicitly.
- Book cover media must reference active image media; upload still belongs to the Media API.
- The Book API does not export PDF, DOCX, EPUB, or print artifacts.
