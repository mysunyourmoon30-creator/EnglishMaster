namespace EnglishMaster.Contracts.Pronunciations;

public sealed record PronunciationSearchResponse(
    IReadOnlyCollection<PronunciationDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
