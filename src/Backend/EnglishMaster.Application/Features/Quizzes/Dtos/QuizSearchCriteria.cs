using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Quizzes.Dtos;

public sealed record QuizSearchCriteria(
    string? SearchTerm,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    Guid? LessonId,
    Guid? CourseId,
    Guid? BookId,
    bool? IsPublished,
    bool? IsActive,
    int PageNumber,
    int PageSize,
    QuizSortBy SortBy,
    QuizSortDirection SortDirection);

public sealed record QuizSearchResult(
    IReadOnlyCollection<Quiz> Items,
    int TotalCount);

public enum QuizSortBy
{
    Title = 0,
    CreatedAt = 1
}

public enum QuizSortDirection
{
    Asc = 0,
    Desc = 1
}
