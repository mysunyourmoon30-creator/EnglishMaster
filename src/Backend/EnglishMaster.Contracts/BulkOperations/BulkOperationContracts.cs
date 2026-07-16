namespace EnglishMaster.Contracts.BulkOperations;

public sealed record BulkOperationDto(
    Guid Id,
    string OperationType,
    string ContentType,
    string Status,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    DateTimeOffset? StartedAt,
    DateTimeOffset? CompletedAt,
    int TotalItems,
    int SucceededItems,
    int FailedItems,
    string ErrorMessage,
    string Note,
    Guid? CategoryId,
    IReadOnlyCollection<Guid> TagIds,
    string ExportFormat,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record BulkOperationItemDto(
    Guid Id,
    Guid BulkOperationId,
    Guid ContentId,
    string Status,
    string ErrorMessage,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record BulkOperationSearchResponse(
    IReadOnlyCollection<BulkOperationDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record CreateBulkOperationRequest(
    string OperationType,
    string ContentType,
    IReadOnlyCollection<Guid> ContentIds,
    string? Note,
    Guid? CategoryId,
    IReadOnlyCollection<Guid>? TagIds,
    string? ExportFormat);
