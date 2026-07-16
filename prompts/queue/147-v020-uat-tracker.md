Prepare and execute v0.2.0 UAT tracking for EnglishMaster.

Project:
EnglishMaster

Current Status:
- v0.2.0 Release Candidate prepared.
- Smoke test has been executed or is ready to execute.
- Feature freeze is active.

Goal:
Prepare UAT execution tracking and record UAT results clearly.

Important:
Do not add new features.
Do not redesign architecture.
Do not mark manual UAT as passed unless there is evidence.
Do not change application code unless fixing broken docs or obvious blockers.

Tasks:

1. Read:
- docs/testing/v0.2.0-uat-plan.md
- docs/testing/v0.2.0-regression-checklist.md
- docs/testing/v0.2.0-smoke-test-results.md
- docs/release/v0.2.0-known-limitations.md

2. Create UAT result document:
- docs/testing/v0.2.0-uat-results.md

Include sections for:
- UAT scope
- UAT environment
- Test roles
- Test data
- Admin scenarios
- Reviewer scenarios
- ContentEditor scenarios
- Student scenarios
- Public visitor scenarios
- Passed scenarios
- Failed scenarios
- Deferred scenarios
- Known limitations
- Sign-off status

3. Create defect tracker document:
- docs/testing/v0.2.0-defect-log.md

Defect format:
- Defect ID
- Title
- Area
- Severity
- Priority
- Steps to reproduce
- Expected result
- Actual result
- Status
- Owner
- Notes

Severity:
- Blocker
- High
- Medium
- Low

Status:
- Open
- In Progress
- Fixed
- Retest
- Closed
- Deferred

4. Populate UAT scenarios from UAT plan:
Admin:
- Manage words
- Manage grammar
- Manage lessons
- Manage courses
- Manage books
- Manage quizzes
- Review content
- Publish content
- Run quality check
- Run import validation
- Run bulk operation
- View reports

Reviewer:
- View review queue
- Approve content
- Request changes
- View quality findings

ContentEditor:
- Create/edit content
- Submit for review
- View revision history
- Run quality check if allowed

Student:
- Login
- View dashboard
- Continue learning
- Search content
- Start lesson
- Complete lesson
- Take quiz
- Practice session
- Study plan
- Goals
- Achievements
- Weekly report

Public visitor:
- Home page
- Public search
- Dictionary
- Course detail
- Lesson detail
- Book detail
- Grammar page
- Quiz preview safety

5. Mark status honestly:
- Not Run if not manually tested.
- Passed only if evidence exists.
- Failed if defect exists.
- Deferred if intentionally delayed.

6. Run:
- dotnet build
- dotnet test

7. Do not fix feature issues in this prompt unless they block UAT documentation.

Output:
1. UAT result file created
2. Defect log file created
3. UAT scenarios prepared
4. Current UAT status
5. Build/test result
6. Open blockers
7. Recommended next step