# Native Local Browser UAT - 2026-08-04

## Result

The no-Docker local UAT passed for the current MVP release candidate.

The API and Web applications ran as native .NET Release processes on loopback.
The API used the Development in-memory database and development seed data. This
environment was intentionally temporary and was not exposed to the Internet.

| Gate | Result | Evidence |
| --- | --- | --- |
| API/Web health | Passed | API and Web liveness/readiness endpoints returned `200`. |
| Automated authenticated smoke | Passed | All 12 checks passed: health, public grammar APIs/pages, anonymous admin redirect, protected API `401`, admin login, dashboard API, and logout. |
| Public grammar browser flow | Passed | Present Simple topic and rule pages rendered their English/Thai explanations and examples. |
| Admin authentication | Passed | Anonymous admin access redirected to login; an authenticated SuperAdmin loaded `/admin`; browser logout returned to `/login`. |
| Admin list routes | Passed | Dashboard plus all 28 documented MVP admin list routes opened without an authentication redirect, fatal error, or HTTP 500 page. |
| Representative content workflow | Passed | A temporary Category was created, opened in Detail, edited, and confirmed in the list. |
| Media validation | Passed | `README.md` was rejected with `ContentType is not allowed`; the 1.12 KB project `favicon.png` was accepted and produced a View Media link. |
| Import validation | Passed | A two-row invalid Words CSV produced `ValidationFailed`, 2 invalid rows, and row errors `WORD_TEXT_REQUIRED`, `WORD_MEANING_TH_REQUIRED`, and `WORD_CEFR_INVALID`. |
| Browser diagnostics | Passed | No browser console warnings or errors were recorded in the final check. |
| Cleanup | Passed | The two native .NET processes were stopped and the temporary runtime directory, logs, credentials, uploaded media, and in-memory test data were removed. |

## Database And Migration Boundary

This local run does not claim SQL Server or migration coverage because it used
the in-memory development provider. SQL Server 2022 Full-Text, the reviewed EF
migration bundle, fresh-database migration, API/Web release images, readiness,
and authenticated smoke remain covered by [Release Build run 30828858339](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828858339).

## Environment Boundary

This run is release-confidence evidence, not a persistent staging deployment.
No staging hostname, public URL, hosted Linux machine, deployment credential,
or GitHub Environment is configured. The release owner chose a no-cost,
no-Docker local verification path, so no paid VPS or Docker Desktop dependency
was introduced.

V1-F06 and later product work remain gated until the release owner either:

1. provides an approved persistent staging target and completes the same smoke
   checks there; or
2. explicitly accepts this local UAT plus disposable CI staging as the release
   gate for the MVP.
