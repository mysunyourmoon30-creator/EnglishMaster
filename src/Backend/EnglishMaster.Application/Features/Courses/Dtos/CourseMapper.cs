using EnglishMaster.Contracts.Courses;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Courses.Dtos;

internal static class CourseMapper
{
    public static CourseDto ToDto(
        Course course,
        Category? category = null,
        MediaEntity? thumbnailMedia = null,
        IReadOnlyCollection<Lesson>? relatedLessons = null)
    {
        var lessonById = relatedLessons?.ToDictionary(lesson => lesson.Id) ?? new Dictionary<Guid, Lesson>();

        return new CourseDto(
            course.Id,
            course.Title,
            course.Slug,
            course.Summary,
            course.Description,
            course.CefrLevel?.ToString(),
            course.CategoryId,
            category is null ? null : new CourseCategoryDto(category.Id, category.Name, category.Slug),
            course.ThumbnailMediaId,
            thumbnailMedia is null ? null : ToMediaDto(thumbnailMedia),
            course.EstimatedMinutes,
            course.SortOrder,
            ToLessonDtos(course.Lessons, lessonById),
            course.IsPublished,
            course.IsActive,
            course.CreatedAt,
            course.UpdatedAt);
    }

    public static IReadOnlyCollection<CourseLessonDto> ToLessonDtos(
        IReadOnlyCollection<CourseLesson> courseLessons,
        IReadOnlyDictionary<Guid, Lesson> lessonById)
    {
        return courseLessons
            .Select(relation => lessonById.TryGetValue(relation.LessonId, out var lesson)
                ? ToLessonDto(relation, lesson)
                : null)
            .Where(relation => relation is not null)
            .Cast<CourseLessonDto>()
            .OrderBy(relation => relation.SortOrder)
            .ThenBy(relation => relation.LessonTitle)
            .ToArray();
    }

    public static CourseLessonDto ToLessonDto(CourseLesson relation, Lesson lesson)
    {
        return new CourseLessonDto(
            relation.Id,
            relation.CourseId,
            relation.LessonId,
            lesson.Title,
            lesson.Slug,
            lesson.Summary,
            lesson.CefrLevel?.ToString(),
            relation.SortOrder,
            relation.IsRequired,
            relation.CreatedAt,
            relation.UpdatedAt);
    }

    public static CourseMediaDto ToMediaDto(MediaEntity media)
    {
        return new CourseMediaDto(
            media.Id,
            media.FileName,
            media.ContentType,
            media.MediaType.ToString(),
            media.PublicUrl,
            media.AltText);
    }
}
