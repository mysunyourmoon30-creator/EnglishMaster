namespace EnglishMaster.Application.Features.ContentRevisions.Dtos;

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
