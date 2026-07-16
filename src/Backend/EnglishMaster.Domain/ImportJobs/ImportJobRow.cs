namespace EnglishMaster.Domain.ImportJobs;

public sealed class ImportJobRow
{
    private readonly List<ImportValidationError> validationErrors = [];

    private ImportJobRow()
    {
        RawDataJson = string.Empty;
        ParsedDataJson = string.Empty;
        ErrorMessage = string.Empty;
        CreatedEntityType = string.Empty;
        UpdatedEntityType = string.Empty;
    }

    private ImportJobRow(Guid importJobId, int rowNumber, string rawDataJson, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ImportJobId = importJobId == Guid.Empty ? throw new ArgumentException("ImportJobId is required.", nameof(importJobId)) : importJobId;
        RowNumber = rowNumber <= 0 ? throw new ArgumentException("RowNumber must be greater than zero.", nameof(rowNumber)) : rowNumber;
        RawDataJson = string.IsNullOrWhiteSpace(rawDataJson) ? throw new ArgumentException("RawDataJson is required.", nameof(rawDataJson)) : rawDataJson;
        ParsedDataJson = string.Empty;
        Status = ImportJobRowStatus.Pending;
        ErrorMessage = string.Empty;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid ImportJobId { get; private set; }
    public int RowNumber { get; private set; }
    public string RawDataJson { get; private set; } = string.Empty;
    public string ParsedDataJson { get; private set; } = string.Empty;
    public ImportJobRowStatus Status { get; private set; }
    public string ErrorMessage { get; private set; } = string.Empty;
    public string CreatedEntityType { get; private set; } = string.Empty;
    public Guid? CreatedEntityId { get; private set; }
    public string UpdatedEntityType { get; private set; } = string.Empty;
    public Guid? UpdatedEntityId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<ImportValidationError> ValidationErrors => validationErrors.AsReadOnly();

    public static ImportJobRow Create(Guid importJobId, int rowNumber, string rawDataJson, DateTimeOffset now) =>
        new(importJobId, rowNumber, rawDataJson, now);

    public void MarkValid(string parsedDataJson, DateTimeOffset now)
    {
        ParsedDataJson = parsedDataJson;
        Status = ImportJobRowStatus.Valid;
        ErrorMessage = string.Empty;
        UpdatedAt = now;
    }

    public void MarkInvalid(string errorMessage, DateTimeOffset now)
    {
        Status = ImportJobRowStatus.Invalid;
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "Row is invalid." : errorMessage.Trim();
        UpdatedAt = now;
    }

    public ImportValidationError AddError(string fieldName, string errorCode, string errorMessage, ImportValidationSeverity severity, DateTimeOffset now)
    {
        var error = ImportValidationError.Create(Id, fieldName, errorCode, errorMessage, severity, now);
        validationErrors.Add(error);
        if (severity is ImportValidationSeverity.Error or ImportValidationSeverity.Critical)
        {
            MarkInvalid(errorMessage, now);
        }

        return error;
    }

    public void MarkImported(string entityType, Guid entityId, DateTimeOffset now)
    {
        Status = ImportJobRowStatus.Imported;
        CreatedEntityType = entityType;
        CreatedEntityId = entityId;
        ErrorMessage = string.Empty;
        UpdatedAt = now;
    }

    public void MarkFailed(string errorMessage, DateTimeOffset now)
    {
        Status = ImportJobRowStatus.Failed;
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "Row import failed." : errorMessage.Trim();
        UpdatedAt = now;
    }

    public void MarkRolledBack(DateTimeOffset now)
    {
        Status = ImportJobRowStatus.RolledBack;
        UpdatedAt = now;
    }
}
