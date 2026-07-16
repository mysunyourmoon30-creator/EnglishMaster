Review and harden Student-facing Learning Pages v0.2.0.

Project:
EnglishMaster

Goal:
Make public/student-facing pages safe, clean, buildable, and consistent.

Check and fix:

1. Clean Architecture
- Public API controllers/endpoints must not contain business logic.
- Blazor pages must not access DbContext directly.
- Public queries should go through Application layer.
- Public DTOs should be separate from admin DTOs if needed.

2. Public Content Rules
Verify:
- Draft content is not shown.
- InReview content is not shown.
- ChangesRequested content is not shown.
- Archived content is not shown.
- Only Published and Active content is shown where applicable.
- Approved but not Published behavior is consistent and documented.

3. Public API Safety
Verify:
- No internal IDs are exposed unnecessarily.
- No password/security fields are exposed.
- No internal file paths are exposed.
- No quiz correct answers are exposed before submit.
- List endpoints use pagination.
- Max page size exists.

4. Public Routes
Verify these routes work:
- /
- /learn
- /learn/courses
- /learn/courses/{slug}
- /learn/lessons
- /learn/lessons/{slug}
- /dictionary
- /dictionary/words
- /dictionary/words/{slug}
- /grammar
- /grammar/topics/{slug}
- /grammar/rules/{slug}
- /books
- /books/{slug}
- /quizzes
- /quizzes/{slug}

5. UX
Verify:
- Public navigation works.
- Pages are readable.
- Loading states exist.
- Empty states exist.
- Error states exist.
- Media renders safely.
- Audio player works if media exists.
- Broken slugs show safe not found behavior.

6. Security
Verify:
- Admin pages remain protected.
- Public pages do not bypass admin authorization.
- Public API does not expose admin-only data.
- File/media URLs are safe.

7. Tests
Verify:
- Public endpoint tests pass.
- Existing admin tests pass.
- Auth tests pass.
- Publishing and review workflow tests pass.

Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Do not add AI.
Do not add student progress.
Do not add new business modules.

Output:
1. Problems found
2. Fixes applied
3. Public content safety result
4. Public API safety result
5. UX result
6. Build/test result
7. Remaining risks