# Grammar Rule

## Purpose

`GrammarRule` explains one grammar pattern within a `GrammarTopic`. It holds the core rule text, bilingual explanations, a structure pattern, common-mistake guidance, correct-usage guidance, a set of `GrammarExample` records, and optional links to related vocabulary `Word` records.

## GrammarRule Fields

| Field | Description |
| --- | --- |
| `Id` | Unique rule identifier. |
| `GrammarTopicId` | Required identifier of the parent topic. |
| `Topic` | Topic summary (`Id`, `Title`, `Slug`, `CefrLevel`) returned by get/search operations. |
| `Title` | Required rule name. |
| `Slug` | URL-safe value generated from `Title`. Unique across all rules. |
| `RuleText` | Required core explanation of the grammar rule. |
| `ExplanationTh` | Optional Thai-language explanation. |
| `ExplanationEn` | Optional English-language explanation. |
| `StructurePattern` | Optional structure template, such as `S + V1`. |
| `CommonMistake` | Optional note describing a frequent learner mistake. |
| `CorrectUsageNote` | Optional note describing how to use the rule correctly. |
| `SortOrder` | Display order among rules. Must be zero or greater. |
| `Examples` | Collection of grammar examples that belong to this rule. |
| `RelatedWords` | Collection of related word summaries (`Id`, `Text`, `Slug`, `MeaningTh`). |
| `IsActive` | Indicates whether the rule is active. Deletes deactivate a rule. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Relationships

### GrammarRule And GrammarTopic

A rule belongs to exactly one topic.

- `GrammarTopicId` is required and cannot be an empty GUID.
- `CreateGrammarRule` and `UpdateGrammarRule` validate that the referenced topic exists and is active before saving.
- `GetGrammarRuleById` and search results include a `Topic` summary.

### GrammarRule And GrammarExample

A rule can have many examples.

- `GrammarExample.GrammarRuleId` is required and references its parent rule.
- Examples are created independently through their own endpoint (`POST /api/v1/grammar-rules/{grammarRuleId}/examples`), not through the `GrammarRule` aggregate itself.
- EF Core maps the relationship with a cascading foreign key.
- See [Grammar Example](grammar-example.md) for full detail.

### GrammarRule And Word

A rule can relate to many words, and a word can relate to many rules, through the `GrammarRuleWord` join entity.

