# Release Container Smoke Execution - 2026-08-03

## Result

The disposable CI release environment passed for application revision `2784568`.

| Gate | Result | Evidence |
| --- | --- | --- |
| Build and Windows test matrix | Passed | [Build run 30828846655](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828846655) |
| Release build and test matrix | Passed | [Release Build run 30828858339](https://github.com/mysunyourmoon30-creator/EnglishMaster/actions/runs/30828858339) |
| Linux migration bundle | Passed | The workflow built and uploaded the self-contained reviewed EF migration bundle. |
| Fresh SQL Server migration | Passed | The custom SQL Server 2022 image installed Full-Text Search, and the migration bundle completed against a fresh database. |
| API and Web containers | Passed | Both release images built, started, and reached their readiness gates. |
| Authenticated release smoke | Passed | Health, public grammar, redirect, protected API rejection, login, admin dashboard, and logout checks completed successfully. |
| Cleanup | Passed | The workflow removed the disposable containers and volumes. |

## Repair Record

The Windows SQL Server runner does not provide Full-Text Search, so the Windows test jobs exclude only `FreshDatabaseMigrationTests`. All remaining tests still execute there. The release container job retains fresh-database migration coverage in an environment that installs `mssql-server-fts`; no assertion or migration check was weakened.

The stock SQL Server container also lacked Full-Text Search. The Staging and Production Compose definitions now build `docker/sqlserver/Dockerfile`, which configures the Microsoft SQL Server 2022 Ubuntu package repository and installs the Full-Text package before database startup.

## Remaining Release Gate

This run proves the release path in a disposable CI network; it is not evidence of deployment to a persistent staging environment. As of 2026-08-03, the repository has no configured staging host or URL, GitHub Environment, deployment secret, or deployment workflow. Persistent deployment and live browser UAT must be completed and recorded before V1-F06 or any later product feature begins.
