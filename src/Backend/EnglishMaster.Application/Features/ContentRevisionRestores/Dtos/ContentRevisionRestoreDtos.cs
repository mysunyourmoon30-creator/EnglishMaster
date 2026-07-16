namespace EnglishMaster.Application.Features.ContentRevisionRestores.Dtos;

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
