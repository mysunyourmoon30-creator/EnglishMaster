# Grammar Rule API

Base path:

```text
/api/v1/grammar-rules
```

The API returns contract DTOs from `EnglishMaster.Contracts.GrammarRules`. It does not expose EF Core entities.

## Endpoints

| Method | Route | Description |
| --- | --- | --- |
| `GET` | `/api/v1/grammar-rules` | Search, filter, and paginate rules. |
| `GET` | `/api/v1/grammar-rules/{id}` | Get one rule by id. |
| `POST` | `/api/v1/grammar-rules` | Create a rule. |
| `PUT` | `/api/v1/grammar-rules/{id}` | Update a rule. |
| `DELETE` | `/api/v1/grammar-rules/{id}` | Deactivate a rule. |
| `POST` | `/api/v1/grammar-rules/{grammarRuleId}/words/{wordId}` | Add a related word to a rule. |
| `DELETE` | `/api/v1/grammar-rules/{grammarRuleId}/words/{wordId}` | Remove a related word from a rule. |
| `GET` | `/api/v1/grammar-topics/{topicId}/rules` | List rules for a topic. Documented in [Grammar Topic API](grammar-topic-api.md). |
| `GET` `POST` | `/api/v1/grammar-rules/{grammarRuleId}/examples` | List/add examples for a rule. Documented in [Grammar Example API](grammar-example-api.md). |

## Search Parameters

`GET /api/v1/grammar-rules` supports these query parameters:

| Parameter | Type | Description |
| --- | --- | --- |
| `search` | string | Searches `Title`, `Slug`, `RuleText`, and `StructurePattern`. |
| `grammarTopicId` | guid | Optional filter to rules under one topic. |
| `isActive` | bool | Optional active-state filter. Defaults to `true`. |
| `pageNumber` | int | Page number. Defaults to `1`. |
| `pageSize` | int | Page size. Defaults to `20`, maximum `100`. |

Example:

```http
GET /api/v1/grammar-rules?search=Positive&grammarTopicId=11111111-1111-1111-1111-111111111111&isActive=true&pageNumber=1&pageSize=20
```

Response:

```json
{
  "items": [
    {
      "id": "55555555-5555-5555-5555-555555555555",
      "grammarTopicId": "11111111-1111-1111-1111-111111111111",
      "topic": {
        "id": "11111111-1111-1111-1111-111111111111",
        "title": "Present Simple",
        "slug": "present-simple",
        "cefrLevel": "A1"
      },
      "title": "Positive Sentences",
      "slug": "positive-sentences",
      "ruleText": "Subject + base verb",
      "explanationTh": "ใช้กับกิจวัตร",
      "explanationEn": "Used for routines",
      "structurePattern": "S + V1",
      "commonMistake": "Missing s for he/she/it",
      "correctUsageNote": "Add s for third person singular",
      "sortOrder": 0,
      "relatedWords": [],
      "isActive": true,
      "createdAt": "2026-07-10T08:00:00+00:00",
      "updatedAt": "2026-07-10T08:00:00+00:00"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

## Get By Id

```http
GET /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555
```

Success returns `200 OK` with a `GrammarRuleDto`. Unknown ids return `404 Not Found`.

## Create Grammar Rule

```http
POST /api/v1/grammar-rules
Content-Type: application/json
```

Request:

```json
{
  "grammarTopicId": "11111111-1111-1111-1111-111111111111",
  "title": "Positive Sentences",
  "ruleText": "Subject + base verb",
  "explanationTh": "ใช้กับกิจวัตร",
  "explanationEn": "Used for routines",
  "structurePattern": "S + V1",
  "commonMistake": "Missing s for he/she/it",
  "correctUsageNote": "Add s for third person singular",
  "sortOrder": 0
}
```

Success returns `201 Created` at `/api/v1/grammar-rules/{id}` with the created `GrammarRuleDto`.

Validation errors return `400 Bad Request`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ruleText": [
      "ruleText is required."
    ]
  }
}
```

A duplicate title returns `400 Bad Request` under the PascalCase field name `Title`:

```json
{
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": [
      "A grammar rule with this title already exists."
    ]
  }
}
```

If `grammarTopicId` does not reference an existing, active topic, the request returns `400 Bad Request` with an error under `grammarTopicId`.

## Update Grammar Rule

```http
PUT /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555
Content-Type: application/json
```

Request:

```json
{
  "grammarTopicId": "11111111-1111-1111-1111-111111111111",
  "title": "Positive Sentences",
  "ruleText": "Subject + base verb",
  "explanationTh": "ใช้กับกิจวัตร",
  "explanationEn": "Used for routines",
  "structurePattern": "S + V1",
  "commonMistake": "Missing s for he/she/it",
  "correctUsageNote": "Add s for third person singular",
  "sortOrder": 1,
  "isActive": true
}
```

Success returns `200 OK` with the updated `GrammarRuleDto`. Unknown ids return `404 Not Found`. Validation errors return `400 Bad Request`.

## Delete Grammar Rule

```http
DELETE /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555
```

Success returns `204 No Content`. The rule is deactivated, not hard-deleted. Unknown ids return `404 Not Found`.

## Related Words

### Add Related Word

```http
POST /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555/words/77777777-7777-7777-7777-777777777777
```

No request body. Success returns `200 OK` (not `201 Created`) with the updated `GrammarRuleDto`, including the new entry in `relatedWords`. Adding the same word twice is idempotent and still returns `200 OK`. Returns `404 Not Found` if the rule does not exist, and `400 Bad Request` if the rule is inactive or the word does not exist or is inactive.

### Remove Related Word

```http
DELETE /api/v1/grammar-rules/55555555-5555-5555-5555-555555555555/words/77777777-7777-7777-7777-777777777777
```

Success returns `204 No Content`. Returns `404 Not Found` if the rule does not exist or the word is not currently related to the rule.

## Known Limitations

- Endpoints are not authenticated yet.
- Search is simple substring matching, not full-text search.
- `DELETE` is a soft delete through `IsActive = false`.
- The duplicate-title validation error uses field name `Title` (PascalCase) while other validation errors use camelCase field names (for example `ruleText`) — a minor inconsistency to account for in client error handling.
- Only active words can be added as related words.
