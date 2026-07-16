using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.Books.Dtos;

internal static class BookMapper
{
    public static BookDto ToDto(
        Book book,
        Category? category = null,
        MediaEntity? coverMedia = null,
        Course? course = null,
        IReadOnlyCollection<Lesson>? relatedLessons = null)
    {
        var lessonById = relatedLessons?.ToDictionary(lesson => lesson.Id) ?? new Dictionary<Guid, Lesson>();

        return new BookDto(
            book.Id,
            book.Title,
            book.Slug,
            book.Subtitle,
            book.Summary,
            book.Description,
            book.CefrLevel?.ToString(),
            book.CategoryId,
            category is null ? null : new BookCategoryDto(category.Id, category.Name, category.Slug),
            book.CoverMediaId,
            coverMedia is null ? null : ToMediaDto(coverMedia),
            book.CourseId,
            course is null ? null : new BookCourseDto(course.Id, course.Title, course.Slug),
            book.AuthorName,
            book.Edition,
            book.Version,
            book.EstimatedPages,
            book.SortOrder,
            ToChapterDtos(book.Chapters, lessonById),
            book.IsPublished,
            book.IsActive,
            book.CreatedAt,
            book.UpdatedAt);
    }

    public static IReadOnlyCollection<BookChapterDto> ToChapterDtos(
        IReadOnlyCollection<BookChapter> chapters,
        IReadOnlyDictionary<Guid, Lesson> lessonById)
    {
        return chapters
            .OrderBy(chapter => chapter.SortOrder)
            .ThenBy(chapter => chapter.Title)
            .Select(chapter => ToChapterDto(chapter, lessonById))
            .ToArray();
    }

    public static BookChapterDto ToChapterDto(
        BookChapter chapter,
        IReadOnlyDictionary<Guid, Lesson>? lessonById = null)
    {
        return new BookChapterDto(
            chapter.Id,
            chapter.BookId,
            chapter.Title,
            chapter.Slug,
            chapter.Summary,
            chapter.ContentMarkdown,
            chapter.SortOrder,
            ToLessonDtos(chapter.Lessons, lessonById ?? new Dictionary<Guid, Lesson>()),
            chapter.IsActive,
            chapter.CreatedAt,
            chapter.UpdatedAt);
    }

    public static IReadOnlyCollection<BookChapterLessonDto> ToLessonDtos(
        IReadOnlyCollection<BookChapterLesson> chapterLessons,
        IReadOnlyDictionary<Guid, Lesson> lessonById)
    {
        return chapterLessons
            .Select(relation => lessonById.TryGetValue(relation.LessonId, out var lesson)
                ? ToLessonDto(relation, lesson)
                : null)
            .Where(relation => relation is not null)
            .Cast<BookChapterLessonDto>()
            .OrderBy(relation => relation.SortOrder)
            .ThenBy(relation => relation.LessonTitle)
            .ToArray();
    }

    public static BookChapterLessonDto ToLessonDto(BookChapterLesson relation, Lesson lesson)
    {
        return new BookChapterLessonDto(
            relation.Id,
            relation.BookChapterId,
            relation.LessonId,
            lesson.Title,
            lesson.Slug,
            lesson.Summary,
            lesson.CefrLevel?.ToString(),
            relation.SortOrder,
            relation.CreatedAt,
            relation.UpdatedAt);
    }

    public static BookMediaDto ToMediaDto(MediaEntity media)
    {
        return new BookMediaDto(
            media.Id,
            media.FileName,
            media.ContentType,
            media.MediaType.ToString(),
            media.PublicUrl,
            media.AltText);
    }
}
