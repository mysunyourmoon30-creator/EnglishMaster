Perform MVP admin UX consistency pass for EnglishMaster.

Goal:
Make the admin system consistent and easier to use without adding new business features.

Do not add new modules.
Do not redesign the app.
Do not change domain logic unless required for UI correctness.

Check and improve:

1. Admin Layout
- Page titles are consistent.
- Navigation labels are consistent.
- Admin menu order is logical.
- Current page indication works if supported.

2. Common Page Patterns
For all admin modules:
- List page
- Create page
- Edit page
- Detail page

Verify:
- loading state
- empty state
- error state
- validation message
- save button
- cancel/back button
- delete confirmation if available

3. Forms
- Required fields are clear.
- Validation messages are readable.
- Submit behavior is consistent.
- Error handling is safe.

4. Tables / Lists
- Search box placement is consistent.
- Filters are consistent.
- Pagination is consistent.
- Empty result message is clear.

5. Security UX
- Unauthorized pages show appropriate message or redirect.
- Viewer role cannot see dangerous actions if simple.
- Security pages are accessible only to proper roles.

6. Documentation
Update:
- docs/ui/admin-ux-guidelines.md
- docs/release/known-limitations.md if some UX limitations remain

7. Quality
Run:
- dotnet build
- dotnet test

Output:
1. UX issues found
2. UX fixes applied
3. Pages improved
4. Remaining UX limitations
5. Build/test result