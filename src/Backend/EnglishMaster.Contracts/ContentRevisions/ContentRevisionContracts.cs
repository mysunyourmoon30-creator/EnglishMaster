namespace EnglishMaster.Contracts.ContentRevisions;

public sealed record ContentRevisionDto(
    Guid Id,
    string ContentType,
    Guid ContentId,
    int RevisionNumber,
    string EventType,
    string Title,
    string Summary,
    string ChangedBy,
    DateTimeOffset ChangedAt,
    string ChangeReason,
    string SnapshotJson,
    string DiffJson,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ContentRevisionSearchResponse(
    IReadOnlyCollection<ContentRevisionDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record CreateContentRevisionRequest(
    string ContentType,
    Guid ContentId,
    string EventType,
    string? Title,
    string? Summary,
    string? ChangeReason,
    string SnapshotJson,
    string? DiffJson);

public sealed record ContentRevisionRestoreRequestDto(
    Guid Id,
    Guid ContentRevisionId,
    string RequestedBy,
    DateTimeOffset RequestedAt,
    string Reason,
    string Status,
    string ReviewedBy,
    DateTimeOffset? ReviewedAt,
    string ReviewNote,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ContentRevisionRestoreRequestSearchResponse(
    IReadOnlyCollection<ContentRevisionRestoreRequestDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record CreateContentRevisionRestoreRequestRequest(
    Guid ContentRevisionId,
    string Reason);

public sealed record ReviewContentRevisionRestoreRequestRequest(
    string? ReviewNote);
