using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Books;

public sealed class BookChapter
{
    private readonly List<BookChapterLesson> lessons = [];

    private BookChapter()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Summary = string.Empty;
        ContentMarkdown = string.Empty;
    }

    private BookChapter(
        Guid id,
        Guid bookId,
        string? title,
        string? summary,
        string? contentMarkdown,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = BookDomainGuard.RequiredId(id, nameof(id));
        BookId = BookDomainGuard.RequiredId(bookId, nameof(bookId));
        CreatedAt = createdAt;
        Apply(title, summary, contentMarkdown, sortOrder, isActive, updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid BookId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Summary { get; private set; } = string.Empty;

    public string ContentMarkdown { get; private set; } = string.Empty;

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<BookChapterLesson> Lessons => lessons.AsReadOnly();

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static BookChapter Create(
        Guid bookId,
        string? title,
        string? summary,
        string? contentMarkdown,
        int sortOrder,
        DateTimeOffset now)
    {
        return new BookChapter(
            Guid.NewGuid(),
            bookId,
            title,
            summary,
            contentMarkdown,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? title,
        string? summary,
        string? contentMarkdown,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(title, summary, contentMarkdown, sortOrder, isActive, now);
    }

    public void Activate(DateTimeOffset now)
    {
        IsActive = true;
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    public void Reorder(int sortOrder, DateTimeOffset now)
    {
        SortOrder = BookDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        UpdatedAt = now;
    }

    public BookChapterLesson AddLesson(Guid lessonId, int sortOrder, DateTimeOffset now)
    {
        BookDomainGuard.RequiredId(lessonId, nameof(lessonId));
        var existing = lessons.SingleOrDefault(item => item.LessonId == lessonId);
        if (existing is not null)
        {
            return existing;
        }

        var relation = BookChapterLesson.Create(Id, lessonId, sortOrder, now);
        lessons.Add(relation);
        UpdatedAt = now;
        return relation;
    }

    public bool RemoveLesson(Guid lessonId, DateTimeOffset now)
    {
        BookDomainGuard.RequiredId(lessonId, nameof(lessonId));
        var relation = lessons.SingleOrDefault(item => item.LessonId == lessonId);
        if (relation is null)
        {
            return false;
        }

        lessons.Remove(relation);
        UpdatedAt = now;
        return true;
    }

    public static string GenerateSlug(string? title)
    {
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), BookChapterFieldLimits.Title);
    }

    private void Apply(
        string? title,
        string? summary,
        string? contentMarkdown,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Title = BookDomainGuard.RequiredText(title, nameof(Title), nameof(title), BookChapterFieldLimits.Title);
        Slug = GenerateSlug(Title);
        Summary = BookDomainGuard.OptionalText(summary, nameof(Summary), nameof(summary), BookChapterFieldLimits.Summary);
        ContentMarkdown = BookDomainGuard.OptionalText(contentMarkdown, nameof(ContentMarkdown), nameof(contentMarkdown), BookChapterFieldLimits.ContentMarkdown);
        SortOrder = BookDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

        if (isActive)
        {
            Activate(updatedAt);
        }
        else
        {
            Deactivate(updatedAt);
        }
    }
}
