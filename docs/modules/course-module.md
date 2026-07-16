# Course Module

## Purpose

The Course Module groups multiple Lessons into a structured learning path. A `Course` is a top-level learning container that can be filtered by CEFR level, optionally grouped under a Category, optionally displayed with a thumbnail Media image, and ordered for admin or learner-facing presentation.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Courses`
- Application: `EnglishMaster.Application.Features.Courses`
- Infrastructure: `EnglishMaster.Infrastructure.Courses`
- API: `EnglishMaster.Api.Endpoints.CourseEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Courses`, shared form components under `EnglishMaster.Web.Components.Courses`

## Course Fields

| Field | Description |
| --- | --- |
| `Id` | Unique course identifier. |
| `Title` | Required course title. Trimmed and used to generate `Slug`. |
| `Slug` | URL-friendly value generated from `Title`; unique in the database. |
| `Summary` | Optional short summary. Stored as an empty string when omitted. |
| `Description` | Optional longer description. Stored as an empty string when omitted. |
| `CefrLevel` | Optional CEFR value: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `CategoryId` | Optional category identifier. |
| `Category` | Optional category summary in response DTOs. |
| `ThumbnailMediaId` | Optional image media identifier for the course thumbnail. |
| `ThumbnailMedia` | Optional media summary in response DTOs. |
| `EstimatedMinutes` | Estimated completion time. Must be zero or greater. |
| `SortOrder` | Admin-defined display order. Must be zero or greater. |
| `Lessons` | Ordered CourseLesson entries that connect the course to lessons. |
| `IsPublished` | Indicates whether the course is published. New courses start as draft. |
| `IsActive` | Indicates whether the course is active. Deletes deactivate the course. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Relationships

### Course And Lesson

A course can contain many lessons through the `CourseLesson` join entity.

- `CourseLesson.CourseId` points to one `Course`.
- `CourseLesson.LessonId` points to one `Lesson`.
- One lesson can belong to many courses.
- `CourseLesson.SortOrder` controls lesson order inside a course.
- `CourseLesson.IsRequired` marks whether the lesson is required or optional.
- Duplicate `LessonId` values inside the same course are prevented by the domain behavior and a unique database index on `(CourseId, LessonId)`.
- The Lesson foreign key uses restrict delete behavior, so a lesson cannot be deleted while course relationships still reference it.

See [Course Lesson](course-lesson.md) for the join entity details.

### Course And Category

`Course.CategoryId` is optional. When provided, the Application layer validates that the category exists and is active before creating or updating the course. EF Core uses `SetNull` delete behavior, so deleting a category clears the course relationship instead of deleting the course.

### Course And Media

`Course.ThumbnailMediaId` is optional. When provided, the Application layer validates that the media exists, is active, and has `MediaType.Image`. Response DTOs expose media summary fields (`FileName`, `ContentType`, `MediaType`, `PublicUrl`, `AltText`) and do not expose internal storage paths.

## Publish And Unpublish Rules

`IsPublished` and `IsActive` are separate lifecycle flags.

- `Publish` sets `IsPublished` to `true` and updates `UpdatedAt`.
- `Unpublish` sets `IsPublished` to `false` and updates `UpdatedAt`.
- `Activate` and `Deactivate` manage `IsActive` separately.
- New courses are created as `IsPublished: false` and `IsActive: true`.
- `DELETE /api/v1/courses/{id}` deactivates the course rather than hard-deleting it.

## Domain Rules

- `Title` is required, trimmed, and length-limited.
- `Slug` is generated from `Title` and must contain at least one letter or digit.
- `Slug` is unique in persistence and checked by Application handlers before create/update.
- `Summary` and `Description` are optional, trimmed, and length-limited.
- `CefrLevel` is optional but must be a valid enum value when provided.
- `CategoryId` and `ThumbnailMediaId` are optional but cannot be empty GUIDs when provided.
- `EstimatedMinutes` and `SortOrder` must be zero or greater.
- Course lesson relation ids must be non-empty GUIDs.
- Duplicate lessons inside a course are not added twice; adding the same lesson again updates the existing relation's `IsRequired` value.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateCourse`
- `UpdateCourse`
- `DeleteCourse`
- `GetCourseById`
- `SearchCourses`
- `PublishCourse`
- `UnpublishCourse`
- `ActivateCourse`
- `DeactivateCourse`
- `AddLessonToCourse`
- `RemoveLessonFromCourse`
- `ReorderCourseLessons`

