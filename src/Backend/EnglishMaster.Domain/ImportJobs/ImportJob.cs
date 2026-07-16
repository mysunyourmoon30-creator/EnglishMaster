namespace EnglishMaster.Domain.ImportJobs;

public sealed class ImportJob
{
    private readonly List<ImportJobRow> rows = [];

    private ImportJob()
    {
        ImportType = string.Empty;
        Format = string.Empty;
        FileName = string.Empty;
        OriginalFileName = string.Empty;
        RequestedBy = string.Empty;
        ErrorMessage = string.Empty;
    }

    private ImportJob(string importType, string format, string fileName, string originalFileName, long fileSize, string? requestedBy, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ImportType = Required(importType, nameof(ImportType), 64);
        Format = Required(format, nameof(Format), 16);
        Status = ImportJobStatus.Uploaded;
        FileName = Required(fileName, nameof(FileName), 260);
        OriginalFileName = Required(originalFileName, nameof(OriginalFileName), 260);
        FileSize = fileSize <= 0 ? throw new ArgumentException("FileSize must be greater than zero.", nameof(fileSize)) : fileSize;
        RequestedBy = Optional(requestedBy, nameof(RequestedBy), 256);
        RequestedAt = now;
        CreatedAt = now;
        UpdatedAt = now;
        ErrorMessage = string.Empty;
    }

    public Guid Id { get; private set; }
    public string ImportType { get; private set; } = string.Empty;
    public string Format { get; private set; } = string.Empty;
    public ImportJobStatus Status { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string OriginalFileName { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? ValidatedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? FailedAt { get; private set; }
    public DateTimeOffset? RolledBackAt { get; private set; }
    public int TotalRows { get; private set; }
    public int ValidRows { get; private set; }
    public int InvalidRows { get; private set; }
    public int ImportedRows { get; private set; }
    public int FailedRows { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<ImportJobRow> Rows => rows.AsReadOnly();

    public static ImportJob Create(string importType, string format, string fileName, string originalFileName, long fileSize, string? requestedBy, DateTimeOffset now) =>
        new(importType, format, fileName, originalFileName, fileSize, requestedBy, now);

    public void AddRow(int rowNumber, string rawDataJson, DateTimeOffset now)
    {
        rows.Add(ImportJobRow.Create(Id, rowNumber, rawDataJson, now));
        TotalRows = rows.Count;
        UpdatedAt = now;
    }

    public void StartValidation(DateTimeOffset now)
    {
        if (Status != ImportJobStatus.Uploaded)
        {
            throw new InvalidOperationException("Import validation can only start from an uploaded job.");
        }

        Status = ImportJobStatus.Validating;
        UpdatedAt = now;
    }

    public void FinishValidation(DateTimeOffset now)
    {
        ValidRows = rows.Count(row => row.Status == ImportJobRowStatus.Valid);
        InvalidRows = rows.Count(row => row.Status == ImportJobRowStatus.Invalid);
        Status = InvalidRows > 0 ? ImportJobStatus.ValidationFailed : ImportJobStatus.PreviewReady;
        ValidatedAt = now;
        UpdatedAt = now;
    }

    public void Confirm(DateTimeOffset now)
    {
        if (Status != ImportJobStatus.PreviewReady)
        {
            throw new InvalidOperationException("Import can only be confirmed after preview is ready.");
        }

        Status = ImportJobStatus.Confirmed;
        ConfirmedAt = now;
        UpdatedAt = now;
    }

    public void StartImport(DateTimeOffset now)
    {
        if (Status != ImportJobStatus.Confirmed)
        {
            throw new InvalidOperationException("Import can only run after confirmation.");
        }

        Status = ImportJobStatus.Importing;
        UpdatedAt = now;
    }

    public void FinishImport(DateTimeOffset now)
    {
        ImportedRows = rows.Count(row => row.Status == ImportJobRowStatus.Imported);
        FailedRows = rows.Count(row => row.Status == ImportJobRowStatus.Failed);
        Status = FailedRows > 0 ? ImportJobStatus.Failed : ImportJobStatus.Completed;
        CompletedAt = Status == ImportJobStatus.Completed ? now : null;
        FailedAt = Status == ImportJobStatus.Failed ? now : null;
        ErrorMessage = FailedRows > 0 ? $"{FailedRows} row(s) failed." : string.Empty;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now)
    {
        if (Status is ImportJobStatus.Completed or ImportJobStatus.RolledBack)
        {
            throw new InvalidOperationException("Completed imports cannot be cancelled.");
        }

        Status = ImportJobStatus.Cancelled;
        UpdatedAt = now;
    }

    public void MarkRolledBack(DateTimeOffset now)
    {
        if (Status != ImportJobStatus.Completed)
        {
            throw new InvalidOperationException("Only completed imports can be rolled back.");
        }

        foreach (var row in rows.Where(row => row.Status == ImportJobRowStatus.Imported))
        {
            row.MarkRolledBack(now);
        }

        Status = ImportJobStatus.RolledBack;
        RolledBackAt = now;
        UpdatedAt = now;
    }

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = Optional(value, fieldName, maxLength);
        return normalized.Length == 0 ? throw new ArgumentException($"{fieldName} is required.", fieldName) : normalized;
    }

    private static string Optional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        return normalized.Length > maxLength ? throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName) : normalized;
    }
}
