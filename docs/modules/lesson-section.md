# Lesson Section

## Purpose

`LessonSection` is an ordered content block within a `Lesson` — a piece of the lesson's flow such as a vocabulary warm-up, a grammar focus block, a listening exercise, or a practice activity. It belongs to one lesson and does not stand alone as a separate top-level learning module, the same embedded-child role `GrammarExample` plays for `GrammarRule`.

## LessonSection Fields

| Field | Description |
| --- | --- |
| `Id` | Unique section identifier. |
| `LessonId` | Required identifier of the parent lesson. |
| `Title` | Required section title. |
| `ContentMarkdown` | Optional Markdown content for the section. |
| `SectionType` | The kind of content this section holds — see SectionType Values below. |
| `MediaId` | Optional media identifier. |
| `Media` | Optional media summary in response DTOs. |
| `SortOrder` | Display order among a lesson's sections. Must be zero or greater. |
| `IsActive` | Indicates whether the section is active. Deletes deactivate a section. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## SectionType Values

`Text`, `Vocabulary`, `Grammar`, `Pronunciation`, `Listening`, `Speaking`, `Reading`, `Writing`, `Practice`, `Summary`.

## Lesson Relationship

A lesson can have many sections.

- `LessonId` is required and cannot be empty.
- The parent lesson must exist and be active before a new section can be added (`AddLessonSectionCommandHandler`) — mirrors `AddGrammarExampleCommandHandler`'s inactive-parent check.
- EF Core maps the relationship from `Lesson` to many `LessonSection` records with a cascading foreign key.
- `Lesson` does not expose an `AddSection`/`RemoveSection` aggregate method — sections are created and managed directly against their own repository, the same pattern `GrammarRule` uses for `GrammarExample`.

## Media Relationship

`LessonSection` can reference one optional `Media` item.

- `MediaId` must reference active media when provided — unlike `Lesson.ThumbnailMediaId`, there is no `MediaType` constraint, so any active media item (image, audio, video, document) can be attached to a section.
- Empty GUID values are rejected.
- Response DTOs expose a media summary (`FileName`, `ContentType`, `MediaType`, `PublicUrl`, `AltText`), not the internal storage path.

## Domain Rules

- `LessonId` is required and cannot be an empty GUID.
- `Title` is required, trimmed, and limited to 200 characters.
- `ContentMarkdown` is optional, trimmed, and limited to 8000 characters.
- `SectionType` must be a defined enum value.
- `MediaId` is optional but cannot be an empty GUID when provided.
- `SortOrder` must be greater than or equal to zero.
- `Activate`/`Deactivate` and `Reorder` (updates `SortOrder` and `UpdatedAt`) behavior exist on the entity.
- `LessonSection` has no `Slug` — sections are not independently browsable or routable, the same reasoning as `GrammarExample`.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `AddLessonSection`
- `UpdateLessonSection`
- `DeleteLessonSection`
- `GetLessonSectionById` *(no API route — see Known Limitations)*
- `GetLessonSectionsByLessonId`
- `ReorderLessonSections`

The Application layer validates input, validates that the parent lesson exists and is active, validates any referenced media, uses cancellation tokens, and returns contract DTOs from `EnglishMaster.Contracts.LessonSections` rather than EF entities.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/lessons/{lessonId}/sections` | List sections for a lesson. |
| `POST` | `/api/v1/lessons/{lessonId}/sections` | Add a section to a lesson. |
| `PUT` | `/api/v1/lesson-sections/{id}` | Update a section. |
| `DELETE` | `/api/v1/lesson-sections/{id}` | Deactivate a section. |
| `POST` | `/api/v1/lessons/{lessonId}/sections/reorder` | Reorder a lesson's sections. |

Full request/response detail and status codes are documented in [Lesson Section API](../api/lesson-section-api.md).

## Request Example

```json
{
  "title": "Vocabulary Warm-Up",
  "contentMarkdown": "## Words to learn\n- wake up\n- brush teeth",
  "sectionType": "Vocabulary",
  "mediaId": null,
  "sortOrder": 0
}
```

## Response Example

```json
{
  "id": "77777777-7777-7777-7777-777777777777",
  "lessonId": "66666666-6666-6666-6666-666666666666",
  "title": "Vocabulary Warm-Up",
  "contentMarkdown": "## Words to learn\n- wake up\n- brush teeth",
  "sectionType": "Vocabulary",
  "mediaId": null,
  "media": null,
  "sortOrder": 0,
  "isActive": true,
  "createdAt": "2026-07-12T00:00:00+00:00",
  "updatedAt": "2026-07-12T00:00:00+00:00"
}
```

## Blazor UI

Lesson Section UI is embedded under the Lesson detail page (`/admin/lessons/{id}`):

- List sections for the lesson, ordered by `SortOrder`.
- Add a new section.
- Edit an existing section in place, without navigating away.
- Delete (deactivate) a section.
- Reorder sections with simple Up/Down buttons per row, which call `ReorderLessonSections` with the full updated order.
- Show loading, empty, error, and validation states through the parent page and form.

There are no standalone Lesson Section pages or routes.

## Test Coverage

Current tests cover:

- Lesson section creation and invariant behavior, including normalization of text fields
- Required-`Title` validation
- `Reorder` updating `SortOrder` and `UpdatedAt`
- `AddLessonSection` happy path when the parent lesson is active
- API add/list/update/reorder flow for sections through the full HTTP pipeline

## Known Limitations

- There is no standalone search or pagination endpoint for lesson sections; they are only listed per lesson.
- **There is no `GET /api/v1/lesson-sections/{id}` route.** `GetLessonSectionByIdQueryHandler` is registered in the Application layer and dependency injection, but no endpoint maps to it — unlike Grammar Example, which is reachable via `GET /api/v1/grammar-examples/{id}`.
- Delete deactivates the section instead of hard-deleting it.
- Reordering requires the submitted id list to exactly match the lesson's current, non-empty, duplicate-free section set — a partial or mismatched list is rejected as a validation error rather than partially applied.
- Lesson sections are managed only from the Lesson detail UI.
- Endpoints are not authenticated yet.

## Next Recommended Module

See [Lesson Module](lesson-module.md#next-recommended-module).
