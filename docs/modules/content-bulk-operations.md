# Content Bulk Operations

## Purpose

Content Bulk Operations let authorized administrators apply a simple action to multiple content items and inspect the result for each item. The first implementation is synchronous and intentionally avoids background queue complexity.

## Supported Content Types

- Word
- Pronunciation
- GrammarTopic
- GrammarRule
- Lesson
- Course
- Book
- Quiz

The most complete action support currently covers Word, Lesson, Course, Book, and Quiz.

## Supported Actions

- `SubmitForReview`
- `Approve`
- `RequestChanges`
- `Publish`
- `Archive`
- `Activate`
- `Deactivate`
- `RunQualityCheck`
- `AssignCategory`
- `AddTags`
- `RemoveTags`
- `Export`

## Workflow Safety

Bulk operations must not bypass normal permission checks. Creating and running an operation requires `bulk-operations.run`, and sensitive actions require an underlying permission such as `publishing.run` or `content-quality.run`.

Each item is processed independently. A failed item does not stop the rest of the operation unless a future action explicitly requires all-or-nothing behavior.

## Partial Success

If every item succeeds, the operation becomes `Completed`. If every item fails, it becomes `Failed`. If some succeed and some fail, it becomes `PartiallyCompleted`.

## Why No Background Queue Yet

The first implementation runs synchronously to keep behavior observable and easy to test. A background queue should only be added after retry rules, cancellation behavior, progress reporting, and operational monitoring are defined.

## Known Limitations

- Content list checkbox integration is deferred.
- Export records operation success but does not yet generate a downloadable file.
- Full content review workflow states are not modeled for every content type.
- Large batches should wait for a background execution design.
