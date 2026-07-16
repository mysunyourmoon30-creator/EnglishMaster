# Content Versioning

## Purpose

Content Versioning keeps a simple revision history for important content changes. It helps editors and reviewers see what changed, when it changed, who changed it, and which snapshot can be used for a controlled restore request.

The first version is intentionally small. It is not a full audit platform, visual diff tool, or rollback engine.

## Supported Content Types

- Word
- Pronunciation
- GrammarTopic
- GrammarRule
- GrammarExample
- Lesson
- Course
- Book
- Quiz
- PublishTemplate

Revision history links are currently exposed from Word, Lesson, Course, Book, and Quiz detail pages.

## Revision Events

- `Created`
- `Updated`
- `SubmittedForReview`
- `Approved`
- `ChangesRequested`
- `Published`
- `Archived`
- `QualityChecked`
- `Restored`

## Snapshot Behavior

`SnapshotJson` stores a sanitized content snapshot. It must not contain passwords, tokens, security stamps, connection strings, API keys, cookies, or secrets.

`DiffJson` is optional. The current implementation stores a sanitized JSON diff when one is supplied, but it does not calculate advanced visual diffs.

## Adding Revision Tracking To A Module

1. Inject `IContentRevisionService` into the application or infrastructure workflow that owns the content change.
2. Call `CreateAsync` after a successful content change.
3. Use a supported `ContentType` and `EventType`.
4. Serialize only content-safe fields into the snapshot.
5. Let the snapshot serializer remove sensitive fields as a final guard.
6. Add tests for revision creation and revision number increments.

## Known Limitations

- Automatic revision capture is not wired into every content command yet.
- Restore requests track approval state, but they do not automatically mutate the original content record.
- Advanced visual diff is intentionally excluded until change payloads and reviewer workflow are more mature.

## Next Recommended Feature

Wire automatic revision capture into the existing Word, Lesson, Course, Book, Quiz, and PublishTemplate command handlers.
