Improve the Publishing Module for EnglishMaster v0.2.0.

Project:
EnglishMaster

Current Status:
- MVP v0.1.0 completed.
- Content Review and Approval Workflow completed.
- Publishing Module foundation already exists.

Goal:
Improve publishing workflow, templates, preview, and export behavior without overengineering.

Important:
Do not add AI.
Do not add Mobile.
Do not add Marketplace.
Do not add Payment.
Do not add Microservices.
Do not redesign the architecture.

Scope:
Improve the existing Publishing Module only.

Supported Source Types:
- Lesson
- Course
- Book
- Quiz

Supported Formats:
- Html
- Markdown
- Pdf placeholder if not fully supported
- Docx placeholder if not fully supported

Publishing Rules:
- Only Approved or Published content should be publishable where review workflow exists.
- Draft content should not be published.
- InReview content should not be published.
- ChangesRequested content should not be published.
- Archived content should not be published.
- If a content type does not support review status yet, document the limitation.

Enhancements Required:

1. Publish Preview
Add preview support before running final publish.

Create use cases:
- PreviewPublishContent
- GetPublishPreviewByJobId if useful

Preview should:
- Build content from source.
- Apply selected template if available.
- Return preview HTML or Markdown.
- Not create final PublishedArtifact unless explicitly published.

2. Template Selection
Update PublishJob to optionally support:
- PublishTemplateId

Rules:
- If template is selected, use it.
- If no template is selected, use default template for format.
- If no default template exists, use simple fallback template.

3. Template Variables
Support simple template variables:
- {{Title}}
- {{Summary}}
- {{Description}}
- {{Content}}
- {{GeneratedAt}}
- {{SourceType}}
- {{CefrLevel}}

Keep it simple. Do not implement complex template engine unless already available.

4. Publishing Validation
Before running a publish job, validate:
- Source exists.
- Source is allowed to publish.
- Format is supported.
- Template format matches publish format.
- Output filename is safe.
- Output path prevents path traversal.

5. Publishing Output
Improve basic outputs:
- Html export should produce valid basic HTML.
- Markdown export should produce readable Markdown.
- Pdf and Docx can remain documented placeholders if no rendering library exists.

6. API Requirements
Add or verify endpoints:
- POST /api/v1/publish-jobs/{id}/preview
- GET /api/v1/publish-jobs/{id}/preview if persisted or simple
- POST /api/v1/publish-jobs/{id}/run
- Existing publish job/template/artifact endpoints must still work.

7. Blazor Requirements
Update Publishing admin pages:
- Create publish job page allows selecting template.
- Publish job detail page shows:
  - Source type
  - Source id
  - Format
  - Template
  - Status
  - Preview button
  - Run button
  - Artifacts
  - Error message
- Add preview panel or preview page.
- Show validation errors clearly.

8. Permissions
Use existing publishing permissions:
- publishing.read
- publishing.create
- publishing.update
- publishing.run

Preview should require publishing.read or publishing.run depending on existing pattern.
Run should require publishing.run.

9. Tests
Add tests for:
- Cannot publish Draft content.
- Can publish Approved content.
- Can publish Published content.
- Template selection works.
- Default template fallback works.
- HTML preview works.
- Markdown preview works.
- Unsafe filename/path is rejected.
- Existing modules still pass.

Quality:
- Keep changes minimal.
- Do not overengineer.
- Run dotnet build.
- Run dotnet test.
- Fix errors until everything passes.

Output:
1. Files created or modified
2. Publishing improvements summary
3. Preview support summary
4. Template selection summary
5. API endpoints updated
6. Blazor pages updated
7. Tests created
8. Build/test result
9. Remaining limitations