Review EnglishMaster v0.3.0 Architecture and Risk before implementation.

Project:
EnglishMaster

Current Status:
- v0.3.0 scope decision completed.
- v0.3.0 MVP slice completed.

Goal:
Check whether the selected v0.3.0 scope fits the existing architecture safely.

Important:
Do not implement features.
Do not write production code.
Do not add migrations.
Architecture review only.

Review selected modules:
- Email Delivery Module
- Notification Delivery Module
- Certificate Module
- Certificate Verification Module
- Analytics Improvement Module

Check:

1. Clean Architecture Fit
Verify:
- Domain entities remain independent.
- Application owns use cases.
- Infrastructure owns external provider implementation.
- API exposes safe endpoints.
- Blazor uses API/Application abstractions.
- No direct DbContext access from Blazor pages.

2. Email Provider Risk
Review:
- SMTP/provider configuration
- Secrets management
- retry behavior
- failed email handling
- email template safety
- no password/token logging
- no provider-specific lock-in if avoidable

3. Certificate Risk
Review:
- certificate generation
- certificate verification code
- public verification safety
- student privacy
- PDF generation limitation
- template safety
- duplicate certificate prevention

4. Analytics Risk
Review:
- query performance
- data privacy
- aggregation only
- no sensitive student detail exposure
- pagination/limits
- no expensive dashboard queries

5. Security Review
Verify planned rules:
- Admin certificate management requires permission.
- Student certificate list requires authentication.
- Public certificate verification exposes only safe data.
- Email admin pages require permission.
- Email settings do not expose secrets.
- Analytics endpoints require permission.

6. Operational Review
Review:
- environment variables
- provider configuration
- email delivery monitoring
- certificate storage/backup
- failure handling
- deployment impact
- rollback impact

7. Testing Strategy
Define test categories:
- Unit tests
- Integration tests
- Authorization tests
- Public verification safety tests
- Email sender mock tests
- Certificate generation tests
- Analytics query tests

8. Create docs:
- docs/architecture/v0.3.0-architecture-review.md
- docs/security/v0.3.0-security-review.md
- docs/operations/v0.3.0-operational-risk.md
- docs/testing/v0.3.0-test-strategy.md

Output:
1. Architecture fit result
2. Email risk summary
3. Certificate risk summary
4. Analytics risk summary
5. Security risk summary
6. Operational risk summary
7. Testing strategy summary
8. Go / No-Go recommendation for implementation

Do not implement any feature.