The API currently routes the CRUD, publish/unpublish, lesson add/remove/list/reorder, get, and search use cases. `ActivateCourse` and `DeactivateCourse` are available in Application but not exposed as dedicated API routes.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/courses` | Search, filter, sort, and paginate courses. |
| `GET` | `/api/v1/courses/{id}` | Get one course by id. |
| `POST` | `/api/v1/courses` | Create a course. |
| `PUT` | `/api/v1/courses/{id}` | Update a course. |
| `DELETE` | `/api/v1/courses/{id}` | Deactivate a course. |
| `POST` | `/api/v1/courses/{id}/publish` | Publish a course. |
| `POST` | `/api/v1/courses/{id}/unpublish` | Unpublish a course. |
| `GET` | `/api/v1/courses/{courseId}/lessons` | List lessons in a course. |
| `POST` | `/api/v1/courses/{courseId}/lessons/{lessonId}` | Add a lesson to a course. |
| `DELETE` | `/api/v1/courses/{courseId}/lessons/{lessonId}` | Remove a lesson from a course. |
| `POST` | `/api/v1/courses/{courseId}/lessons/reorder` | Reorder course lessons. |

Full request/response examples are documented in [Course API](../api/course-api.md).

## Search And Filter Parameters

`GET /api/v1/courses` supports:

- `search` - searches `Title`, `Slug`, and `Summary`.
- `cefrLevel` - optional CEFR filter.
- `categoryId` - optional category filter.
- `isPublished` - optional published-state filter.
- `isActive` - optional active-state filter; defaults to `true`.
- `pageNumber` - defaults to `1`.
- `pageSize` - defaults to `20`, maximum `100`.
- `sortBy` - `Title` or `CreatedAt`, defaults to `Title`.
- `sortDirection` - `Asc` or `Desc`, defaults to `Asc`.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Course list | `/admin/courses` | Search, filter, paginate, view, edit, and delete courses. |
| Create course | `/admin/courses/create` | Create a draft course. |
| Course detail | `/admin/courses/{id:guid}` | View course details, thumbnail, publish state, and related lessons. |
| Edit course | `/admin/courses/{id:guid}/edit` | Update course metadata and status. |

The list page includes search, CEFR filter, category filter, published filter, active filter, page size selection, pagination controls, loading, empty, and error states. The detail page supports adding lessons, removing lessons, simple up/down reordering, and publish/unpublish actions.

## Admin Routes And Dashboard

Course routes are centralized under `AdminRoutes.Courses` in `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs`. The admin navigation includes a `Courses` link. The dashboard includes a `Total Courses` card that calls the course search endpoint with `PageSize: 1` and `IsActive: null`, then reads `TotalCount`.

See [Admin Routes](../routes/admin-routes.md).

## Test Coverage

- `tests/EnglishMaster.UnitTests/Courses/CourseTests.cs` covers entity creation, normalization, slug generation, required title, nonnegative estimated minutes, publish/unpublish, activate/deactivate, duplicate lesson handling, and course lesson reorder behavior.
- `tests/EnglishMaster.UnitTests/Courses/CourseUseCaseTests.cs` covers create, duplicate slug validation, publish/unpublish, activate/deactivate, add lesson, duplicate lesson behavior, reorder, search by CEFR, and search by published status.
- `tests/EnglishMaster.IntegrationTests/Courses/CourseEndpointsTests.cs` covers the HTTP flow for create, search, add lesson, duplicate lesson handling, list course lessons, reorder, publish/unpublish, update, remove lesson, soft delete, active/inactive search, missing-title validation, and duplicate-title validation.
- `tests/EnglishMaster.ArchitectureTests` covers project reference rules and layer boundaries for the module.

## Known Limitations

- Course admin endpoints and pages are not protected by authentication or authorization yet.
- Search uses simple database `Contains` matching rather than full-text search.
- `DELETE` deactivates a course instead of hard-deleting it.
- Dedicated `ActivateCourse` and `DeactivateCourse` API routes do not exist yet, although handlers exist and update can set `IsActive`.
- Course lesson reordering uses simple up/down UI controls and requires a complete ordered list from the API caller.
- Course thumbnail selection supports existing active image media only; uploading new media still happens through the Media Module.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
