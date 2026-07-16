# Tag Module

## Purpose

The Tag Module adds flexible labeling for vocabulary words. Tags are useful for cross-cutting concepts such as topic, usage context, difficulty note, or classroom grouping where a single category is too restrictive.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Tags`
- Application: `EnglishMaster.Application.Features.Tags`
- Infrastructure: `EnglishMaster.Infrastructure.Tags`
- API: `EnglishMaster.Api.Endpoints.TagEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Tags`

## Tag Fields

| Field | Description |
| --- | --- |
| `Id` | Unique tag identifier. |
| `Name` | Required tag name. |
| `Slug` | URL-safe value generated from `Name`. |
| `Description` | Optional description stored as an empty string when not provided. |
| `IsActive` | Indicates whether the tag can be assigned to words. Deletes deactivate a tag. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Word Relationship

A word can have many tags through the `WordTags` join table.

Tag assignment is handled through the Word use cases:

- `CreateWord` accepts `TagIds`.
- `UpdateWord` accepts `TagIds`.
- `GetWordById` returns tag summary data.
- `SearchWords` can filter by `TagId`.

The Application layer validates that assigned tags exist and are active. Blazor word create/edit pages load active tags and allow multiple tags to be selected.

## Domain Rules

- `Name` is required and trimmed.
- `Slug` is generated from `Name` using the shared domain slug generator.
- `Slug` must contain at least one letter or digit.
- `Description` is trimmed and stored as an empty string when not provided.
- `Activate` and `Deactivate` behavior exists on the `Tag` entity.
- `Word.SetTags` removes duplicate tag ids and rejects empty ids.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateTag`
- `UpdateTag`
- `DeleteTag`
- `GetTagById`
- `SearchTags`

The Application layer validates input, checks slug uniqueness, uses cancellation tokens, and returns contract DTOs rather than EF entities.

## Search And Pagination

`SearchTags` supports:

- Search by `Name`, `Slug`, and `Description`
- Filter by `IsActive`
- Sorting by `Name`

Default behavior returns active tags. Pagination is not implemented for tags yet because the current list is expected to stay small.

## Persistence

EF Core maps `Tag` to the `Tags` table and `WordTag` to the `WordTags` table. The SQL Server configuration includes:

- Required `Name`, `Slug`, `Description`, `IsActive`, `CreatedAt`, and `UpdatedAt`
- Unique index on `Slug`
- Indexes on `Name` and `IsActive`
- Composite key on `WordTags` using `WordId` and `TagId`
- Index on `WordTags.TagId`
- Cascade delete from `Words` to `WordTags`
- Restrict delete from `Tags` to `WordTags`

The migration `AddCategoryTagModule` adds the `Tags` and `WordTags` tables.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Tag list | `/tags` | Search, filter, view, edit, and deactivate tags. |
| Create tag | `/tags/new` | Create a tag with validation. |
| Tag detail | `/tags/{id}` | View tag details. |
| Edit tag | `/tags/{id}/edit` | Update tag data and active status. |

The pages include loading, empty, error, and validation states.

## Test Coverage

Current tests cover:

- Tag entity creation
- Tag slug generation
- API create/get/search/update/delete flow
- Active and inactive tag search behavior
- Architecture project reference rules
- Word assignment and search by `TagId`

## Known Limitations

- Tag admin endpoints and pages are not protected by authentication or authorization yet.
- Delete currently deactivates a tag instead of hard-deleting it.
- Tag search does not paginate yet.
- No audit user is recorded for create/update/delete actions.
- Existing words have no tags until explicitly assigned.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
