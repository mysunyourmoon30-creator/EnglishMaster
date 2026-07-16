# API Route Overview

This is a single cross-cutting index of every backend API route and the conventions that govern them. It complements — and does not duplicate — the detailed per-module docs listed below, which hold full request/response examples and status codes.

## API Route Structure

Every route is prefixed `/api/v1/{resource}`. There is no versioning scheme beyond the `v1` prefix yet — a `v2` would require a new prefix and, most likely, new endpoint files rather than in-place breaking changes to `v1`.

All endpoint files live under `src/Backend/EnglishMaster.Api/Endpoints/` and are registered in `src/Backend/EnglishMaster.Api/Program.cs` via one `Map*Endpoints()` extension method call per file. Every handler method is a thin pass-through: bind request/route/query parameters, call the matching `EnglishMaster.Application.Features.*` command/query handler, map the returned `Result`/`Result<T>` to an `IResult` via a shared `ToHttpResult` helper. No endpoint file contains business logic, validation rules, or direct EF Core/`DbContext` access — that all lives in the Application layer.

## Existing Module Routes

| Module | Base path | File | Detailed docs |
| --- | --- | --- | --- |
| Word | `/api/v1/words` | `WordEndpoints.cs` | [word-api.md](word-api.md) |
| Category | `/api/v1/categories` | `CategoryEndpoints.cs` | [category-api.md](category-api.md) |
| Certificate | `/api/v1/admin/certificate-templates`, `/api/v1/me/certificates`, `/api/v1/public/certificates` | `CertificateEndpoints.cs` | [certificate-api.md](certificate-api.md) |
| Tag | `/api/v1/tags` | `TagEndpoints.cs` | [tag-api.md](tag-api.md) |
| Media | `/api/v1/media` | `MediaEndpoints.cs` | [media-api.md](media-api.md) |
| Pronunciation | `/api/v1/pronunciations` | `PronunciationEndpoints.cs` | [pronunciation-api.md](pronunciation-api.md) |
| Minimal Pair | `/api/v1/pronunciations/{id}/minimal-pairs`, `/api/v1/minimal-pairs` | `PronunciationEndpoints.cs` | [minimal-pair-api.md](minimal-pair-api.md) |
| Grammar Topic | `/api/v1/grammar-topics` | `GrammarEndpoints.cs` | [grammar-topic-api.md](grammar-topic-api.md) |
| Grammar Rule | `/api/v1/grammar-rules` | `GrammarEndpoints.cs` | [grammar-rule-api.md](grammar-rule-api.md) |
| Grammar Example | `/api/v1/grammar-rules/{id}/examples`, `/api/v1/grammar-examples` | `GrammarEndpoints.cs` | [grammar-example-api.md](grammar-example-api.md) |
| Lesson | `/api/v1/lessons` | `LessonEndpoints.cs` | [lesson-api.md](lesson-api.md) |
| Lesson Section | `/api/v1/lessons/{id}/sections`, `/api/v1/lesson-sections` | `LessonEndpoints.cs` | [lesson-section-api.md](lesson-section-api.md) |
| Course | `/api/v1/courses` | `CourseEndpoints.cs` | [course-api.md](course-api.md) |
| Book | `/api/v1/books` | `BookEndpoints.cs` | [book-api.md](book-api.md) |
| Book Chapter | `/api/v1/books/{id}/chapters`, `/api/v1/book-chapters` | `BookEndpoints.cs` | [book-chapter-api.md](book-chapter-api.md) |
| Quiz | `/api/v1/quizzes` | `QuizEndpoints.cs` | [quiz-api.md](quiz-api.md) |
| Quiz Question | `/api/v1/quizzes/{id}/questions`, `/api/v1/quiz-questions` | `QuizEndpoints.cs` | [quiz-question-api.md](quiz-question-api.md) |
| Quiz Choice | `/api/v1/quiz-questions/{id}/choices`, `/api/v1/quiz-choices` | `QuizEndpoints.cs` | [quiz-choice-api.md](quiz-choice-api.md) |
| Publish Job | `/api/v1/publish-jobs` | `PublishingEndpoints.cs` | [publishing-api.md](publishing-api.md) |
| Publish Template | `/api/v1/publish-templates` | `PublishingEndpoints.cs` | [publishing-api.md](publishing-api.md) |
| Published Artifact | `/api/v1/published-artifacts` | `PublishingEndpoints.cs` | [publishing-api.md](publishing-api.md) |
| Auth | `/api/v1/auth` | `SecurityEndpoints.cs` | [auth-api.md](auth-api.md) |
| Users | `/api/v1/users` | `SecurityEndpoints.cs` | [user-api.md](user-api.md) |
| Roles | `/api/v1/roles` | `SecurityEndpoints.cs` | [role-api.md](role-api.md) |
| Permissions | `/api/v1/permissions` | `SecurityEndpoints.cs` | [role-api.md](role-api.md) |

