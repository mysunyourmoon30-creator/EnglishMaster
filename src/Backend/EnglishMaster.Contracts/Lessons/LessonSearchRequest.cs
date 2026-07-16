namespace EnglishMaster.Contracts.Lessons;

public sealed record LessonSearchRequest(
    string? Search = null,
    string? CefrLevel = null,
    Guid? CategoryId = null,
    bool? IsPublished = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "Title",
    string? SortDirection = "Asc");
