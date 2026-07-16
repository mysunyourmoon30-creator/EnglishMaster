using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Dtos;

internal static class GrammarRuleReferenceValidator
{
    public static async Task<GrammarRuleReferenceValidation> ValidateTopicAsync(
        IGrammarTopicRepository grammarTopicRepository,
        Guid grammarTopicId,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        GrammarTopic? topic = null;

        topic = await grammarTopicRepository.GetByIdAsync(grammarTopicId, cancellationToken);
        if (topic is null || !topic.IsActive)
        {
            errors.Add(new ValidationError(nameof(grammarTopicId), "Grammar topic was not found or is inactive."));
        }

        return new GrammarRuleReferenceValidation(topic, null, errors);
    }

    public static async Task<GrammarRuleReferenceValidation> ValidateWordAsync(
        IWordRepository wordRepository,
        Guid wordId,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        Word? word = null;

        word = await wordRepository.GetByIdAsync(wordId, cancellationToken);
        if (word is null || !word.IsActive)
        {
            errors.Add(new ValidationError(nameof(wordId), "Word was not found or is inactive."));
        }

        return new GrammarRuleReferenceValidation(null, word, errors);
    }
}

internal sealed record GrammarRuleReferenceValidation(
    GrammarTopic? Topic,
    Word? Word,
    IReadOnlyCollection<ValidationError> Errors);
