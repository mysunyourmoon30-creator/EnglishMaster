using EnglishMaster.Contracts.QuizChoices;

namespace EnglishMaster.Contracts.QuizQuestions;

public sealed record QuizQuestionDto(
    Guid Id,
    Guid QuizId,
    string QuestionText,
    string QuestionType,
    string ExplanationTh,
    string ExplanationEn,
    int Points,
    int SortOrder,
    Guid? WordId,
    QuizQuestionWordDto? Word,
    Guid? GrammarRuleId,
    QuizQuestionGrammarRuleDto? GrammarRule,
    Guid? PronunciationId,
    QuizQuestionPronunciationDto? Pronunciation,
    IReadOnlyCollection<QuizChoiceDto> Choices,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record QuizQuestionWordDto(
    Guid Id,
    string Text,
    string Slug);

public sealed record QuizQuestionGrammarRuleDto(
    Guid Id,
    string Title,
    string Slug);

public sealed record QuizQuestionPronunciationDto(
    Guid Id,
    Guid WordId);
