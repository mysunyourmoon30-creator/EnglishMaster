# Media Module

## Purpose

The Media Module manages reusable media assets for the English learning platform. It currently supports storing metadata, uploading local files, searching media records, assigning image or audio media to vocabulary words, and supporting pronunciation audio, minimal-pair audio, and mouth-position images.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Media`
- Application: `EnglishMaster.Application.Features.Media`
- Infrastructure: `EnglishMaster.Infrastructure.Media`
- API: `EnglishMaster.Api.Endpoints.MediaEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Media`

## Media Fields

| Field | Description |
| --- | --- |
| `Id` | Unique media identifier. |
| `FileName` | Required stored file name. Uploads generate a safe server-side file name. |
| `OriginalFileName` | Required original client file name. Path-like names are rejected during upload. |
| `FileExtension` | File extension such as `.jpg`, `.mp3`, or `.pdf`. |
| `ContentType` | Required MIME content type. |
| `FileSize` | Required file size in bytes. Must be greater than zero. |
| `MediaType` | Required logical media type. |
| `StoragePath` | Internal relative storage path. It is stored in the domain and database but is not exposed by API response DTOs. |
| `PublicUrl` | Public local URL path used by Blazor preview/player controls. |
| `AltText` | Optional alt text, especially for images. |
| `Description` | Optional administrative description. |
| `IsActive` | Indicates whether the media can be selected for new relationships. Deletes deactivate media. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## MediaType Values

- `Image`
- `Audio`
- `Video`
- `Document`
- `Other`

## Word Relationship

Words can optionally reference media:

- `ImageMediaId`: zero or one image media item.
- `AudioMediaId`: zero or one audio media item.

Media is reusable. A single media record can be referenced by more than one word. The Application layer validates that:

- `ImageMediaId` points to an active media item with `MediaType = Image`.
- `AudioMediaId` points to an active media item with `MediaType = Audio`.
- Empty GUID values are rejected.

`GetWordById` and `SearchWords` return lightweight media summaries for assigned image and audio media.

## Pronunciation Relationship

Pronunciations can optionally reference media:

- `AudioSlowMediaId`: zero or one slow pronunciation audio item.
- `AudioNormalMediaId`: zero or one normal-speed pronunciation audio item.
- `MouthImageMediaId`: zero or one mouth-position image item.

Minimal pairs can optionally reference media:

- `AudioMediaId`: zero or one audio item for the pair word.

Media is reusable across words, pronunciations, and minimal pairs. The Application layer validates that:

- Slow pronunciation audio points to active `MediaType = Audio`.
- Normal pronunciation audio points to active `MediaType = Audio`.
- Minimal pair audio points to active `MediaType = Audio`.
- Mouth image media points to active `MediaType = Image`.
- Empty GUID values are rejected.

Pronunciation and Minimal Pair response DTOs return lightweight media summaries with `PublicUrl`, not internal `StoragePath`.

## Domain Rules

- `FileName` is required and trimmed.
- `OriginalFileName` is required and trimmed.
- `ContentType` is required and trimmed.
- `FileSize` must be greater than zero.
- `StoragePath` is required and trimmed.
- `MediaType` must be a defined enum value.
- Optional text fields are trimmed and stored as empty strings when not provided.
- `Activate` and `Deactivate` behavior exists on the `Media` entity.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateMedia`
- `UpdateMedia`
- `DeleteMedia`
- `ActivateMedia`
- `DeactivateMedia`
- `UploadMedia`
- `GetMediaById`
- `SearchMedia`

The Application layer validates metadata, uses cancellation tokens, calls storage through `IMediaStorageService`, and returns contract DTOs rather than EF entities.

## Storage Abstraction

`IMediaStorageService` lives in the Application layer so upload use cases depend on an abstraction. The Infrastructure layer implements it with `LocalMediaStorageService`.

The storage abstraction returns `StoredMediaFile`, which includes the generated file name, content type, media type, internal storage path, and public URL. Application handlers create the `Media` entity from this storage result.

## Local Storage Behavior

`LocalMediaStorageService` stores uploaded files under a local `media` directory beneath the API application base directory. The API serves that directory at:

```text
/media
```

Uploaded files receive generated file names using a GUID and a safe extension derived from the verified content type. The internal `StoragePath` is stored for persistence but is not returned to API clients.

Manual metadata creation is supported for trusted admin workflows. For metadata-only records, the Application layer derives the internal storage path from the validated file name.

## Upload Rules

Upload is implemented through:

```text
POST /api/v1/media/upload
```

