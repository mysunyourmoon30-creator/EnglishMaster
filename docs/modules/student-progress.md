# Student Progress Module

## Scope

V1-F06 adds a read-only, authenticated learner overview for the existing
`LessonProgress`, `CourseProgress`, and `BookProgress` records.

## Routes

- API: `GET /api/v1/me/progress?limit=20`
- Web: `/learn/progress`

The API derives ownership from the authenticated name-identifier claim, clamps
the per-content limit to 1–50, and returns only active, published content. Each
collection is ordered by most recent access with stable tie-breakers.

## UI States

The page provides loading, error, empty, and populated states. Populated results
show total, in-progress, and completed counts plus separate lesson, course, and
book cards with progress, status, last access, and a learner-safe content link.

## Security And Privacy

- Progress is filtered by the authenticated user before projection.
- User IDs and internal authoring metadata are not returned.
- Draft and inactive content is excluded.
- The endpoint requires authorization.

## Known Limitation

This first slice displays existing progress records. It does not create or
update progress, enroll a learner, infer completion, or issue certificates.

## Work Item Review Gate

```text
Applicable AI modules:
- Required: 5 API-First, 8 Structured Outputs
- Conditional: 24 Reliability at release-level verification
- Not applicable: 1-4, 6-7, 9-23 because this feature has no LLM, RAG,
  vector database, model tool, agent workflow, or external automation

Review roles:
- English Teacher: Not applicable — no English lesson content or assessment
  behavior changes
- UX/UI Designer: Required — reviewed loading, error, empty, desktop, and
  mobile states with no horizontal overflow
- AI Engineer: Not applicable — no AI inference path
- Developer: Required — API, application query, EF projection, web client,
  Blazor page, and focused tests reviewed
- Security/System: Required — authorization metadata, claim-derived ownership,
  cross-user isolation, and draft/inactive filtering verified
```

## Verification — 2026-08-04

- `dotnet build EnglishMaster.sln --no-restore`: passed with 0 warnings and
  0 errors.
- Focused Student Progress integration tests: 5/5 passed.
- Architecture tests: 7/7 passed.
- Unit tests: 220/220 passed.
- Non-LocalDB integration tests: 213/213 passed.
- Native no-Docker browser check passed at 1280×720 and 390×844 with no
  horizontal overflow and no browser console warnings or errors.
- The full local suite reached the existing fresh-database migration test, but
  this machine could not create a SQL Server LocalDB automatic instance. The
  GitHub Actions Windows runner remains the final SQL/LocalDB verification gate.
