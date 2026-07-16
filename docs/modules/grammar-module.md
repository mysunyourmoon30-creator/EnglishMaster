# Grammar Module

## Purpose

The Grammar Module manages grammar learning content for the English learning platform. A `GrammarTopic` groups related grammar rules by theme and CEFR level. A `GrammarRule` explains one grammar pattern within a topic, with structure guidance, common-mistake notes, and correct-usage guidance. A `GrammarExample` demonstrates a rule with a correct or incorrect usage sentence. A `GrammarRule` can also link to related vocabulary from the Word Module.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Grammar`
- Application: `EnglishMaster.Application.Features.GrammarTopics`, `EnglishMaster.Application.Features.GrammarRules`, `EnglishMaster.Application.Features.GrammarExamples`
- Infrastructure: `EnglishMaster.Infrastructure.Grammar`
- API: `EnglishMaster.Api.Endpoints.GrammarEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.GrammarTopics`, `EnglishMaster.Web.Components.Pages.GrammarRules`, shared form components under `EnglishMaster.Web.Components.Grammar`

## Entity Relationship Overview

- `GrammarTopic` has many `GrammarRule` (one-to-many, `GrammarRules.GrammarTopicId`).
- `GrammarRule` has many `GrammarExample` (one-to-many, `GrammarExamples.GrammarRuleId`).
- `GrammarRule` can relate to many `Word` records (many-to-many through the `GrammarRuleWord` join entity).
- Deleting a topic or rule cascades at the database level, but the Application layer only ever soft-deletes (`Deactivate`) through the API, so cascades act as a safety net rather than a normal code path.
- The `GrammarRuleWord` foreign key to `Words` uses `Restrict` delete behavior, so a word referenced by a grammar rule cannot be removed underneath it.

See the individual entity docs for full field lists, domain rules, and API details:

- [Grammar Topic](grammar-topic.md) / [Grammar Topic API](../api/grammar-topic-api.md)
- [Grammar Rule](grammar-rule.md) / [Grammar Rule API](../api/grammar-rule-api.md)
- [Grammar Example](grammar-example.md) / [Grammar Example API](../api/grammar-example-api.md)

## CEFR Usage

CEFR classification lives only on `GrammarTopic.CefrLevel`, reusing the shared `EnglishMaster.Domain.Words.CefrLevel` enum (`A1, A2, B1, B2, C1, C2`). `GrammarRule` and `GrammarExample` do not carry their own CEFR level — a rule and its examples inherit their level from the topic they belong to. Topic search supports filtering by `CefrLevel`.

## Test Coverage

- `tests/EnglishMaster.UnitTests/Grammar/GrammarTopicTests.cs`
- `tests/EnglishMaster.UnitTests/Grammar/GrammarRuleTests.cs`
- `tests/EnglishMaster.UnitTests/Grammar/GrammarExampleTests.cs`
- `tests/EnglishMaster.UnitTests/Grammar/GrammarUseCaseTests.cs` (Application-layer handler tests using fake repositories)
- `tests/EnglishMaster.IntegrationTests/Grammar/GrammarEndpointsTests.cs` (end-to-end HTTP flow: topic → rule → related word → example → soft delete)
- `tests/EnglishMaster.ArchitectureTests` project reference rules also cover Grammar's layer boundaries.

## Known Limitations

- Grammar admin endpoints and pages are not protected by authentication or authorization yet.
- Delete deactivates a topic, rule, or example instead of hard-deleting it.
- There is no CEFR override at the rule or example level; it is inherited from the parent topic only.
- Search uses basic database `Contains` matching rather than full-text search.
- `GrammarRule.Slug` uniqueness was only recently added and required a dedicated migration (`MakeGrammarRuleSlugUnique`) after the initial Grammar Module rollout.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
