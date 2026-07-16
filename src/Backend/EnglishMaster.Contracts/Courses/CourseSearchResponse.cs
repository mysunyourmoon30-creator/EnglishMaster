namespace EnglishMaster.Contracts.Courses;

public sealed record CourseSearchResponse(
    IReadOnlyCollection<CourseDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);
