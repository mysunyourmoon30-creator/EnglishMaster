using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Application.Features.Books.Dtos;

namespace EnglishMaster.Application.Features.BookChapters.Dtos;

internal static class BookChapterReadModelBuilder
{
    public static async Task<BookChapterDto> MapAsync(
        BookChapter chapter,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var lessons = await LoadLessonsAsync(
            chapter.Lessons.Select(relation => relation.LessonId).Distinct().ToArray(),
            lessonRepository,
            cancellationToken);

        return BookMapper.ToChapterDto(chapter, lessons.ToDictionary(lesson => lesson.Id));
    }

    public static async Task<IReadOnlyCollection<BookChapterDto>> MapAsync(
        IReadOnlyCollection<BookChapter> chapters,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var lessonIds = chapters
            .SelectMany(chapter => chapter.Lessons)
            .Select(relation => relation.LessonId)
            .Distinct()
            .ToArray();
        var lessons = await LoadLessonsAsync(lessonIds, lessonRepository, cancellationToken);
        var lessonById = lessons.ToDictionary(lesson => lesson.Id);

        return chapters
            .OrderBy(chapter => chapter.SortOrder)
            .ThenBy(chapter => chapter.Title)
            .Select(chapter => BookMapper.ToChapterDto(chapter, lessonById))
            .ToArray();
    }

    public static async Task<IReadOnlyCollection<BookChapterLessonDto>> MapLessonsAsync(
        IReadOnlyCollection<BookChapterLesson> chapterLessons,
        ILessonRepository lessonRepository,
        CancellationToken cancellationToken)
    {
        var lessonIds = chapterLessons.Select(relation => relation.LessonId).Distinct().ToArray();
        var lessons = await LoadLessonsAsync(lessonIds, lessonRepository, cancellationToken);
        var lessonById = lessons.ToDictionary(lesson => lesson.Id);

        return BookMapper.ToLessonDtos(chapterLessons, lessonById);
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
