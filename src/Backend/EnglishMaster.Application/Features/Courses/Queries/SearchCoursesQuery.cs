namespace EnglishMaster.Application.Features.Courses.Queries;

public sealed record SearchCoursesQuery(
    string? Search,
    string? CefrLevel,
    Guid? CategoryId,
    bool? IsPublished,
    bool? IsActive,
    int? PageNumber,
    int? PageSize,
    string? SortBy = null,
    string? SortDirection = null);
