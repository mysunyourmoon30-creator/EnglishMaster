Create EnglishMaster v0.3.0 Scope Decision and MVP Slice.

Project:
EnglishMaster

Current Status:
- v0.2.0 completed and closed.
- v0.3.0 roadmap prioritization completed.

Goal:
Define a clear, small, shippable v0.3.0 scope.

Important:
Do not implement features.
Do not write production code.
Do not add migrations.
Planning only.

Recommended default direction:
1. External Email Provider + Notification Delivery
2. Certificate System / Learning Completion Evidence
3. Advanced Analytics Foundation

Do not include unless explicitly selected:
- AI Tutor full implementation
- Payment
- Marketplace
- Mobile app
- Full external search engine

Tasks:

1. Define v0.3.0 Goal
Example:
"Make EnglishMaster more production-useful by delivering real notification delivery, completion evidence, and stronger admin/student insights."

2. Define Must Have
Include only small release-safe scope.

Suggested Must Have:
- External Email Provider abstraction
- SMTP provider or configurable email provider
- Email delivery status
- Retry failed email manually
- Notification delivery settings
- Certificate template foundation
- Generate certificate for completed course
- Student certificate page
- Admin certificate management page
- Analytics foundation improvements
- More useful admin metrics
- Student learning trend summary

3. Define Should Have
Suggested:
- Email test send page
- Email provider health check
- Certificate verification code
- Public certificate verification page
- Export analytics CSV

4. Define Could Have
Suggested:
- Better email templates
- Certificate PDF polish
- Weekly email summary
- Admin analytics filters

5. Define Won't Have for v0.3.0
Suggested:
- AI Tutor
- Mobile app
- Payment
- Marketplace
- External search engine
- Leaderboard/social features
- Multi-tenant SaaS

6. Create v0.3.0 module list
Suggested modules:
- Email Delivery Module
- Notification Delivery Module
- Certificate Module
- Certificate Verification Module
- Analytics Improvement Module

7. Define acceptance criteria
For each module, define:
- Functional acceptance criteria
- Security acceptance criteria
- Performance acceptance criteria
- Documentation acceptance criteria
- Test acceptance criteria

8. Create docs:
- docs/roadmap/v0.3.0-scope-decision.md
- docs/roadmap/v0.3.0-mvp-slice.md
- docs/roadmap/v0.3.0-acceptance-criteria.md
- docs/roadmap/v0.3.0-out-of-scope.md

Output:
1. v0.3.0 goal
2. Must Have list
3. Should Have list
4. Could Have list
5. Won't Have list
6. Module list
7. Acceptance criteria
8. Out-of-scope list
9. Final v0.3.0 scope recommendation

Do not implement any feature.