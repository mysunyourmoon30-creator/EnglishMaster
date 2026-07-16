namespace EnglishMaster.Contracts.Quizzes;

public sealed record QuizSearchRequest(
    string? Search = null,
    string? CefrLevel = null,
    Guid? CategoryId = null,
    Guid? LessonId = null,
    Guid? CourseId = null,
    Guid? BookId = null,
    bool? IsPublished = null,
    bool? IsActive = true,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "Title",
    string? SortDirection = "Asc");
