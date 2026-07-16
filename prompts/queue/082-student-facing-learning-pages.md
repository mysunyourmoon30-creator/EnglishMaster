Create Student-facing Learning Pages for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review and Approval Workflow completed.
- Better Publishing v0.2.0 completed.
- Admin system can manage content, review, approve, and publish.

Goal:
Create public/student-facing read-only learning pages using published content.

Important:
Do not add AI.
Do not add Mobile.
Do not add Marketplace.
Do not add Payment.
Do not add Microservices.
Do not add full student progress tracking yet.
Do not redesign the architecture.

Scope:
Create public learning pages only.

Student Pages Required:

1. Landing Page
Route:
- /

Content:
- Simple EnglishMaster landing page
- Links to Courses, Lessons, Dictionary, Grammar, Books, Quizzes

2. Learning Home
Route:
- /learn

Content:
- Featured courses
- Latest lessons
- Grammar topics
- Word dictionary shortcut

3. Course Pages
Routes:
- /learn/courses
- /learn/courses/{slug}

Requirements:
- Show only Published and Active courses.
- Course detail shows lessons in order.
- Show CEFR level, summary, estimated minutes, thumbnail if available.

4. Lesson Pages
Routes:
- /learn/lessons
- /learn/lessons/{slug}

Requirements:
- Show only Published and Active lessons.
- Lesson detail shows sections in order.
- Show related words.
- Show related grammar rules.
- Show media safely if available.

5. Word Dictionary Pages
Routes:
- /dictionary
- /dictionary/words
- /dictionary/words/{slug}

Requirements:
- Show only Active and Published or Approved words where workflow exists.
- Search by word text.
- Filter by CEFR if available.
- Word detail shows:
  - Text
  - Meaning TH
  - Meaning EN
  - IPA UK
  - IPA US
  - ThaiReading
  - Example EN
  - Example TH
  - Pronunciation audio if available
  - Image if available
  - Minimal pairs if available
  - Related grammar rules if available

6. Grammar Pages
Routes:
- /grammar
- /grammar/topics/{slug}
- /grammar/rules/{slug}

Requirements:
- Show only Active and Published or Approved grammar content.
- Grammar topic detail shows rules.
- Grammar rule detail shows examples and related words.

7. Book Pages
Routes:
- /books
- /books/{slug}

Requirements:
- Show only Published and Active books.
- Book detail shows chapters and lessons.
- Show cover image if available.

8. Quiz Pages
Routes:
- /quizzes
- /quizzes/{slug}

Requirements:
- Show only Published and Active quizzes.
- Do not expose correct answers directly in initial page data.
- If simple, allow local practice mode:
  - user selects answers
  - show result after submit
  - do not save progress yet
- If practice mode is too large, create read-only quiz preview and document limitation.

Public API Requirements:
Create safe read-only public endpoints if existing admin endpoints are not suitable.

Suggested endpoints:
- GET /api/v1/public/courses
- GET /api/v1/public/courses/{slug}
- GET /api/v1/public/lessons
- GET /api/v1/public/lessons/{slug}
- GET /api/v1/public/words
- GET /api/v1/public/words/{slug}
- GET /api/v1/public/grammar-topics
- GET /api/v1/public/grammar-topics/{slug}
- GET /api/v1/public/grammar-rules/{slug}
- GET /api/v1/public/books
- GET /api/v1/public/books/{slug}
- GET /api/v1/public/quizzes
- GET /api/v1/public/quizzes/{slug}

Public API Rules:
- Return only public-safe DTOs.
- Do not expose internal IDs unnecessarily if slugs are available.
- Do not expose security fields.
- Do not expose internal storage paths.
- Do not expose quiz correct answers before submission.
- Only return Published and Active content where applicable.
- Apply pagination to list endpoints.
- Add max page size.

Blazor Requirements:
Create or update public layout:
- Public navigation
- Home / Learn / Courses / Dictionary / Grammar / Books / Quizzes
- Responsive layout if simple
- Loading state
- Empty state
- Error state

Student UX Requirements:
- Pages should be clean and easy to read.
- Avoid admin-style forms on public pages.
- Show breadcrumbs if simple.
- Show back links where useful.
- Use existing media public URLs safely.
- Do not require login for public pages unless existing security requires it.

Search Requirements:
- Dictionary search
- Course search if simple
- Lesson search if simple
- Grammar search if simple

Testing Requirements:
Add tests for:
- Public course list returns only published active courses.
- Public lesson list returns only published active lessons.
- Public word dictionary does not expose draft content.
- Public quiz endpoint does not expose correct answers.
- Public pages/routes build correctly if test pattern supports it.
- Existing admin modules still pass.

Quality:
- Keep changes minimal.
- Do not overengineer.
- Do not add student progress yet.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Public routes created
3. Public API endpoints created
4. Student pages created
5. Public DTO safety summary
6. Tests created
7. Build/test result
8. Remaining limitations