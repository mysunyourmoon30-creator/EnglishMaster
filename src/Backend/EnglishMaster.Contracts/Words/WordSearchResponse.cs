namespace EnglishMaster.Contracts.Words;

public sealed record WordSearchResponse(
    IReadOnlyCollection<WordDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
