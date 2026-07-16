namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishJobSearchResponse(
    IReadOnlyCollection<PublishJobDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
