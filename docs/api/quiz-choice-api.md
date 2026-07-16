# Quiz Choice API

Quiz choices are managed from the Quiz endpoint file. Choice collection routes are nested under `/api/v1/quiz-questions/{questionId}/choices`; item routes use `/api/v1/quiz-choices`.

The API returns contract DTOs from `EnglishMaster.Contracts.QuizChoices`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/quiz-questions/{questionId}/choices` | List choices in a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices` | Add a choice to a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices/reorder` | Reorder choices in a question. |
| `GET` | `/api/v1/quiz-choices/{id}` | Get one choice by id. |
| `PUT` | `/api/v1/quiz-choices/{id}` | Update a choice. |
| `DELETE` | `/api/v1/quiz-choices/{id}` | Deactivate a choice. |
| `POST` | `/api/v1/quiz-choices/{id}/activate` | Activate a choice. |
| `POST` | `/api/v1/quiz-choices/{id}/deactivate` | Deactivate a choice. |

## List Quiz Choices

```http
GET /api/v1/quiz-questions/11111111-1111-1111-1111-111111111111/choices
```

Success returns `200 OK` with a collection of `QuizChoiceDto` rows ordered by `SortOrder`. Unknown question ids return `404 Not Found`.

Response:

```json
[
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
]
```

## Add Quiz Choice

```http
POST /api/v1/quiz-questions/11111111-1111-1111-1111-111111111111/choices
Content-Type: application/json
```

Request:

```json
{
  "choiceText": "Hello",
  "isCorrect": true,
  "explanationTh": "This is the Thai explanation.",
  "explanationEn": "This is a greeting.",
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/quiz-choices/{id}` with the created `QuizChoiceDto`. Unknown question ids return `404 Not Found`. Validation errors return `400 Bad Request`.

For `SingleChoice` and `TrueFalse` questions, trying to add a second active correct choice returns `400 Bad Request`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "IsCorrect": [
      "SingleChoice questions can have only one correct choice."
    ]
  }
}
```

## Get Quiz Choice

```http
GET /api/v1/quiz-choices/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `200 OK` with a `QuizChoiceDto`. Unknown ids return `404 Not Found`.

## Update Quiz Choice

```http
PUT /api/v1/quiz-choices/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Content-Type: application/json
```

Request:

```json
{
  "choiceText": "Hello",
  "isCorrect": true,
  "explanationTh": "This is the Thai explanation.",
  "explanationEn": "This is a greeting.",
  "sortOrder": 0,
  "isActive": true
}
```

Success returns `200 OK` with the updated `QuizChoiceDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Quiz Choice

```http
DELETE /api/v1/quiz-choices/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `204 No Content`. The choice is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Activate / Deactivate Quiz Choice

```http
POST /api/v1/quiz-choices/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/activate
POST /api/v1/quiz-choices/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/deactivate
```

Both return `200 OK` with the updated `QuizChoiceDto`. Unknown ids return `404 Not Found`.

## Reorder Quiz Choices

```http
POST /api/v1/quiz-questions/11111111-1111-1111-1111-111111111111/choices/reorder
Content-Type: application/json
```

Request:

```json
{
  "orderedChoiceIds": [
    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
  ]
}
```

Success returns `200 OK` with the reordered `QuizChoiceDto` collection. The submitted list must include the same choice ids currently attached to the question, without duplicates.

## Security Note About Correct Answers

`QuizChoiceDto` exposes `isCorrect` because this is an admin authoring API. Public learner-facing quiz endpoints must use separate DTOs that do not reveal the answer key before the learner submits an answer.

## Known Limitations

- Endpoints are not authenticated yet.
- Choice delete is a soft delete through `IsActive = false`.
- Reorder endpoints require a complete ordered id list.
- Correct-answer visibility is currently admin-only by convention, not enforced by authorization yet.
