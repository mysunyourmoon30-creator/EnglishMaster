# Category Module

## Purpose

The Category Module organizes vocabulary words into one optional primary grouping. It gives administrators a simple way to group words by learning area, theme, or curriculum section without creating a separate learning module yet.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Categories`
- Application: `EnglishMaster.Application.Features.Categories`
- Infrastructure: `EnglishMaster.Infrastructure.Categories`
- API: `EnglishMaster.Api.Endpoints.CategoryEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Categories`

## Category Fields

| Field | Description |
| --- | --- |
| `Id` | Unique category identifier. |
| `Name` | Required category name. |
| `Slug` | URL-safe value generated from `Name`. |
| `Description` | Optional description stored as an empty string when not provided. |
| `SortOrder` | Numeric display order for category lists. |
| `IsActive` | Indicates whether the category can be assigned to words. Deletes deactivate a category. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Word Relationship

A word can have zero or one category through `Word.CategoryId`.

Category assignment is handled through the Word use cases:

- `CreateWord` accepts `CategoryId`.
- `UpdateWord` accepts `CategoryId`.
- `GetWordById` returns category summary data when assigned.
- `SearchWords` can filter by `CategoryId`.

The Application layer validates that assigned categories exist and are active. Blazor word create/edit pages load active categories and allow one category to be selected.

## Domain Rules

- `Name` is required and trimmed.
- `Slug` is generated from `Name` using the shared domain slug generator.
- `Slug` must contain at least one letter or digit.
- `Description` is trimmed and stored as an empty string when not provided.
- `Activate` and `Deactivate` behavior exists on the `Category` entity.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateCategory`
- `UpdateCategory`
- `DeleteCategory`
- `GetCategoryById`
- `SearchCategories`

The Application layer validates input, checks slug uniqueness, uses cancellation tokens, and returns contract DTOs rather than EF entities.

## Search And Pagination

`SearchCategories` supports:

- Search by `Name`, `Slug`, and `Description`
- Filter by `IsActive`
- Sorting by `SortOrder`, then `Name`

Default behavior returns active categories. Pagination is not implemented for categories yet because the current list is expected to stay small.

## Persistence

EF Core maps `Category` to the `Categories` table. The SQL Server configuration includes:

- Required `Name`, `Slug`, `Description`, `SortOrder`, `IsActive`, `CreatedAt`, and `UpdatedAt`
- Unique index on `Slug`
- Indexes on `Name` and `IsActive`
- Optional `Words.CategoryId` foreign key with `SetNull` delete behavior

The migration `AddCategoryTagModule` adds the `Categories` table and the nullable `CategoryId` column on `Words`.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Category list | `/categories` | Search, filter, view, edit, and deactivate categories. |
| Create category | `/categories/new` | Create a category with validation. |
| Category detail | `/categories/{id}` | View category details. |
| Edit category | `/categories/{id}/edit` | Update category data and active status. |

The pages include loading, empty, error, and validation states.

## Test Coverage

Current tests cover:

- Category entity creation
- Category slug generation
- API create/get/search/update/delete flow
- Active and inactive category search behavior
- Architecture project reference rules
- Word assignment and search by `CategoryId`

## Known Limitations

- Category admin endpoints and pages are not protected by authentication or authorization yet.
- Delete currently deactivates a category instead of hard-deleting it.
- Category search does not paginate yet.
- No audit user is recorded for create/update/delete actions.
- Existing words keep `CategoryId` as `null` until explicitly assigned.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
