# Student-Facing Learning Pages v0.2.0

## Purpose

Student-facing learning pages expose published EnglishMaster learning content to learners through public, read-only pages. The v0.2.0 scope focuses on discovery and content consumption: learners can browse courses, lessons, dictionary entries, grammar pages, books, quizzes, and related media without needing an account.

These pages are intended to be safe public surfaces over curated content. They must not expose authoring-only fields, internal workflow metadata, unpublished records, moderation notes, private storage paths, answer keys, student progress, or login-specific learning state.

## Page Areas

### Course Pages

Course pages present published course information and link to published child content such as lessons, books, grammar pages, dictionary entries, and quizzes where applicable.

Course pages should show:

- Public title, slug, summary, description, level, and thumbnail or cover media when available.
- Published lesson sequence or public course modules.
- Only content that is independently published and visible.

Course pages should not show:

- Draft modules or lessons.
- Internal authoring notes.
- Enrollment state, student progress, completion status, or certificates.

### Lesson Pages

Lesson pages present published instructional content in a learner-friendly layout.

Lesson pages should show:

- Public title, slug, summary, lesson body, level, duration, tags, and safe media.
- Links to related published course, dictionary, grammar, book, or quiz content.

Lesson pages should not show:

- Draft blocks.
- Teacher-only answer notes.
- Student-specific completion, score, attempts, bookmarks, or recommendations.

### Dictionary Pages

Dictionary pages present published vocabulary entries and related learning examples.

Dictionary pages should show:

- Headword, pronunciation, part of speech, definition, examples, level, tags, and safe audio or image media.
- Related published words or lessons when available.

Dictionary pages should not expose:

- Internal curation status.
- Private source notes.
- Unreviewed examples.

### Grammar Pages

Grammar pages present published grammar explanations, patterns, examples, and related exercises.

Grammar pages should show:

- Public title, slug, explanation, examples, level, tags, and related published content.

Grammar pages should not show:

- Draft explanations.
- Teacher-only notes.
- Hidden validation rules that reveal future quiz answers.

### Book Pages

Book pages present published reading material and metadata.

Book pages should show:

- Public title, slug, summary, author or source attribution when allowed, level, chapters or sections, cover media, and reading content.

Book pages should not show:

- Licensed or private source material that is not cleared for public display.
- Draft chapters.
- Internal content acquisition notes.

### Quiz Pages

Quiz pages present published quiz prompts and answer choices for practice.

Quiz pages should show:

- Public title, slug, summary, instructions, visible questions, answer choices, and allowed media.

Quiz pages must not expose:

- Correct answer ids or flags in initial public payloads.
- Explanation fields that reveal the correct answer before submission.
- Scoring keys, answer weights, moderation metadata, or private validation rules.

For v0.2.0, public quiz pages are display-oriented. If interactive quiz submission is added in this version, answer validation must happen through a dedicated submission endpoint that returns only the result permitted for the current interaction.

## Published Content Visibility Rules

All student-facing pages must apply published-content filtering consistently:

- Content must be in a published state before it appears on public pages.
- Child content must also be published before being linked or embedded.
- Public pages should return `404 Not Found` for missing, unpublished, archived, or private content.
- Public pages should not disclose whether hidden content exists.
- Slugs are public identifiers, but internal database ids should be avoided unless they are already part of an intentional public contract.
- Scheduled publishing should treat future content as unavailable until the configured publish time.

## Media Rendering Rules

Public media rendering must use safe, display-ready media references:

- Render only media attached to published content and approved for public use.
- Prefer stable public URLs or application-controlled media endpoints over raw storage paths.
- Do not expose private bucket names, local file paths, temporary upload paths, or signed URLs with excessive lifetime.
- Use alt text for instructional images where available.
- Avoid rendering arbitrary HTML from media captions or descriptions unless sanitized.
- Video, audio, image, and document previews should fail closed when the media is missing, private, or unpublished.

## Known Limitations

v0.2.0 is intentionally focused on public learning content. The following are not included yet:

- Student progress tracking.
- Login-based learning experiences.
- Certificates.
- AI tutor flows.
- Personalized recommendations.
- Enrollments.
- Paid course access rules.
- Full quiz attempt history.
- Offline learning mode.

## Next Recommended v0.2.0 Feature

The next recommended feature is a public lesson-to-quiz practice flow that keeps correct answers server-side. This should introduce a submission endpoint for published quizzes, return per-question feedback only after submission, and continue to avoid exposing answer keys in public DTOs.
