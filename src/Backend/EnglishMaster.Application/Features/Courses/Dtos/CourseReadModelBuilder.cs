using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Courses.Dtos;

internal static class CourseReadModelBuilder
{
    public static async Task<CourseDto> MapAsync(
        Course course,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var category = course.CategoryId.HasValue
            ? await categoryRepository.GetByIdAsync(course.CategoryId.Value, cancellationToken)
            : null;
        var thumbnailMedia = course.ThumbnailMediaId.HasValue
            ? await mediaRepository.GetByIdAsync(course.ThumbnailMediaId.Value, cancellationToken)
            : null;
        var lessons = await LoadLessonsAsync(
            course.Lessons.Select(relation => relation.LessonId).Distinct().ToArray(),
            lessonRepository,
            cancellationToken);

        return CourseMapper.ToDto(course, category, thumbnailMedia, lessons);
    }

    public static async Task<IReadOnlyCollection<CourseDto>> MapAsync(
        IReadOnlyCollection<Course> courses,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var categoryIds = courses
            .Where(course => course.CategoryId.HasValue)
            .Select(course => course.CategoryId!.Value)
            .Distinct()
            .ToArray();
        var categories = categoryIds.Length == 0
            ? []
            : await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryById = categories.ToDictionary(category => category.Id);

        var mediaIds = courses
            .Where(course => course.ThumbnailMediaId.HasValue)
            .Select(course => course.ThumbnailMediaId!.Value)
            .Distinct()
            .ToArray();
        var mediaItems = mediaIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(mediaIds, cancellationToken);
        var mediaById = mediaItems.ToDictionary(media => media.Id);

        var lessonIds = courses
            .SelectMany(course => course.Lessons.Select(relation => relation.LessonId))
            .Distinct()
            .ToArray();
        var lessons = await LoadLessonsAsync(lessonIds, lessonRepository, cancellationToken);

        return courses
            .Select(course =>
            {
                Category? category = null;
                if (course.CategoryId.HasValue)
                {
                    categoryById.TryGetValue(course.CategoryId.Value, out category);
                }

                MediaEntity? thumbnailMedia = null;
                if (course.ThumbnailMediaId.HasValue)
                {
                    mediaById.TryGetValue(course.ThumbnailMediaId.Value, out thumbnailMedia);
                }

                return CourseMapper.ToDto(course, category, thumbnailMedia, lessons);
            })
            .ToArray();
    }

    public static async Task<IReadOnlyCollection<CourseLessonDto>> MapLessonsAsync(
        IReadOnlyCollection<CourseLesson> courseLessons,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var lessonIds = courseLessons.Select(relation => relation.LessonId).Distinct().ToArray();
        var lessons = await LoadLessonsAsync(lessonIds, lessonRepository, cancellationToken);
        var lessonById = lessons.ToDictionary(lesson => lesson.Id);

        return CourseMapper.ToLessonDtos(courseLessons, lessonById);
    }

    private static async Task<IReadOnlyCollection<Lesson>> LoadLessonsAsync(
        IReadOnlyCollection<Guid> lessonIds,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        if (lessonIds.Count == 0)
        {
            return [];
        }

        return await lessonRepository.GetByIdsAsync(lessonIds, cancellationToken);
    }
}
