# Production File Storage

## Storage Paths

The API uses configurable local paths for uploaded media and publishing artifacts:

```text
Media__LocalStoragePath
Publishing__LocalStoragePath
```

Both paths must point to durable storage in production.

## Media Storage

Uploaded media is stored under `Media__LocalStoragePath` and served through `/media`. Upload handling validates file names, allowed content types, content signatures, and maximum file size. Stored file names are generated server-side.

## Publishing Storage

Published artifacts are stored under `Publishing__LocalStoragePath` and served through `/publishing`. Internal file paths should not be exposed when a public URL is sufficient.

## Backup Requirements

Back up:

- Uploaded media files.
- Publishing artifacts.
- SQL Server database records that reference those files.

Database and file backups should be coordinated so file references remain consistent after restore.

See also [File Storage Backup](../operations/file-storage-backup.md).

## Known Limitations

- Storage is local-path based in this version.
- Cloud object storage is not implemented yet.
- Malware scanning is not implemented yet.
