using EnglishMaster.Application.Features.PublicGrammar;
using EnglishMaster.Application.Features.PublicGrammar.Dtos;
using EnglishMaster.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.PublicGrammar;

internal sealed class EfPublicGrammarRepository(EnglishMasterDbContext dbContext) : IPublicGrammarRepository
{
    public async Task<PublicGrammarTopicDetailDto?> GetTopicBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        var topic = await dbContext.GrammarTopics
            .AsNoTracking()
            .Where(item => item.IsActive && item.Slug == slug)
            .Select(item => new
            {
                item.Id,
                item.Title,
                item.Slug,
                item.Summary,
                item.CefrLevel
            })
            .SingleOrDefaultAsync(cancellationToken);
        if (topic is null)
        {
            return null;
        }

        var rules = await dbContext.GrammarRules
            .AsNoTracking()
            .Where(item => item.IsActive && item.GrammarTopicId == topic.Id)
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.Title)
            .Select(item => new PublicGrammarRuleSummaryDto(
                item.Title,
                item.Slug,
                item.RuleText,
                item.StructurePattern))
            .ToArrayAsync(cancellationToken);

        return new PublicGrammarTopicDetailDto(
            topic.Title,
            topic.Slug,
            topic.Summary,
            topic.CefrLevel.ToString(),
            rules);
    }

    public async Task<PublicGrammarRuleDetailDto?> GetRuleBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        var rule = await (
                from grammarRule in dbContext.GrammarRules.AsNoTracking()
                join topic in dbContext.GrammarTopics.AsNoTracking()
                    on grammarRule.GrammarTopicId equals topic.Id
                where grammarRule.IsActive && topic.IsActive && grammarRule.Slug == slug
                select new
                {
                    grammarRule.Id,
                    grammarRule.Title,
                    grammarRule.Slug,
                    grammarRule.RuleText,
                    grammarRule.ExplanationTh,
                    grammarRule.ExplanationEn,
                    grammarRule.StructurePattern,
                    grammarRule.CommonMistake,
                    grammarRule.CorrectUsageNote,
                    TopicTitle = topic.Title,
                    TopicSlug = topic.Slug,
                    TopicCefrLevel = topic.CefrLevel
                })
            .SingleOrDefaultAsync(cancellationToken);
        if (rule is null)
        {
            return null;
        }

        var examples = await dbContext.GrammarExamples
            .AsNoTracking()
            .Where(item => item.IsActive && item.GrammarRuleId == rule.Id)
            .OrderBy(item => item.SortOrder)
            .ThenBy(item => item.ExampleEn)
            .Select(item => new PublicGrammarExampleDto(
                item.ExampleEn,
                item.TranslationTh,
                item.ExplanationTh,
                item.IsCorrectExample))
            .ToArrayAsync(cancellationToken);

        var relatedWords = await (
                from relation in dbContext.GrammarRuleWords.AsNoTracking()
                join word in dbContext.Words.AsNoTracking()
                    on relation.WordId equals word.Id
                where relation.GrammarRuleId == rule.Id && word.IsActive
                orderby word.Text
                select new PublicGrammarRelatedWordDto(
                    word.Text,
                    word.Slug,
                    word.MeaningTh))
            .ToArrayAsync(cancellationToken);

        return new PublicGrammarRuleDetailDto(
            rule.Title,
            rule.Slug,
            rule.RuleText,
            rule.ExplanationTh,
            rule.ExplanationEn,
            rule.StructurePattern,
            rule.CommonMistake,
            rule.CorrectUsageNote,
            new PublicGrammarTopicSummaryDto(
                rule.TopicTitle,
                rule.TopicSlug,
                rule.TopicCefrLevel.ToString()),
            examples,
            relatedWords);
    }
}