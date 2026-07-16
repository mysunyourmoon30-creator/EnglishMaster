# Content Quality Rule

## Purpose

`ContentQualityRule` describes a named quality rule that can be managed by admins. The initial evaluator uses built-in rule logic, while stored rules provide a foundation for future configurable checks.

## Fields

| Field | Purpose |
| --- | --- |
| `Id` | Rule identifier. |
| `Code` | Unique stable rule code. |
| `Name` | Human-readable rule name. |
| `Description` | Optional rule explanation. |
| `ContentType` | Target content type. |
| `Severity` | Info, Warning, Error, or Critical. |
| `IsActive` | Whether the rule is active. |
| `CreatedAt` / `UpdatedAt` | Audit timestamps. |

## Behavior

- Code is required and unique.
- Name and content type are required.
- Severity must be valid.
- Rules support activate and deactivate.

## Adding A Rule

Admins can create rules from `/admin/content-quality/rules/create` or through `POST /api/v1/content-quality/rules`.