Rules:

- Maximum upload size is 25 MB.
- File size must be greater than zero.
- File size declared by the request must match the stream content written by local storage.
- Original file name must be a file name, not a path.
- Content type must be allowed.
- File signature must match the declared content type.
- Files that fail validation after writing begins are deleted.

Currently allowed upload content types:

| Content Type | Extension | MediaType |
| --- | --- | --- |
| `image/png` | `.png` | `Image` |
| `image/jpeg` | `.jpg` | `Image` |
| `image/gif` | `.gif` | `Image` |
| `image/webp` | `.webp` | `Image` |
| `audio/mpeg` | `.mp3` | `Audio` |
| `audio/wav` | `.wav` | `Audio` |
| `video/mp4` | `.mp4` | `Video` |
| `application/pdf` | `.pdf` | `Document` |
| `text/plain` | `.txt` | `Document` |

## Security Rules For File Handling

- API responses do not expose internal `StoragePath`.
- Upload paths are generated by the server.
- Path traversal is rejected for original file names, storage paths, and public URL paths.
- Public URLs must be safe local paths.
- Extension is determined from allowed content type during upload.
- Content signature is checked before the file is accepted.
- Upload request size is limited at the API endpoint and checked again in Application and Infrastructure.
- Internal exceptions are handled by the API exception handler outside development/test environments.

## Search And Pagination

`SearchMedia` supports:

- Search by `FileName`, `OriginalFileName`, `AltText`, and `Description`
- Filter by `MediaType`
- Filter by `ContentType`
- Filter by `IsActive`
- Pagination with `PageNumber` and `PageSize`

Default behavior returns active media, sorted by `CreatedAt` descending and then `FileName`.

## Persistence

EF Core maps `Media` to the `Media` table. The SQL Server configuration includes:

- Required fields and maximum lengths
- String enum conversion for `MediaType`
- Indexes on `FileName`, `MediaType`, `ContentType`, `IsActive`, and `CreatedAt`
- Nullable `Words.ImageMediaId` and `Words.AudioMediaId` foreign keys to `Media.Id`
- Nullable `Pronunciations.AudioSlowMediaId`, `Pronunciations.AudioNormalMediaId`, and `Pronunciations.MouthImageMediaId` foreign keys to `Media.Id`
- Nullable `MinimalPairs.AudioMediaId` foreign key to `Media.Id`
- `SetNull` delete behavior for both Word-to-Media relationships
- `SetNull` delete behavior for Pronunciation-to-Media and MinimalPair-to-Media relationships

The migration `AddMediaModule` adds the `Media` table and nullable media columns on `Words`. The later Pronunciation migration adds nullable media references for pronunciation and minimal-pair usage. These migrations do not drop or rewrite existing Word, Category, or Tag tables.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Media list | `/media` | Search, filter, paginate, view, edit, and deactivate media. |
| Create media | `/media/new` | Create a metadata-only media record with validation. |
| Upload media | `/media/upload` | Upload a file with alt text and description. |
| Media detail | `/media/{id}` | View media metadata and image/audio preview when available. |
| Edit media | `/media/{id}/edit` | Update metadata and active status. |

Word create/edit pages also load active image and audio media so administrators can assign one image and one audio file to a word. Pronunciation create/edit pages load active audio and image media for pronunciation audio and mouth image selection. Pronunciation detail and Word detail render audio players and mouth image previews when public URLs are available.

## Test Coverage

Current tests cover:

- Media entity creation and invariants
- Required file name validation
- Positive file size validation
- Undefined `MediaType` rejection
- Activate and deactivate behavior
- SearchMedia filtering by `MediaType`
- Invalid media type validation
- API create/get/search/update/delete flow
- API validation responses
- API response does not expose `storagePath`
- Unsafe public URL rejection
- Word creation and update with image and audio media
- Word search and get responses with media summaries
- Pronunciation creation and update with slow audio, normal audio, and mouth image media
- Minimal Pair creation and update with audio media
- Existing Word, Category, Tag, and architecture tests

## Known Limitations

- Media admin endpoints and pages are not protected by authentication or authorization yet.
- Delete currently deactivates media instead of hard-deleting metadata or physical files.
- Local storage is intended for the initial module only; cloud/object storage is not implemented.
- Virus scanning and image processing are not implemented.
- Thumbnail generation is not implemented.
- File replacement is not implemented; metadata edit does not upload a new file.
- Upload progress UI is not implemented.
- Media deactivation does not automatically deactivate words, pronunciations, or minimal pairs that already reference it.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
