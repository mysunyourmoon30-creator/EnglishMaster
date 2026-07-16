using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Lessons.Dtos;

internal static class LessonMapper
{
    public static LessonDto ToDto(
        Lesson lesson,
        Category? category = null,
        MediaEntity? thumbnailMedia = null,
        IReadOnlyCollection<Word>? relatedWords = null,
        IReadOnlyCollection<GrammarRule>? relatedGrammarRules = null)
    {
        var wordById = relatedWords?.ToDictionary(word => word.Id) ?? new Dictionary<Guid, Word>();
        var grammarRuleById = relatedGrammarRules?.ToDictionary(rule => rule.Id) ?? new Dictionary<Guid, GrammarRule>();

        return new LessonDto(
            lesson.Id,
            lesson.Title,
            lesson.Slug,
            lesson.Summary,
            lesson.Description,
            lesson.CefrLevel?.ToString(),
            lesson.CategoryId,
            category is null ? null : new LessonCategoryDto(category.Id, category.Name, category.Slug),
            lesson.ThumbnailMediaId,
            thumbnailMedia is null ? null : ToMediaDto(thumbnailMedia),
            lesson.EstimatedMinutes,
            lesson.SortOrder,
            lesson.Words
                .Select(relation => wordById.TryGetValue(relation.WordId, out var word) ? word : null)
                .Where(word => word is not null)
                .Cast<Word>()
                .OrderBy(word => word.Text)
                .Select(word => new LessonWordSummaryDto(word.Id, word.Text, word.Slug, word.MeaningTh))
                .ToArray(),
            lesson.GrammarRules
                .Select(relation => grammarRuleById.TryGetValue(relation.GrammarRuleId, out var rule) ? rule : null)
                .Where(rule => rule is not null)
                .Cast<GrammarRule>()
                .OrderBy(rule => rule.Title)
                .Select(rule => new LessonGrammarRuleSummaryDto(rule.Id, rule.Title, rule.Slug))
                .ToArray(),
            lesson.IsPublished,
            lesson.IsActive,
            lesson.CreatedAt,
            lesson.UpdatedAt);
    }

    public static LessonMediaDto ToMediaDto(MediaEntity media)
    {
        return new LessonMediaDto(
            media.Id,
            media.FileName,
            media.ContentType,
            media.MediaType.ToString(),
            media.PublicUrl,
            media.AltText);
    }
}
