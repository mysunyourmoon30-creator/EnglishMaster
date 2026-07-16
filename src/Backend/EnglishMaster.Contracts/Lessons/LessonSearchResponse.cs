namespace EnglishMaster.Contracts.Lessons;

public sealed record LessonSearchResponse(
    IReadOnlyCollection<LessonDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
