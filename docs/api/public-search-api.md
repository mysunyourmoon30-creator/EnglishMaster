# Public Search API

## Endpoints

| Method | Route | Purpose | Auth |
| --- | --- | --- | --- |
| `GET` | `/api/v1/public/search` | Search public content. | Anonymous |
| `GET` | `/api/v1/public/search/filters` | Get available filter options. | Anonymous |
| `GET` | `/api/v1/public/search/suggestions` | Get simple search suggestions. | Anonymous |

## Search Parameters

| Parameter | Alias | Purpose |
| --- | --- | --- |
| `q` | - | Keyword query. |
| `contentType` | `type` | Filter by content type. |
| `cefrLevel` | `cefr` | Filter by CEFR level. |
| `categoryId` | - | Filter by category ID. |
| `tagId` | - | Filter by tag ID where supported. |
| `pageNumber` | - | 1-based page number. |
| `pageSize` | - | Page size, capped at 50. |
| `sortBy` | - | `title` or default updated date. |
| `sortDirection` | - | `asc` or descending default. |

## Response

Search returns:

- `contentType`
- `title`
- `slug`
- `summary`
- `cefrLevel`
- `categoryName`
- `tags`
- `url`
- `highlightText`
- `updatedAt`

The response intentionally excludes admin fields, review state, quiz answer keys, internal media paths, and raw storage keys.

## Suggestions

Suggestions are simple active Word title suggestions. They are not a full autocomplete index.

