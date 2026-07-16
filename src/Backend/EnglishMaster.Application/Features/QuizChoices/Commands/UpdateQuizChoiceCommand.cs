namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed record UpdateQuizChoiceCommand(
    Guid Id,
    string ChoiceText,
    bool IsCorrect,
    string? ExplanationTh,
    string? ExplanationEn,
    int SortOrder,
    bool IsActive);
