# Minimal Pair

## Purpose

Minimal Pair is the practice sub-slice of the Pronunciation Module. It stores words or phrases that contrast with the main pronunciation so learners can notice and practice sound differences.

Minimal Pair belongs to Pronunciation and does not stand alone as a separate top-level learning module.

## MinimalPair Fields

| Field | Description |
| --- | --- |
| `Id` | Unique minimal pair identifier. |
| `PronunciationId` | Required pronunciation identifier. |
| `PairWordText` | Required contrasting word or phrase. |
| `PairIpa` | Optional IPA transcription for the pair word. |
| `PairThaiReading` | Optional Thai reading aid for the pair word. |
| `DifferenceNote` | Optional note explaining the sound difference. |
| `AudioMediaId` | Optional audio media identifier. |
| `AudioMedia` | Optional audio media summary in response DTOs. |
| `SortOrder` | Display order within the pronunciation. |
| `IsActive` | Indicates whether the minimal pair is active. Deletes deactivate a minimal pair. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Pronunciation Relationship

A pronunciation can have many minimal pairs.

- `MinimalPair.PronunciationId` is required.
- EF Core maps the relationship from `Pronunciation` to many `MinimalPairs`.
- Minimal pairs cascade when the parent pronunciation row is hard-deleted.
- Application handlers require the parent pronunciation to exist.
- New minimal pairs cannot be added to inactive pronunciations.

## Media Relationship

Minimal Pair can reference one optional audio media item.

- `AudioMediaId` must reference active `MediaType = Audio` when provided.
- Empty GUID values are rejected.
- Response DTOs expose a media summary, not the internal storage path.

## Domain Rules

- `PronunciationId` is required and cannot be empty.
- `PairWordText` is required and trimmed.
- `SortOrder` must be greater than or equal to zero.
- Optional text fields are trimmed and stored as empty strings when omitted.
- Optional `AudioMediaId` cannot be an empty GUID when provided.
- `Activate` and `Deactivate` behavior exists.
- A minimal pair cannot be reassigned to another pronunciation through update.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `AddMinimalPair`
- `UpdateMinimalPair`
- `DeleteMinimalPair`
- `GetMinimalPairById`
- `GetMinimalPairsByPronunciationId`

The Application layer validates input, validates parent pronunciation existence, checks audio media type, uses cancellation tokens, and returns contract DTOs rather than EF entities.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/pronunciations/{pronunciationId}/minimal-pairs` | List minimal pairs for a pronunciation. |
| `POST` | `/api/v1/pronunciations/{pronunciationId}/minimal-pairs` | Add a minimal pair to a pronunciation. |
| `GET` | `/api/v1/minimal-pairs/{id}` | Get one minimal pair by id. |
| `PUT` | `/api/v1/minimal-pairs/{id}` | Update a minimal pair. |
| `DELETE` | `/api/v1/minimal-pairs/{id}` | Deactivate a minimal pair. |

## Request Example

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

## Response Example

```json
{
  "id": "33333333-3333-3333-3333-333333333333",
  "pronunciationId": "11111111-1111-1111-1111-111111111111",
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
```

## Blazor UI

Minimal Pair UI is embedded under the Pronunciation detail page:

- List minimal pairs for the pronunciation.
- Add a new minimal pair.
- Edit an existing minimal pair.
- Deactivate a minimal pair.
- Play minimal pair audio when media is available.
- Show loading, empty, error, and validation states through the parent page and form.

## Test Coverage

Current tests cover:

- MinimalPair entity creation and invariants
- Required pair word validation
- Sort order validation
- AddMinimalPair behavior
- AddMinimalPair validation
- Rejecting add when the parent pronunciation is inactive
- API add/list/update/delete flow
- Word detail excludes inactive minimal pairs from the pronunciation summary

## Known Limitations

- There is no standalone minimal-pair search endpoint.
- Delete deactivates the minimal pair instead of hard-deleting it.
- Minimal pairs are managed only from the Pronunciation detail UI.
- No automatic ordering or drag-and-drop sort UI is implemented.
- Endpoints are not authenticated yet.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
