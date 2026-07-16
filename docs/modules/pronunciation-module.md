# Pronunciation Module

## Purpose

The Pronunciation Module manages pronunciation learning data for vocabulary words. It stores IPA variants, Thai reading aids, syllable and stress notes, mouth and tongue guidance, pronunciation audio, mouth-position imagery, and minimal-pair practice data.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Pronunciations`
- Application: `EnglishMaster.Application.Features.Pronunciations`
- Infrastructure: `EnglishMaster.Infrastructure.Pronunciations`
- API: `EnglishMaster.Api.Endpoints.PronunciationEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Pronunciations`

## Pronunciation Fields

| Field | Description |
| --- | --- |
| `Id` | Unique pronunciation identifier. |
| `WordId` | Required word identifier. A pronunciation belongs to one word. |
| `IpaUk` | UK IPA transcription. Required when `IpaUs` is empty. |
| `IpaUs` | US IPA transcription. Required when `IpaUk` is empty. |
| `ThaiReading` | Optional Thai reading aid for learners. |
| `Syllables` | Optional syllable breakdown. |
| `StressPattern` | Optional stress or rhythm note. |
| `MouthPosition` | Optional mouth-shape guidance. |
| `TonguePosition` | Optional tongue-placement guidance. |
| `CommonMistake` | Optional learner mistake note. |
| `PracticeNote` | Optional practice instruction. |
| `AudioSlowMediaId` | Optional slow pronunciation audio media identifier. |
| `AudioSlowMedia` | Optional slow audio media summary in response DTOs. |
| `AudioNormalMediaId` | Optional normal-speed pronunciation audio media identifier. |
| `AudioNormalMedia` | Optional normal audio media summary in response DTOs. |
| `MouthImageMediaId` | Optional mouth-position image media identifier. |
| `MouthImageMedia` | Optional mouth image media summary in response DTOs. |
| `MinimalPairs` | Minimal pair practice items for this pronunciation. |
| `IsActive` | Indicates whether the pronunciation is active. Deletes deactivate a pronunciation. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Word Relationship

A word can have zero or one pronunciation record.

- `Pronunciation.WordId` is required.
- EF Core enforces a unique index on `Pronunciations.WordId`.
- The Application layer checks the one-pronunciation-per-word rule before create and update.
- `GET /api/v1/words/{wordId}/pronunciation` returns the pronunciation for a word.
- `GetWordById` returns a pronunciation summary when one exists.
- `SearchWords` remains lightweight and does not load pronunciation data.

## Media Relationship

Pronunciation can reference reusable Media records:

- `AudioSlowMediaId` must reference active `MediaType = Audio` when provided.
- `AudioNormalMediaId` must reference active `MediaType = Audio` when provided.
- `MouthImageMediaId` must reference active `MediaType = Image` when provided.
- Empty GUID values are rejected.
- Media summaries expose `PublicUrl`, content type, media type, file name, and alt text.
- Internal media `StoragePath` is not exposed.

## IPA UK And IPA US Usage

`IpaUk` and `IpaUs` store pronunciation variants for UK and US English. At least one value is required so every pronunciation record has a usable phonetic reference. When both values are provided, the UI can show both variants side by side.

## ThaiReading Usage

`ThaiReading` is optional and intended as a learner aid for Thai speakers. It should not replace IPA as the primary pronunciation model. It can be used to help early learners approximate the sound while they learn IPA and audio cues.

## Mouth And Tongue Guidance

`MouthPosition` stores visible articulation guidance such as lip shape or jaw opening. `TonguePosition` stores tongue-placement guidance such as tongue height, tongue tip position, or contact points. These fields are plain text guidance managed by admins.

## Audio And Mouth Image Usage

Slow and normal audio are separate optional media references so learners can compare careful pronunciation with natural-speed pronunciation. Mouth image media is optional and should show a clear mouth-position reference when available.

## Domain Rules

- `Pronunciation` belongs to one word through `WordId`.
- `WordId` cannot be empty.
- A word can have only one pronunciation record.
- `IpaUk` or `IpaUs` is required.
- Optional text fields are trimmed and stored as empty strings when omitted.
- Optional media ids cannot be empty GUIDs when provided.
- `Activate` and `Deactivate` behavior exists on the entity.
- Domain code is independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreatePronunciation`
- `UpdatePronunciation`
- `DeletePronunciation`
- `GetPronunciationById`
- `GetPronunciationByWordId`
- `SearchPronunciations`
- `ActivatePronunciation`
- `DeactivatePronunciation`

The Application layer validates input, checks word and media references, enforces one pronunciation per word, uses cancellation tokens, and returns contract DTOs rather than EF entities.

