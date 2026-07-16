Prepare EnglishMaster v0.2.0 Release Candidate and Final Stabilization.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Better Publishing completed.
- Student-facing pages completed.
- Student Progress completed.
- Basic Reporting completed.
- Notification foundation completed.
- Production hardening completed.
- Content Quality completed.
- Content Versioning completed.
- Bulk Operations completed.
- Advanced Import completed.
- Public Search completed.
- Learning Recommendations completed.
- Practice System completed.
- Learning Goals completed.
- Daily Study Plan completed.
- Streaks and Achievements completed.
- Weekly Learning Report completed.

Goal:
Stabilize all v0.2.0 features and prepare a Release Candidate.

Important:
Feature freeze starts now.
Do not add new business features.
Do not add AI.
Do not add mobile.
Do not add payment.
Do not add marketplace.
Do not add microservices.
Do not redesign architecture.

Scope:
Final stabilization only.

Tasks:

1. Build Stabilization
Run:
- dotnet restore
- dotnet build
- dotnet test

Fix:
- Build errors
- Test failures
- Broken references
- Missing usings
- Broken route constants
- Broken dependency injection registrations
- EF Core configuration issues
- Migration inconsistencies if obvious and safe

2. Route Stabilization
Verify routes exist and compile:

Public routes:
- /
- /learn
- /learn/dashboard
- /learn/courses
- /learn/lessons
- /dictionary
- /grammar
- /books
- /quizzes
- /search
- /learn/recommendations
- /learn/practice
- /learn/goals
- /learn/study-plan
- /learn/motivation
- /learn/achievements
- /learn/activity
- /learn/reports

Admin routes:
- /admin
- /admin/words
- /admin/categories
- /admin/tags
- /admin/media
- /admin/pronunciations
- /admin/grammar-topics
- /admin/lessons
- /admin/courses
- /admin/books
- /admin/quizzes
- /admin/publishing
- /admin/content-review
- /admin/reports
- /admin/notifications
- /admin/email-messages
- /admin/content-quality
- /admin/content-revisions
- /admin/bulk-operations
- /admin/import-jobs
- /admin/achievement-definitions

3. Security Stabilization
Verify:
- Admin routes require authentication.
- Admin routes require correct permissions.
- /me endpoints require authentication.
- Users can only access their own student data.
- Public endpoints expose only published/active content.
- Quiz correct answers are not exposed before submit.
- Draft/InReview/ChangesRequested/Archived content does not appear publicly.
- No internal file paths are exposed.
- No passwords/tokens/security stamps are exposed.
- No secrets are committed.

4. Data Access Stabilization
Verify:
- DbContext configurations compile.
- Entity relationships are configured.
- Unique indexes are intentional.
- Migrations are consistent if migration files exist.
- No Blazor page accesses DbContext directly.
- No API controller contains heavy business logic.
- Application layer owns use cases.

5. Performance Stabilization
Verify:
- List endpoints use pagination.
- Max page size exists.
- AsNoTracking is used for read-only queries where appropriate.
- No obvious unbounded queries.
- No unnecessary Include chains.
- Search/recommendation/report queries are limited.
- Admin reports do not load huge datasets.

6. UI Stabilization
Verify:
- Public navigation works.
- Admin navigation works.
- Student dashboard works.
- Admin dashboard works.
- Loading states exist.
- Empty states exist.
- Error states exist.
- Broken slugs show safe not found behavior.
- Logged-out users see safe behavior.

7. Documentation Check
Verify key docs exist:
- README.md
- docs/architecture/*
- docs/api/*
- docs/security/*
- docs/routes/*
- docs/modules/*
- docs/deployment/*
- docs/operations/*
- docs/roadmap/v0.2.0-roadmap.md
- docs/release/*

8. Release Candidate Notes
Create or update:
- docs/release/v0.2.0-release-candidate.md

Include:
- Feature summary
- Major modules included
- Security notes
- Known limitations
- Required manual testing
- Deployment notes
- Rollback notes
- Open risks

9. Quality Gate
Run:
- dotnet build
- dotnet test

Fix errors until everything passes.

Output:
1. Build stabilization result
2. Test stabilization result
3. Route verification result
4. Security stabilization result
5. Performance stabilization result
6. UI stabilization result
7. Docs updated
8. Release candidate notes created
9. Remaining release blockers