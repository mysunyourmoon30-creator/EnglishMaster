# Content Quality Admin UI

## Pages

| Page | Purpose |
| --- | --- |
| `/admin/content-quality` | Dashboard with summary counts and run form. |
| `/admin/content-quality/checks` | Search quality checks. |
| `/admin/content-quality/checks/{id:guid}` | View findings and resolve them. |
| `/admin/content-quality/rules` | List quality rules. |
| `/admin/content-quality/rules/create` | Create a rule. |
| `/admin/content-quality/rules/{id:guid}/edit` | Edit a rule. |

## States

Pages include loading, empty, and error states. The detail page shows findings and resolution actions.

## Known Limitations

- The first version uses a simple content type/id run form.
- Deep per-content detail widgets can be expanded later.
- Stored custom rules are managed but not yet interpreted as dynamic expressions.
