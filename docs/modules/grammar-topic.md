# Grammar Topic

## Purpose

`GrammarTopic` is the top-level grammar concept in the Grammar Module. It groups related grammar rules by theme (for example, "Present Simple") and classifies them by CEFR level so learners can browse grammar content by difficulty.

## GrammarTopic Fields

| Field | Description |
| --- | --- |
| `Id` | Unique topic identifier. |
| `Title` | Required topic name. |
| `Slug` | URL-safe value generated from `Title`. Unique across all topics. |
| `Summary` | Optional short description of the topic. |
| `CefrLevel` | CEFR level: `A1`, `A2`, `B1`, `B2`, `C1`, or `C2`. |
| `SortOrder` | Display order among topics. Must be zero or greater. |
| `Rules` | Collection of grammar rules that belong to this topic. |
| `IsActive` | Indicates whether the topic is active. Deletes deactivate a topic. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Relationships

### GrammarTopic And GrammarRule

A topic can have many grammar rules.

- `GrammarRule.GrammarTopicId` is required and references its parent topic.
- `CreateGrammarRule` and `UpdateGrammarRule` validate that the referenced topic exists and is active.
- `GetGrammarRulesByTopicId` (`GET /api/v1/grammar-topics/{topicId}/rules`) returns the rules for a topic.
- EF Core maps `GrammarTopic.Rules` as a one-to-many relationship with a cascading foreign key, but rules are only ever soft-deleted through the API, not physically removed with their topic.

## CEFR Usage

`CefrLevel` classifies a topic's difficulty using the shared `EnglishMaster.Domain.Words.CefrLevel` enum (`A1, A2, B1, B2, C1, C2`), the same enum used by the Word Module. It is stored as a string column (`HasConversion<string>()`) and is required on create and update. `SearchGrammarTopics` supports filtering by `CefrLevel` so learners or admins can browse topics by difficulty. Rules and examples under a topic do not have their own CEFR level — they inherit the topic's classification.

## Domain Rules

- `Title` is required, trimmed, and limited to 200 characters.
- `Summary` is optional, trimmed, and limited to 1000 characters; stored as an empty string when not provided.
- `Slug` is generated from `Title` using the shared domain slug generator and must contain at least one letter or digit.
- `Slug` is unique at the persistence layer (unique index) and checked in the Application layer via `SlugExistsAsync` before create and update.
- `CefrLevel` must parse to a defined enum value; an invalid or missing value is rejected.
- `SortOrder` must be greater than or equal to zero.
- `Activate` and `Deactivate` behavior is handled on the `GrammarTopic` entity.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateGrammarTopic`
- `UpdateGrammarTopic`
- `DeleteGrammarTopic`
- `GetGrammarTopicById`
- `SearchGrammarTopics`
- `GetGrammarRulesByTopicId`

The Application layer validates input, generates and checks slug uniqueness, uses cancellation tokens, and returns contract DTOs from `EnglishMaster.Contracts.GrammarTopics` rather than EF entities.

## Search And Pagination

`SearchGrammarTopics` supports:

- Search by `Title`, `Slug`, and `Summary`
- Filter by `CefrLevel`
- Filter by `IsActive` (defaults to `true`)
- Pagination with `PageNumber` (default `1`) and `PageSize` (default `20`, maximum `100`)

Default behavior returns active topics.

## Persistence

EF Core maps `GrammarTopic` to the `GrammarTopics` table. The configuration includes:

- Required fields and maximum lengths (`Title` 200, `Slug` 220, `Summary` 1000)
- String enum conversion for `CefrLevel`
- Unique index on `GrammarTopics.Slug`
- Indexes on `Title`, `CefrLevel`, and `IsActive`
- A cascading foreign key from `GrammarRules.GrammarTopicId` to `GrammarTopics.Id`

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Grammar topic list | `/grammar-topics` | Search, filter by CEFR level and active status, paginate, view, edit, and delete topics. |
| Create grammar topic | `/grammar-topics/new` | Create a new topic with title, CEFR level, sort order, and summary. |
| Grammar topic detail | `/grammar-topics/{id}` | View topic details and the table of grammar rules that belong to it. |
| Edit grammar topic | `/grammar-topics/{id}/edit` | Update topic data and active status. |

## Test Coverage

Current tests cover:

- Grammar topic creation and invariant behavior, including normalization of `Title` and `Summary`
- Slug generation from titles containing spaces, underscores, and punctuation
- Required-title validation
- Activate and deactivate behavior
- `CreateGrammarTopic` happy path and slug assignment
- `SearchGrammarTopics` filtering by CEFR level
- `CreateGrammarTopic` duplicate-slug validation
- API create/search/get flow through the full HTTP pipeline

## Known Limitations

- Grammar topic endpoints and pages are not protected by authentication or authorization yet.
- Delete deactivates a topic instead of hard-deleting it.
- Search uses basic database `Contains` matching rather than full-text search.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

See [Grammar Module](grammar-module.md#next-recommended-module).
