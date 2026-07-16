# Word API

Base path:

```text
/api/v1/words
```

The API returns contract DTOs from `EnglishMaster.Contracts.Words`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/words` | Search, filter, sort, and paginate words. |
| `GET` | `/api/v1/words/{id}` | Get one word by id. |
| `POST` | `/api/v1/words` | Create a word. |
| `PUT` | `/api/v1/words/{id}` | Update a word. |
| `DELETE` | `/api/v1/words/{id}` | Deactivate a word. |

## Search Parameters

`GET /api/v1/words` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Text`, `Slug`, `MeaningTh`, and `MeaningEn`. |
| `cefrLevel` | string | Optional CEFR filter: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `partOfSpeech` | string | Optional part-of-speech filter. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `categoryId` | guid | Optional category filter. |
| `tagId` | guid | Optional tag filter. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |
| `sortBy` | string | `Text` or `CreatedAt`. Defaults to `Text`. |
| `sortDirection` | string | `Asc` or `Desc`. Defaults to `Asc`. |

Example:

```http
GET /api/v1/words?search=hello&cefrLevel=A1&isActive=true&categoryId=22222222-2222-2222-2222-222222222222&tagId=33333333-3333-3333-3333-333333333333&pageNumber=1&pageSize=20&sortBy=Text&sortDirection=Asc
```

Response:

```json
{
  "items": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "text": "hello",
      "slug": "hello",
      "ipaUk": "/he'lo/",
      "ipaUs": "/he'lo/",
      "thaiReading": "heh-lo",
      "meaningTh": "Thai meaning",
      "meaningEn": "greeting",
      "partOfSpeech": "Interjection",
      "cefrLevel": "A1",
      "exampleEn": "Hello there.",
      "exampleTh": "Thai example",
      "categoryId": "22222222-2222-2222-2222-222222222222",
      "category": {
        "id": "22222222-2222-2222-2222-222222222222",
        "name": "Basics",
        "slug": "basics"
      },
      "tags": [
        {
          "id": "33333333-3333-3333-3333-333333333333",
          "name": "Travel",
          "slug": "travel"
        }
      ],
      "isActive": true,
      "createdAt": "2026-07-09T15:00:00+00:00",
      "updatedAt": "2026-07-09T15:00:00+00:00"
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
GET /api/v1/words/11111111-1111-1111-1111-111111111111
```

Success returns `200 OK` with a `WordDto`. Unknown ids return `404 Not Found`.

## Create Word

```http
POST /api/v1/words
Content-Type: application/json
```

Request:

```json
{
  "text": "hello",
  "ipaUk": "/he'lo/",
  "ipaUs": "/he'lo/",
  "thaiReading": "heh-lo",
  "meaningTh": "Thai meaning",
  "meaningEn": "greeting",
  "partOfSpeech": "Interjection",
  "cefrLevel": "A1",
  "exampleEn": "Hello there.",
  "exampleTh": "Thai example",
  "categoryId": "22222222-2222-2222-2222-222222222222",
  "tagIds": [
    "33333333-3333-3333-3333-333333333333"
  ]
}
```

Success returns `201 Created` with the created `WordDto`.

Validation errors return `400 Bad Request` with validation problem details:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "text": [
      "text is required."
    ],
    "meaningTh": [
      "meaningTh is required."
    ]
  }
}
```

## Update Word

```http
PUT /api/v1/words/11111111-1111-1111-1111-111111111111
Content-Type: application/json
```

Request:

```json
{
  "text": "hello",
  "ipaUk": "/he'lo/",
  "ipaUs": "/he'lo/",
  "thaiReading": "heh-lo",
  "meaningTh": "Thai meaning",
  "meaningEn": "updated greeting",
  "partOfSpeech": "Interjection",
  "cefrLevel": "A2",
  "exampleEn": "Hello there.",
  "exampleTh": "Thai example",
  "isActive": true,
  "categoryId": "22222222-2222-2222-2222-222222222222",
  "tagIds": [
    "33333333-3333-3333-3333-333333333333"
  ]
}
```

Success returns `200 OK` with the updated `WordDto`. Unknown ids return `404 Not Found`.

## Delete Word

```http
DELETE /api/v1/words/11111111-1111-1111-1111-111111111111
```

Success returns `204 No Content`. The word is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- Word create/update can assign only active categories and tags.
