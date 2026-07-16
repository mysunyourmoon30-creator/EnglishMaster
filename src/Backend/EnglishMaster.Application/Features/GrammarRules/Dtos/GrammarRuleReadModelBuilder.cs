using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.GrammarRules.Dtos;

internal static class GrammarRuleReadModelBuilder
{
    public static async Task<GrammarRuleDto> MapAsync(
        GrammarRule grammarRule,
        IGrammarTopicRepository grammarTopicRepository,
        IWordRepository wordRepository,
        CancellationToken cancellationToken)
    {
        var topic = await grammarTopicRepository.GetByIdAsync(grammarRule.GrammarTopicId, cancellationToken);
        var relatedWords = await LoadRelatedWordsAsync(grammarRule.RelatedWords, wordRepository, cancellationToken);

        return GrammarRuleMapper.ToDto(grammarRule, topic, relatedWords);
    }

    public static async Task<IReadOnlyCollection<GrammarRuleDto>> MapAsync(
        IReadOnlyCollection<GrammarRule> grammarRules,
        IGrammarTopicRepository grammarTopicRepository,
        IWordRepository wordRepository,
        CancellationToken cancellationToken)
    {
        var topicIds = grammarRules
            .Select(rule => rule.GrammarTopicId)
            .Distinct()
            .ToArray();
        var topics = topicIds.Length == 0
            ? []
            : await grammarTopicRepository.GetByIdsAsync(topicIds, cancellationToken);
        var topicById = topics.ToDictionary(topic => topic.Id);

        var wordIds = grammarRules
            .SelectMany(rule => rule.RelatedWords.Select(relation => relation.WordId))
            .Distinct()
            .ToArray();
        var relatedWords = await LoadWordsAsync(wordIds, wordRepository, cancellationToken);

        return grammarRules
            .Select(rule =>
            {
                topicById.TryGetValue(rule.GrammarTopicId, out var topic);
                return GrammarRuleMapper.ToDto(rule, topic, relatedWords);
            })
            .ToArray();
    }

    private static async Task<IReadOnlyCollection<Word>> LoadRelatedWordsAsync(
        IReadOnlyCollection<GrammarRuleWord> relations,
        IWordRepository wordRepository,
        CancellationToken cancellationToken)
    {
        var wordIds = relations
            .Select(relation => relation.WordId)
            .Distinct()
            .ToArray();

        return await LoadWordsAsync(wordIds, wordRepository, cancellationToken);
    }

    private static async Task<IReadOnlyCollection<Word>> LoadWordsAsync(
        IReadOnlyCollection<Guid> wordIds,
        IWordRepository wordRepository,
        CancellationToken cancellationToken)
    {
        if (wordIds.Count == 0)
        {
            return [];
        }

        return await wordRepository.GetByIdsAsync(wordIds, cancellationToken);
    }
}
