using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Media;
using EnglishMaster.Shared.Results;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Books.Dtos;

internal static class BookReferenceValidator
{
    public static async Task<BookReferenceValidation> ValidateReferencesAsync(
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ICourseRepository courseRepository,
        BookInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        Category? category = null;
        MediaEntity? coverMedia = null;
        Course? course = null;

        if (input.CategoryId.HasValue)
        {
            category = await categoryRepository.GetByIdAsync(input.CategoryId.Value, cancellationToken);
            if (category is null || !category.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.CategoryId), "Category was not found or is inactive."));
            }
        }

        if (input.CoverMediaId.HasValue)
        {
            coverMedia = await mediaRepository.GetByIdAsync(input.CoverMediaId.Value, cancellationToken);
            if (coverMedia is null || !coverMedia.IsActive || coverMedia.MediaType != MediaType.Image)
            {
                errors.Add(new ValidationError(nameof(input.CoverMediaId), "Cover media was not found, is inactive, or is not an image."));
            }
        }

        if (input.CourseId.HasValue)
        {
            course = await courseRepository.GetByIdAsync(input.CourseId.Value, cancellationToken);
            if (course is null || !course.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.CourseId), "Course was not found or is inactive."));
            }
        }

        return new BookReferenceValidation(category, coverMedia, course, errors);
    }
}

internal sealed record BookReferenceValidation(
    Category? Category,
    MediaEntity? CoverMedia,
    Course? Course,
    IReadOnlyCollection<ValidationError> Errors);
