using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Courses.Dtos;

internal static class CourseReferenceValidator
{
    public static async Task<CourseReferenceValidation> ValidateReferencesAsync(
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        CourseInput input,
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

        return new CourseReferenceValidation(category, thumbnailMedia, errors);
    }

    public static async Task<IReadOnlyCollection<ValidationError>> ValidateLessonAsync(
        ILessonRepository lessonRepository,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson is null || !lesson.IsActive)
        {
            return [new ValidationError(nameof(lessonId), "Lesson was not found or is inactive.")];
        }

        return [];
    }
}

internal sealed record CourseReferenceValidation(
    Category? Category,
    MediaEntity? ThumbnailMedia,
    IReadOnlyCollection<ValidationError> Errors);
