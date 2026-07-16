namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed record UpdateQuizQuestionCommand(
    Guid Id,
    string QuestionText,
    string QuestionType,
    string? ExplanationTh,
    string? ExplanationEn,
    int Points,
    int SortOrder,
    Guid? WordId,
    Guid? GrammarRuleId,
    Guid? PronunciationId,
    bool IsActive);
