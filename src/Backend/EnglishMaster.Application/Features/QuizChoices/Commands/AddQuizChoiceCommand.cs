namespace EnglishMaster.Application.Features.QuizChoices.Commands;

public sealed record AddQuizChoiceCommand(
    Guid QuizQuestionId,
    string ChoiceText,
    bool IsCorrect,
    string? ExplanationTh,
    string? ExplanationEn,
    int SortOrder);
