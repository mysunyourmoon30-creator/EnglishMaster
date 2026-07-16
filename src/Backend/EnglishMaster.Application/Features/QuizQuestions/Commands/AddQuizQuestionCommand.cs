namespace EnglishMaster.Application.Features.QuizQuestions.Commands;

public sealed record AddQuizQuestionCommand(
    Guid QuizId,
    string QuestionText,
    string QuestionType,
    string? ExplanationTh,
    string? ExplanationEn,
    int Points,
    int SortOrder,
    Guid? WordId,
    Guid? GrammarRuleId,
    Guid? PronunciationId);
