# Grammar Topic API

Base path:

```text
/api/v1/grammar-topics
```

The API returns contract DTOs from `EnglishMaster.Contracts.GrammarTopics`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/grammar-topics` | Search, filter, and paginate topics. |
| `GET` | `/api/v1/grammar-topics/{id}` | Get one topic by id. |
| `POST` | `/api/v1/grammar-topics` | Create a topic. |
| `PUT` | `/api/v1/grammar-topics/{id}` | Update a topic. |
| `DELETE` | `/api/v1/grammar-topics/{id}` | Deactivate a topic. |
| `GET` | `/api/v1/grammar-topics/{topicId}/rules` | List rules for a topic. See [Grammar Rule API](grammar-rule-api.md) for full detail. |

## Search Parameters

`GET /api/v1/grammar-topics` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Title`, `Slug`, and `Summary`. |
| `cefrLevel` | string | Optional CEFR filter: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |

Example:

```http
GET /api/v1/grammar-topics?search=Present&cefrLevel=A1&isActive=true&pageNumber=1&pageSize=20
```

Response:

```json
{
  "items": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "title": "Present Simple",
      "slug": "present-simple",
      "summary": "Daily routines",
      "cefrLevel": "A1",
      "sortOrder": 0,
      "isActive": true,
      "createdAt": "2026-07-10T08:00:00+00:00",
      "updatedAt": "2026-07-10T08:00:00+00:00"
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
GET /api/v1/grammar-topics/11111111-1111-1111-1111-111111111111
```

Success returns `200 OK` with a `GrammarTopicDto`. Unknown ids return `404 Not Found`.

## Create Grammar Topic

```http
POST /api/v1/grammar-topics
Content-Type: application/json
```

Request:

```json
{
  "title": "Present Simple",
  "summary": "Daily routines",
  "cefrLevel": "A1",
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/grammar-topics/{id}` with the created `GrammarTopicDto`.

Validation errors return `400 Bad Request` with validation problem details:

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

A duplicate title (and therefore duplicate slug) also returns `400 Bad Request`, but under the PascalCase field name `Title`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "A grammar topic with this title already exists."
    ]
  }
}
```

## Update Grammar Topic

```http
PUT /api/v1/grammar-topics/11111111-1111-1111-1111-111111111111
Content-Type: application/json
```

Request:

```json
{
  "title": "Present Simple",
  "summary": "Daily routines and habits",
  "cefrLevel": "A1",
  "sortOrder": 0,
  "isActive": true
}
```

Success returns `200 OK` with the updated `GrammarTopicDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Grammar Topic

```http
DELETE /api/v1/grammar-topics/11111111-1111-1111-1111-111111111111
```

Success returns `204 No Content`. The topic is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- Deactivating a topic does not automatically deactivate its rules.
