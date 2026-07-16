# Word Module

## Purpose

The Word Module manages vocabulary words for the English learning platform. It is the core vocabulary slice that Category and Tag organize, Media enriches with optional image and audio assets, and Pronunciation extends with IPA, articulation guidance, audio practice, mouth imagery, and minimal pairs.

The module follows the existing Clean Architecture layout:

- Domain: `EnglishMaster.Domain.Words`
- Application: `EnglishMaster.Application.Features.Words`
- Infrastructure: `EnglishMaster.Infrastructure.Words`
- API: `EnglishMaster.Api.Endpoints.WordEndpoints`
- Blazor: `EnglishMaster.Web.Components.Pages.Words`

## Word Fields

| Field | Description |
| --- | --- |
| `Id` | Unique word identifier. |
| `Text` | Required English word or phrase. |
| `Slug` | URL-safe value generated from `Text`. |
| `IpaUk` | Optional UK IPA pronunciation. |
| `IpaUs` | Optional US IPA pronunciation. |
| `ThaiReading` | Optional Thai reading aid. |
| `MeaningTh` | Required Thai meaning. |
| `MeaningEn` | Optional English meaning. |
| `PartOfSpeech` | Word type: `Noun`, `Pronoun`, `Verb`, `Adjective`, `Adverb`, `Preposition`, `Conjunction`, `Interjection`, `Determiner`, `Phrase`, or `Other`. |
| `CefrLevel` | CEFR level: `A1`, `A2`, `B1`, `B2`, `C1`, or `C2`. |
| `ExampleEn` | Optional English example sentence. |
| `ExampleTh` | Optional Thai example sentence. |
| `CategoryId` | Optional category identifier. |
| `Tags` | Collection of assigned tag summaries. |
| `ImageMediaId` | Optional image media identifier. |
| `ImageMedia` | Optional image media summary returned by get/search operations. |
| `AudioMediaId` | Optional audio media identifier. |
| `AudioMedia` | Optional audio media summary returned by get/search operations. |
| `Pronunciation` | Optional pronunciation summary returned by word detail operations. |
| `IsActive` | Indicates whether the word is active. Deletes deactivate a word. |
| `CreatedAt` | Creation timestamp. |
| `UpdatedAt` | Last update timestamp. |

## Relationships

### Word And Category

A word can have zero or one category through `CategoryId`.

- `CreateWord` accepts `CategoryId`.
- `UpdateWord` accepts `CategoryId`.
- `GetWordById` returns `Category` summary data when assigned.
- `SearchWords` supports filtering by `CategoryId`.
- Only active categories can be assigned to a word.

### Word And Tag

A word can have many tags through the `WordTags` join table.

- `CreateWord` accepts `TagIds`.
- `UpdateWord` accepts `TagIds`.
- `GetWordById` returns assigned tag summaries.
- `SearchWords` supports filtering by `TagId`.
- Only active tags can be assigned to a word.

### Word And Media

A word can have zero or one image media item and zero or one audio media item.

- `CreateWord` accepts `ImageMediaId` and `AudioMediaId`.
- `UpdateWord` accepts `ImageMediaId` and `AudioMediaId`.
- `GetWordById` returns image and audio media summaries when assigned.
- `SearchWords` includes media summaries for the current page.
- Only active image media can be assigned to `ImageMediaId`.
- Only active audio media can be assigned to `AudioMediaId`.

Media records are reusable. The same image or audio item can be referenced by multiple words.

### Word And Pronunciation

A word can have zero or one pronunciation record through the Pronunciation Module.

- `Pronunciation.WordId` is required.
- EF Core enforces a unique index on `Pronunciations.WordId`.
- `GetWordById` returns `Pronunciation` summary data when available.
- The word pronunciation summary includes IPA UK, IPA US, Thai reading, syllable and stress notes, mouth and tongue guidance, audio media summaries, mouth image media summary, and active minimal pairs.
- `SearchWords` does not include pronunciation data so list queries stay lightweight.
- Pronunciation create/edit flows live in the Pronunciation Module and are linked from Word detail and Word edit pages.

### Word And Grammar Rule

A word can be related to many grammar rules, and a grammar rule can be related to many words, through the `GrammarRuleWords` join table.

