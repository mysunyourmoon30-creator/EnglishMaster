using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Books.Dtos;

internal static class BookReadModelBuilder
{
    public static async Task<BookDto> MapAsync(
        Book book,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var category = book.CategoryId.HasValue
            ? await categoryRepository.GetByIdAsync(book.CategoryId.Value, cancellationToken)
            : null;
        var coverMedia = book.CoverMediaId.HasValue
            ? await mediaRepository.GetByIdAsync(book.CoverMediaId.Value, cancellationToken)
            : null;
        var course = book.CourseId.HasValue
            ? await courseRepository.GetByIdAsync(book.CourseId.Value, cancellationToken)
            : null;
        var lessons = await LoadLessonsAsync(GetLessonIds([book]), lessonRepository, cancellationToken);

        return BookMapper.ToDto(book, category, coverMedia, course, lessons);
    }

    public static async Task<IReadOnlyCollection<BookDto>> MapAsync(
        IReadOnlyCollection<Book> books,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var categoryIds = books
            .Where(book => book.CategoryId.HasValue)
            .Select(book => book.CategoryId!.Value)
            .Distinct()
            .ToArray();
        var categories = categoryIds.Length == 0
            ? []
            : await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryById = categories.ToDictionary(category => category.Id);

        var mediaIds = books
            .Where(book => book.CoverMediaId.HasValue)
            .Select(book => book.CoverMediaId!.Value)
            .Distinct()
            .ToArray();
        var mediaItems = mediaIds.Length == 0
            ? []
            : await mediaRepository.GetByIdsAsync(mediaIds, cancellationToken);
        var mediaById = mediaItems.ToDictionary(media => media.Id);

        var courseById = new Dictionary<Guid, Course>();
        foreach (var courseId in books.Where(book => book.CourseId.HasValue).Select(book => book.CourseId!.Value).Distinct())
        {
            var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course is not null)
            {
                courseById[course.Id] = course;
            }
        }

        var lessons = await LoadLessonsAsync(GetLessonIds(books), lessonRepository, cancellationToken);

        return books
            .Select(book =>
            {
                Category? category = null;
                if (book.CategoryId.HasValue)
                {
                    categoryById.TryGetValue(book.CategoryId.Value, out category);
                }

                MediaEntity? coverMedia = null;
                if (book.CoverMediaId.HasValue)
                {
                    mediaById.TryGetValue(book.CoverMediaId.Value, out coverMedia);
                }

                Course? course = null;
                if (book.CourseId.HasValue)
                {
                    courseById.TryGetValue(book.CourseId.Value, out course);
                }

                return BookMapper.ToDto(book, category, coverMedia, course, lessons);
            })
            .ToArray();
    }

    private static IReadOnlyCollection<Guid> GetLessonIds(IReadOnlyCollection<Book> books)
    {
        return books
            .SelectMany(book => book.Chapters)
            .SelectMany(chapter => chapter.Lessons)
            .Select(relation => relation.LessonId)
            .Distinct()
            .ToArray();
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
