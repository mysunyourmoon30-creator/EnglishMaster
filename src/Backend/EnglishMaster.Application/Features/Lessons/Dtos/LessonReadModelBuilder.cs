using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Lessons.Dtos;

internal static class LessonReadModelBuilder
{
    public static async Task<LessonDto> MapAsync(
        Lesson lesson,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        CancellationToken cancellationToken)
    {
        var category = lesson.CategoryId.HasValue
            ? await categoryRepository.GetByIdAsync(lesson.CategoryId.Value, cancellationToken)
            : null;
        var thumbnailMedia = lesson.ThumbnailMediaId.HasValue
            ? await mediaRepository.GetByIdAsync(lesson.ThumbnailMediaId.Value, cancellationToken)
            : null;
        var relatedWords = await LoadWordsAsync(
            lesson.Words.Select(relation => relation.WordId).Distinct().ToArray(),
            wordRepository,
            cancellationToken);
        var relatedGrammarRules = await LoadGrammarRulesAsync(
            lesson.GrammarRules.Select(relation => relation.GrammarRuleId).Distinct().ToArray(),
            grammarRuleRepository,
            cancellationToken);

        return LessonMapper.ToDto(lesson, category, thumbnailMedia, relatedWords, relatedGrammarRules);
    }

    public static async Task<IReadOnlyCollection<LessonDto>> MapAsync(
        IReadOnlyCollection<Lesson> lessons,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        IWordRepository wordRepository,
        IGrammarRuleRepository grammarRuleRepository,
        CancellationToken cancellationToken)
    {
        var categoryIds = lessons
            .Where(lesson => lesson.CategoryId.HasValue)
            .Select(lesson => lesson.CategoryId!.Value)
            .Distinct()
            .ToArray();
        var categories = categoryIds.Length == 0
            ? []
            : await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryById = categories.ToDictionary(category => category.Id);

        var mediaIds = lessons
            .Where(lesson => lesson.ThumbnailMediaId.HasValue)
            .Select(lesson => lesson.ThumbnailMediaId!.Value)
            .Distinct()
            .ToArray();
        var mediaItems = mediaIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(mediaIds, cancellationToken);
        var mediaById = mediaItems.ToDictionary(media => media.Id);

        var wordIds = lessons
            .SelectMany(lesson => lesson.Words.Select(relation => relation.WordId))
            .Distinct()
            .ToArray();
        var words = await LoadWordsAsync(wordIds, wordRepository, cancellationToken);

        var grammarRuleIds = lessons
            .SelectMany(lesson => lesson.GrammarRules.Select(relation => relation.GrammarRuleId))
            .Distinct()
            .ToArray();
        var grammarRules = await LoadGrammarRulesAsync(grammarRuleIds, grammarRuleRepository, cancellationToken);

        return lessons
            .Select(lesson =>
            {
                Category? category = null;
                if (lesson.CategoryId.HasValue)
                {
                    categoryById.TryGetValue(lesson.CategoryId.Value, out category);
                }

                MediaEntity? thumbnailMedia = null;
                if (lesson.ThumbnailMediaId.HasValue)
                {
                    mediaById.TryGetValue(lesson.ThumbnailMediaId.Value, out thumbnailMedia);
                }

                return LessonMapper.ToDto(lesson, category, thumbnailMedia, words, grammarRules);
            })
            .ToArray();
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

    private static async Task<IReadOnlyCollection<GrammarRule>> LoadGrammarRulesAsync(
        IReadOnlyCollection<Guid> grammarRuleIds,
        IGrammarRuleRepository grammarRuleRepository,
        CancellationToken cancellationToken)
    {
        if (grammarRuleIds.Count == 0)
        {
            return [];
        }

        return await grammarRuleRepository.GetByIdsAsync(grammarRuleIds, cancellationToken);
    }
}
