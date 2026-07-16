Create Content Quality and QA System for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Better Publishing completed.
- Student-facing pages completed.
- Student Progress completed.
- Basic Reporting completed.
- Notification foundation completed.
- Production hardening completed.

Goal:
Create a simple content quality checking system for existing content modules.

Important:
Do not add AI.
Do not add advanced NLP.
Do not add marketplace.
Do not add mobile.
Do not add payment.
Do not add microservices.
Do not redesign architecture.

Scope:
Create rule-based content quality checks only.

Target Content Types:
- Word
- Pronunciation
- GrammarTopic
- GrammarRule
- GrammarExample
- Lesson
- Course
- Book
- Quiz
- Publishing

Quality Check Examples:

Word:
- Text is required.
- MeaningTh is required.
- MeaningEn should exist.
- IPA UK or IPA US should exist.
- ExampleEn should exist.
- ExampleTh should exist.

Pronunciation:
- WordId is required.
- IPA UK or IPA US should exist.
- Audio is recommended.
- Minimal pairs are recommended for pronunciation-focused words.

Grammar:
- GrammarTopic must have at least one rule.
- GrammarRule must have RuleText.
- GrammarRule should have examples.
- GrammarExample must have ExampleEn.

Lesson:
- Lesson must have at least one section.
- Lesson should have related words or grammar rules.
- Published lesson should not be empty.

Course:
- Course must have at least one lesson.
- Published course should contain published lessons.

Book:
- Book must have at least one chapter.
- BookChapter should have lessons or content.
- Published book should not be empty.

Quiz:
- Quiz must have at least one question.
- Question must have valid choices when choice-based.
- SingleChoice should have exactly one correct choice.
- TrueFalse should have exactly one correct choice.
- Published quiz should not expose correct answers publicly.

Publishing:
- PublishJob source must exist.
- Draft content should not be publishable.
- Output filename must be safe.

Domain Entities:

1. ContentQualityRule

Fields:
- Id
- Code
- Name
- Description
- ContentType
- Severity
- IsActive
- CreatedAt
- UpdatedAt

Severity values:
- Info
- Warning
- Error
- Critical

Rules:
- Code is required.
- Name is required.
- ContentType is required.
- Severity is required.
- ContentQualityRule supports Activate and Deactivate behavior.

2. ContentQualityCheck

Fields:
- Id
- ContentType
- ContentId
- Status
- CheckedAt
- CheckedBy
- Score
- CreatedAt
- UpdatedAt

Status values:
- Passed
- Failed
- Warning
- NotChecked

Rules:
- ContentType is required.
- ContentId is required.
- Score must be between 0 and 100.
- A check can have many findings.

3. ContentQualityFinding

Fields:
- Id
- ContentQualityCheckId
- RuleCode
- Severity
- Message
- FieldName
- Recommendation
- IsResolved
- ResolvedAt
- CreatedAt
- UpdatedAt

Rules:
- Finding belongs to one ContentQualityCheck.
- RuleCode is required.
- Message is required.
- Severity is required.
- Finding can be marked resolved.

Application Requirements:

Create CQRS-style folders:
- Features/ContentQuality/Commands
- Features/ContentQuality/Queries
- Features/ContentQuality/Dtos

Use cases:
- RunQualityCheck
- RunQualityCheckForContent
- GetQualityCheckById
- GetLatestQualityCheckForContent
- SearchQualityChecks
- GetQualityFindings
- MarkFindingResolved
- SearchQualityRules
- CreateQualityRule
- UpdateQualityRule
- ActivateQualityRule
- DeactivateQualityRule

Service Abstractions:
Create:
- IContentQualityService
- IContentQualityRuleProvider

Infrastructure Requirements:
- Create EF Core configuration for ContentQualityRule.
- Create EF Core configuration for ContentQualityCheck.
- Create EF Core configuration for ContentQualityFinding.
- Configure relationships:
  - ContentQualityCheck one-to-many ContentQualityFinding
- Configure indexes:
  - ContentQualityRule Code unique
  - ContentQualityRule ContentType
  - ContentQualityRule Severity
  - ContentQualityCheck ContentType
  - ContentQualityCheck ContentId
  - ContentQualityCheck Status
  - ContentQualityCheck CheckedAt
  - ContentQualityFinding RuleCode
  - ContentQualityFinding Severity
  - ContentQualityFinding IsResolved
- Add migrations if EF Core tools are available.
- Do not break existing modules.

API Requirements:

Create endpoints:
- GET /api/v1/content-quality/rules
- POST /api/v1/content-quality/rules
- PUT /api/v1/content-quality/rules/{id}
- POST /api/v1/content-quality/rules/{id}/activate
- POST /api/v1/content-quality/rules/{id}/deactivate

- POST /api/v1/content-quality/checks/run
- POST /api/v1/content-quality/checks/{contentType}/{contentId}/run
- GET /api/v1/content-quality/checks
- GET /api/v1/content-quality/checks/{id}
- GET /api/v1/content-quality/{contentType}/{contentId}/latest
- GET /api/v1/content-quality/checks/{id}/findings
- POST /api/v1/content-quality/findings/{id}/resolve

Security:
- All content quality endpoints require authentication.
- Running quality checks requires content quality permission.
- Managing rules requires admin permission.

Permissions:
Add:
- content-quality.read
- content-quality.run
- content-quality.manage

Role mapping:
- SuperAdmin: all
- Admin: all
- ContentEditor: read and run
- Reviewer: read and run
- Viewer: no access unless existing pattern allows read-only admin access

Blazor Requirements:

Create admin pages:
- /admin/content-quality
- /admin/content-quality/checks
- /admin/content-quality/checks/{id:guid}
- /admin/content-quality/rules
- /admin/content-quality/rules/create
- /admin/content-quality/rules/{id:guid}/edit

Content Quality Dashboard:
- Total checks
- Failed checks
- Warning checks
- Critical findings
- Recent findings
- Quality score summary

Content detail integration:
Add simple quality status display to detail pages if safe:
- Word detail
- Lesson detail
- Course detail
- Book detail
- Quiz detail

Add button:
- Run Quality Check

Update:
- AdminRoutes constants
- Admin navigation
- Admin dashboard if safe

Testing Requirements:
Add tests for:
- ContentQualityRule creation
- Run quality check for Word with missing IPA
- Run quality check for Lesson with no sections
- Run quality check for Quiz with no correct choice
- Quality score calculation
- Mark finding resolved
- Permission checks
- Existing modules still pass

Quality:
- Keep implementation simple.
- Rule-based only.
- Do not add AI.
- Do not overengineer.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Quality service summary
4. API endpoints created
5. Blazor pages created
6. Permissions added
7. Tests created
8. Build/test result
9. Remaining limitations