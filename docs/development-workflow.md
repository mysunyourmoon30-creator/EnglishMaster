# Development Workflow

## Local Prerequisites

- Visual Studio 2022 with ASP.NET and web development workload
- .NET 9 SDK
- SQL Server or SQL Server Developer Edition when persistence work begins

## Standard Loop

1. Pull the latest changes.
2. Restore dependencies.
3. Build the solution.
4. Run tests.
5. Make focused changes.
6. Rerun the smallest relevant test command.
7. Run the full build and test commands before opening a pull request.

```powershell
dotnet restore EnglishMaster.sln
dotnet build EnglishMaster.sln --configuration Release --no-restore
dotnet test EnglishMaster.sln --configuration Release --no-build
```

## Branching

Use short branch names that identify the purpose of the change, for example:

```text
feature/initial-foundation
fix/build-pipeline
docs/architecture-notes
```

## Pull Requests

Every pull request should include:

- Summary of the change.
- Verification commands run.
- Any known limitations or follow-up work.

Do not include identity, AI features, marketplace, mobile, plugins, microservices, or new learning modules until those areas are explicitly planned.

## Word Module Workflow

The Word Module is implemented as the first vertical slice. Word-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Words`
- Use cases and validation: `src/Backend/EnglishMaster.Application/Features/Words`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Words` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/WordEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Words`
- Tests: `tests/EnglishMaster.UnitTests/Words` and `tests/EnglishMaster.IntegrationTests/Words`

When changing Word behavior:

1. Keep domain invariants in the `Word` entity.
2. Keep orchestration and validation in Application handlers.
3. Keep EF Core access in Infrastructure only.
4. Keep API endpoints and Blazor pages thin.
5. Add or update focused unit and integration tests.
6. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/word-module.md`
- `docs/api/word-api.md`

## Category And Tag Module Workflow

The Category and Tag Modules organize the Word Module. Category and Tag changes should stay within the existing module boundaries:

- Category domain rules: `src/Backend/EnglishMaster.Domain/Categories`
- Tag domain rules: `src/Backend/EnglishMaster.Domain/Tags`
- Category use cases and validation: `src/Backend/EnglishMaster.Application/Features/Categories`
- Tag use cases and validation: `src/Backend/EnglishMaster.Application/Features/Tags`
- Word relationship validation: `src/Backend/EnglishMaster.Application/Features/Words`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Categories`, `src/Backend/EnglishMaster.Infrastructure/Tags`, and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/CategoryEndpoints.cs` and `src/Backend/EnglishMaster.Api/Endpoints/TagEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Categories` and `src/Frontend/EnglishMaster.Web/Components/Pages/Tags`
- Tests: `tests/EnglishMaster.UnitTests/Categories`, `tests/EnglishMaster.UnitTests/Tags`, `tests/EnglishMaster.IntegrationTests/Categories`, and `tests/EnglishMaster.IntegrationTests/Tags`

When changing Category or Tag behavior:

1. Keep entity invariants in `Category` and `Tag`.
2. Keep Word relationship validation in the Application layer.
3. Keep EF Core access in Infrastructure only.
4. Keep API endpoints and Blazor pages thin.
5. Update Word tests when Category or Tag assignment behavior changes.
6. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/category-module.md`
- `docs/modules/tag-module.md`
- `docs/api/category-api.md`
- `docs/api/tag-api.md`

## Media Module Workflow

The Media Module manages reusable image, audio, video, document, and other media metadata. It also provides local upload support for the initial platform.

Media-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Media`
- Use cases, validation, and storage abstraction: `src/Backend/EnglishMaster.Application/Features/Media`
- Local storage and persistence: `src/Backend/EnglishMaster.Infrastructure/Media` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/MediaEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Media`
- Word media selection: `src/Frontend/EnglishMaster.Web/Components/Words` and Word create/edit/detail pages
- Tests: `tests/EnglishMaster.UnitTests/Media`, `tests/EnglishMaster.IntegrationTests/Media`, and Word relationship tests

When changing Media behavior:

1. Keep entity invariants in `Media`.
2. Keep metadata validation and upload orchestration in Application handlers.
3. Keep storage implementation in Infrastructure behind `IMediaStorageService`.
4. Do not expose internal `StoragePath` through public API or Blazor forms.
5. Keep file handling path-safe and content-type validated.
6. Update Word tests when image or audio relationship behavior changes.
7. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/media-module.md`
- `docs/api/media-api.md`

