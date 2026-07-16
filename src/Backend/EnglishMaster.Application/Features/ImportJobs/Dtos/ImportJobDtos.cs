namespace EnglishMaster.Application.Features.ImportJobs.Dtos;

public sealed record ImportJobDto(
    Guid Id,
    string ImportType,
    string Format,
    string Status,
    string FileName,
    string OriginalFileName,
    long FileSize,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset? ValidatedAt,
    DateTimeOffset? ConfirmedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset? FailedAt,
    DateTimeOffset? RolledBackAt,
    int TotalRows,
    int ValidRows,
    int InvalidRows,
    int ImportedRows,
    int FailedRows,
    string ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ImportJobRowDto(
    Guid Id,
    Guid ImportJobId,
    int RowNumber,
    string RawDataJson,
    string ParsedDataJson,
    string Status,
    string ErrorMessage,
    string CreatedEntityType,
    Guid? CreatedEntityId,
    string UpdatedEntityType,
    Guid? UpdatedEntityId,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ImportValidationErrorDto(Guid Id, Guid ImportJobRowId, string FieldName, string ErrorCode, string ErrorMessage, string Severity, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);

public sealed record ImportJobSearchResponse(IReadOnlyCollection<ImportJobDto> Items, int PageNumber, int PageSize, int TotalCount, int TotalPages, bool HasPreviousPage, bool HasNextPage);
