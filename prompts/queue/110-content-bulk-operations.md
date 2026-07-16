Create Content Bulk Operations for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review Workflow completed.
- Content Quality / QA System completed.
- Content Versioning / Revision History completed.

Goal:
Add simple bulk operations for managing multiple content items efficiently.

Important:
Do not add AI.
Do not add marketplace.
Do not add mobile.
Do not add payment.
Do not add microservices.
Do not redesign architecture.
Do not add background queue unless already simple and available.

Scope:
Create bulk operations for existing content modules only.

Target Content Types:
- Word
- Pronunciation
- GrammarTopic
- GrammarRule
- Lesson
- Course
- Book
- Quiz

Supported Bulk Actions:
- SubmitForReview
- Approve
- RequestChanges
- Publish
- Archive
- Activate
- Deactivate
- RunQualityCheck
- AssignCategory
- AddTags
- RemoveTags
- Export

Domain Entities:

1. BulkOperation

Fields:
- Id
- OperationType
- ContentType
- Status
- RequestedBy
- RequestedAt
- StartedAt
- CompletedAt
- TotalItems
- SucceededItems
- FailedItems
- ErrorMessage
- CreatedAt
- UpdatedAt

Status values:
- Pending
- Running
- Completed
- Failed
- PartiallyCompleted
- Cancelled

Rules:
- OperationType is required.
- ContentType is required.
- Status is required.
- BulkOperation starts as Pending.
- TotalItems must not be negative.
- SucceededItems must not be negative.
- FailedItems must not be negative.
- CompletedAt is set when completed, failed, or partially completed.

2. BulkOperationItem

Fields:
- Id
- BulkOperationId
- ContentId
- Status
- ErrorMessage
- CreatedAt
- UpdatedAt

Status values:
- Pending
- Success
- Failed
- Skipped

Rules:
- BulkOperationItem belongs to one BulkOperation.
- ContentId is required.
- Status is required.

Application Requirements:

Create CQRS-style folders:
- Features/BulkOperations/Commands
- Features/BulkOperations/Queries
- Features/BulkOperations/Dtos

Use cases:
- CreateBulkOperation
- RunBulkOperation
- GetBulkOperationById
- SearchBulkOperations
- GetBulkOperationItems
- CancelBulkOperation

Bulk action handlers:
- BulkSubmitForReview
- BulkApprove
- BulkRequestChanges
- BulkPublish
- BulkArchive
- BulkActivate
- BulkDeactivate
- BulkRunQualityCheck
- BulkAssignCategory
- BulkAddTags
- BulkRemoveTags
- BulkExport

If full support for all content types is too large, implement first for:
- Word
- Lesson
- Course
- Book
- Quiz

and document TODO for remaining content types.

Infrastructure Requirements:
- Create EF Core configuration for BulkOperation.
- Create EF Core configuration for BulkOperationItem.
- Configure relationship:
  - BulkOperation one-to-many BulkOperationItem
- Configure indexes:
  - BulkOperation OperationType
  - BulkOperation ContentType
  - BulkOperation Status
  - BulkOperation RequestedBy
  - BulkOperation RequestedAt
  - BulkOperationItem BulkOperationId
  - BulkOperationItem ContentId
  - BulkOperationItem Status
- Add migrations if EF Core tools are available.
- Do not break existing modules.

API Requirements:

Create endpoints:
- GET /api/v1/bulk-operations
- GET /api/v1/bulk-operations/{id}
- GET /api/v1/bulk-operations/{id}/items
- POST /api/v1/bulk-operations
- POST /api/v1/bulk-operations/{id}/run
- POST /api/v1/bulk-operations/{id}/cancel

Create request DTO that supports:
- operationType
- contentType
- contentIds
- note
- categoryId if AssignCategory
- tagIds if AddTags or RemoveTags
- exportFormat if Export

Security:
- All bulk operation endpoints require authentication.
- Bulk actions require permissions matching the underlying action.
- Bulk publish requires content.review.publish or equivalent publish permission.
- Bulk approve requires content.review.approve.
- Bulk archive requires content.review.archive.
- Bulk quality check requires content-quality.run.
- Bulk export requires export permission if existing.
- User must not bypass normal workflow rules through bulk operations.

Permissions:
Add:
- bulk-operations.read
- bulk-operations.run
- bulk-operations.cancel

Role mapping:
- SuperAdmin: all
- Admin: all
- ContentEditor: limited run for submit/review/quality/export if appropriate
- Reviewer: approve/request changes/quality if appropriate
- Viewer: no bulk operation access

Blazor Requirements:

Create admin pages:
- /admin/bulk-operations
- /admin/bulk-operations/create
- /admin/bulk-operations/{id:guid}

Bulk Operation list:
- Filter by OperationType
- Filter by ContentType
- Filter by Status
- Show requested by
- Show requested at
- Pagination

Create Bulk Operation page:
- Select content type
- Select operation type
- Select content items
- Input note
- Select category/tags/export format when needed
- Submit

Bulk Operation detail:
- Show status
- Show total/succeeded/failed items
- Show item results
- Show errors
- Run button if Pending
- Cancel button if allowed

Content list integration:
If safe and simple, add checkboxes and bulk action button to:
- Words
- Lessons
- Courses
- Books
- Quizzes

Otherwise create standalone bulk operation page and document limitation.

Update:
- AdminRoutes constants
- Admin navigation
- Admin dashboard if safe

Testing Requirements:
Add tests for:
- Create bulk operation
- Run bulk quality check
- Bulk publish respects workflow rules
- Bulk operation records item success/failure
- Bulk operation partially completed status
- Permission checks
- Existing modules still pass

Quality:
- Keep implementation simple.
- Do not overengineer.
- Avoid long-running complexity.
- Do not add background queue unless already available.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Domain entities created
3. Bulk operation service summary
4. API endpoints created
5. Blazor pages created
6. Permissions added
7. Tests created
8. Build/test result
9. Remaining limitations