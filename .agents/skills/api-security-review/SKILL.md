---
name: api-security-review
description: Use for security review or hardening of EnglishMaster ASP.NET Core 9 APIs, endpoints, authentication, authorization, Blazor server interactions, input validation, EF Core data access, secrets, CORS, logging, and common web vulnerabilities.
---

# API Security Review

## Project Context

EnglishMaster is an ASP.NET Core 9, Blazor, EF Core, SQL Server application using Clean Architecture and a modular monolith style. Review security at the boundary where user input, identity, data access, and module ownership meet.

## Review Workflow

1. Inventory the affected endpoints, pages, handlers, services, policies, and data paths.
2. Identify trust boundaries: browser to server, anonymous to authenticated, user to admin, module to module, and application to database.
3. Check authentication and authorization first. Confirm protected operations require the right user, role, policy, or ownership check.
4. Check input handling: validation, normalization, size limits, model binding, over-posting, file handling, and error behavior.
5. Check data access: tenant or owner filtering, SQL injection resistance, sensitive field exposure, mass assignment, pagination, and query bounds.
6. Check platform configuration: HTTPS, cookies, anti-forgery, CORS, security headers, rate limiting, secrets, logging, and exception handling.
7. Report findings before broad cleanup. Include file and line references when possible.

## Finding Format

Lead with risks, ordered by severity:

- Severity and short title.
- File and line reference.
- Why it is exploitable or risky.
- A concrete fix.
- Suggested verification.

If no issues are found, say that clearly and note any review limits.

## Fix Rules

- Prefer framework-supported security controls over custom checks.
- Keep authorization close to the operation being protected.
- Do not log secrets, tokens, passwords, connection strings, or personal data.
- Do not expose internal exception details to users.
- Do not rely on Blazor UI visibility as the only access control.
- Keep security fixes scoped and covered by focused tests where feasible.
