# Quiz Choice

## Purpose

`QuizChoice` is the ordered answer option entity inside a `QuizQuestion`. It stores display text, correctness, optional explanations, order, and active state for admin-authored choice-based exercises.

## QuizChoice Fields

| Field | Description |
| --- | --- |
| `Id` | Unique choice identifier. |
| `QuizQuestionId` | Required parent question identifier. |
| `ChoiceText` | Required choice text. |
| `IsCorrect` | Indicates whether the choice is part of the answer key. |
| `ExplanationTh` | Optional Thai explanation. Stored as an empty string when omitted. |
| `ExplanationEn` | Optional English explanation. Stored as an empty string when omitted. |
| `SortOrder` | Display order inside the question. Must be zero or greater. |
| `IsActive` | Indicates whether the choice is active. Deletes deactivate the choice. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## QuizQuestion Relationship

`QuizChoice` belongs to one `QuizQuestion`.

- `QuizQuestion` owns the `Choices` collection.
- EF Core maps `QuizQuestion` to many `QuizChoice` records with cascade delete from question to choice rows.
- The Application layer validates that the parent question exists and is active before a choice can be added.
- Choice ordering is controlled by `SortOrder`.

## Domain Rules

- `Id` and `QuizQuestionId` cannot be empty GUIDs.
- `ChoiceText` is required, trimmed, and length-limited to 1000 characters.
- `ExplanationTh` and `ExplanationEn` are optional and length-limited to 2000 characters.
- `SortOrder` must be zero or greater.
- `Activate` and `Deactivate` update `IsActive` and `UpdatedAt`.
- `Reorder` changes `SortOrder` and updates `UpdatedAt`.
- Active correct-choice count is controlled by the parent `QuizQuestion`: `SingleChoice` and `TrueFalse` questions allow only one active correct choice; `MultipleChoice` allows multiple.
- Domain code does not reference EF Core, ASP.NET Core, Blazor, or infrastructure.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/quiz-questions/{questionId}/choices` | List choices in a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices` | Add a choice to a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices/reorder` | Reorder all choices in a question. |
| `GET` | `/api/v1/quiz-choices/{id}` | Get one choice by id. |
| `PUT` | `/api/v1/quiz-choices/{id}` | Update a choice. |
| `DELETE` | `/api/v1/quiz-choices/{id}` | Deactivate a choice. |
| `POST` | `/api/v1/quiz-choices/{id}/activate` | Activate a choice. |
| `POST` | `/api/v1/quiz-choices/{id}/deactivate` | Deactivate a choice. |

Full status codes and examples are documented in [Quiz Choice API](../api/quiz-choice-api.md).

## Request Example

```json
{
  "choiceText": "Hello",
  "isCorrect": true,
  "explanationTh": "This is the Thai explanation.",
  "explanationEn": "This is a greeting.",
  "sortOrder": 0
}
```

## Response Example

```json
{
  "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "quizQuestionId": "11111111-1111-1111-1111-111111111111",
  "choiceText": "Hello",
  "isCorrect": true,
  "explanationTh": "This is the Thai explanation.",
  "explanationEn": "This is a greeting.",
  "sortOrder": 0,
  "isActive": true,
  "createdAt": "2026-07-13T08:00:00+00:00",
  "updatedAt": "2026-07-13T08:00:00+00:00"
}
```

## Security Note About Correct Answers

`QuizChoiceDto` includes `IsCorrect` because the current endpoints are admin endpoints for authoring and reviewing answer keys. Public learner-facing endpoints must use separate DTOs that hide `IsCorrect` until after a submitted answer is evaluated.

## Blazor UI

Quiz choices are managed inline on `/admin/quizzes/{id:guid}`, nested under each question.

- Admins can add a choice.
- Admins can edit a choice.
- Admins can delete a choice, which deactivates it.
- Admins can reorder choices using Up and Down actions.
- Validation errors are shown near the choice form without losing entered values.
- Loading, empty, error, and validation states are shown through the Quiz detail page.

## Test Coverage

Current tests cover:

- Creating `QuizChoice`
- Required choice text
- Duplicate correct-choice prevention through the parent question rules
- Adding choices through the Application layer
- Reordering choices
- Listing, adding, updating, deleting, and reordering choices through the API integration test

## Known Limitations

- There is no standalone `/admin/quiz-choices` list page.
- Choice delete is a soft delete through `IsActive = false`.
- Reorder requires the submitted id list to exactly match the question's current choice set.
- Choice correctness is admin-visible only; public quiz-taking DTOs do not exist yet.

## Next Recommended Module

See [Quiz / Exercise Module](quiz-module.md#next-recommended-module).
