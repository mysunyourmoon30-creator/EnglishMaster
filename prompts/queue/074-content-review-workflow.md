Create Content Review and Approval Workflow for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Pilot testing preparation completed.
- Bug fix sprint completed.
- Admin UX consistency pass completed.
- v0.2.0 roadmap completed.

Goal:
Add a simple content review and approval workflow for existing content modules.

Important:
Do not add AI.
Do not add Mobile.
Do not add Marketplace.
Do not add Payment.
Do not add Microservices.
Do not redesign the architecture.

Scope:
Add review workflow to existing content modules only.

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

Workflow Status Values:
- Draft
- InReview
- ChangesRequested
- Approved
- Published
- Archived

Rules:
- New content should start as Draft where appropriate.
- Draft content can be submitted for review.
- InReview content can be approved.
- InReview content can request changes.
- ChangesRequested content can be edited and resubmitted.
- Approved content can be published.
- Published content can be archived.
- Archived content should not be edited unless restored if simple.
- Existing IsPublished fields should remain compatible.
- Do not break existing Publish / Unpublish behavior.
- Keep the implementation simple.

Domain Requirements:
Add a reusable review status concept.

Options:
- If the codebase already has enums/value objects, follow existing pattern.
- Otherwise create a simple ContentReviewStatus enum.

Add tracking fields where appropriate:
- ReviewStatus
- SubmittedForReviewAt
- SubmittedBy
- ReviewedAt
- ReviewedBy
- ReviewNote
- PublishedAt
- PublishedBy

Do not add sensitive personal data.

Application Requirements:
Create use cases:
- SubmitForReview
- ApproveContent
- RequestChanges
- PublishContent
- ArchiveContent

If one generic workflow service is simpler, create:
- IContentReviewWorkflowService

If generic implementation becomes too complex, implement per-module handlers for:
- Words
- GrammarRules
- Lessons
- Courses
- Books
- Quizzes

Permissions:
Add permission constants:
- content.review.submit
- content.review.approve
- content.review.request_changes
- content.review.publish
- content.review.archive

Update role permissions:
- SuperAdmin: all
- Admin: all content workflow permissions
- ContentEditor: submit for review
- Reviewer: approve, request changes
- Viewer: read only

API Requirements:
Create workflow endpoints if practical:

Generic style preferred:
- POST /api/v1/content-workflow/{contentType}/{id}/submit
- POST /api/v1/content-workflow/{contentType}/{id}/approve
- POST /api/v1/content-workflow/{contentType}/{id}/request-changes
- POST /api/v1/content-workflow/{contentType}/{id}/publish
- POST /api/v1/content-workflow/{contentType}/{id}/archive

If generic routing is difficult, create module-specific endpoints following existing patterns.

Security:
- Submit requires content.review.submit.
- Approve requires content.review.approve.
- Request changes requires content.review.request_changes.
- Publish requires content.review.publish.
- Archive requires content.review.archive.
- Do not allow unauthorized workflow transitions.

Blazor Requirements:
Add workflow UI to existing admin detail pages:
- Show current ReviewStatus.
- Show review note if available.
- Show Submit for Review button when allowed.
- Show Approve button when allowed.
- Show Request Changes button when allowed.
- Show Publish button when allowed.
- Show Archive button when allowed.

Add admin page:
- /admin/content-review

Content Review page should show:
- Items in Draft
- Items InReview
- Items ChangesRequested
- Items Approved
- Items Published
- Filter by content type
- Filter by status
- Search if simple

Update Admin Navigation:
- Add Content Review menu item.
- Update AdminRoutes constants.
- Update dashboard with counts if safe:
  - Draft items
  - InReview items
  - Approved items
  - Published items

Testing Requirements:
Add tests for:
- Draft to InReview
- InReview to Approved
- InReview to ChangesRequested
- Approved to Published
- Published to Archived
- Invalid transition blocked
- Permission check if existing test pattern supports it
- Existing modules still pass

Quality:
- Keep changes minimal.
- Do not overengineer.
- Preserve existing publish behavior.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Workflow status implementation
3. API endpoints created
4. Blazor pages updated
5. Permissions added
6. Dashboard/navigation updates
7. Tests created
8. Build/test result
9. Remaining limitations