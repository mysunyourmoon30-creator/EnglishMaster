namespace EnglishMaster.Domain.Books;

public sealed class BookChapterLesson
{
    private BookChapterLesson()
    {
    }

    private BookChapterLesson(
        Guid id,
        Guid bookChapterId,
        Guid lessonId,
        int sortOrder,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = BookDomainGuard.RequiredId(id, nameof(id));
        BookChapterId = BookDomainGuard.RequiredId(bookChapterId, nameof(bookChapterId));
        LessonId = BookDomainGuard.RequiredId(lessonId, nameof(lessonId));
        SortOrder = BookDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }

    public Guid BookChapterId { get; private set; }

    public Guid LessonId { get; private set; }

    public int SortOrder { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static BookChapterLesson Create(
        Guid bookChapterId,
        Guid lessonId,
        int sortOrder,
        DateTimeOffset now)
    {
        return new BookChapterLesson(
            Guid.NewGuid(),
            bookChapterId,
            lessonId,
            sortOrder,
            now,
            now);
    }

    public void Reorder(int sortOrder, DateTimeOffset now)
    {
        SortOrder = BookDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        UpdatedAt = now;
    }
}
