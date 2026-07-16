using EnglishMaster.Contracts.QuizChoices;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.QuizChoices.Dtos;

internal static class QuizChoiceMapper
{
    public static QuizChoiceDto ToDto(QuizChoice choice)
    {
        return new QuizChoiceDto(
            choice.Id,
            choice.QuizQuestionId,
            choice.ChoiceText,
            choice.IsCorrect,
            choice.ExplanationTh,
            choice.ExplanationEn,
            choice.SortOrder,
            choice.IsActive,
            choice.CreatedAt,
            choice.UpdatedAt);
    }

    public static IReadOnlyCollection<QuizChoiceDto> ToDtos(IReadOnlyCollection<QuizChoice> choices)
    {
        return choices
            .OrderBy(choice => choice.SortOrder)
            .ThenBy(choice => choice.ChoiceText)
            .Select(ToDto)
            .ToArray();
    }
}
