# File Storage Backup

## Scope

Back up all durable file locations used by EnglishMaster:

- Uploaded media under `Media__LocalStoragePath`.
- Published artifacts under `Publishing__LocalStoragePath`.
- Import/export working files if an environment retains them outside temporary processing.

## Backup Strategy

- Back up file storage at least daily for production.
- Coordinate file backups with database backups so file references remain consistent.
- Keep retention aligned with database backup retention.
- Store backups in a location separate from the running app host.

## Restore Strategy

Restore database records and file storage together. If the database references files that are missing from restored storage, media previews and published artifact links can break.

## Future Cloud Storage Note

If cloud object storage is introduced later, document bucket/container versioning, lifecycle policies, restore procedure, access control, and key rotation.
