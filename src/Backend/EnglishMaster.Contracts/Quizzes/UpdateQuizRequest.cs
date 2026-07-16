namespace EnglishMaster.Contracts.Quizzes;

public sealed record UpdateQuizRequest(
    string Title,
    string? Summary,
    string? Description,
    string? CefrLevel,
    Guid? CategoryId,
    Guid? LessonId,
    Guid? CourseId,
    Guid? BookId,
    int TimeLimitMinutes,
    int PassingScore,
    int SortOrder,
    bool IsPublished,
    bool IsActive);
