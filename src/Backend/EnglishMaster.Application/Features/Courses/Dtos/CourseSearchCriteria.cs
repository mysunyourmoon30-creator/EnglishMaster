using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Courses.Dtos;

public sealed record CourseSearchCriteria(
    string? SearchTerm,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    bool? IsPublished,
    bool? IsActive,
    int PageNumber,
    int PageSize,
    CourseSortBy SortBy,
    CourseSortDirection SortDirection);

public sealed record CourseSearchResult(
    IReadOnlyCollection<Course> Items,
    int TotalCount);

public enum CourseSortBy
{
    Title = 0,
    CreatedAt = 1
}

public enum CourseSortDirection
{
    Asc = 0,
    Desc = 1
}
