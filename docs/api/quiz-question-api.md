# Quiz Question API

Quiz questions are managed from the Quiz endpoint file. Question collection routes are nested under `/api/v1/quizzes/{quizId}/questions`; item and choice routes use `/api/v1/quiz-questions`.

The API returns contract DTOs from `EnglishMaster.Contracts.QuizQuestions`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/quizzes/{quizId}/questions` | List questions in a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions` | Add a question to a quiz. |
| `POST` | `/api/v1/quizzes/{quizId}/questions/reorder` | Reorder questions in a quiz. |
| `GET` | `/api/v1/quiz-questions/{id}` | Get one question by id. |
| `PUT` | `/api/v1/quiz-questions/{id}` | Update a question. |
| `DELETE` | `/api/v1/quiz-questions/{id}` | Deactivate a question. |
| `POST` | `/api/v1/quiz-questions/{id}/activate` | Activate a question. |
| `POST` | `/api/v1/quiz-questions/{id}/deactivate` | Deactivate a question. |
| `GET` | `/api/v1/quiz-questions/{questionId}/choices` | List choices in a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices` | Add a choice to a question. |
| `POST` | `/api/v1/quiz-questions/{questionId}/choices/reorder` | Reorder choices in a question. |

## List Quiz Questions

```http
GET /api/v1/quizzes/11111111-1111-1111-1111-111111111111/questions
```

Success returns `200 OK` with a collection of `QuizQuestionDto` rows ordered by `SortOrder`. Unknown quiz ids return `404 Not Found`.

Response:

```json
[
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
]
```

## Add Quiz Question

```http
POST /api/v1/quizzes/11111111-1111-1111-1111-111111111111/questions
Content-Type: application/json
```

Request:

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

Success returns `201 Created` at `/api/v1/quiz-questions/{id}` with the created `QuizQuestionDto`. Unknown quiz ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Get Quiz Question

```http
GET /api/v1/quiz-questions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `200 OK` with a `QuizQuestionDto`. Unknown ids return `404 Not Found`.

## Update Quiz Question

```http
PUT /api/v1/quiz-questions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
Content-Type: application/json
```

Request:

```json
{
  "questionText": "Choose the correct greeting updated.",
  "questionType": "SingleChoice",
  "explanationTh": "Use the common Thai explanation here.",
  "explanationEn": "Updated explanation.",
  "points": 1,
  "sortOrder": 0,
  "wordId": null,
  "grammarRuleId": null,
  "pronunciationId": null,
  "isActive": true
}
```

Success returns `200 OK` with the updated `QuizQuestionDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Quiz Question

```http
DELETE /api/v1/quiz-questions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa
```

Success returns `204 No Content`. The question is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Activate / Deactivate Quiz Question

```http
POST /api/v1/quiz-questions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/activate
POST /api/v1/quiz-questions/aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa/deactivate
```

Both return `200 OK` with the updated `QuizQuestionDto`. Unknown ids return `404 Not Found`.

## Reorder Quiz Questions

```http
POST /api/v1/quizzes/11111111-1111-1111-1111-111111111111/questions/reorder
Content-Type: application/json
```

Request:

```json
{
  "orderedQuestionIds": [
    "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb",
    "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"
  ]
}
```

Success returns `200 OK` with the reordered `QuizQuestionDto` collection. The submitted list must include the same question ids currently attached to the quiz, without duplicates.

## Choices

Question choices are managed through nested routes:

- `GET /api/v1/quiz-questions/{questionId}/choices`
- `POST /api/v1/quiz-questions/{questionId}/choices`
- `POST /api/v1/quiz-questions/{questionId}/choices/reorder`

See [Quiz Choice API](quiz-choice-api.md) for request and response examples.

## QuestionType Values

Valid values are:

- `SingleChoice`
- `MultipleChoice`
- `TrueFalse`
- `FillBlank`
- `ShortAnswer`

Invalid or empty values return `400 Bad Request`.

## Known Limitations

- Endpoints are not authenticated yet.
- Question delete is a soft delete through `IsActive = false`.
- Only active words, grammar rules, and pronunciations can be referenced.
- Reorder endpoints require a complete ordered id list.
- FillBlank and ShortAnswer learner-facing answer evaluation is not implemented yet.
