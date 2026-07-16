# Public Search UI

## Route

```text
/search
```

## Page Behavior

The search page includes:

- Search box.
- Content type filter.
- CEFR filter.
- Category filter.
- Tag filter.
- Result list.
- Loading state.
- Empty state.
- Error state.
- Previous/next pagination.
- Links to public detail URLs.

## Result Links

Results link to:

```text
/dictionary/{slug}
/grammar/{slug}
/lessons/{slug}
/courses/{slug}
/books/{slug}
/quizzes/{slug}
```

## Navigation

The main navigation includes a Search link. Search is public and should not require login.

## Known UI Limitations

- Suggestions are API-only for now.
- Filters are simple select controls.
- No advanced ranking explanation is shown.
- No typo-tolerant search message is shown.

