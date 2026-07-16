# Category API

Base path:

```text
/api/v1/categories
```

The API returns contract DTOs from `EnglishMaster.Contracts.Categories`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/categories` | Search and filter categories. |
| `GET` | `/api/v1/categories/{id}` | Get one category by id. |
| `POST` | `/api/v1/categories` | Create a category. |
| `PUT` | `/api/v1/categories/{id}` | Update a category. |
| `DELETE` | `/api/v1/categories/{id}` | Deactivate a category. |

## Search Parameters

`GET /api/v1/categories` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Name`, `Slug`, and `Description`. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |

Pagination parameters are not supported for categories yet.

Example:

```http
GET /api/v1/categories?search=basic&isActive=true
```

Response:

```json
{
  "items": [
    {
      "id": "22222222-2222-2222-2222-222222222222",
      "name": "Basics",
      "slug": "basics",
      "description": "Core vocabulary",
      "sortOrder": 10,
      "isActive": true,
      "createdAt": "2026-07-10T09:00:00+00:00",
      "updatedAt": "2026-07-10T09:00:00+00:00"
    }
  ]
}
```

## Get By Id

```http
GET /api/v1/categories/22222222-2222-2222-2222-222222222222
```

Success returns `200 OK` with a `CategoryDto`. Unknown ids return `404 Not Found`.

## Create Category

```http
POST /api/v1/categories
Content-Type: application/json
```

Request:

```json
{
  "name": "Basics",
  "description": "Core vocabulary",
  "sortOrder": 10
}
```

Success returns `201 Created` with the created `CategoryDto`.

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

## Update Category

```http
PUT /api/v1/categories/22222222-2222-2222-2222-222222222222
Content-Type: application/json
```

Request:

```json
{
  "name": "Basics",
  "description": "Updated core vocabulary",
  "sortOrder": 20,
  "isActive": true
}
```

Success returns `200 OK` with the updated `CategoryDto`. Unknown ids return `404 Not Found`.

## Delete Category

```http
DELETE /api/v1/categories/22222222-2222-2222-2222-222222222222
```

Success returns `204 No Content`. The category is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Word Relationship

Words reference categories through `categoryId`. Word endpoints support:

- `POST /api/v1/words` with `categoryId`
- `PUT /api/v1/words/{id}` with `categoryId`
- `GET /api/v1/words/{id}` returning `category`
- `GET /api/v1/words?categoryId={id}` filtering by category

Only active categories can be assigned to words.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- Category search does not support pagination yet.
