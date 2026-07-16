# Learning Activity Log

## Purpose

`LearningActivityLog` records learner activity used for streaks, summaries, and achievements.

## Fields

```text
Id
StudentProfileId
ActivityType
ContentType
ContentId
Title
OccurredAt
MinutesSpent
MetadataJson
CreatedAt
UpdatedAt
```

## Rules

- Activity belongs to one student profile.
- `ActivityType` and `OccurredAt` are required.
- `MinutesSpent` must not be negative.
- Activity history is bounded by API limits.
- Metadata is sanitized for obvious sensitive auth keys such as password, token, secret, api_key, authorization, cookie, and credential.
