using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.QuizChoices;

namespace EnglishMaster.Web.Services.Quizzes;

public sealed class QuizChoiceFormModel
{
    [Required]
    [StringLength(1000)]
    public string ChoiceText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    [StringLength(2000)]
    public string? ExplanationTh { get; set; }

    [StringLength(2000)]
    public string? ExplanationEn { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static QuizChoiceFormModel FromDto(QuizChoiceDto choice)
    {
        return new QuizChoiceFormModel
        {
            ChoiceText = choice.ChoiceText,
            IsCorrect = choice.IsCorrect,
            ExplanationTh = choice.ExplanationTh,
            ExplanationEn = choice.ExplanationEn,
            SortOrder = choice.SortOrder,
            IsActive = choice.IsActive
        };
    }

    public CreateQuizChoiceRequest ToCreateRequest()
    {
        return new CreateQuizChoiceRequest(
            ChoiceText,
            IsCorrect,
            ExplanationTh,
            ExplanationEn,
            SortOrder);
    }

    public UpdateQuizChoiceRequest ToUpdateRequest()
    {
        return new UpdateQuizChoiceRequest(
            ChoiceText,
            IsCorrect,
            ExplanationTh,
            ExplanationEn,
            SortOrder,
            IsActive);
    }
}
