using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Application.Features.GrammarRules.Dtos;

internal static class GrammarRuleMapper
{
    public static GrammarRuleDto ToDto(
        GrammarRule grammarRule,
        GrammarTopic? topic = null,
        IReadOnlyCollection<Word>? relatedWords = null)
    {
        var relatedWordById = relatedWords?.ToDictionary(word => word.Id) ?? new Dictionary<Guid, Word>();

        return new GrammarRuleDto(
            grammarRule.Id,
            grammarRule.GrammarTopicId,
            topic is null
                ? null
                : new GrammarRuleTopicDto(
                    topic.Id,
                    topic.Title,
                    topic.Slug,
                    topic.CefrLevel.ToString()),
            grammarRule.Title,
            grammarRule.Slug,
            grammarRule.RuleText,
            grammarRule.ExplanationTh,
            grammarRule.ExplanationEn,
            grammarRule.StructurePattern,
            grammarRule.CommonMistake,
            grammarRule.CorrectUsageNote,
            grammarRule.SortOrder,
            grammarRule.RelatedWords
                .Select(relation => relatedWordById.TryGetValue(relation.WordId, out var word) ? word : null)
                .Where(word => word is not null)
                .Cast<Word>()
                .OrderBy(word => word.Text)
                .Select(word => new GrammarRuleRelatedWordDto(
                    word.Id,
                    word.Text,
                    word.Slug,
                    word.MeaningTh))
                .ToArray(),
            grammarRule.IsActive,
            grammarRule.CreatedAt,
            grammarRule.UpdatedAt);
    }
}
