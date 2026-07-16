# Public Routes v0.2.0

## Purpose

Public routes provide student-facing entry points for browsing published EnglishMaster learning content. These routes are read-only and should work without login.

## Route Structure

Recommended public route structure:

```text
/search
/learn/dashboard
/learn/recommendations
/learn/practice
/learn/practice/session/{id}
/learn/practice/history
/learn/goals
/learn/goals/create
/learn/goals/{id}
/learn/study-plan
/learn/study-plan/history
/learn/motivation
/learn/achievements
/learn/activity
/learn/reports
/learn/reports/current-week
/learn/reports/{id}
/courses
/courses/{courseSlug}
/courses/{courseSlug}/lessons
/courses/{courseSlug}/lessons/{lessonSlug}

/lessons
/lessons/{lessonSlug}

/dictionary
/dictionary/{entrySlug}

/grammar
/grammar/{grammarSlug}

/books
/books/{bookSlug}

/quizzes
/quizzes/{quizSlug}
```

Route parameters should use stable slugs. Public pages should avoid exposing internal database ids unless the project intentionally defines them as public identifiers.

## Page Behavior

Public pages should:

- Render only published content.
- Return a normal not-found experience for missing, unpublished, archived, private, or future scheduled content.
- Avoid explaining whether hidden content exists.
- Use canonical links where duplicate paths can resolve to the same content.
- Keep route names stable once linked from public pages.

## Route Areas

### Search Route

The public search route is:

```text
/search
```

Supported query examples:

```text
/search?q=hello
/search?type=word
/search?cefr=A1
```

The backing API is documented in [Public Search API](../api/public-search-api.md).

### Authenticated Learning Routes

Learning recommendation routes are:

```text
/learn/dashboard
/learn/recommendations
/learn/practice
/learn/practice/session/{id}
/learn/practice/history
/learn/goals
/learn/goals/create
/learn/goals/{id}
/learn/study-plan
/learn/study-plan/history
/learn/motivation
/learn/achievements
/learn/activity
/learn/reports
/learn/reports/current-week
/learn/reports/{id}
```

These routes are for authenticated learners and use `/api/v1/me/learning/*`, `/api/v1/me/practice/*`, `/api/v1/me/learning-goals/*`, `/api/v1/me/study-plan/*`, `/api/v1/me/motivation/*`, and `/api/v1/me/learning-reports/*` endpoints.

Practice routes are not public anonymous routes. They are listed here only to keep learner-facing route documentation in one place.

### Course Routes

Course routes support course discovery and course detail views.

```text
/courses
/courses/{courseSlug}
```

Course detail pages may link to published child lessons, books, grammar pages, dictionary entries, and quizzes.

### Lesson Routes

Lesson routes support standalone lessons and course-scoped lesson navigation.

```text
/lessons/{lessonSlug}
/courses/{courseSlug}/lessons/{lessonSlug}
```

When both routes are supported, the course-scoped route should preserve course navigation context while still enforcing published visibility for both the course and lesson.

### Dictionary Routes

Dictionary routes support vocabulary browsing and detail views.

```text
/dictionary
/dictionary/{entrySlug}
```

### Grammar Routes

Grammar routes support grammar topic browsing and detail views.

```text
/grammar
/grammar/{grammarSlug}
```

### Book Routes

Book routes support reading material browsing and detail views.

```text
/books
/books/{bookSlug}
```

### Quiz Routes

Quiz routes support quiz browsing and display.

```text
/quizzes
/quizzes/{quizSlug}
```

Correct answers must not be embedded in the route payload or page initialization data.

## Intentionally Not Included

The v0.2.0 public route scope does not include:

- Student progress routes.
- Public notification routes.
- Anonymous access to login-only learning dashboards.
- Anonymous access to practice routes.
- Anonymous access to learning goals and study plan routes.
- Anonymous access to motivation, achievements, and activity routes.
- Anonymous access to learning report routes.
- Certificate routes.
- AI tutor routes.
- Enrollment routes.
- Payment or gated access routes.

## Next Recommended v0.2.0 Feature

Add dedicated public detail pages for any search result type that still lacks a learner-facing detail route, then add server-side quiz submission safety.
