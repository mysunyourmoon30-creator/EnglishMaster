namespace EnglishMaster.Application.Features.Lessons.Queries;

public sealed record SearchLessonsQuery(
    string? Search,
    string? CefrLevel,
    Guid? CategoryId,
    bool? IsPublished,
    bool? IsActive,
    int? PageNumber = null,
    int? PageSize = null,
    string? SortBy = null,
    string? SortDirection = null);
