using EnglishMaster.Application.Features.QuizChoices.Dtos;
using EnglishMaster.Contracts.QuizQuestions;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.QuizQuestions.Dtos;

internal static class QuizQuestionMapper
{
    public static QuizQuestionDto ToDto(
        QuizQuestion question,
        Word? word = null,
        GrammarRule? grammarRule = null,
        Pronunciation? pronunciation = null)
    {
        return new QuizQuestionDto(
            question.Id,
            question.QuizId,
            question.QuestionText,
            question.QuestionType.ToString(),
            question.ExplanationTh,
            question.ExplanationEn,
            question.Points,
            question.SortOrder,
            question.WordId,
            word is null ? null : new QuizQuestionWordDto(word.Id, word.Text, word.Slug),
            question.GrammarRuleId,
            grammarRule is null ? null : new QuizQuestionGrammarRuleDto(grammarRule.Id, grammarRule.Title, grammarRule.Slug),
            question.PronunciationId,
            pronunciation is null ? null : new QuizQuestionPronunciationDto(pronunciation.Id, pronunciation.WordId),
            QuizChoiceMapper.ToDtos(question.Choices),
            question.IsActive,
            question.CreatedAt,
            question.UpdatedAt);
    }

    public static IReadOnlyCollection<QuizQuestionDto> ToDtos(IReadOnlyCollection<QuizQuestion> questions)
    {
        return questions
            .OrderBy(question => question.SortOrder)
            .ThenBy(question => question.QuestionText)
            .Select(question => ToDto(question))
            .ToArray();
    }
}
