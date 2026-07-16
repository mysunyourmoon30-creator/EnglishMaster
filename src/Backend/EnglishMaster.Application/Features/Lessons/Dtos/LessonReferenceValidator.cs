using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Media;
using EnglishMaster.Domain.Words;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Lessons.Dtos;

internal static class LessonReferenceValidator
{
    public static async Task<LessonReferenceValidation> ValidateReferencesAsync(
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        LessonInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        Category? category = null;
        MediaEntity? thumbnailMedia = null;

        if (input.CategoryId.HasValue)
        {
            category = await categoryRepository.GetByIdAsync(input.CategoryId.Value, cancellationToken);
            if (category is null || !category.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.CategoryId), "Category was not found or is inactive."));
            }
        }

        if (input.ThumbnailMediaId.HasValue)
        {
            thumbnailMedia = await mediaRepository.GetByIdAsync(input.ThumbnailMediaId.Value, cancellationToken);
            if (thumbnailMedia is null || !thumbnailMedia.IsActive || thumbnailMedia.MediaType != MediaType.Image)
            {
                errors.Add(new ValidationError(nameof(input.ThumbnailMediaId), "Thumbnail media was not found, is inactive, or is not an image."));
            }
        }

        return new LessonReferenceValidation(category, thumbnailMedia, errors);
    }

    public static async Task<IReadOnlyCollection<ValidationError>> ValidateWordAsync(
        IWordRepository wordRepository,
        Guid wordId,
        CancellationToken cancellationToken)
    {
        var word = await wordRepository.GetByIdAsync(wordId, cancellationToken);
        if (word is null || !word.IsActive)
        {
            return [new ValidationError(nameof(wordId), "Word was not found or is inactive.")];
        }

        return [];
    }

    public static async Task<IReadOnlyCollection<ValidationError>> ValidateGrammarRuleAsync(
        IGrammarRuleRepository grammarRuleRepository,
        Guid grammarRuleId,
        CancellationToken cancellationToken)
    {
        var grammarRule = await grammarRuleRepository.GetByIdAsync(grammarRuleId, cancellationToken);
        if (grammarRule is null || !grammarRule.IsActive)
        {
            return [new ValidationError(nameof(grammarRuleId), "Grammar rule was not found or is inactive.")];
        }

        return [];
    }
}

internal sealed record LessonReferenceValidation(
    Category? Category,
    MediaEntity? ThumbnailMedia,
    IReadOnlyCollection<ValidationError> Errors);
