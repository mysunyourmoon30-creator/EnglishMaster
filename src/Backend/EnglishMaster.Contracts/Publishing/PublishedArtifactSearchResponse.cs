namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishedArtifactSearchResponse(
    IReadOnlyCollection<PublishedArtifactDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
