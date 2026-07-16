namespace EnglishMaster.Contracts.Media;

public sealed record MediaSearchResponse(
    IReadOnlyCollection<MediaDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
