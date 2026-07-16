# Public Search Safety

## Purpose

Public Search is anonymous, so every result must be safe for learners and safe to expose without authentication.

## Public-Safe DTO Rules

Public search DTOs must not include:

- Internal storage paths.
- Raw upload paths.
- Admin-only notes.
- Review workflow state.
- Draft or archive metadata.
- Audit-only fields.
- Quiz correct answer IDs.
- `IsCorrect` flags.
- Hidden answer explanations.

## Published Visibility

Current visibility rules:

- Words: `IsActive`.
- Grammar topics and rules: `IsActive`.
- Lessons, courses, books, quizzes: `IsActive && IsPublished`.

If future modules add review states such as Draft, InReview, ChangesRequested, or Archived to these entities, public search must filter them before returning results.

## Quiz Answer Safety

Quiz search results return only title, summary, level/category metadata, URL, and timestamps. They do not load questions, choices, scoring keys, or correct-answer flags.

## Error Safety

Unexpected server errors should use the global API exception handler. Public search endpoints must not leak SQL, stack traces, storage paths, or entity internals.

