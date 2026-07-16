using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.QuizQuestions.Dtos;

internal sealed record QuizQuestionInput(
    string QuestionText,
    QuizQuestionType QuestionType,
    string ExplanationTh,
    string ExplanationEn,
    int Points,
    int SortOrder,
    Guid? WordId,
    Guid? GrammarRuleId,
    Guid? PronunciationId,
    bool IsActive);
