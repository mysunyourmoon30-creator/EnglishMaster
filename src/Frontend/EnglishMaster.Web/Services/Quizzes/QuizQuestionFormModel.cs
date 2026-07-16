using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.QuizQuestions;

namespace EnglishMaster.Web.Services.Quizzes;

public sealed class QuizQuestionFormModel
{
    [Required]
    [StringLength(2000)]
    public string QuestionText { get; set; } = string.Empty;

    [Required]
    public string QuestionType { get; set; } = "SingleChoice";

    [StringLength(2000)]
    public string? ExplanationTh { get; set; }

    [StringLength(2000)]
    public string? ExplanationEn { get; set; }

    [Range(1, int.MaxValue)]
    public int Points { get; set; } = 1;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public Guid? WordId { get; set; }

    public Guid? GrammarRuleId { get; set; }

    public Guid? PronunciationId { get; set; }

    public bool IsActive { get; set; } = true;

    public static QuizQuestionFormModel FromDto(QuizQuestionDto question)
    {
        return new QuizQuestionFormModel
        {
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            ExplanationTh = question.ExplanationTh,
            ExplanationEn = question.ExplanationEn,
            Points = question.Points,
            SortOrder = question.SortOrder,
            WordId = question.WordId,
            GrammarRuleId = question.GrammarRuleId,
            PronunciationId = question.PronunciationId,
            IsActive = question.IsActive
        };
    }

    public CreateQuizQuestionRequest ToCreateRequest()
    {
        return new CreateQuizQuestionRequest(
            QuestionText,
            QuestionType,
            ExplanationTh,
            ExplanationEn,
            Points,
            SortOrder,
            WordId,
            GrammarRuleId,
            PronunciationId);
    }

    public UpdateQuizQuestionRequest ToUpdateRequest()
    {
        return new UpdateQuizQuestionRequest(
            QuestionText,
            QuestionType,
            ExplanationTh,
            ExplanationEn,
            Points,
            SortOrder,
            WordId,
            GrammarRuleId,
            PronunciationId,
            IsActive);
    }
}
