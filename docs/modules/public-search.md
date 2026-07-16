# Public Search

## Purpose

Public Search gives learners one read-only place to discover published EnglishMaster content without signing in. It uses existing database content and simple EF Core queries. It intentionally does not use AI, Elasticsearch, or external search services.

## Searchable Content Types

- Words
- Grammar topics
- Grammar rules
- Lessons
- Courses
- Books
- Quizzes

## Public Route

```text
/search
/search?q={query}
/search?type=word
/search?cefr=A1
```

## Visibility Rules

Search results must be public-safe:

- Words and grammar results must be active.
- Lessons, courses, books, and quizzes must be active and published.
- Draft, review, changes-requested, archived, inactive, or unpublished content must not appear.
- Quiz answer keys and correct-answer flags must not appear.
- Internal storage paths and admin-only metadata must not appear.

## Filters

Supported filters:

- Keyword query.
- Content type.
- CEFR level.
- Category.
- Tag where available.

Tag filtering currently applies to Word results.

## Pagination

The API caps page size at 50. The UI uses page size 10 and shows previous/next controls.

## Performance Notes

The MVP uses bounded per-type EF Core `AsNoTracking` queries and combines the results for cross-type search. This is sufficient for small v0.2.0 datasets. Larger content libraries should add indexing, better ranking, and query-specific database tuning before increasing limits.

## Why No External Search Engine Yet

External search is intentionally deferred to avoid extra infrastructure, secrets, indexing pipelines, monitoring, and operational failure modes before the product needs them.

## Known Limitations

- Relevance is simple contains/date/title sorting.
- Cross-type ranking is basic.
- Suggestions currently use active Words.
- No typo tolerance, stemming, synonyms, or phrase ranking.
- Tag filtering is Word-first.

