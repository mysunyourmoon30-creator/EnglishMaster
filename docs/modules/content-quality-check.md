# Content Quality Check

## Purpose

`ContentQualityCheck` records one QA run for one content item.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Check identifier. |
| `ContentType` | Checked content type. |
| `ContentId` | Checked content id. |
| `Status` | Passed, Failed, Warning, or NotChecked. |
| `CheckedAt` | Time the check ran. |
| `CheckedBy` | User name or actor that ran the check. |
| `Score` | 0-100 quality score. |
| `CreatedAt` / `UpdatedAt` | Audit timestamps. |

## Running A Check

Use:

```text
POST /api/v1/content-quality/checks/{contentType}/{contentId}/run
```

The dashboard also includes a simple run form for content type and id.

## Latest Check

Use:

```text
GET /api/v1/content-quality/{contentType}/{contentId}/latest
```
