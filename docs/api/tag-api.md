# Tag API

Base path:

```text
/api/v1/tags
```

The API returns contract DTOs from `EnglishMaster.Contracts.Tags`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/tags` | Search and filter tags. |
| `GET` | `/api/v1/tags/{id}` | Get one tag by id. |
| `POST` | `/api/v1/tags` | Create a tag. |
| `PUT` | `/api/v1/tags/{id}` | Update a tag. |
| `DELETE` | `/api/v1/tags/{id}` | Deactivate a tag. |

## Search Parameters

`GET /api/v1/tags` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Name`, `Slug`, and `Description`. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |

Pagination parameters are not supported for tags yet.

Example:

```http
GET /api/v1/tags?search=travel&isActive=true
```

Response:

```json
{
  "items": [
    {
      "id": "33333333-3333-3333-3333-333333333333",
      "name": "Travel",
      "slug": "travel",
      "description": "Travel vocabulary",
      "isActive": true,
      "createdAt": "2026-07-10T09:00:00+00:00",
      "updatedAt": "2026-07-10T09:00:00+00:00"
    }
  ]
}
```

## Get By Id

```http
GET /api/v1/tags/33333333-3333-3333-3333-333333333333
```

Success returns `200 OK` with a `TagDto`. Unknown ids return `404 Not Found`.

## Create Tag

```http
POST /api/v1/tags
Content-Type: application/json
```

Request:

```json
{
  "name": "Travel",
  "description": "Travel vocabulary"
}
```

Success returns `201 Created` with the created `TagDto`.

Validation errors return `400 Bad Request` with validation problem details:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "name": [
      "name is required."
    ]
  }
}
```

## Update Tag

```http
PUT /api/v1/tags/33333333-3333-3333-3333-333333333333
Content-Type: application/json
```

Request:

```json
{
  "name": "Travel",
  "description": "Updated travel vocabulary",
  "isActive": true
}
```

Success returns `200 OK` with the updated `TagDto`. Unknown ids return `404 Not Found`.

## Delete Tag

```http
DELETE /api/v1/tags/33333333-3333-3333-3333-333333333333
```

Success returns `204 No Content`. The tag is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Word Relationship

Words reference tags through the `WordTags` join table. Word endpoints support:

- `POST /api/v1/words` with `tagIds`
- `PUT /api/v1/words/{id}` with `tagIds`
- `GET /api/v1/words/{id}` returning `tags`
- `GET /api/v1/words?tagId={id}` filtering by tag

Only active tags can be assigned to words.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- Tag search does not support pagination yet.