## Search And Pagination

`SearchPronunciations` supports:

- Search by IPA and pronunciation helper fields
- Filter by `WordId`
- Filter by `IsActive`
- Pagination with `PageNumber` and `PageSize`

Default behavior returns active pronunciations, sorted by `CreatedAt` descending.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/pronunciations` | Search, filter, and paginate pronunciations. |
| `GET` | `/api/v1/pronunciations/{id}` | Get one pronunciation by id. |
| `GET` | `/api/v1/words/{wordId}/pronunciation` | Get the pronunciation for one word. |
| `POST` | `/api/v1/pronunciations` | Create a pronunciation. |
| `PUT` | `/api/v1/pronunciations/{id}` | Update a pronunciation. |
| `DELETE` | `/api/v1/pronunciations/{id}` | Deactivate a pronunciation. |

## Request Example

```json
{
  "wordId": "11111111-1111-1111-1111-111111111111",
  "ipaUk": "/hallo/",
  "ipaUs": "/hello/",
  "thaiReading": "heh-lo",
  "syllables": "hel-lo",
  "stressPattern": "first syllable",
  "mouthPosition": "Open the mouth slightly and relax the lips.",
  "tonguePosition": "Keep the tongue low and relaxed.",
  "commonMistake": "Avoid dropping the initial h sound.",
  "practiceNote": "Practice slowly before using normal speed.",
  "audioSlowMediaId": "22222222-2222-2222-2222-222222222222",
  "audioNormalMediaId": "33333333-3333-3333-3333-333333333333",
  "mouthImageMediaId": "44444444-4444-4444-4444-444444444444"
}
```

## Response Example

```json
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
  "mouthPosition": "Open the mouth slightly and relax the lips.",
  "tonguePosition": "Keep the tongue low and relaxed.",
  "commonMistake": "Avoid dropping the initial h sound.",
  "practiceNote": "Practice slowly before using normal speed.",
  "audioSlowMediaId": "22222222-2222-2222-2222-222222222222",
  "audioSlowMedia": {
    "id": "22222222-2222-2222-2222-222222222222",
    "fileName": "hello-slow.mp3",
    "contentType": "audio/mpeg",
    "mediaType": "Audio",
    "publicUrl": "/media/hello-slow.mp3",
    "altText": ""
  },
  "audioNormalMediaId": "33333333-3333-3333-3333-333333333333",
  "audioNormalMedia": {
    "id": "33333333-3333-3333-3333-333333333333",
    "fileName": "hello-normal.mp3",
    "contentType": "audio/mpeg",
    "mediaType": "Audio",
    "publicUrl": "/media/hello-normal.mp3",
    "altText": ""
  },
  "mouthImageMediaId": "44444444-4444-4444-4444-444444444444",
  "mouthImageMedia": {
    "id": "44444444-4444-4444-4444-444444444444",
    "fileName": "hello-mouth.jpg",
    "contentType": "image/jpeg",
    "mediaType": "Image",
    "publicUrl": "/media/hello-mouth.jpg",
    "altText": "Mouth position for hello"
  },
  "minimalPairs": [],
  "isActive": true,
  "createdAt": "2026-07-10T00:00:00+00:00",
  "updatedAt": "2026-07-10T00:00:00+00:00"
}
```

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Pronunciation list | `/pronunciations` | Search, filter, paginate, view, edit, and deactivate pronunciations. |
| Create pronunciation | `/pronunciations/new` | Create a pronunciation with word, audio, and mouth image selections. |
| Pronunciation detail | `/pronunciations/{id}` | View pronunciation details, audio players, mouth image preview, and minimal pairs. |
| Edit pronunciation | `/pronunciations/{id}/edit` | Update pronunciation data, media references, and active status. |

Word detail also shows pronunciation data when available and links to create or edit the pronunciation.

## Test Coverage

Current tests cover:

- Pronunciation entity creation and invariants
- IPA UK or IPA US validation
- Activate and deactivate behavior
- One word has one pronunciation rule
- Media type validation for pronunciation media slots
- `GetPronunciationByWordId`
- Word detail with pronunciation summary
- API create/get/search/update/delete flow
- API word pronunciation endpoint
- Existing Word, Category, Tag, Media, and architecture tests

## Known Limitations

- Pronunciation admin endpoints and pages are not protected by authentication or authorization yet.
- Delete deactivates a pronunciation rather than hard-deleting it.
- Search is simple substring matching, not full-text or phonetic search.
- No waveform, recording, or speech scoring features are implemented.
- Audio and image quality review is an admin responsibility.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
