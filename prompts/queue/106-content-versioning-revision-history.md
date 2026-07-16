Create Content Versioning and Revision History for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Content Quality / QA System completed.
- Production readiness completed.

Goal:
Add simple content revision history for existing content modules.

Important:
Do not add AI.
Do not add advanced audit platform.
Do not add marketplace.
Do not add mobile.
Do not add payment.
Do not add microservices.
Do not redesign architecture.

Scope:
Create simple revision history for content changes.

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
- PublishTemplate

Revision Events:
- Created
- Updated
- SubmittedForReview
- Approved
- ChangesRequested
- Published
- Archived
- QualityChecked
- Restored

Domain Entities:

1. ContentRevision

Fields:
- Id
- ContentType
- ContentId
- RevisionNumber
- EventType
- Title
- Summary
- ChangedBy
- ChangedAt
- ChangeReason
- SnapshotJson
- DiffJson
- CreatedAt
- UpdatedAt

Rules:
- ContentType is required.
- ContentId is required.
- RevisionNumber must be greater than zero.
- EventType is required.
- ChangedAt is required.
- SnapshotJson stores a safe serialized snapshot.
- DiffJson is optional.
- Do not store passwords, tokens, secrets, or sensitive auth fields.

2. ContentRevisionRestoreRequest

Fields:
- Id
- ContentRevisionId
- RequestedBy
- RequestedAt
- Reason
- Status
- ReviewedBy
- ReviewedAt
- ReviewNote
- CreatedAt
- UpdatedAt

Status values:
- Pending
- Approved
- Rejected
- Completed
- Cancelled

Rules:
- ContentRevisionId is required.
- RequestedBy is required.
- Reason is required.
- Restore request starts as Pending.
- Approved restore can be completed.
- Rejected restore must keep review note if provided.

Application Requirements:

Create CQRS-style folders:
- Features/ContentRevisions/Commands
- Features/ContentRevisions/Queries
- Features/ContentRevisions/Dtos
- Features/ContentRevisionRestores/Commands
- Features/ContentRevisionRestores/Queries
- Features/ContentRevisionRestores/Dtos

Use cases:
- CreateContentRevision
- GetContentRevisionById
- GetRevisionsForContent
- SearchContentRevisions
- GetLatestRevisionForContent
- CreateRestoreRequest
- ApproveRestoreRequest
- RejectRestoreRequest
- CompleteRestoreRequest
- SearchRestoreRequests

Service Abstractions:
Create:
- IContentRevisionService
- IContentSnapshotSerializer

Revision Capture:
Add revision capture for key actions if safe:
- content create
- content update
- submit for review
- approve
- request changes
- publish
- archive
- quality check

If full integration is too large, implement for:
- Word
- Lesson
- Course
- Book
- Quiz

and document TODO for the rest.

Infrastructure Requirements:
- Create EF Core configuration for ContentRevision.
- Create EF Core configuration for ContentRevisionRestoreRequest.
- Configure indexes:
  - ContentRevision ContentType + ContentId
  - ContentRevision RevisionNumber
  - ContentRevision EventType
  - ContentRevision ChangedBy
  - ContentRevision ChangedAt
  - RestoreRequest ContentRevisionId
  - RestoreRequest Status
  - RestoreRequest RequestedBy
  - RestoreRequest RequestedAt
- Add migrations if EF Core tools are available.
- Do not break existing modules.

API Requirements:

Create endpoints:
- GET /api/v1/content-revisions
- GET /api/v1/content-revisions/{id}
- GET /api/v1/content-revisions/{contentType}/{contentId}
- GET /api/v1/content-revisions/{contentType}/{contentId}/latest

Restore endpoints:
- GET /api/v1/content-revision-restore-requests
- POST /api/v1/content-revision-restore-requests
- POST /api/v1/content-revision-restore-requests/{id}/approve
- POST /api/v1/content-revision-restore-requests/{id}/reject
- POST /api/v1/content-revision-restore-requests/{id}/complete

Security:
- All revision endpoints require authentication.
- Reading revisions requires permission.
- Restore requests require permission.
- Approving restore requests requires admin/reviewer permission.
- Do not expose sensitive fields in snapshots.

Permissions:
Add:
- content-revisions.read
- content-revisions.restore.request
- content-revisions.restore.approve
- content-revisions.manage

Role mapping:
- SuperAdmin: all
- Admin: all
- Reviewer: read + approve restore if appropriate
- ContentEditor: read + request restore
- Viewer: no access unless existing pattern allows read-only admin

Blazor Requirements:

Create admin pages:
- /admin/content-revisions
- /admin/content-revisions/{id:guid}
- /admin/content-revisions/{contentType}/{contentId}
- /admin/content-revision-restores
- /admin/content-revision-restores/{id:guid}

Revision UI:
- Show revision timeline.
- Show event type.
- Show changed by.
- Show changed at.
- Show reason/note.
- Show snapshot summary.
- Show diff if available.

Content detail integration:
Add simple Revision History link or panel to:
- Word detail
- Lesson detail
- Course detail
- Book detail
- Quiz detail

Update:
- AdminRoutes constants
- Admin navigation
- Admin dashboard if safe

Testing Requirements:
Add tests for:
- Create revision
- Revision number increments
- Get revisions by content
- Snapshot does not expose sensitive fields
- Create restore request
- Approve restore request
- Reject restore request
- Permission checks
- Existing modules still pass

Quality:
- Keep implementation simple.
- Do not overengineer.
- Do not implement complex visual diff unless simple.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Revision service summary
4. API endpoints created
5. Blazor pages created
6. Permissions added
7. Tests created
8. Build/test result
9. Remaining limitations