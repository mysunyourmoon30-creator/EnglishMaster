# Advanced Analytics

## Overview

Advanced analytics adds read-only aggregate insight for administrators and learners. It builds on existing progress, activity, quiz, practice, certificate, and streak data.

## Scope

Included in v0.3.0:

- Admin analytics overview API.
- Student analytics overview API.
- Admin analytics dashboard at `/admin/analytics`.
- Student analytics dashboard at `/learn/analytics`.

## Aggregated Data

Admin overview includes:

- Active learners.
- Study minutes.
- Learning activities.
- Course completions.
- Quiz attempts and average score.
- Practice sessions completed.
- Certificates issued.

Student overview includes:

- Study minutes and learning activities.
- Lessons, courses, and books completed.
- Quiz attempts and average score.
- Practice sessions completed.
- Certificates earned.
- Current and longest streak.

## Architecture

Files are organized under:

```text
EnglishMaster.Contracts.Analytics
EnglishMaster.Application.Features.Analytics
EnglishMaster.Infrastructure.Analytics
EnglishMaster.Api.Endpoints.AnalyticsEndpoints
EnglishMaster.Web.Services.Analytics
EnglishMaster.Web.Components.Pages.Analytics
```

The API endpoint layer only maps HTTP inputs to `AnalyticsQueryHandler`. EF Core aggregation stays in `EfAnalyticsRepository`.

## Privacy Rules

- Admin dashboards do not expose learner names, emails, or user ids.
- Student dashboards are scoped to the authenticated user.
- No public analytics API exists.
- No raw quiz answers or item-level answer detail is returned.

## Known Limitations

- Trend series and breakdown endpoints are not implemented yet.
- Aggregates are live EF queries, not cached rollups.
- No export or external BI integration is included.
