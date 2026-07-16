# Public API

## Purpose

The public API supports student-facing learning pages with safe, read-only access to published content. It is intended for public browsing and learning workflows, not authoring, administration, student tracking, or authenticated learning state.

## Endpoint Structure

Public endpoints should be grouped under a stable public prefix:

```text
/api/public
```

Recommended endpoint shape:

```text
GET /api/public/courses
GET /api/public/courses/{slug}
GET /api/public/courses/{slug}/lessons

GET /api/public/lessons
GET /api/public/lessons/{slug}

GET /api/public/dictionary
GET /api/public/dictionary/{slug}

GET /api/public/grammar
GET /api/public/grammar/{slug}

GET /api/public/books
GET /api/public/books/{slug}

GET /api/public/quizzes
GET /api/public/quizzes/{slug}
```

If quiz submission is introduced in v0.2.0, use a separate endpoint that does not rely on client-visible answer keys:

```text
POST /api/public/quizzes/{slug}/submissions
```

## Public DTO Safety Rules

Public DTOs must be designed separately from admin, authoring, or persistence models.

Public DTOs may include:

- Public id or slug.
- Title.
- Summary.
- Description or body content approved for public display.
- Level.
- Tags.
- Published date.
- Public media references.
- Public relationships to other published content.

Public DTOs must not include:

- Internal database ids unless intentionally public.
- Draft, review, moderation, or workflow fields.
- Authoring notes.
- Private source notes.
- Storage paths.
- Permission flags.
- Student-specific progress.
- Enrollment state.
- Quiz correct answer ids.
- Quiz `isCorrect` flags.
- Hidden explanation fields that reveal the answer before submission.
- Soft-delete, archive, tenant, audit, or operational metadata.

## Published Content Filtering

Every public API query must enforce published visibility:

- Return only content with public visibility and published status.
- Exclude drafts, archived records, private records, and future scheduled content.
- Filter nested collections independently.
- Return `404 Not Found` for unpublished detail records.
- Do not return alternate status messages that reveal hidden content.

## Page-Specific Payload Notes

### Courses

Course DTOs should include public course metadata and published child summaries. Course detail responses may include ordered lesson summaries and related published content.

### Lessons

Lesson DTOs should include public lesson content, safe media, and related published resources. They must not include student completion or attempt data.

### Dictionary

Dictionary DTOs should include public vocabulary fields, examples, pronunciation data, and safe media. They must not include internal curation notes.

### Grammar

Grammar DTOs should include public explanations, examples, level, and related published lessons or quizzes.

### Books

Book DTOs should include public reading content and cleared attribution. They must not expose private or uncleared source material.

### Quizzes

Quiz DTOs should include prompts and answer choices needed for display. They must not include answer keys, correctness flags, scoring weights, or pre-submission explanations that reveal the correct answer.

## Known Limitations

The public API does not yet cover:

- Student progress.
- Login-based learning.
- AI tutor.
- Personalized learning state.
- Enrollments.
- Full quiz attempt history.

Certificate verification is covered by `GET /api/v1/public/certificates/{verificationCode}` and returns only public-safe verification fields.

## Next Recommended v0.2.0 Feature

Add a public quiz submission contract:

```text
POST /api/public/quizzes/{slug}/submissions
```

The endpoint should accept selected answer ids, validate server-side against published quiz data, and return only safe feedback for the submitted attempt.
