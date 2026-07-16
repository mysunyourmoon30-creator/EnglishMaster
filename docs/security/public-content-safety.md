# Public Content Safety v0.2.0

## Purpose

Public content safety rules protect EnglishMaster from leaking private, draft, administrative, or answer-key data through student-facing pages and public APIs.

## Core Rule

Public surfaces must expose only content intentionally published for students. If content is not published and public, the public page or API should behave as if it does not exist.

## Published Content Visibility

Apply these rules to pages, APIs, nested content, search results, and related-content widgets:

- Draft content is never public.
- Archived content is never public unless explicitly restored and republished.
- Private content is never public.
- Future scheduled content is not public until its publish time.
- Parent content being published does not automatically make child content public.
- Child content must be independently checked before display.
- Unpublished detail requests should return `404 Not Found`.
- Error messages must not reveal whether hidden content exists.

## Public Search Safety

Public search must apply the same visibility rules as public detail pages. Search results may expose titles, slugs, short summaries, CEFR level, category name, tags, public URL, and updated timestamp. Search results must not expose review state, admin notes, internal IDs as navigation targets, storage paths, or quiz answers.

See [Public Search Safety](public-search-safety.md).

## Public DTO Safety

Public DTOs must be separate from admin DTOs, entity models, and authoring view models.

Never include:

- Internal ids unless intentionally public.
- Draft state.
- Review state.
- Moderation notes.
- Authoring notes.
- Audit fields.
- Soft-delete fields.
- Tenant or operational metadata.
- Private file paths.
- Raw storage keys.
- Student progress.
- Enrollment state.
- Certificate state.
- Login-specific personalization.
- Quiz answer keys.

## Quiz Correct-Answer Safety Rule

Public quiz payloads must not include data that allows the client to determine the correct answer before submission.

Do not include:

- Correct answer ids.
- `isCorrect` flags.
- Scoring weights that identify correct choices.
- Hidden explanations that reveal the answer.
- Validation rules that reveal the answer.
- Full answer keys in serialized page state.

If interactive quiz answering is added, validate answers on the server. Return only feedback appropriate for the submitted attempt.

## Media Safety

Media displayed on public pages must be safe for public rendering:

- Render only media attached to published public content.
- Do not expose local paths, private storage keys, private buckets, or temporary upload locations.
- Use application-controlled media URLs or approved public asset URLs.
- Keep signed URL lifetimes short when signed URLs are unavoidable.
- Sanitize captions, descriptions, transcripts, and embedded HTML.
- Provide alt text for instructional images where available.
- Fail closed when media is missing, private, rejected, or unpublished.

## Known Limitations

v0.2.0 does not yet include:

- Student progress.
- Login-based learning.
- Certificates.
- AI tutor.
- Per-student personalization.
- Paid content access enforcement.
- Full quiz attempt storage.
- External search indexing.

## Next Recommended v0.2.0 Feature

Implement a server-side quiz submission flow with tests that prove correct answers are absent from public quiz DTOs and serialized page state.
