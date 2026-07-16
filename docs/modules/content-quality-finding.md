# Content Quality Finding

## Purpose

`ContentQualityFinding` stores one issue found during a content quality check.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Finding identifier. |
| `ContentQualityCheckId` | Parent quality check. |
| `RuleCode` | Rule that produced the finding. |
| `Severity` | Info, Warning, Error, or Critical. |
| `Message` | Human-readable issue. |
| `FieldName` | Related field if applicable. |
| `Recommendation` | Suggested action. |
| `IsResolved` | Whether the finding was marked resolved. |
| `ResolvedAt` | Resolution timestamp. |
| `CreatedAt` / `UpdatedAt` | Audit timestamps. |

## Resolution

Findings can be marked resolved through:

```text
POST /api/v1/content-quality/findings/{id}/resolve
```

Resolution keeps the historical finding record.
