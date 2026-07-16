namespace EnglishMaster.Application.Features.Quizzes.Commands;

public sealed record UpdateQuizCommand(
    Guid Id,
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
