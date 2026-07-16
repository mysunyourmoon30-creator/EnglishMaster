using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Media;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Words;

internal static class WordReferenceValidator
{
    public static async Task<WordReferenceValidation> ValidateAsync(
        ICategoryRepository categoryRepository,
        ITagRepository tagRepository,
        IMediaRepository mediaRepository,
        WordInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        Category? category = null;
        IReadOnlyCollection<Tag> tags = [];
        MediaEntity? imageMedia = null;
        MediaEntity? audioMedia = null;

        if (input.CategoryId.HasValue)
        {
            category = await categoryRepository.GetByIdAsync(input.CategoryId.Value, cancellationToken);
            if (category is null || !category.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.CategoryId), "Category was not found or is inactive."));
            }
        }

        if (input.TagIds.Count > 0)
        {
            tags = await tagRepository.GetByIdsAsync(input.TagIds, cancellationToken);
            var activeTagIds = tags
                .Where(tag => tag.IsActive)
                .Select(tag => tag.Id)
                .ToHashSet();

            if (input.TagIds.Any(tagId => !activeTagIds.Contains(tagId)))
            {
                errors.Add(new ValidationError(nameof(input.TagIds), "One or more tags were not found or are inactive."));
            }
        }

        if (input.ImageMediaId.HasValue)
        {
            imageMedia = await mediaRepository.GetByIdAsync(input.ImageMediaId.Value, cancellationToken);
            if (imageMedia is null || !imageMedia.IsActive || imageMedia.MediaType != MediaType.Image)
            {
                errors.Add(new ValidationError(nameof(input.ImageMediaId), "Image media was not found, is inactive, or is not an image."));
            }
        }

        if (input.AudioMediaId.HasValue)
        {
            audioMedia = await mediaRepository.GetByIdAsync(input.AudioMediaId.Value, cancellationToken);
            if (audioMedia is null || !audioMedia.IsActive || audioMedia.MediaType != MediaType.Audio)
            {
                errors.Add(new ValidationError(nameof(input.AudioMediaId), "Audio media was not found, is inactive, or is not audio."));
            }
        }

        return new WordReferenceValidation(category, tags, imageMedia, audioMedia, errors);
    }
}

internal sealed record WordReferenceValidation(
    Category? Category,
    IReadOnlyCollection<Tag> Tags,
    MediaEntity? ImageMedia,
    MediaEntity? AudioMedia,
    IReadOnlyCollection<ValidationError> Errors);
