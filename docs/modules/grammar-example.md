# Grammar Example

## Purpose

`GrammarExample` is the practice sub-slice of the Grammar Module. It stores an English example sentence that demonstrates a `GrammarRule`, either as a correct usage example or as a common-mistake (incorrect usage) example for contrast.

Grammar Example belongs to Grammar Rule and does not stand alone as a separate top-level learning module.

## GrammarExample Fields

| Field | Description |
| --- | --- |
| `Id` | Unique example identifier. |
| `GrammarRuleId` | Required identifier of the parent rule. |
| `ExampleEn` | Required English example sentence. |
| `TranslationTh` | Optional Thai translation of the example. |
| `ExplanationTh` | Optional Thai-language explanation of the example. |
| `IsCorrectExample` | Indicates whether the example shows correct usage (`true`) or a common mistake (`false`). |
| `SortOrder` | Display order among a rule's examples. Must be zero or greater. |
| `IsActive` | Indicates whether the example is active. Deletes deactivate an example. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## GrammarRule Relationship

A rule can have many examples.

- `GrammarRuleId` is required and cannot be empty.
- The parent grammar rule must exist and be active before a new example can be added (`AddGrammarExampleCommandHandler`).
- EF Core maps the relationship from `GrammarRule` to many `GrammarExample` records with a cascading foreign key.
- `GrammarRule` does not expose an `AddExample`/`RemoveExample` aggregate method — examples are created and managed directly against their own repository, unlike the Rule-to-Word relationship.

## ExampleEn And IsCorrectExample Usage

`ExampleEn` is the required sentence shown to learners and is the only mandatory field besides the parent rule reference. `IsCorrectExample` flags whether the sentence demonstrates correct usage or illustrates a common mistake, letting the UI present "do this" and "avoid this" examples side by side for the same rule.

## Domain Rules

- `GrammarRuleId` is required and cannot be an empty GUID.
- `ExampleEn` is required, trimmed, and limited to 1000 characters.
- `TranslationTh` is optional, trimmed, and limited to 1000 characters.
- `ExplanationTh` is optional, trimmed, and limited to 2000 characters.
- `SortOrder` must be greater than or equal to zero.
- `Activate` and `Deactivate` behavior is handled on the `GrammarExample` entity.
- `GrammarExample` has no `Slug` — examples are not independently browsable or routable, unlike `GrammarTopic` and `GrammarRule`.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `AddGrammarExample`
- `UpdateGrammarExample`
- `DeleteGrammarExample`
- `GetGrammarExampleById`
- `GetGrammarExamplesByRuleId`

The Application layer validates input, validates that the parent rule exists and is active, uses cancellation tokens, and returns contract DTOs from `EnglishMaster.Contracts.GrammarExamples` rather than EF entities.

## API Endpoints

| Method | Route | Purpose |
| --- | --- | --- |
| `GET` | `/api/v1/grammar-rules/{grammarRuleId}/examples` | List examples for a rule. |
| `POST` | `/api/v1/grammar-rules/{grammarRuleId}/examples` | Add an example to a rule. |
| `GET` | `/api/v1/grammar-examples/{id}` | Get one example by id. |
| `PUT` | `/api/v1/grammar-examples/{id}` | Update an example. |
| `DELETE` | `/api/v1/grammar-examples/{id}` | Deactivate an example. |

Full request/response detail and status codes are documented in [Grammar Example API](../api/grammar-example-api.md).

## Request Example

```json
{
  "exampleEn": "She walks to school.",
  "translationTh": "เธอเดินไปโรงเรียน",
  "explanationTh": "Add s after she.",
  "isCorrectExample": true,
  "sortOrder": 0
}
```

## Response Example

```json
{
  "id": "66666666-6666-6666-6666-666666666666",
  "grammarRuleId": "55555555-5555-5555-5555-555555555555",
  "exampleEn": "She walks to school.",
  "translationTh": "เธอเดินไปโรงเรียน",
  "explanationTh": "Add s after she.",
  "isCorrectExample": true,
  "sortOrder": 0,
  "isActive": true,
  "createdAt": "2026-07-10T00:00:00+00:00",
  "updatedAt": "2026-07-10T00:00:00+00:00"
}
```

## Blazor UI

Grammar Example UI is embedded under the Grammar Rule detail page (`/grammar-rules/{id}`), using the shared `GrammarExampleForm.razor` component:

- List examples for the rule.
- Add a new example.
- Edit an existing example in place, without navigating away.
- Delete (deactivate) an example.
- Show loading, empty, error, and validation states through the parent page and form.

There are no standalone Grammar Example pages or routes.

## Test Coverage

Current tests cover:

- Grammar example creation and invariant behavior, including normalization of text fields
- Required-`ExampleEn` validation
- `AddGrammarExample` happy path when the parent rule is active
- API add/list/update flow for examples through the full HTTP pipeline

## Known Limitations

- There is no standalone search or pagination endpoint for grammar examples; they are only listed per rule.
- Delete deactivates the example instead of hard-deleting it.
- Grammar examples are managed only from the Grammar Rule detail UI.
- Endpoints are not authenticated yet.

## Next Recommended Module

See [Grammar Module](grammar-module.md#next-recommended-module).
