# Media API

Base path:

```text
/api/v1/media
```

The API returns contract DTOs from `EnglishMaster.Contracts.Media`. It does not expose EF Core entities or internal storage paths.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/media` | Search, filter, and paginate media. |
| `GET` | `/api/v1/media/{id}` | Get one media item by id. |
| `POST` | `/api/v1/media` | Create a metadata-only media record. |
| `POST` | `/api/v1/media/upload` | Upload a local media file and create metadata. |
| `PUT` | `/api/v1/media/{id}` | Update media metadata. |
| `POST` | `/api/v1/media/{id}/activate` | Activate media. |
| `POST` | `/api/v1/media/{id}/deactivate` | Deactivate media. |
| `DELETE` | `/api/v1/media/{id}` | Deactivate media. |

## Search Parameters

`GET /api/v1/media` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `FileName`, `OriginalFileName`, `AltText`, and `Description`. |
| `mediaType` | string | Optional media type filter: `Image`, `Audio`, `Video`, `Document`, or `Other`. |
| `contentType` | string | Optional content type filter such as `image/jpeg`. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |

Example:

```http
GET /api/v1/media?search=hello&mediaType=Audio&contentType=audio/mpeg&isActive=true&pageNumber=1&pageSize=20
```

Response:

```json
{
  "items": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "fileName": "hello-audio.mp3",
      "originalFileName": "hello.mp3",
      "fileExtension": ".mp3",
      "contentType": "audio/mpeg",
      "fileSize": 20480,
      "mediaType": "Audio",
      "publicUrl": "/media/hello-audio.mp3",
      "altText": "",
      "description": "Audio pronunciation for hello.",
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

## Get By Id

```http
GET /api/v1/media/11111111-1111-1111-1111-111111111111
```

Success returns `200 OK` with a `MediaDto`. Unknown ids return `404 Not Found`.

Response:

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "fileName": "hello-image.jpg",
  "originalFileName": "hello.jpg",
  "fileExtension": ".jpg",
  "contentType": "image/jpeg",
  "fileSize": 51200,
  "mediaType": "Image",
  "publicUrl": "/media/hello-image.jpg",
  "altText": "Illustration for hello",
  "description": "Image used on the word detail page.",
  "isActive": true,
  "createdAt": "2026-07-10T00:00:00+00:00",
  "updatedAt": "2026-07-10T00:00:00+00:00"
}
```

`storagePath` is intentionally omitted from responses.

## Create Media Metadata

```http
POST /api/v1/media
Content-Type: application/json
```

Request:

```json
{
  "fileName": "hello-image.jpg",
  "originalFileName": "hello.jpg",
  "fileExtension": ".jpg",
  "contentType": "image/jpeg",
  "fileSize": 51200,
  "mediaType": "Image",
  "publicUrl": "/media/hello-image.jpg",
  "altText": "Illustration for hello",
  "description": "Image used on the word detail page."
}
```

Success returns `201 Created` with the created `MediaDto`.

Validation errors return `400 Bad Request` with validation problem details:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "fileName": [
      "fileName is required."
    ],
    "fileSize": [
      "fileSize must be greater than zero."
    ]
  }
}
```

The Application layer derives the internal storage path from the validated file name. Clients do not provide `storagePath`.

## Upload Media

```http
POST /api/v1/media/upload
Content-Type: multipart/form-data
```

Form fields:

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `file` | file | Yes | File to upload. |
| `altText` | string | No | Alt text for image media. |
| `description` | string | No | Administrative description. |

Example multipart fields:

```text
file: hello.mp3
altText:
description: Audio pronunciation for hello.
```

Success returns `201 Created` with the created `MediaDto`.

Upload validation:

- Maximum file size is 25 MB.
- File size must be greater than zero.
- Original file name must not contain path separators.
- Content type must be allowed.
- File signature must match the declared content type.
- Failed partial writes are deleted.

## Update Media

```http
PUT /api/v1/media/11111111-1111-1111-1111-111111111111
Content-Type: application/json
```

Request:

```json
{
  "fileName": "hello-image.jpg",
  "originalFileName": "hello.jpg",
  "fileExtension": ".jpg",
  "contentType": "image/jpeg",
  "fileSize": 51200,
  "mediaType": "Image",
  "publicUrl": "/media/hello-image.jpg",
  "altText": "Updated illustration for hello",
  "description": "Updated metadata.",
  "isActive": true
}
```

Success returns `200 OK` with the updated `MediaDto`. Unknown ids return `404 Not Found`.

The existing internal storage path is preserved during updates.

## Activate Media

```http
POST /api/v1/media/11111111-1111-1111-1111-111111111111/activate
```

Success returns `200 OK` with the activated `MediaDto`. Unknown ids return `404 Not Found`.

## Deactivate Or Delete Media

```http
POST /api/v1/media/11111111-1111-1111-1111-111111111111/deactivate
DELETE /api/v1/media/11111111-1111-1111-1111-111111111111
```

`POST /deactivate` returns `200 OK` with the deactivated `MediaDto`.

`DELETE` returns `204 No Content`. It deactivates media rather than hard-deleting metadata or physical files.

## Known Limitations

- Endpoints are not authenticated yet.
- Local file storage is the only implemented storage provider.
- Upload does not perform virus scanning.
- Delete does not remove physical files.
- Metadata create/update does not upload or replace file content.
