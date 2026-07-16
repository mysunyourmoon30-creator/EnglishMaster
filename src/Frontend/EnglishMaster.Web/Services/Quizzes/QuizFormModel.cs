using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Quizzes;

namespace EnglishMaster.Web.Services.Quizzes;

public sealed class QuizFormModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Summary { get; set; }

    [StringLength(4000)]
    public string? Description { get; set; }

    public string? CefrLevel { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? LessonId { get; set; }

    public Guid? CourseId { get; set; }

    public Guid? BookId { get; set; }

    [Range(0, int.MaxValue)]
    public int TimeLimitMinutes { get; set; }

    [Range(0, 100)]
    public int PassingScore { get; set; } = 70;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; } = true;

    public static QuizFormModel FromDto(QuizDto quiz)
    {
        return new QuizFormModel
        {
            Title = quiz.Title,
            Summary = quiz.Summary,
            Description = quiz.Description,
            CefrLevel = quiz.CefrLevel,
            CategoryId = quiz.CategoryId,
            LessonId = quiz.LessonId,
            CourseId = quiz.CourseId,
            BookId = quiz.BookId,
            TimeLimitMinutes = quiz.TimeLimitMinutes,
            PassingScore = quiz.PassingScore,
            SortOrder = quiz.SortOrder,
            IsPublished = quiz.IsPublished,
            IsActive = quiz.IsActive
        };
    }

    public CreateQuizRequest ToCreateRequest()
    {
        return new CreateQuizRequest(
            Title,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            LessonId,
            CourseId,
            BookId,
            TimeLimitMinutes,
            PassingScore,
            SortOrder);
    }

    public UpdateQuizRequest ToUpdateRequest()
    {
        return new UpdateQuizRequest(
            Title,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            LessonId,
            CourseId,
            BookId,
            TimeLimitMinutes,
            PassingScore,
            SortOrder,
            IsPublished,
            IsActive);
    }
}