## Pronunciation Module Workflow

The Pronunciation Module extends Word with IPA, Thai reading aids, articulation guidance, pronunciation audio, mouth-position imagery, and minimal-pair practice.

Pronunciation-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Pronunciations`
- Pronunciation use cases and validation: `src/Backend/EnglishMaster.Application/Features/Pronunciations`
- Minimal Pair use cases and validation: `src/Backend/EnglishMaster.Application/Features/MinimalPairs`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Pronunciations` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/PronunciationEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Pronunciations`
- Blazor forms and API client: `src/Frontend/EnglishMaster.Web/Components/Pronunciations` and `src/Frontend/EnglishMaster.Web/Services/Pronunciations`
- Word detail integration: `src/Backend/EnglishMaster.Application/Features/Words` and `src/Frontend/EnglishMaster.Web/Components/Pages/Words`
- Tests: `tests/EnglishMaster.UnitTests/Pronunciations`, `tests/EnglishMaster.UnitTests/MinimalPairs`, and `tests/EnglishMaster.IntegrationTests/Pronunciations`

When changing Pronunciation or Minimal Pair behavior:

1. Keep entity invariants in `Pronunciation` and `MinimalPair`.
2. Keep Word, Media, and parent relationship validation in Application handlers.
3. Keep EF Core access in Infrastructure only.
4. Keep API endpoints and Blazor pages thin.
5. Preserve the one-word-to-one-pronunciation rule.
6. Validate media type rules for audio and mouth images.
7. Update Word tests when pronunciation summary behavior changes.
8. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/pronunciation-module.md`
- `docs/modules/minimal-pair.md`
- `docs/api/pronunciation-api.md`
- `docs/api/minimal-pair-api.md`

## Grammar Module Workflow

The Grammar Module manages grammar topics, grammar rules, grammar examples, and optional links from grammar rules to related words.

Grammar-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Grammar`
- Grammar Topic, Grammar Rule, and Grammar Example use cases and validation: `src/Backend/EnglishMaster.Application/Features/GrammarTopics`, `src/Backend/EnglishMaster.Application/Features/GrammarRules`, and `src/Backend/EnglishMaster.Application/Features/GrammarExamples`
- Word relationship validation: `src/Backend/EnglishMaster.Application/Features/GrammarRules` and `src/Backend/EnglishMaster.Application/Features/Words`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Grammar` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/GrammarEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/GrammarTopics` and `src/Frontend/EnglishMaster.Web/Components/Pages/GrammarRules`
- Blazor forms and API client: `src/Frontend/EnglishMaster.Web/Components/Grammar` and `src/Frontend/EnglishMaster.Web/Services/Grammar`
- Tests: `tests/EnglishMaster.UnitTests/Grammar` and `tests/EnglishMaster.IntegrationTests/Grammar`

When changing Grammar behavior:

1. Keep entity invariants in `GrammarTopic`, `GrammarRule`, and `GrammarExample`.
2. Keep orchestration and validation in Application handlers.
3. Keep EF Core access in Infrastructure only.
4. Keep slug-uniqueness checks symmetric across Grammar Topic and Grammar Rule; both have a unique `Slug` index and a `SlugExistsAsync` check before create and update.
5. Keep API endpoints and Blazor pages thin.
6. Validate that referenced topics and words are active before linking them to a rule.
7. Update Word tests when related-word behavior changes.
8. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/grammar-module.md`
- `docs/modules/grammar-topic.md`
- `docs/modules/grammar-rule.md`
- `docs/modules/grammar-example.md`
- `docs/api/grammar-topic-api.md`
- `docs/api/grammar-rule-api.md`
- `docs/api/grammar-example-api.md`

