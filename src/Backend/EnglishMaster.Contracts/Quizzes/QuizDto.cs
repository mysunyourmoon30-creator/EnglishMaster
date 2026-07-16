using EnglishMaster.Contracts.QuizQuestions;

namespace EnglishMaster.Contracts.Quizzes;

public sealed record QuizDto(
    Guid Id,
    string Title,
    string Slug,
    string Summary,
    string Description,
    string? CefrLevel,
    Guid? CategoryId,
    QuizCategoryDto? Category,
    Guid? LessonId,
    QuizLessonDto? Lesson,
    Guid? CourseId,
    QuizCourseDto? Course,
    Guid? BookId,
    QuizBookDto? Book,
    int TimeLimitMinutes,
    int PassingScore,
    int SortOrder,
    IReadOnlyCollection<QuizQuestionDto> Questions,
    bool IsPublished,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record QuizCategoryDto(
    Guid Id,
    string Name,
    string Slug);

public sealed record QuizLessonDto(
    Guid Id,
    string Title,
    string Slug);

public sealed record QuizCourseDto(
    Guid Id,
    string Title,
    string Slug);

public sealed record QuizBookDto(
    Guid Id,
    string Title,
    string Slug);
