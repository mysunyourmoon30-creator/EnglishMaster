Create Better Public Search and Discovery for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Student-facing pages completed.
- Student Progress completed.
- Content Review Workflow completed.
- Advanced Import Validation completed.

Goal:
Improve public search and discovery for learners.

Important:
Do not add AI.
Do not add Elasticsearch.
Do not add external search services.
Do not add marketplace.
Do not add mobile.
Do not add payment.
Do not add microservices.
Do not redesign architecture.

Scope:
Improve public search using existing database and existing content.

Search Targets:
- Words
- Grammar Topics
- Grammar Rules
- Lessons
- Courses
- Books
- Quizzes

Search Features:
1. Unified public search page
2. Search by keyword
3. Filter by content type
4. Filter by CEFR level
5. Filter by category
6. Filter by tag if available
7. Show only public-safe content
8. Show only active and published content where applicable
9. Pagination
10. Sort by relevance if simple, otherwise sort by title/date

Public Routes:
- /search
- /search?q={query}
- /search?type=word
- /search?cefr=A1

Application Requirements:

Create:
- Features/PublicSearch/Queries
- Features/PublicSearch/Dtos

Use cases:
- SearchPublicContent
- GetPublicSearchFilters
- GetPublicSearchSuggestions if simple

PublicSearchResult DTO:
- ContentType
- Title
- Slug
- Summary
- CefrLevel
- CategoryName
- Tags
- Url
- HighlightText if simple
- UpdatedAt

Search Rules:
- Draft content must not appear.
- InReview content must not appear.
- ChangesRequested content must not appear.
- Archived content must not appear.
- Admin-only fields must not appear.
- Quiz correct answers must not appear.
- Internal storage paths must not appear.

Infrastructure Requirements:
- Use EF Core queries.
- Use AsNoTracking where appropriate.
- Avoid loading large related collections.
- Apply pagination.
- Add max page size.
- Avoid expensive Include chains.
- Use simple contains search for MVP.

API Requirements:
Create public endpoints:
- GET /api/v1/public/search
- GET /api/v1/public/search/filters
- GET /api/v1/public/search/suggestions if simple

Query parameters:
- q
- contentType
- cefrLevel
- categoryId
- tagId
- pageNumber
- pageSize
- sortBy
- sortDirection

Blazor Requirements:
Create public search page:
- /search

Page should include:
- Search box
- Content type filter
- CEFR filter
- Category filter if simple
- Tag filter if simple
- Results list
- Empty state
- Loading state
- Error state
- Pagination
- Link to each content detail page

Update public navigation:
- Add Search link
- Add search box in header if simple

Testing Requirements:
Add tests for:
- Search returns only published/active content.
- Draft content does not appear.
- Search by word text works.
- Search by grammar title works.
- Search by lesson title works.
- Filter by content type works.
- Filter by CEFR works.
- Pagination works.
- Quiz correct answers are not exposed.
- Existing modules still pass.

Quality:
- Keep implementation simple.
- Do not add external search engine.
- Do not overengineer.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Search query implementation
3. API endpoints created
4. Public search page created
5. Public navigation updates
6. Tests created
7. Build/test result
8. Remaining limitations