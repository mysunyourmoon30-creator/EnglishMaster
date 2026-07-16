namespace EnglishMaster.Application.Features.QuizChoices.Dtos;

internal sealed record QuizChoiceInput(
    string ChoiceText,
    bool IsCorrect,
    string ExplanationTh,
    string ExplanationEn,
    int SortOrder,
    bool IsActive);
