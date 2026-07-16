# Minimal Pair API

Minimal pair routes are split between the parent pronunciation route and direct item routes.

Parent base path:

```text
/api/v1/pronunciations/{pronunciationId}/minimal-pairs
```

Item base path:

```text
/api/v1/minimal-pairs
```

The API returns contract DTOs from `EnglishMaster.Contracts.MinimalPairs`. It does not expose EF Core entities or internal media storage paths.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/pronunciations/{pronunciationId}/minimal-pairs` | List minimal pairs for a pronunciation. |
| `POST` | `/api/v1/pronunciations/{pronunciationId}/minimal-pairs` | Add a minimal pair. |
| `GET` | `/api/v1/minimal-pairs/{id}` | Get one minimal pair by id. |
| `PUT` | `/api/v1/minimal-pairs/{id}` | Update a minimal pair. |
| `DELETE` | `/api/v1/minimal-pairs/{id}` | Deactivate a minimal pair. |

## List By Pronunciation

```http
GET /api/v1/pronunciations/55555555-5555-5555-5555-555555555555/minimal-pairs
```

Success returns `200 OK` with an array of `MinimalPairDto` items ordered by `SortOrder` and `PairWordText`. Unknown pronunciation ids return `404 Not Found`.

Response:

```json
[
  {
    "id": "33333333-3333-3333-3333-333333333333",
    "pronunciationId": "55555555-5555-5555-5555-555555555555",
    "pairWordText": "yellow",
    "pairIpa": "/jellow/",
    "pairThaiReading": "yel-low",
    "differenceNote": "Practice the initial y sound.",
    "audioMediaId": "22222222-2222-2222-2222-222222222222",
    "audioMedia": {
      "id": "22222222-2222-2222-2222-222222222222",
      "fileName": "yellow.mp3",
      "contentType": "audio/mpeg",
      "mediaType": "Audio",
      "publicUrl": "/media/yellow.mp3",
      "altText": ""
    },
    "sortOrder": 1,
    "isActive": true,
    "createdAt": "2026-07-10T00:00:00+00:00",
    "updatedAt": "2026-07-10T00:00:00+00:00"
  }
]
```

## Add Minimal Pair

```http
POST /api/v1/pronunciations/55555555-5555-5555-5555-555555555555/minimal-pairs
Content-Type: application/json
```

Request:

```json
{
  "pairWordText": "yellow",
  "pairIpa": "/jellow/",
  "pairThaiReading": "yel-low",
  "differenceNote": "Practice the initial y sound.",
  "audioMediaId": "22222222-2222-2222-2222-222222222222",
  "sortOrder": 1
}
```

Success returns `201 Created` with the created `MinimalPairDto`.

Validation errors return `400 Bad Request`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "pairWordText": [
      "pairWordText is required."
    ]
  }
}
```

## Get By Id

```http
GET /api/v1/minimal-pairs/33333333-3333-3333-3333-333333333333
```

Success returns `200 OK` with a `MinimalPairDto`. Unknown ids return `404 Not Found`.

## Update Minimal Pair

```http
PUT /api/v1/minimal-pairs/33333333-3333-3333-3333-333333333333
Content-Type: application/json
```

Request:

```json
{
  "pairWordText": "jello",
  "pairIpa": "/jello/",
  "pairThaiReading": "jel-lo",
  "differenceNote": "Practice the soft j sound.",
  "audioMediaId": "22222222-2222-2222-2222-222222222222",
  "sortOrder": 2,
  "isActive": true
}
```

Success returns `200 OK` with the updated `MinimalPairDto`. Unknown ids return `404 Not Found`.

## Delete Minimal Pair

```http
DELETE /api/v1/minimal-pairs/33333333-3333-3333-3333-333333333333
```

Success returns `204 No Content`. The minimal pair is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Domain And Validation Notes

- The parent pronunciation must exist.
- New minimal pairs cannot be added to inactive pronunciations.
- `PairWordText` is required.
- `SortOrder` must be greater than or equal to zero.
- `AudioMediaId` must reference active audio media when provided.
- A minimal pair cannot be reassigned to another pronunciation through update.

## Known Limitations

- There is no standalone search or pagination endpoint for minimal pairs.
- Endpoints are not authenticated yet.
- Delete is a soft delete through `IsActive = false`.
- No pronunciation scoring or automatic minimal-pair generation exists yet.
