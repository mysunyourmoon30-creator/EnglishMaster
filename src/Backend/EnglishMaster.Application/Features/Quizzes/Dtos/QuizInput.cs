using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.Quizzes.Dtos;

internal sealed record QuizInput(
    string Title,
    string Slug,
    string Summary,
    string Description,
    CefrLevel? CefrLevel,
    Guid? CategoryId,
    Guid? LessonId,
    Guid? CourseId,
    Guid? BookId,
    int TimeLimitMinutes,
    int PassingScore,
    int SortOrder,
    bool IsPublished,
    bool IsActive);
