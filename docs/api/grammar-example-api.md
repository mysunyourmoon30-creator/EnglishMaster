# Grammar Example API

Grammar example routes are split between the parent rule route and direct item routes.

Parent base path:

```text
/api/v1/grammar-rules/{grammarRuleId}/examples
```

Item base path:

```text
/api/v1/grammar-examples
```

The API returns contract DTOs from `EnglishMaster.Contracts.GrammarExamples`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/grammar-rules/{grammarRuleId}/examples` | List examples for a rule. |
| `POST` | `/api/v1/grammar-rules/{grammarRuleId}/examples` | Add an example to a rule. |
| `GET` | `/api/v1/grammar-examples/{id}` | Get one example by id. |
| `PUT` | `/api/v1/grammar-examples/{id}` | Update an example. |
| `DELETE` | `/api/v1/grammar-examples/{id}` | Deactivate an example. |

## List By Rule

```http
GET /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555/examples
```

Success returns `200 OK` with a plain array of `GrammarExampleDto` items — there is no pagination envelope. Unknown rule ids return `404 Not Found`.

Response:

```json
[
  {
    "id": "66666666-6666-6666-6666-666666666666",
    "grammarRuleId": "55555555-5555-5555-5555-555555555555",
    "exampleEn": "She walks to school.",
    "translationTh": "เธอเดินไปโรงเรียน",
    "explanationTh": "Add s after she.",
    "isCorrectExample": true,
    "sortOrder": 0,
    "isActive": true,
    "createdAt": "2026-07-10T08:00:00+00:00",
    "updatedAt": "2026-07-10T08:00:00+00:00"
  }
]
```

## Add Example

```http
POST /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555/examples
Content-Type: application/json
```

Request:

```json
{
  "exampleEn": "She walks to school.",
  "translationTh": "เธอเดินไปโรงเรียน",
  "explanationTh": "Add s after she.",
  "isCorrectExample": true,
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/grammar-examples/{id}` with the created `GrammarExampleDto`.

Validation errors return `400 Bad Request`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "exampleEn": [
      "exampleEn is required."
    ]
  }
}
```

Adding an example to an unknown rule returns `404 Not Found`. Adding an example to an inactive rule returns `400 Bad Request` with an error under `grammarRuleId`.

## Get By Id

```http
GET /api/v1/grammar-examples/66666666-6666-6666-6666-666666666666
```

Success returns `200 OK` with a `GrammarExampleDto`. Unknown ids return `404 Not Found`.

## Update Example

```http
PUT /api/v1/grammar-examples/66666666-6666-6666-6666-666666666666
Content-Type: application/json
```

Request:

```json
{
  "exampleEn": "He plays football.",
  "translationTh": "เขาเล่นฟุตบอล",
  "explanationTh": "Add s after he.",
  "isCorrectExample": true,
  "sortOrder": 1,
  "isActive": true
}
```

Success returns `200 OK` with the updated `GrammarExampleDto`. Unknown ids return `404 Not Found`.

## Delete Example

```http
DELETE /api/v1/grammar-examples/66666666-6666-6666-6666-666666666666
```

Success returns `204 No Content`. The example is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Domain And Validation Notes

- The parent grammar rule must exist to add an example.
- New examples cannot be added to an inactive rule.
- `ExampleEn` is required.
- `SortOrder` must be greater than or equal to zero.

## Known Limitations

- There is no standalone search or pagination endpoint for grammar examples.
- Endpoints are not authenticated yet.
- Delete is a soft delete through `IsActive = false`.
