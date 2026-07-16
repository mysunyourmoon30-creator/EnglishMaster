# Pronunciation API

Base path:

```text
/api/v1/pronunciations
```

The API returns contract DTOs from `EnglishMaster.Contracts.Pronunciations`. It does not expose EF Core entities or internal media storage paths.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/pronunciations` | Search, filter, and paginate pronunciations. |
| `GET` | `/api/v1/pronunciations/{id}` | Get one pronunciation by id. |
| `GET` | `/api/v1/words/{wordId}/pronunciation` | Get the pronunciation for one word. |
| `POST` | `/api/v1/pronunciations` | Create a pronunciation. |
| `PUT` | `/api/v1/pronunciations/{id}` | Update a pronunciation. |
| `DELETE` | `/api/v1/pronunciations/{id}` | Deactivate a pronunciation. |

## Search Parameters

`GET /api/v1/pronunciations` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches IPA and helper fields such as Thai reading, syllables, and stress pattern. |
| `wordId` | guid | Optional word filter. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |

Example:

```http
GET /api/v1/pronunciations?search=hallo&wordId=11111111-1111-1111-1111-111111111111&isActive=true&pageNumber=1&pageSize=20
```

Response:

```json
{
  "items": [
    {
      "id": "55555555-5555-5555-5555-555555555555",
      "wordId": "11111111-1111-1111-1111-111111111111",
      "word": {
        "id": "11111111-1111-1111-1111-111111111111",
        "text": "hello",
        "slug": "hello"
      },
      "ipaUk": "/hallo/",
      "ipaUs": "/hello/",
      "thaiReading": "heh-lo",
      "syllables": "hel-lo",
      "stressPattern": "first syllable",
      "mouthPosition": "Open the mouth slightly.",
      "tonguePosition": "Keep the tongue low.",
      "commonMistake": "Dropping the initial h sound.",
      "practiceNote": "Practice slowly first.",
      "audioSlowMediaId": null,
      "audioSlowMedia": null,
      "audioNormalMediaId": null,
      "audioNormalMedia": null,
      "mouthImageMediaId": null,
      "mouthImageMedia": null,
      "minimalPairs": [],
      "isActive": true,
      "createdAt": "2026-07-10T00:00:00+00:00",
      "updatedAt": "2026-07-10T00:00:00+00:00"
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

Search responses intentionally omit minimal pairs for the list path.

## Get By Id

```http
GET /api/v1/pronunciations/55555555-5555-5555-5555-555555555555
```

Success returns `200 OK` with a `PronunciationDto`. Unknown ids return `404 Not Found`.

## Get By Word Id

```http
GET /api/v1/words/11111111-1111-1111-1111-111111111111/pronunciation
```

Success returns `200 OK` with the word pronunciation. If the word has no pronunciation, the response is `404 Not Found`.

## Create Pronunciation

```http
POST /api/v1/pronunciations
Content-Type: application/json
```

Request:

```json
{
  "wordId": "11111111-1111-1111-1111-111111111111",
  "ipaUk": "/hallo/",
  "ipaUs": "/hello/",
  "thaiReading": "heh-lo",
  "syllables": "hel-lo",
  "stressPattern": "first syllable",
  "mouthPosition": "Open the mouth slightly.",
  "tonguePosition": "Keep the tongue low.",
  "commonMistake": "Dropping the initial h sound.",
  "practiceNote": "Practice slowly first.",
  "audioSlowMediaId": "22222222-2222-2222-2222-222222222222",
  "audioNormalMediaId": "33333333-3333-3333-3333-333333333333",
  "mouthImageMediaId": "44444444-4444-4444-4444-444444444444"
}
```

Success returns `201 Created` with the created `PronunciationDto`.

Validation errors return `400 Bad Request`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ipaUk": [
      "IPA UK or IPA US is required."
    ]
  }
}
```

## Update Pronunciation

```http
PUT /api/v1/pronunciations/55555555-5555-5555-5555-555555555555
Content-Type: application/json
```

Request:

```json
{
  "wordId": "11111111-1111-1111-1111-111111111111",
  "ipaUk": "/hullo/",
  "ipaUs": "/hello/",
  "thaiReading": "heh-lo",
  "syllables": "hel-lo",
  "stressPattern": "first syllable",
  "mouthPosition": "Open the mouth slightly.",
  "tonguePosition": "Keep the tongue low.",
  "commonMistake": "Dropping the initial h sound.",
  "practiceNote": "Practice daily.",
  "audioSlowMediaId": "22222222-2222-2222-2222-222222222222",
  "audioNormalMediaId": "33333333-3333-3333-3333-333333333333",
  "mouthImageMediaId": "44444444-4444-4444-4444-444444444444",
  "isActive": true
}
```

Success returns `200 OK` with the updated `PronunciationDto`. Unknown ids return `404 Not Found`.

## Delete Pronunciation

```http
DELETE /api/v1/pronunciations/55555555-5555-5555-5555-555555555555
```

Success returns `204 No Content`. The pronunciation is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Domain And Validation Notes

- `WordId` is required.
- At least one of `IpaUk` or `IpaUs` is required.
- A word can have only one pronunciation.
- Slow and normal audio ids must reference active audio media.
- Mouth image id must reference active image media.
- Empty GUID values are rejected.

## Known Limitations

- Endpoints are not authenticated yet.
- Delete is a soft delete through `IsActive = false`.
- Search is simple substring matching, not phonetic search.
- No speech recording or scoring endpoints exist yet.
