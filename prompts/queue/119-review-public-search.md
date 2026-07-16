Review and harden Better Public Search and Discovery.

Project:
EnglishMaster

Goal:
Make public search safe, fast enough for MVP, buildable, and consistent.

Check and fix:

1. Clean Architecture
- Search logic must not be inside API controllers.
- Blazor pages must not access DbContext directly.
- Application layer owns public search use cases.
- Infrastructure handles EF Core only.

2. Public Content Safety
Verify:
- Draft content does not appear.
- InReview content does not appear.
- ChangesRequested content does not appear.
- Archived content does not appear.
- Only active/published content appears where applicable.
- Admin-only fields are not exposed.
- Quiz correct answers are not exposed.
- Internal file paths are not exposed.

3. Query Performance
Verify:
- AsNoTracking is used where appropriate.
- Pagination exists.
- Max page size exists.
- No unbounded result sets.
- No unnecessary Include chains.
- Search handles empty query safely.
- Search handles special characters safely.

4. API
Verify:
- GET /api/v1/public/search works.
- GET /api/v1/public/search/filters works.
- Suggestions endpoint works if implemented.
- Query parameters validate safely.
- Internal exceptions are not leaked.

5. Blazor
Verify:
- /search works.
- Search box works.
- Filters work.
- Pagination works.
- Result links work.
- Empty state works.
- Loading state works.
- Error state works.
- Public navigation link works.

6. Tests
Verify:
- Public search tests pass.
- Public content safety tests pass.
- Existing admin/public/student modules still pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add Elasticsearch.
Do not add external services.

Output:
1. Problems found
2. Fixes applied
3. Public content safety result
4. Query performance result
5. Build/test result
6. Remaining risks