Minimal Pair, Grammar Example, and Lesson Section all live inside their parent module's endpoint file (`PronunciationEndpoints.cs`, `GrammarEndpoints.cs`, `LessonEndpoints.cs`) rather than a file of their own, since they are embedded child entities, not independent top-level modules.

Course Lesson routes live inside `CourseEndpoints.cs`, since `CourseLesson` is an ordered join entity managed through its parent Course rather than a standalone module. Book Chapter and Book Chapter Lesson routes live inside `BookEndpoints.cs`, since both are managed from the parent Book detail workflow. Quiz Question and Quiz Choice routes live inside `QuizEndpoints.cs`, since both are managed from the parent Quiz detail workflow.

Every route across all endpoint files is unique: no two route groups register the same HTTP method + path combination.

Most `/api/v1` endpoints require authentication. Module endpoints use route-level permission policies such as `words.read`, `words.create`, `users.update`, and `publishing.run`. `POST /api/v1/auth/login` is the anonymous entry point. See [authorization.md](../security/authorization.md) for the policy model.

## Route Naming Conventions

The default convention is a **plural, kebab-case resource noun** plus standard CRUD verbs (`GET` list, `GET /{id}` item, `POST` create, `PUT /{id}` update, `DELETE /{id}` deactivate): `words`, `categories`, `tags`, `pronunciations`, `minimal-pairs`, `grammar-topics`, `grammar-rules`, `grammar-examples`, `lessons`, `lesson-sections`, `courses`, `books`, `book-chapters`, `quizzes`, `quiz-questions`, `quiz-choices`, `publish-jobs`, `publish-templates`, and `published-artifacts`.

Two accepted exceptions exist — both intentional, both already covered by tests, and **not** something to "fix" by renaming:

- **`GET /api/v1/words/{wordId:guid}/pronunciation`** uses a singular `pronunciation` segment. This is semantically deliberate: a word has at most one pronunciation, so the singular form represents a to-one sub-resource rather than a collection.
- **`POST /api/v1/media/upload`, `POST /api/v1/media/{id}/activate`, `POST /api/v1/media/{id}/deactivate`** use RPC-style verb segments instead of resource nouns. This is a standard, accepted REST pattern for actions that aren't plain CRUD.
- **`POST /api/v1/publish-jobs/{id}/run` and `POST /api/v1/publish-jobs/{id}/cancel`** are action routes for publish-job lifecycle operations that are not plain CRUD.

## How To Add A New API Route

1. Define the request/response record(s) in the matching `EnglishMaster.Contracts.{Module}` folder — plain `record` types, no logic.
2. Add a Command/Query record and its Handler class under `EnglishMaster.Application.Features.{Module}`, following the existing "one `HandleAsync` method, constructor-injected repository interface(s) + `TimeProvider`" pattern (no MediatR — this codebase uses plain handler classes). Add a repository interface method (and its EF implementation under `EnglishMaster.Infrastructure.{Module}`) only if new persistence access is needed.
3. Add a thin handler method to the module's `Endpoints.cs` file that only binds parameters, calls the Application handler, and maps the `Result`/`Result<T>` via the existing `ToHttpResult` pattern — copy the shape of any existing handler in `GrammarEndpoints.cs`, the clearest example of this pattern in the codebase.
4. Add the correct `.RequireAuthorization(Permissions.SomePermission)` policy to the mapped endpoint.
5. If it's a brand-new module (new endpoint file), register it in `Program.cs` with a new `Map{Module}Endpoints()` call.
6. Add unit tests for the new Application handler (with a fake repository) and an integration test exercising the new route through the real HTTP pipeline.
7. Run `dotnet build` and `dotnet test`.

## Known Limitations

- Blazor navigation is not yet dynamically filtered by permission, although API endpoints enforce permission policies.
- Quiz choice admin DTOs include `IsCorrect`; future public quiz-taking endpoints must use separate DTOs that do not expose correct answers before submission.
- The two naming exceptions documented above are deliberate, not defects.
- `ActivateGrammarTopicCommandHandler`, `DeactivateGrammarTopicCommandHandler`, `ActivateGrammarRuleCommandHandler`, `DeactivateGrammarRuleCommandHandler`, `ActivatePronunciationCommandHandler`, `DeactivatePronunciationCommandHandler`, `ActivateLessonCommandHandler`, `DeactivateLessonCommandHandler`, `ActivateCourseCommandHandler`, and `DeactivateCourseCommandHandler` are registered in dependency injection but have no route mapped to them anywhere (unlike Media, which has `POST /{id}/activate`/`/deactivate` wired up) — dead code, flagged for a future cleanup or route-completion pass, not fixed here.
- `GetLessonSectionByIdQueryHandler` is likewise registered but has no route — there is no `GET /api/v1/lesson-sections/{id}` endpoint.
- Publishing `Pdf` and `Docx` generation are placeholder implementations documented in [publishing-api.md](publishing-api.md).

## Next Recommended Module

Student Progress should be added next now that the API surface has admin authentication and permission-based authorization.
