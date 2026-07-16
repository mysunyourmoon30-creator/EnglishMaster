# Quiz / Exercise Module

## Purpose

The Quiz / Exercise Module lets admins create publishable practice quizzes for the English learning platform. A `Quiz` is an ordered assessment container with optional CEFR level and optional links to Category, Lesson, Course, and Book. It owns ordered questions, and each question owns ordered choices.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Quizzes`
- Application: `EnglishMaster.Application.Features.Quizzes`, `EnglishMaster.Application.Features.QuizQuestions`, and `EnglishMaster.Application.Features.QuizChoices`
- Infrastructure: `EnglishMaster.Infrastructure.Quizzes`
- API: `EnglishMaster.Api.Endpoints.QuizEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Quizzes`, shared form components under `EnglishMaster.Web.Components.Quizzes`

## Quiz Fields

| Field | Description |
| --- | --- |
| `Id` | Unique quiz identifier. |
| `Title` | Required title. Trimmed and used to generate `Slug`. |
| `Slug` | URL-friendly value generated from `Title`; unique in the database. |
| `Summary` | Optional short summary. Stored as an empty string when omitted. |
| `Description` | Optional longer description. Stored as an empty string when omitted. |
| `CefrLevel` | Optional CEFR value: `A1`, `A2`, `B1`, `B2`, `C1`, `C2`. |
| `CategoryId` | Optional category identifier. |
| `Category` | Optional category summary in response DTOs. |
| `LessonId` | Optional lesson identifier. |
| `Lesson` | Optional lesson summary in response DTOs. |
| `CourseId` | Optional course identifier. |
| `Course` | Optional course summary in response DTOs. |
| `BookId` | Optional book identifier. |
| `Book` | Optional book summary in response DTOs. |
| `TimeLimitMinutes` | Optional time limit expressed as minutes. `0` means no limit. |
| `PassingScore` | Passing percentage from `0` to `100`. |
| `SortOrder` | Admin-defined display order. Must be zero or greater. |
| `Questions` | Ordered `QuizQuestion` rows that belong to the quiz. |
| `IsPublished` | Indicates whether the quiz is published. New quizzes start as draft. |
| `IsActive` | Indicates whether the quiz is active. Deletes deactivate the quiz. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Relationships

### Quiz And Lesson

`Quiz.LessonId` is optional. When provided, the Application layer validates that the lesson exists and is active before creating or updating the quiz. EF Core uses `SetNull` delete behavior, so deleting a lesson clears the relationship rather than deleting the quiz.

### Quiz And Course

`Quiz.CourseId` is optional. When provided, the Application layer validates that the course exists and is active. EF Core uses `SetNull` delete behavior.

### Quiz And Book

`Quiz.BookId` is optional. When provided, the Application layer validates that the book exists and is active. EF Core uses `SetNull` delete behavior.

### Quiz And Question

A quiz has many ordered questions.

- `QuizQuestion.QuizId` points to one `Quiz`.
- EF Core cascades from `Quiz` to `QuizQuestion` rows.
- Question order is controlled by `QuizQuestion.SortOrder`.
- Questions are managed through the Quiz detail page and nested API routes.

See [Quiz Question](quiz-question.md).

## Publish And Unpublish Rules

`IsPublished` and `IsActive` are separate lifecycle flags.

- `Publish` sets `IsPublished` to `true` and updates `UpdatedAt`.
- `Unpublish` sets `IsPublished` to `false` and updates `UpdatedAt`.
- `Activate` and `Deactivate` manage `IsActive` separately.
- New quizzes are created as `IsPublished: false` and `IsActive: true`.
- `DELETE /api/v1/quizzes/{id}` deactivates the quiz rather than hard-deleting it.

## Domain Rules

- `Title` is required, trimmed, and length-limited to 200 characters.
- `Slug` is generated from `Title`, must contain at least one letter or digit, and is length-limited to 220 characters.
- `Slug` is unique in persistence and checked by Application handlers before create/update.
- `Summary` and `Description` are optional, trimmed, and length-limited.
- `CefrLevel` is optional but must be a valid enum value when provided.
- `CategoryId`, `LessonId`, `CourseId`, and `BookId` are optional but cannot be empty GUIDs when provided.
- `TimeLimitMinutes` must be zero or greater.
- `PassingScore` must be between `0` and `100`.
- `SortOrder` must be zero or greater.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

Quiz use cases:

- `CreateQuiz`
- `UpdateQuiz`
- `DeleteQuiz`
- `GetQuizById`
- `SearchQuizzes`
- `PublishQuiz`
- `UnpublishQuiz`
- `ActivateQuiz`
- `DeactivateQuiz`

Quiz question use cases:

- `AddQuizQuestion`
- `UpdateQuizQuestion`
- `DeleteQuizQuestion`
- `GetQuizQuestionById`
- `GetQuizQuestionsByQuizId`
- `ReorderQuizQuestions`
- `ActivateQuizQuestion`
- `DeactivateQuizQuestion`

Quiz choice use cases:

- `AddQuizChoice`
- `UpdateQuizChoice`
- `DeleteQuizChoice`
- `GetQuizChoiceById`
- `GetQuizChoicesByQuestionId`
- `ReorderQuizChoices`
- `ActivateQuizChoice`
- `DeactivateQuizChoice`

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/quizzes` | Search, filter, sort, and paginate quizzes. |
| `GET` | `/api/v1/quizzes/{id}` | Get one quiz by id. |
| `POST` | `/api/v1/quizzes` | Create a quiz. |
| `PUT` | `/api/v1/quizzes/{id}` | Update a quiz. |
| `DELETE` | `/api/v1/quizzes/{id}` | Deactivate a quiz. |
| `POST` | `/api/v1/quizzes/{id}/publish` | Publish a quiz. |
| `POST` | `/api/v1/quizzes/{id}/unpublish` | Unpublish a quiz. |
| `POST` | `/api/v1/quizzes/{id}/activate` | Activate a quiz. |
| `POST` | `/api/v1/quizzes/{id}/deactivate` | Deactivate a quiz. |
| `GET` | `/api/v1/quizzes/{quizId}/questions` | List questions in a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions` | Add a question to a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions/reorder` | Reorder quiz questions. |
| `GET` | `/api/v1/quiz-questions/{id}` | Get one question by id. |
| `PUT` | `/api/v1/quiz-questions/{id}` | Update a question. |
| `DELETE` | `/api/v1/quiz-questions/{id}` | Deactivate a question. |
| `GET` | `/api/v1/quiz-questions/{questionId}/choices` | List choices in a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices` | Add a choice to a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices/reorder` | Reorder question choices. |
| `GET` | `/api/v1/quiz-choices/{id}` | Get one choice by id. |
| `PUT` | `/api/v1/quiz-choices/{id}` | Update a choice. |
| `DELETE` | `/api/v1/quiz-choices/{id}` | Deactivate a choice. |

