# Import / Export API

Base path: `/api/v1`

All endpoints require authentication.

## Import Words

`POST /api/v1/import/words`

Permission: `words.create`

Content type: `multipart/form-data`

Form fields:

- `file`: CSV or JSON file, 1 MB maximum.

CSV headers:

```csv
Text,MeaningTh,PartOfSpeech,CefrLevel,IpaUk,IpaUs,ThaiReading,MeaningEn,ExampleEn,ExampleTh,CategorySlug,Tags
hello,kham thakthai,Interjection,A1,heh-LOH,heh-LOH,heh-lo,A greeting,Hello there.,,vocabulary,beginner;daily-english
```

JSON example:

```json
[
  {
    "text": "hello",
    "meaningTh": "kham thakthai",
    "partOfSpeech": "Interjection",
    "cefrLevel": "A1",
    "ipaUk": "heh-LOH",
    "ipaUs": "heh-LOH",
    "thaiReading": "heh-lo",
    "meaningEn": "A greeting",
    "exampleEn": "Hello there.",
    "categorySlug": "vocabulary",
    "tagSlugs": ["beginner", "daily-english"]
  }
]
```

Success response:

```json
{
  "totalRows": 2,
  "importedCount": 1,
  "failedCount": 1,
  "errors": [
    {
      "rowNumber": 2,
      "field": "Text",
      "message": "A word with this text already exists."
    }
  ]
}
```

Validation errors return `400` with validation problem details for file-level issues.

## Export Words

`GET /api/v1/export/words?format=csv`

Permission: `words.read`

Formats:

- `csv`
- `json`

Returns a downloadable file.

## Export Grammar Topics

`GET /api/v1/export/grammar-topics?format=csv`

Permission: `grammar.read`

Formats:

- `csv`
- `json`

Returns a downloadable file.

## Export Lessons

`GET /api/v1/export/lessons?format=csv`

Permission: `lessons.read`

Formats:

- `csv`
- `json`

Returns a downloadable file.

## Export Courses

`GET /api/v1/export/courses?format=csv`

Permission: `courses.read`

Formats:

- `csv`
- `json`

Returns a downloadable file.

## Export Books

`GET /api/v1/export/books?format=csv`

Permission: `books.read`

Formats:

- `csv`
- `json`

Returns a downloadable file.

## Export Quizzes

`GET /api/v1/export/quizzes?format=csv`

Permission: `quizzes.read`

Formats:

- `csv`
- `json`

Returns a downloadable file.

## Security Notes

- Upload size is limited to 1 MB.
- Only CSV and JSON files are accepted.
- Uploaded file names are normalized and are not used as storage paths.
- Uploaded content is parsed as data only.
- Existing module permissions protect each route.

## Current Limitations

- Only Words import is implemented.
- Export data is flat and intended for content review and simple movement.
- Import files are capped at 1 MB.
- Export endpoints stream a generated response for the requested entity type, but they are not a replacement for database backup or bulk archival tooling.
- Structured import for Lessons, Courses, Books, Quizzes, and Grammar Rules is pending.
