# Import / Export Module

## Purpose

The Import / Export Module gives admins a simple way to move MVP content in and out of EnglishMaster for development, review, and content operations.

This module is intentionally small:

- Words can be imported from CSV or JSON.
- Words, Grammar Topics, Lessons, Courses, Books, and Quizzes can be exported as CSV or JSON.
- Import for Categories, Tags, Grammar Rules, Lessons, Courses, Books, and Quizzes is a documented follow-up.

## Supported Content

Current MVP support:

- Words: import and export
- Grammar Topics: export
- Lessons: export
- Courses: export
- Books: export
- Quizzes: export

## Supported Formats

- CSV for simple lists.
- JSON for structured transfer.

CSV import currently supports one Word per row.

## Word Import Fields

Required:

- Text
- MeaningTh
- PartOfSpeech
- CefrLevel

Optional:

- IpaUk
- IpaUs
- ThaiReading
- MeaningEn
- ExampleEn
- ExampleTh
- CategorySlug
- Tags

`Tags` is a semicolon-separated list of existing tag slugs. `CategorySlug` must match an existing category slug when supplied.

## Validation Rules

- File is required.
- File must be CSV or JSON.
- File must be 1 MB or smaller.
- Required Word fields must be present.
- PartOfSpeech and CefrLevel must match supported enum values.
- Duplicate Word text is rejected by slug.
- Unknown category or tag slugs are reported as row-level errors.
- Valid rows can import while invalid rows are returned with row-level errors.

## Security Rules

- Import uses the existing `words.create` permission.
- Export uses each module's existing read permission.
- Uploaded content is read as data only; it is not executed.
- File paths from uploads are not trusted or used for storage.
- Internal exceptions are not exposed intentionally.

## Blazor Pages

- `/admin/import`
- `/admin/export`

The Import page uploads Words CSV or JSON and shows row-level errors. The Export page fetches the selected content and provides a download link.

## Known Limitations

- Only Words import is implemented.
- CSV parsing is intentionally simple and intended for ordinary list files.
- Import files are capped at 1 MB and are processed synchronously.
- Export is a flat administrative snapshot, not a full relational package.
- Export endpoints are intended for MVP-sized administrative data, not large archival exports.
- Quiz export does not include question and choice bodies in this first MVP pass.
- Large import jobs, background processing, advanced Excel formatting, and AI import are out of scope.

## Next Steps

Recommended follow-up:

1. Add Category and Tag import.
2. Add structured JSON import for Lessons, Courses, Books, and Quizzes.
3. Add question and choice export for Quizzes.
4. Add import preview and dry-run mode before committing rows.