- Only active words can be linked to a grammar rule (`GrammarRuleReferenceValidator.ValidateWordAsync`).
- The database foreign key from `GrammarRuleWords.WordId` to `Words.Id` uses `Restrict` delete behavior, so a word referenced by a grammar rule cannot be removed while the relation exists.
- `GetWordById` and `SearchWords` do not include grammar rule links; the relationship stays lightweight from the Word side.
- Related-word management happens through the Grammar Rule detail page and the Grammar Rule API, not from Word pages. See [Grammar Rule](grammar-rule.md#grammarrule-and-word).

## Domain Rules

- `Text` is required and trimmed.
- `MeaningTh` is required and trimmed.
- `Slug` is generated from `Text` using the shared domain slug generator.
- `Slug` must contain at least one letter or digit.
- `Slug` is unique at the persistence layer and checked in the Application layer before create/update.
- Optional text fields are trimmed and stored as empty strings when not provided.
- `CategoryId` is optional but cannot be an empty GUID when provided.
- `TagIds` cannot contain empty GUID values and duplicate tag ids are removed.
- `ImageMediaId` and `AudioMediaId` are optional but cannot be empty GUIDs when provided.
- `Activate` and `Deactivate` behavior is handled on the `Word` entity.
- Domain code stays independent from EF Core, ASP.NET Core, Blazor, and infrastructure concerns.

## Application Use Cases

- `CreateWord`
- `UpdateWord`
- `DeleteWord`
- `GetWordById`
- `SearchWords`

The Application layer validates input, parses enum values, checks slug uniqueness, validates active Category, Tag, and Media references, uses cancellation tokens, and returns contract DTOs rather than EF entities. Word detail also maps a pronunciation summary through the Application layer when a pronunciation exists.

`IWordRepository` also exposes `GetByIdsAsync` for batched lookups by a set of ids. It is used by other modules that need to resolve several related words at once — for example, the Grammar Rule read model batches related-word lookups through this method instead of looking up each word individually.

## Search And Pagination

`SearchWords` supports:

- Search by `Text`, `Slug`, `MeaningTh`, and `MeaningEn`
- Filter by `CefrLevel`
- Filter by `PartOfSpeech`
- Filter by `IsActive`
- Filter by `CategoryId`
- Filter by `TagId`
- Sort by `Text`
- Sort by `CreatedAt`
- Pagination with `PageNumber` and `PageSize`

Default behavior returns active words, sorted by text ascending.

## Persistence

EF Core maps `Word` to the `Words` table and `WordTag` to the `WordTags` table. The SQL Server configuration includes:

- Required fields and maximum lengths
- String enum conversion for `PartOfSpeech` and `CefrLevel`
- Unique index on `Word.Slug`
- Indexes on `Text`, `Slug`, `CefrLevel`, `IsActive`, and `CategoryId`
- Optional foreign key from `Words.CategoryId` to `Categories.Id`
- Optional foreign keys from `Words.ImageMediaId` and `Words.AudioMediaId` to `Media.Id`
- Indexes on `ImageMediaId` and `AudioMediaId`
- `SetNull` delete behavior for Category and Media relationships
- Composite key on `WordTags` with `WordId` and `TagId`
- Index on `WordTags.TagId`

The SQL Server connection string is read from `ConnectionStrings:DefaultConnection` in the API configuration.

## Blazor Pages

| Page | Route | Purpose |
| --- | --- | --- |
| Word list | `/words` | Search, filter, sort, paginate, view, edit, and delete words. |
| Create word | `/words/new` | Create a new word with validation, category, tag, image, and audio selection. |
| Word detail | `/words/{id}` | View word details, category, tags, image preview, audio player, pronunciation summary, pronunciation media, and minimal pairs when assigned. |
| Edit word | `/words/{id}/edit` | Update word data, category, tags, image, audio, active status, and link to create/edit pronunciation. |

The list page includes search, CEFR filter, part-of-speech filter, active filter, category filter, tag filter, media indicators, loading state, empty state, error state, and pagination controls. The create/edit form shows validation messages and loads active image and audio media choices. Word detail safely shows pronunciation data only when the API returns it.

## Test Coverage

Current tests cover:

- Word entity creation and invariant behavior
- Slug generation
- Word with optional category
- Word with tags and duplicate tag removal
- Word with optional image and audio media
- CreateWord validation and duplicate slug handling
- CreateWord with Category and Tags
- CreateWord with Image and Audio Media
- UpdateWord behavior and duplicate slug handling
- UpdateWord Category and Tag changes
- UpdateWord Image and Audio Media changes
- SearchWords default active filtering
- Search pagination and text sorting
- CEFR filtering
- Active/inactive filtering
- Search by `CategoryId`
- Search by `TagId`
- Search result mapping for image and audio media summaries
- API create/get/search/update/delete/deactivate flow
- API validation responses
- API search by meaning fields
- API Word relationship behavior with Category and Tags
- API Word relationship behavior with Image and Audio Media
- Word detail with Pronunciation summary
- Word pronunciation summary hides inactive minimal pairs
- Architecture project reference rules

## Known Limitations

- Word admin endpoints and pages are not protected by authentication or authorization yet.
- Delete currently deactivates a word instead of hard-deleting it.
- Search uses basic database `Contains` matching rather than full-text search.
- Word list filtering supports one category and one tag at a time.
- Word list does not filter by media assignment yet.
- Word list does not include pronunciation data; use Word detail or Pronunciation search for that data.
- Assigned media files are managed by the Media Module and are not deleted when a word is deactivated.
- Pronunciation data is managed by the Pronunciation Module and is not edited directly from the Word form.
- No audit user is recorded for create/update/delete actions.

## Next Recommended Module

Student Progress is the next recommended module after admin security.