- `GrammarRule.AddRelatedWord(wordId, now)` and `RemoveRelatedWord(wordId, now)` are aggregate methods on `GrammarRule` itself — this relationship, unlike Topic-to-Rule and Rule-to-Example, is managed directly on the rule.
- Adding a related word is idempotent: adding the same word twice does not create a duplicate relation.
- Removing a related word returns `false` when the relation does not exist, rather than throwing.
- The referenced word must exist and be active (`GrammarRuleReferenceValidator.ValidateWordAsync`).
- The database foreign key from `GrammarRuleWords.WordId` to `Words.Id` uses `Restrict` delete behavior, so a word referenced by a rule cannot be removed while the relation exists.
- See [Word Module — Word And Grammar Rule](word-module.md#word-and-grammar-rule) for the relationship from the Word side.

## RuleText Usage

`RuleText` is the required core explanation of the grammar rule and is the only mandatory descriptive field beyond `Title`. It should contain the primary explanation shown to learners; `ExplanationTh`/`ExplanationEn` can add further bilingual detail.

## StructurePattern Usage

`StructurePattern` is an optional short template that summarizes the grammatical structure, such as `S + V1` or `S + is/am/are + V-ing`. It gives learners a quick-reference formula alongside the fuller `RuleText` explanation.

## CommonMistake Usage

`CommonMistake` is an optional note describing an error learners frequently make with this rule, such as forgetting the third-person singular `-s`. It is intended to highlight pitfalls rather than restate the rule itself.

## CorrectUsageNote Usage

`CorrectUsageNote` is an optional note describing how to apply the rule correctly, often written as guidance that directly addresses the mistake described in `CommonMistake`.

## Domain Rules

- `GrammarTopicId` is required and cannot be an empty GUID.
- `Title` is required, trimmed, and limited to 200 characters.
- `RuleText` is required, trimmed, and limited to 4000 characters.
- `ExplanationTh` and `ExplanationEn` are optional, trimmed, and limited to 4000 characters each.
- `StructurePattern` is optional, trimmed, and limited to 1000 characters.
- `CommonMistake` and `CorrectUsageNote` are optional, trimmed, and limited to 2000 characters each.
- `Slug` is generated from `Title` and must contain at least one letter or digit.
- `Slug` is unique at the persistence layer (unique index) and checked in the Application layer via `SlugExistsAsync` before create and update.
- `SortOrder` must be greater than or equal to zero.
- `Activate` and `Deactivate` behavior is handled on the `GrammarRule` entity.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateGrammarRule`
- `UpdateGrammarRule`
- `DeleteGrammarRule`
- `GetGrammarRuleById`
- `SearchGrammarRules`
- `AddRelatedWordToGrammarRule`
- `RemoveRelatedWordFromGrammarRule`

The Application layer validates input, generates and checks slug uniqueness, validates that the parent topic and any referenced word exist and are active, uses cancellation tokens, and returns contract DTOs from `EnglishMaster.Contracts.GrammarRules` rather than EF entities.

## Search And Pagination

`SearchGrammarRules` supports:

- Search by `Title`, `Slug`, `RuleText`, and `StructurePattern`
- Filter by `GrammarTopicId`
- Filter by `IsActive` (defaults to `true`)
- Pagination with `PageNumber` (default `1`) and `PageSize` (default `20`, maximum `100`)

## Persistence

EF Core maps `GrammarRule` to the `GrammarRules` table and `GrammarRuleWord` to the `GrammarRuleWords` table. The configuration includes:

- Required fields and maximum lengths for `Title`, `Slug`, `RuleText`, `ExplanationTh`, `ExplanationEn`, `StructurePattern`, `CommonMistake`, and `CorrectUsageNote`
- Unique index on `GrammarRules.Slug` (added by the `MakeGrammarRuleSlugUnique` migration)
- Indexes on `GrammarTopicId`, `Title`, and `IsActive`
- A cascading foreign key from `GrammarExamples.GrammarRuleId` to `GrammarRules.Id`
- A cascading foreign key from `GrammarRuleWords.GrammarRuleId` to `GrammarRules.Id`
- A composite primary key on `GrammarRuleWords` (`GrammarRuleId`, `WordId`) with a `Restrict` foreign key from `GrammarRuleWords.WordId` to `Words.Id`

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Grammar rule list | `/grammar-rules` | Search, filter by topic (`?topicId=`) and active status, paginate, view, edit, and delete rules. |
| Create grammar rule | `/grammar-rules/new` | Create a new rule, optionally pre-selecting a topic via `?topicId=`. |
| Grammar rule detail | `/grammar-rules/{id}` | View rule details; manage related words (add/remove); manage grammar examples inline (add, edit-in-place, delete) without leaving the page. |
| Edit grammar rule | `/grammar-rules/{id}/edit` | Update rule data and active status. |

## Test Coverage

Current tests cover:

- Grammar rule creation and invariant behavior, including normalization of all text fields
- Required-`RuleText` validation
- `AddRelatedWord` duplicate prevention
- `RemoveRelatedWord` returning `false` when the relation does not exist
- `CreateGrammarRule` happy path under an active topic
- `AddRelatedWordToGrammarRule` happy path
- `SearchGrammarRules` filtering by topic
- `CreateGrammarRule` validation error when `RuleText` is missing
- `CreateGrammarRule` duplicate-slug validation
- API create/search/get/update/delete flow, related-word add/remove, and example add/list/update through the full HTTP pipeline

## Known Limitations

- Grammar rule endpoints and pages are not protected by authentication or authorization yet.
- Delete deactivates a rule instead of hard-deleting it.
- The duplicate-title validation error uses the field name `Title` (PascalCase) while other field-level validation errors use camelCase names such as `ruleText` — a minor inconsistency to be aware of when handling error responses.
- Search uses basic database `Contains` matching rather than full-text search.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

See [Grammar Module](grammar-module.md#next-recommended-module).