## Admin Routing And Navigation Workflow

The admin routing layer centralizes every Blazor `/admin/*` page route in `AdminRoutes.cs` and every backend route in each module's `Endpoints.cs` file. Routing changes should stay within these boundaries:

- Route constants: `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs`
- Navigation: `src/Frontend/EnglishMaster.Web/Components/Layout/MainLayout.razor`
- Dashboard: `src/Frontend/EnglishMaster.Web/Components/Pages/AdminDashboard.razor`
- Admin pages: `src/Frontend/EnglishMaster.Web/Components/Pages/{Module}`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/*.cs` and `src/Backend/EnglishMaster.Api/Program.cs`

When adding a new admin route:

1. Add the `@page "..."` literal to the new `.razor` page.
2. Add the matching constant(s) and helper method(s) to `AdminRoutes.cs`.
3. Add a nav link in `MainLayout.razor` if it's a new top-level module.
4. Reuse the existing List/Create/Detail/Edit page pattern and a shared `*Form.razor` component rather than inventing new structure.
5. Run:

```powershell
dotnet build
dotnet test
```

When adding a new API route:

1. Define request/response records in `EnglishMaster.Contracts`.
2. Add a Command/Query and Handler under `EnglishMaster.Application.Features.{Module}`.
3. Add a thin endpoint method in the module's `Endpoints.cs` that only binds parameters, calls the handler, and maps the result — no business logic in the endpoint.
4. Register a new endpoint file in `Program.cs` if it's a new module.
5. Add unit and integration tests.
6. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/routes/admin-routes.md`
- `docs/api/api-route-overview.md`

## Lesson Module Workflow

The Lesson Module combines Words, Grammar Rules, Category, and Media into structured lessons with ordered sections.

Lesson-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Lessons`
- Lesson and Lesson Section use cases and validation: `src/Backend/EnglishMaster.Application/Features/Lessons` and `src/Backend/EnglishMaster.Application/Features/LessonSections`
- Word, Grammar Rule, Category, and Media relationship validation: `src/Backend/EnglishMaster.Application/Features/Words`, `src/Backend/EnglishMaster.Application/Features/GrammarRules`, `src/Backend/EnglishMaster.Application/Features/Categories`, and `src/Backend/EnglishMaster.Application/Features/Media`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Lessons` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/LessonEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Lessons`
- Blazor forms and API client: `src/Frontend/EnglishMaster.Web/Components/Lessons` and `src/Frontend/EnglishMaster.Web/Services/Lessons`
- Tests: `tests/EnglishMaster.UnitTests/Lessons` and `tests/EnglishMaster.IntegrationTests/Lessons`

When changing Lesson behavior:

1. Keep entity invariants in `Lesson` and `LessonSection`.
2. Keep orchestration and validation in Application handlers.
3. Keep EF Core access in Infrastructure only.
4. Keep the `Lesson.Slug` unique index and `SlugExistsAsync` check, matching the pattern already established for Grammar Topic and Grammar Rule.
5. Validate that referenced Category, Thumbnail Media, Word, and Grammar Rule are active before linking them to a lesson.
6. Keep API endpoints and Blazor pages thin.
7. Update Word and Grammar Rule tests when related-content behavior changes.
8. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/lesson-module.md`
- `docs/modules/lesson-section.md`
- `docs/api/lesson-api.md`
- `docs/api/lesson-section-api.md`

## Course Module Workflow

The Course Module groups Lessons into reusable learning paths with optional CEFR, Category, thumbnail Media, publish state, and ordered CourseLesson relationships.

Course-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Courses`
- Course use cases and validation: `src/Backend/EnglishMaster.Application/Features/Courses`
- Lesson, Category, and Media relationship validation: `src/Backend/EnglishMaster.Application/Features/Lessons`, `src/Backend/EnglishMaster.Application/Features/Categories`, and `src/Backend/EnglishMaster.Application/Features/Media`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Courses` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/CourseEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Courses`
- Blazor form and API client: `src/Frontend/EnglishMaster.Web/Components/Courses` and `src/Frontend/EnglishMaster.Web/Services/Courses`
- Tests: `tests/EnglishMaster.UnitTests/Courses` and `tests/EnglishMaster.IntegrationTests/Courses`

When changing Course behavior:

1. Keep entity invariants in `Course` and `CourseLesson`.
2. Keep orchestration, slug uniqueness, and relationship validation in Application handlers.
3. Keep EF Core access in Infrastructure only.
4. Keep the `Course.Slug` unique index and `SlugExistsAsync` check, matching the pattern established for Lesson and Grammar.
5. Validate that referenced Category, thumbnail Media, and Lesson records are active before linking them to a course.
6. Keep API endpoints and Blazor pages thin.
7. Update Lesson-related tests when CourseLesson relationship behavior changes.
8. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/course-module.md`
- `docs/modules/course-lesson.md`
- `docs/api/course-api.md`

## Book Module Workflow

The Book Module organizes Courses and Lessons into reusable book-style learning products with optional CEFR, Category, cover Media, related Course, publish state, ordered BookChapters, and ordered BookChapterLesson relationships.

Book-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Books`
- Book use cases and validation: `src/Backend/EnglishMaster.Application/Features/Books`
- Book Chapter and Book Chapter Lesson use cases and validation: `src/Backend/EnglishMaster.Application/Features/BookChapters`
- Course, Category, Media, and Lesson relationship validation: `src/Backend/EnglishMaster.Application/Features/Courses`, `src/Backend/EnglishMaster.Application/Features/Categories`, `src/Backend/EnglishMaster.Application/Features/Media`, and `src/Backend/EnglishMaster.Application/Features/Lessons`
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Books` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/BookEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Books`
- Blazor forms and API client: `src/Frontend/EnglishMaster.Web/Components/Books` and `src/Frontend/EnglishMaster.Web/Services/Books`
- Tests: `tests/EnglishMaster.UnitTests/Books` and `tests/EnglishMaster.IntegrationTests/Books`

When changing Book behavior:

1. Keep entity invariants in `Book`, `BookChapter`, and `BookChapterLesson`.
2. Keep orchestration, slug uniqueness, and relationship validation in Application handlers.
3. Keep EF Core access in Infrastructure only.
4. Keep the `Book.Slug` unique index and `SlugExistsAsync` check, matching the pattern established for Course, Lesson, and Grammar.
5. Validate that referenced Category, cover Media, Course, and Lesson records are active before linking them to a book or chapter.
6. Keep API endpoints and Blazor pages thin.
7. Update Course and Lesson relationship tests when Book relationships change.
8. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/book-module.md`
- `docs/modules/book-chapter.md`
- `docs/modules/book-chapter-lesson.md`
- `docs/api/book-api.md`
- `docs/api/book-chapter-api.md`

## Quiz / Exercise Module Workflow

The Quiz / Exercise Module adds admin-authored quizzes with ordered questions and ordered choices. Quiz-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Quizzes`
- Quiz use cases and validation: `src/Backend/EnglishMaster.Application/Features/Quizzes`
- Quiz Question use cases and validation: `src/Backend/EnglishMaster.Application/Features/QuizQuestions`
- Quiz Choice use cases and validation: `src/Backend/EnglishMaster.Application/Features/QuizChoices`
- Category, Lesson, Course, Book, Word, Grammar Rule, and Pronunciation relationship validation: the corresponding Application feature folders
- Persistence: `src/Backend/EnglishMaster.Infrastructure/Quizzes` and `Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/QuizEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Quizzes`
- Blazor forms and API client: `src/Frontend/EnglishMaster.Web/Components/Quizzes` and `src/Frontend/EnglishMaster.Web/Services/Quizzes`
- Tests: `tests/EnglishMaster.UnitTests/Quizzes` and `tests/EnglishMaster.IntegrationTests/Quizzes`

When changing Quiz behavior:

1. Keep entity invariants in `Quiz`, `QuizQuestion`, and `QuizChoice`.
2. Keep orchestration, slug uniqueness, relationship validation, and correct-choice validation in Application handlers and domain methods.
3. Keep EF Core access in Infrastructure only.
4. Keep the `Quiz.Slug` unique index and `SlugExistsAsync` check, matching the pattern established for Book, Course, Lesson, and Grammar.
5. Validate that referenced Category, Lesson, Course, Book, Word, Grammar Rule, and Pronunciation records are active before linking them.
6. Keep API endpoints and Blazor pages thin.
7. Do not reuse admin quiz DTOs for public learner-facing quiz-taking flows because `QuizChoiceDto` exposes `IsCorrect`.
8. Update tests when question ordering, choice ordering, or correct-answer rules change.
9. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/quiz-module.md`
- `docs/modules/quiz-question.md`
- `docs/modules/quiz-choice.md`
- `docs/api/quiz-api.md`
- `docs/api/quiz-question-api.md`
- `docs/api/quiz-choice-api.md`

## Publishing Module Workflow

The Publishing Module exports existing EnglishMaster content into generated artifacts. It covers publish jobs, reusable publish templates, artifact records, local file storage, and admin workflows for running and reviewing exports.

Publishing-related changes should stay within the existing module boundaries:

- Domain rules: `src/Backend/EnglishMaster.Domain/Publishing`
- Publish job use cases and validation: `src/Backend/EnglishMaster.Application/Features/PublishJobs`
- Publish template use cases and validation: `src/Backend/EnglishMaster.Application/Features/PublishTemplates`
- Published artifact queries: `src/Backend/EnglishMaster.Application/Features/PublishedArtifacts`
- Publishing abstractions: `src/Backend/EnglishMaster.Application/Publishing`
- Persistence, content building, and storage implementations: `src/Backend/EnglishMaster.Infrastructure/Publishing` and `src/Backend/EnglishMaster.Infrastructure/Persistence/Configurations`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/PublishingEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Publishing`
- Blazor API client: `src/Frontend/EnglishMaster.Web/Services/Publishing`
- Tests: `tests/EnglishMaster.UnitTests/Publishing` and `tests/EnglishMaster.IntegrationTests/Publishing`

When changing Publishing behavior:

1. Keep status transition rules in `PublishJob`.
2. Keep publish-template invariants in `PublishTemplate`.
3. Keep generated artifact invariants in `PublishedArtifact`.
4. Keep orchestration in Application through `IPublishingService`.
5. Keep content generation behind `IPublishContentBuilder`.
6. Keep file storage behind `IPublishFileStorage`, with local file implementation in Infrastructure only.
7. Do not expose absolute server paths through API responses or Blazor pages.
8. Keep file names path-safe and verify stored paths remain under the configured publishing root.
9. Treat PDF and DOCX generation as placeholders until a renderer is selected deliberately.
10. Keep API endpoints and Blazor pages thin.
11. Update tests when lifecycle transitions, storage behavior, supported formats, or artifact metadata changes.
12. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/publishing-module.md`
- `docs/modules/publish-job.md`
- `docs/modules/publish-template.md`
- `docs/modules/published-artifact.md`
- `docs/api/publishing-api.md`

## Authentication, Roles, Permissions, And Admin Security Workflow

The security module protects the admin surface and backend APIs with cookie authentication and permission-based authorization.

Security-related changes should stay within the existing module boundaries:

- Domain entities: `src/Backend/EnglishMaster.Domain/Security`
- Security use cases and permission constants: `src/Backend/EnglishMaster.Application/Features/Security`
- EF Core, password hashing, and seeding: `src/Backend/EnglishMaster.Infrastructure/Security` and `Persistence/Configurations/SecurityConfiguration.cs`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/SecurityEndpoints.cs`
- API authentication and policy registration: `src/Backend/EnglishMaster.Api/Program.cs`
- Blazor login, admin redirect, and API cookie forwarding: `src/Frontend/EnglishMaster.Web/Program.cs` and `src/Frontend/EnglishMaster.Web/Services/Security`
- Admin security pages: `src/Frontend/EnglishMaster.Web/Components/Pages/Security`
- Admin route constants: `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs`
- Tests: `tests/EnglishMaster.IntegrationTests/Security` and any module tests affected by route-level authorization

When changing authentication:

1. Keep password hashing in Infrastructure through framework-supported password hashing.
2. Never expose password hashes, normalized identity fields, or internal EF entities through contracts.
3. Keep login/logout/current-user endpoints thin and routed through Application handlers.
4. Confirm unauthenticated requests return `401` and forbidden requests return `403`.
5. Keep cookie settings production-ready and avoid hardcoded credentials.

When changing authorization:

1. Add permission constants in one place only: `Permissions`.
2. Register every new permission in `Permissions.All`.
3. Add the permission to default role seeding deliberately.
4. Protect new API endpoints with `.RequireAuthorization(Permissions.SomePermission)`.
5. Keep Blazor page protection and navigation as user experience helpers only; API policies remain the security boundary.
6. Add or update tests for high-risk permission paths.

Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/security/authentication.md`
- `docs/security/authorization.md`
- `docs/security/roles-and-permissions.md`
- `docs/api/auth-api.md`
- `docs/api/user-api.md`
- `docs/api/role-api.md`
- `docs/routes/admin-routes.md`

## Development Seed Data Workflow

Development seed data provides a small local dataset for MVP smoke testing. Seed-related changes should stay within Infrastructure and configuration:

- Security and seed orchestration: `src/Backend/EnglishMaster.Infrastructure/Security/SecuritySeeder.cs`
- Development content seed: `src/Backend/EnglishMaster.Infrastructure/Security/DevelopmentSeedDataSeeder.cs`
- Development toggle: `src/Backend/EnglishMaster.Api/appsettings.Development.json`

When changing seed data:

1. Keep credentials out of committed source.
2. Read the initial SuperAdmin email and password only from configuration.
3. Keep demo content small and idempotent.
4. Use domain factories and behavior methods instead of setting entity state directly.
5. Keep development content behind `DevelopmentSeed:Enabled`.
6. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/development-seed-data.md`

## Import / Export Workflow

The Import / Export Module supports small content-operation tasks for existing MVP content. Import/export changes should stay within the existing boundaries:

- Application abstractions: `src/Backend/EnglishMaster.Application/Features/ImportExport`
- Infrastructure implementation: `src/Backend/EnglishMaster.Infrastructure/ImportExport`
- API endpoints: `src/Backend/EnglishMaster.Api/Endpoints/ContentImportExportEndpoints.cs`
- Blazor pages: `src/Frontend/EnglishMaster.Web/Components/Pages/ImportExport`
- Web API client: `src/Frontend/EnglishMaster.Web/Services/ImportExport`

When changing import/export behavior:

1. Keep parsing and EF Core access out of API endpoints and Blazor pages.
2. Validate file size, content type, required fields, and referenced slugs before import.
3. Report row-level import errors instead of silently skipping invalid content.
4. Protect routes with existing module permissions.
5. Keep exports small, explicit, and suitable for content review.
6. Run:

```powershell
dotnet build
dotnet test
```

See:

- `docs/modules/import-export.md`
- `docs/api/import-export-api.md`

## MVP Performance Workflow

MVP performance work should improve predictable hot paths without changing the modular monolith shape:

1. Keep read-only EF Core queries on `AsNoTracking` unless entity tracking is required.
2. Keep list/search endpoints paginated with a default page size and a maximum page size.
3. Avoid loading child collections on list pages when detail pages can fetch them separately.
4. Keep media list pages on metadata and public URLs only; do not load file bytes for lists.
5. Keep import files size-limited and validate before parsing.
6. Treat export and publishing as synchronous MVP workflows until a worker queue is introduced deliberately.
7. Run:

```powershell
dotnet build
dotnet test
```
