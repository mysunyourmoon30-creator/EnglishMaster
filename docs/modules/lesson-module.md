# Lesson Module

## Purpose

The Lesson Module combines Words, Grammar Rules, Category, and Media into structured, ordered lessons. A `Lesson` is the top-level container — it optionally belongs to a `Category`, optionally has a thumbnail image, and holds an ordered set of `LessonSection` content blocks. A `Lesson` can also relate to many `Word` records and many `GrammarRule` records, letting a lesson bundle the vocabulary and grammar it teaches.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Lessons`
- Application: `EnglishMaster.Application.Features.Lessons`, `EnglishMaster.Application.Features.LessonSections`
- Infrastructure: `EnglishMaster.Infrastructure.Lessons`
- API: `EnglishMaster.Api.Endpoints.LessonEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Lessons`, shared form components under `EnglishMaster.Web.Components.Lessons`

## Entity Relationship Overview

- `Lesson` has many `LessonSection` (one-to-many, `LessonSections.LessonId`, cascade delete).
- `Lesson` can relate to many `Word` records (many-to-many through the `LessonWord` join entity, mirroring `GrammarRuleWord`).
- `Lesson` can relate to many `GrammarRule` records (many-to-many through the `LessonGrammarRule` join entity, same pattern).
- `Lesson` optionally belongs to one `Category` (`CategoryId`, `SetNull` on delete).
- `Lesson` optionally has one thumbnail `Media` item (`ThumbnailMediaId`, `SetNull` on delete, must be an active `MediaType.Image`).
- `LessonSection` optionally references one `Media` item (`MediaId`, `SetNull` on delete, no media-type constraint).
- The `LessonWord`/`LessonGrammarRule` foreign keys to `Words`/`GrammarRules` use `Restrict` delete behavior, so a word or grammar rule referenced by a lesson cannot be removed while the relation exists — the same protective pattern used by `GrammarRuleWord`.

See the individual entity docs for full field lists, domain rules, and API details:

- [Lesson Section](lesson-section.md) / [Lesson Section API](../api/lesson-section-api.md)
- [Lesson API](../api/lesson-api.md)

## Publish/Unpublish

`Lesson` has two fully independent lifecycle flags: `IsPublished` and `IsActive`. `Publish`/`Unpublish` and `Activate`/`Deactivate` are four separate methods on the entity — publishing does not imply active, and deactivating does not imply unpublished. A lesson can be active-but-draft (being edited, not yet shown to learners) or inactive-but-published (soft-deleted after being live). This is a new pattern in the codebase, modeled directly on the existing `Activate`/`Deactivate` shape (a bool plus `UpdatedAt`, no separate `PublishedAt` timestamp).

## CEFR Usage

`CefrLevel` is optional on `Lesson` (unlike `GrammarTopic.CefrLevel`, which is required), reusing the shared `EnglishMaster.Domain.Words.CefrLevel` enum (`A1, A2, B1, B2, C1, C2`). `LessonSection` has no CEFR field of its own — a section's difficulty is implied by its parent lesson.

## Test Coverage

- `tests/EnglishMaster.UnitTests/Lessons/LessonTests.cs` (entity invariants, slug generation, Activate/Deactivate, Publish/Unpublish, AddWord/RemoveWord, AddGrammarRule/RemoveGrammarRule)
- `tests/EnglishMaster.UnitTests/Lessons/LessonSectionTests.cs` (entity invariants, required Title, Reorder)
- `tests/EnglishMaster.UnitTests/Lessons/LessonUseCaseTests.cs` (Application-layer handler tests using fake repositories: create, duplicate-slug validation, add word/grammar rule including negative-`SortOrder` validation, search by CEFR, search by published status, add section)
- `tests/EnglishMaster.IntegrationTests/Lessons/LessonEndpointsTests.cs` (end-to-end HTTP flow: create → search → add word → add grammar rule → add/reorder/update sections → publish/unpublish → update → remove relations → soft delete; plus validation-problem tests for missing and duplicate titles)
- `tests/EnglishMaster.ArchitectureTests` project reference rules also cover the Lesson Module's layer boundaries.

## Known Limitations

- Lesson admin endpoints and pages are not protected by authentication or authorization yet.
- Delete deactivates a lesson or section instead of hard-deleting it.
- There is no CEFR override at the section level; it is inherited from the parent lesson only.
- Search uses basic database `Contains` matching rather than full-text search.
- `ActivateLesson`/`DeactivateLesson` use cases exist in the Application layer but have no API route — only `publish`/`unpublish` are routed, mirroring the same already-documented gap for Grammar Topic/Rule/Pronunciation's Activate/Deactivate handlers.
- `GetLessonSectionById` exists in the Application layer but has no API route (see [Lesson Section](lesson-section.md#known-limitations)).
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
