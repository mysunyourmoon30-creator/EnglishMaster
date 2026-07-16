# Public UI Guidelines v0.2.0

## Purpose

Student-facing public UI should make published EnglishMaster content easy to browse, read, and practice without requiring login. The interface should be clear, content-first, and safe for anonymous learners.

## General Principles

- Prioritize readable learning content over administrative detail.
- Use stable public routes and learner-friendly slugs.
- Show only published content.
- Keep unavailable, unpublished, archived, or private content out of navigation and search results.
- Avoid UI states that imply student progress, enrollment, certificates, or personalized learning until those features exist.
- Use consistent empty, loading, error, and not-found states across public pages.

## Page Guidelines

### Search Page

The public search page should help learners find published content across words, grammar, lessons, courses, books, and quizzes.

Recommended UI elements:

- Keyword search box.
- Content type filter.
- CEFR filter.
- Category and tag filters where useful.
- Results with content type, title, summary, public URL, level, and metadata.
- Loading, empty, error, and pagination states.

Avoid:

- Showing admin-only fields.
- Showing draft or unpublished content.
- Showing quiz answer hints or correctness metadata.
- Linking to admin routes.

### Course Pages

Course pages should help students understand what the course covers and move into published lessons or related materials.

Recommended UI elements:

- Course title and summary.
- Level, estimated duration, tags, or category.
- Published lesson list.
- Related published quizzes, books, dictionary entries, or grammar pages when available.
- Public cover image or thumbnail when approved.

Avoid:

- Progress bars.
- Enrollment buttons.
- Certificate promises.
- Draft lesson placeholders.

### Lesson Pages

Lesson pages should provide a focused reading or practice experience.

Recommended UI elements:

- Lesson title and summary.
- Main instructional body.
- Safe media blocks.
- Related published dictionary, grammar, book, quiz, or course links.

Avoid:

- Completion checkboxes.
- Attempt history.
- Login-only calls to action as the primary learning path.

### Dictionary Pages

Dictionary pages should make vocabulary easy to scan and understand.

Recommended UI elements:

- Headword.
- Pronunciation.
- Part of speech.
- Definition.
- Example sentences.
- Related words or lessons.
- Audio controls when safe audio is available.

### Grammar Pages

Grammar pages should make grammar topics readable and example-driven.

Recommended UI elements:

- Grammar topic title.
- Short explanation.
- Patterns or structures.
- Examples.
- Related published lessons and quizzes.

### Book Pages

Book pages should support comfortable reading.

Recommended UI elements:

- Book title and summary.
- Level and metadata.
- Cover media.
- Chapter or section navigation.
- Reading content with accessible typography.

### Quiz Pages

Quiz pages should present practice questions without leaking answers.

Recommended UI elements:

- Quiz title and instructions.
- Question prompts.
- Answer choices.
- Submit or check controls only when server-side validation exists.

Avoid:

- Embedding correct answer flags in page data.
- Revealing explanations before submission when they identify the answer.
- Displaying score, attempt history, or progress unless those features are implemented safely.

## Media Rendering

Public UI should render media consistently:

- Use approved image, audio, video, and document display components.
- Show alt text or accessible labels where available.
- Do not render unsafe raw HTML.
- Hide or replace unavailable media with a neutral unavailable state.
- Do not show raw storage paths or upload filenames as learner-facing labels.

## Intentionally Not Included

The v0.2.0 public UI does not include:

- Student progress dashboards.
- Login-based learning.
- Certificates.
- AI tutor chat.
- Personalized recommendations.
- Enrollment or payment UI.

## Next Recommended v0.2.0 Feature

Add safe public detail pages and a quiz practice interaction that submits selected answers to the server, then renders feedback after validation without exposing the answer key in the initial page state.
