namespace EnglishMaster.Domain.ImportJobs;

public enum ImportJobStatus
{
    Uploaded = 1,
    Validating = 2,
    ValidationFailed = 3,
    PreviewReady = 4,
    Confirmed = 5,
    Importing = 6,
    Completed = 7,
    Failed = 8,
    RolledBack = 9,
    Cancelled = 10
}

public enum ImportJobRowStatus
{
    Pending = 1,
    Valid = 2,
    Invalid = 3,
    Imported = 4,
    Failed = 5,
    RolledBack = 6,
    Skipped = 7
}

public enum ImportValidationSeverity
{
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}
