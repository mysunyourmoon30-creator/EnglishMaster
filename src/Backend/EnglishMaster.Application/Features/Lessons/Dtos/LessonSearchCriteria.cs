using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Lessons.Dtos;

public sealed record LessonSearchCriteria(
    string? SearchTerm,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    bool? IsPublished,
    bool? IsActive,
    int PageNumber,
    int PageSize,
    LessonSortBy SortBy,
    LessonSortDirection SortDirection);

public sealed record LessonSearchResult(
    IReadOnlyCollection<Lesson> Items,
    int TotalCount);

public enum LessonSortBy
{
    Title = 0,
    CreatedAt = 1
}

public enum LessonSortDirection
{
    Asc = 0,
    Desc = 1
}
