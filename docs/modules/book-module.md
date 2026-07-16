# Book Module

## Purpose

The Book Module organizes existing Courses and Lessons into reusable book-style learning products. A `Book` is a publishable content container with optional CEFR level, Category, Cover Media, and Course relationships. It owns ordered chapters, and chapters can contain ordered lessons through `BookChapterLesson`.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Books`
- Application: `EnglishMaster.Application.Features.Books` and `EnglishMaster.Application.Features.BookChapters`
- Infrastructure: `EnglishMaster.Infrastructure.Books`
- API: `EnglishMaster.Api.Endpoints.BookEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Books`, shared form components under `EnglishMaster.Web.Components.Books`

## Book Fields

| Field | Description |
| --- | --- |
| `Id` | Unique book identifier. |
| `Title` | Required title. Trimmed and used to generate `Slug`. |
| `Slug` | URL-friendly value generated from `Title`; unique in the database. |
| `Subtitle` | Optional subtitle. Stored as an empty string when omitted. |
| `Summary` | Optional short summary. Stored as an empty string when omitted. |
| `Description` | Optional longer description. Stored as an empty string when omitted. |
| `CefrLevel` | Optional CEFR value: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `CategoryId` | Optional category identifier. |
| `Category` | Optional category summary in response DTOs. |
| `CoverMediaId` | Optional image media identifier for the book cover. |
| `CoverMedia` | Optional media summary in response DTOs. |
| `CourseId` | Optional related course identifier. |
| `Course` | Optional course summary in response DTOs. |
| `AuthorName` | Optional author name. Stored as an empty string when omitted. |
| `Edition` | Optional edition label. Stored as an empty string when omitted. |
| `Version` | Optional version label. Stored as an empty string when omitted. |
| `EstimatedPages` | Estimated page count. Must be zero or greater. |
| `SortOrder` | Admin-defined display order. Must be zero or greater. |
| `Chapters` | Ordered `BookChapter` records that belong to the book. |
| `IsPublished` | Indicates whether the book is published. New books start as draft. |
| `IsActive` | Indicates whether the book is active. Deletes deactivate the book. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Relationships

### Book And Course

`Book.CourseId` is optional. When provided, the Application layer validates that the course exists and is active before creating or updating the book. EF Core uses `SetNull` delete behavior, so deleting a course clears the relationship rather than deleting the book.

### Book And Category

`Book.CategoryId` is optional. When provided, the Application layer validates that the category exists and is active. EF Core uses `SetNull` delete behavior, so deleting a category clears the book relationship rather than deleting the book.

### Book And Media

`Book.CoverMediaId` is optional. When provided, the Application layer validates that the media exists, is active, and has `MediaType.Image`. Response DTOs expose media summary fields (`FileName`, `ContentType`, `MediaType`, `PublicUrl`, `AltText`) and do not expose internal storage paths.

### Book And Chapter

A book has many ordered chapters.

- `BookChapter.BookId` points to one `Book`.
- EF Core cascades from `Book` to `BookChapter` rows.
- Chapter order is controlled by `BookChapter.SortOrder`.
- Chapters are managed through the Book detail page and nested API routes.

See [Book Chapter](book-chapter.md).

### BookChapter And Lesson

A chapter can contain many lessons through `BookChapterLesson`.

- `BookChapterLesson.BookChapterId` points to one chapter.
- `BookChapterLesson.LessonId` points to one lesson.
- One lesson can appear in many book chapters.
- Duplicate `LessonId` values inside the same chapter are prevented by domain behavior and a unique database index on `(BookChapterId, LessonId)`.

See [Book Chapter Lesson](book-chapter-lesson.md).

## Publish And Unpublish Rules

`IsPublished` and `IsActive` are separate lifecycle flags.

- `Publish` sets `IsPublished` to `true` and updates `UpdatedAt`.
- `Unpublish` sets `IsPublished` to `false` and updates `UpdatedAt`.
- `Activate` and `Deactivate` manage `IsActive` separately.
- New books are created as `IsPublished: false` and `IsActive: true`.
- `DELETE /api/v1/books/{id}` deactivates the book rather than hard-deleting it.

## Domain Rules

- `Title` is required, trimmed, and length-limited to 200 characters.
- `Slug` is generated from `Title`, must contain at least one letter or digit, and is length-limited to 220 characters.
- `Slug` is unique in persistence and checked by Application handlers before create/update.
- `Subtitle`, `Summary`, `Description`, `AuthorName`, `Edition`, and `Version` are optional, trimmed, and length-limited.
- `CefrLevel` is optional but must be a valid enum value when provided.
- `CategoryId`, `CoverMediaId`, and `CourseId` are optional but cannot be empty GUIDs when provided.
- `EstimatedPages` and `SortOrder` must be zero or greater.
- Book chapter and lesson relation ids must be non-empty GUIDs.
- Duplicate lessons inside a chapter are not added twice.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

Book use cases:

- `CreateBook`
- `UpdateBook`
- `DeleteBook`
- `GetBookById`
- `SearchBooks`
- `PublishBook`
- `UnpublishBook`
- `ActivateBook`
- `DeactivateBook`

Book chapter use cases:

- `AddBookChapter`
- `UpdateBookChapter`
- `DeleteBookChapter`
- `GetBookChapterById`
- `GetBookChaptersByBookId`
- `ReorderBookChapters`

Book chapter lesson use cases:

- `AddLessonToBookChapter`
- `RemoveLessonFromBookChapter`
- `GetBookChapterLessonsByBookChapterId`
- `ReorderBookChapterLessons`

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/books` | Search, filter, sort, and paginate books. |
| `GET` | `/api/v1/books/{id}` | Get one book by id. |
| `POST` | `/api/v1/books` | Create a book. |
| `PUT` | `/api/v1/books/{id}` | Update a book. |
| `DELETE` | `/api/v1/books/{id}` | Deactivate a book. |
| `POST` | `/api/v1/books/{id}/publish` | Publish a book. |
| `POST` | `/api/v1/books/{id}/unpublish` | Unpublish a book. |
| `POST` | `/api/v1/books/{id}/activate` | Activate a book. |
| `POST` | `/api/v1/books/{id}/deactivate` | Deactivate a book. |
| `GET` | `/api/v1/books/{bookId}/chapters` | List chapters in a book. |
| `POST` | `/api/v1/books/{bookId}/chapters` | Add a chapter to a book. |
| `POST` | `/api/v1/books/{bookId}/chapters/reorder` | Reorder book chapters. |
| `GET` | `/api/v1/book-chapters/{id}` | Get one book chapter by id. |
| `PUT` | `/api/v1/book-chapters/{id}` | Update a book chapter. |
| `DELETE` | `/api/v1/book-chapters/{id}` | Deactivate a book chapter. |
| `GET` | `/api/v1/book-chapters/{chapterId}/lessons` | List lessons in a chapter. |
| `POST` | `/api/v1/book-chapters/{chapterId}/lessons/{lessonId}` | Add a lesson to a chapter. |
| `DELETE` | `/api/v1/book-chapters/{chapterId}/lessons/{lessonId}` | Remove a lesson from a chapter. |
| `POST` | `/api/v1/book-chapters/{chapterId}/lessons/reorder` | Reorder lessons in a chapter. |