Full request/response examples are documented in [Quiz API](../api/quiz-api.md), [Quiz Question API](../api/quiz-question-api.md), and [Quiz Choice API](../api/quiz-choice-api.md).

## Search And Filter Parameters

`GET /api/v1/quizzes` supports:

- `search` - searches `Title`, `Slug`, and `Summary`.
- `cefrLevel` - optional CEFR filter.
- `categoryId` - optional category filter.
- `lessonId` - optional lesson filter.
- `courseId` - optional course filter.
- `bookId` - optional book filter.
- `isPublished` - optional published-state filter.
- `isActive` - optional active-state filter; defaults to `true`.
- `pageNumber` - defaults to `1`.
- `pageSize` - defaults to `20`, maximum `100`.
- `sortBy` - `Title` or `CreatedAt`, defaults to `Title`.
- `sortDirection` - `Asc` or `Desc`, defaults to `Asc`.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Quiz list | `/admin/quizzes` | Search, filter, paginate, view, edit, and delete quizzes. |
| Create quiz | `/admin/quizzes/create` | Create a draft quiz. |
| Quiz detail | `/admin/quizzes/{id:guid}` | View quiz details, publish state, questions, and choices. |
| Edit quiz | `/admin/quizzes/{id:guid}/edit` | Update quiz metadata and status. |

The list page includes search, CEFR filter, Category filter, Lesson filter, Course filter, Book filter, published filter, active filter, page size selection, sort selection, pagination controls, loading, empty, and error states. The detail page supports publish/unpublish, adding/editing/deleting questions, adding/editing/deleting choices, and simple up/down reordering for questions and choices.

## Admin Routes And Dashboard

Quiz routes are centralized under `AdminRoutes.Quizzes` in `src/Frontend/EnglishMaster.Web/Routes/AdminRoutes.cs`. The admin navigation includes a `Quizzes` link. The dashboard includes a `Total Quizzes` card that calls the quiz search endpoint with `PageSize: 1` and `IsActive: null`, then reads `TotalCount`.

See [Admin Routes](../routes/admin-routes.md).

## Security Note About Correct Answers

The current Quiz API is an admin-oriented API and returns `QuizChoiceDto.IsCorrect` so admins can build and review answer keys. Do not reuse these DTOs for learner-facing or public quiz-taking endpoints. Future public quiz DTOs must omit correct-answer flags and should reveal answer correctness only after the intended submission/evaluation workflow.

## Test Coverage

- `tests/EnglishMaster.UnitTests/Quizzes/QuizTests.cs` covers quiz creation, normalization, slug generation, required title, time-limit validation, passing-score validation, lifecycle behavior, question creation, choice creation, duplicate correct-choice prevention, and invalid question-type protection.
- `tests/EnglishMaster.UnitTests/Quizzes/QuizUseCaseTests.cs` covers create, duplicate slug validation, publish/unpublish, activate/deactivate, add question, reorder questions, add choice, duplicate correct-choice validation, reorder choices, search by CEFR, and search by published status.
- `tests/EnglishMaster.IntegrationTests/Quizzes/QuizEndpointsTests.cs` covers the HTTP flow for create, search, add/update/reorder/delete questions, add/list/reorder/delete choices, duplicate correct-choice validation, publish/unpublish, update, soft delete, active/inactive search, missing-title validation, and duplicate-title validation.
- `tests/EnglishMaster.ArchitectureTests` covers project reference rules and layer boundaries for the module.

## Known Limitations

- Quiz admin endpoints and pages are not protected by authentication or authorization yet.
- Search uses simple database `Contains` matching rather than full-text search.
- `DELETE` deactivates quizzes, questions, and choices instead of hard-deleting them.
- Reordering uses simple up/down UI controls and requires a complete ordered id list from the API caller.
- The module stores quiz definitions only; there is no learner attempt, scoring, progress tracking, analytics, or result history yet.
- Correct answers are exposed by admin DTOs and must not be reused for public quiz-taking surfaces.
- There is no randomization, timed attempt enforcement, or question bank import/export yet.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
