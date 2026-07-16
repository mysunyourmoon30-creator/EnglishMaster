namespace EnglishMaster.Domain.ImportJobs;

public sealed class ImportValidationError
{
    private ImportValidationError()
    {
        FieldName = string.Empty;
        ErrorCode = string.Empty;
        ErrorMessage = string.Empty;
    }

    private ImportValidationError(Guid importJobRowId, string? fieldName, string errorCode, string errorMessage, ImportValidationSeverity severity, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        ImportJobRowId = importJobRowId == Guid.Empty ? throw new ArgumentException("ImportJobRowId is required.", nameof(importJobRowId)) : importJobRowId;
        FieldName = fieldName?.Trim() ?? string.Empty;
        ErrorCode = string.IsNullOrWhiteSpace(errorCode) ? throw new ArgumentException("ErrorCode is required.", nameof(errorCode)) : errorCode.Trim();
        ErrorMessage = string.IsNullOrWhiteSpace(errorMessage) ? throw new ArgumentException("ErrorMessage is required.", nameof(errorMessage)) : errorMessage.Trim();
        Severity = severity;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid ImportJobRowId { get; private set; }
    public string FieldName { get; private set; } = string.Empty;
    public string ErrorCode { get; private set; } = string.Empty;
    public string ErrorMessage { get; private set; } = string.Empty;
    public ImportValidationSeverity Severity { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static ImportValidationError Create(Guid importJobRowId, string? fieldName, string errorCode, string errorMessage, ImportValidationSeverity severity, DateTimeOffset now) =>
        new(importJobRowId, fieldName, errorCode, errorMessage, severity, now);
}