Full request/response examples are documented in [Book API](../api/book-api.md) and [Book Chapter API](../api/book-chapter-api.md).

## Search And Filter Parameters

`GET /api/v1/books` supports:

- `search` - searches `Title`, `Slug`, and `Summary`.
- `cefrLevel` - optional CEFR filter.
- `categoryId` - optional category filter.
- `courseId` - optional course filter.
- `isPublished` - optional published-state filter.
- `isActive` - optional active-state filter; defaults to `true`.
- `pageNumber` - defaults to `1`.
- `pageSize` - defaults to `20`, maximum `100`.
- `sortBy` - `Title` or `CreatedAt`, defaults to `Title`.
- `sortDirection` - `Asc` or `Desc`, defaults to `Asc`.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Book list | `/admin/books` | Search, filter, paginate, view, edit, and delete books. |
| Create book | `/admin/books/create` | Create a draft book. |
| Book detail | `/admin/books/{id:guid}` | View book details, cover image, publish state, chapters, and chapter lessons. |
| Edit book | `/admin/books/{id:guid}/edit` | Update book metadata and status. |

The list page includes search, CEFR filter, category filter, course filter, published filter, active filter, page size selection, sort selection, pagination controls, loading, empty, and error states. The detail page supports publish/unpublish, adding/editing/deleting chapters, adding/removing lessons from chapters, and simple up/down reordering for chapters and chapter lessons.

## Admin Routes And Dashboard

Book routes are centralized under `AdminRoutes.Books` in `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs`. The admin navigation includes a `Books` link. The dashboard includes a `Total Books` card that calls the book search endpoint with `PageSize: 1` and `IsActive: null`, then reads `TotalCount`.

See [Admin Routes](../routes/admin-routes.md).

## Test Coverage

- `tests/EnglishMaster.UnitTests/Books/BookTests.cs` covers entity creation, normalization, slug generation, required title, nonnegative estimated pages, publish/unpublish, activate/deactivate, chapter creation, duplicate lesson prevention, and chapter lesson reorder behavior.
- `tests/EnglishMaster.UnitTests/Books/BookUseCaseTests.cs` covers create, duplicate slug validation, update, publish/unpublish, activate/deactivate, add chapter, add lesson, duplicate lesson behavior, reorder chapters, reorder chapter lessons, search by CEFR, and search by published status.
- `tests/EnglishMaster.IntegrationTests/Books/BookEndpointsTests.cs` covers the HTTP flow for create, search, add/update/reorder/delete chapter, add/list/reorder/remove chapter lessons, publish/unpublish, update, soft delete, active/inactive search, missing-title validation, and duplicate-title validation.
- `tests/EnglishMaster.ArchitectureTests` covers project reference rules and layer boundaries for the module.

## Known Limitations

- Book admin endpoints and pages are not protected by authentication or authorization yet.
- Search uses simple database `Contains` matching rather than full-text search.
- `DELETE` deactivates a book or chapter instead of hard-deleting it.
- Book chapter delete deactivates the chapter; it does not remove the row.
- Book chapter lesson rows are removed from the chapter rather than soft-deactivated.
- Reordering uses simple up/down UI controls and requires a complete ordered id list from the API caller.
- Cover media selection supports existing active image media only; uploading new media still happens through the Media Module.
- There is no PDF, DOCX, EPUB, or print export in this module.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
