# Quiz Question

## Purpose

`QuizQuestion` is the ordered child entity inside a `Quiz`. It stores the question prompt, question type, optional explanations, point value, optional links to learning content, and the ordered choices used for selectable-answer exercises.

## QuizQuestion Fields

| Field | Description |
| --- | --- |
| `Id` | Unique question identifier. |
| `QuizId` | Required parent quiz identifier. |
| `QuestionText` | Required question prompt. |
| `QuestionType` | Required question type. See [QuestionType Values](#questiontype-values). |
| `ExplanationTh` | Optional Thai explanation. Stored as an empty string when omitted. |
| `ExplanationEn` | Optional English explanation. Stored as an empty string when omitted. |
| `Points` | Points awarded by the question. Must be greater than zero. |
| `SortOrder` | Display order inside the quiz. Must be zero or greater. |
| `WordId` | Optional related word identifier. |
| `Word` | Optional related word summary in response DTOs. |
| `GrammarRuleId` | Optional related grammar rule identifier. |
| `GrammarRule` | Optional related grammar rule summary in response DTOs. |
| `PronunciationId` | Optional related pronunciation identifier. |
| `Pronunciation` | Optional related pronunciation summary in response DTOs. |
| `Choices` | Ordered `QuizChoice` records that belong to the question. |
| `IsActive` | Indicates whether the question is active. Deletes deactivate the question. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## QuestionType Values

| Value | Intended use |
| --- | --- |
| `SingleChoice` | One correct answer from multiple choices. Active correct choices are limited to one. |
| `MultipleChoice` | One or more correct answers from multiple choices. |
| `TrueFalse` | True/false style question. Active correct choices are limited to one. |
| `FillBlank` | Fill-in-the-blank exercise. Choice support is available for admin setup but learner-facing behavior is future work. |
| `ShortAnswer` | Free text answer exercise. Choice support is available for admin setup but learner-facing behavior is future work. |

## Quiz Relationship

`QuizQuestion` belongs to one `Quiz`.

- `Quiz` owns the `Questions` collection.
- EF Core maps `Quiz` to many `QuizQuestion` records with cascade delete from quiz to question rows.
- The Application layer validates that the parent quiz exists and is active before a question can be added.
- Question ordering is controlled by `SortOrder`.

## Word Relationship

`QuizQuestion.WordId` is optional. When provided, the Application layer validates that the word exists and is active. EF Core uses `SetNull` delete behavior, so deleting a word clears the question relationship rather than deleting the question.

## GrammarRule Relationship

`QuizQuestion.GrammarRuleId` is optional. When provided, the Application layer validates that the grammar rule exists and is active. EF Core uses `SetNull` delete behavior.

## Pronunciation Relationship

`QuizQuestion.PronunciationId` is optional. When provided, the Application layer validates that the pronunciation exists and is active. EF Core uses `SetNull` delete behavior.

## Choice Relationship

A question has many ordered choices.

- `QuizChoice.QuizQuestionId` points to one `QuizQuestion`.
- EF Core cascades from `QuizQuestion` to `QuizChoice` rows.
- Choice order is controlled by `QuizChoice.SortOrder`.
- Choices are managed through the Quiz detail page and nested API routes.

See [Quiz Choice](quiz-choice.md).

## Domain Rules

- `Id` and `QuizId` cannot be empty GUIDs.
- `QuestionText` is required, trimmed, and length-limited to 2000 characters.
- `QuestionType` must be a defined `QuizQuestionType` value.
- `ExplanationTh` and `ExplanationEn` are optional and length-limited to 2000 characters.
- `Points` must be greater than zero.
- `SortOrder` must be zero or greater.
- `WordId`, `GrammarRuleId`, and `PronunciationId` are optional but cannot be empty GUIDs when provided.
- `Activate` and `Deactivate` update `IsActive` and `UpdatedAt`.
- `Reorder` changes `SortOrder` and updates `UpdatedAt`.
- `SingleChoice` and `TrueFalse` questions can have only one active correct choice.
- `MultipleChoice` questions can have multiple active correct choices.
- Domain code does not reference EF Core, ASP.NET Core, Blazor, or infrastructure.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/quizzes/{quizId}/questions` | List questions in a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions` | Add a question to a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions/reorder` | Reorder all questions in a quiz. |
| `GET` | `/api/v1/quiz-questions/{id}` | Get one question by id. |
| `PUT` | `/api/v1/quiz-questions/{id}` | Update a question. |
| `DELETE` | `/api/v1/quiz-questions/{id}` | Deactivate a question. |
| `POST` | `/api/v1/quiz-questions/{id}/activate` | Activate a question. |
| `POST` | `/api/v1/quiz-questions/{id}/deactivate` | Deactivate a question. |

Full status codes and examples are documented in [Quiz Question API](../api/quiz-question-api.md).

## Request Example

```json
{
  "questionText": "Choose the correct greeting.",
  "questionType": "SingleChoice",
  "explanationTh": "Use the common Thai explanation here.",
  "explanationEn": "Use a normal greeting.",
  "points": 1,
  "sortOrder": 0,
  "wordId": null,
  "grammarRuleId": null,
  "pronunciationId": null
}
```

## Response Example

```json
{
  "id": "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa",
  "quizId": "11111111-1111-1111-1111-111111111111",
  "questionText": "Choose the correct greeting.",
  "questionType": "SingleChoice",
  "explanationTh": "Use the common Thai explanation here.",
  "explanationEn": "Use a normal greeting.",
  "points": 1,
  "sortOrder": 0,
  "wordId": null,
  "word": null,
  "grammarRuleId": null,
  "grammarRule": null,
  "pronunciationId": null,
  "pronunciation": null,
  "choices": [],
  "isActive": true,
  "createdAt": "2026-07-13T08:00:00+00:00",
  "updatedAt": "2026-07-13T08:00:00+00:00"
}
```

## Blazor UI

Quiz questions are managed inline on `/admin/quizzes/{id:guid}`.

- Admins can add a question.
- Admins can edit a question.
- Admins can delete a question, which deactivates it.
- Admins can reorder questions using Up and Down actions.
- The form can select active Words, Grammar Rules, and Pronunciations.
- Loading, empty, error, and validation states are shown through the Quiz detail page.

## Test Coverage

Current tests cover:

- Creating `QuizQuestion`
- Required question text and positive points
- Invalid question type rejection
- Adding questions through the Application layer
- Reordering questions
- Listing, adding, updating, deleting, and reordering questions through the API integration test

## Known Limitations

- There is no standalone `/admin/quiz-questions` list page.
- Question delete is a soft delete through `IsActive = false`.
- Reorder requires the submitted id list to exactly match the quiz's current question set.
- FillBlank and ShortAnswer are stored as question types, but learner-facing answer evaluation is future work.

## Next Recommended Module

See [Quiz / Exercise Module](quiz-module.md#next-recommended-module).
