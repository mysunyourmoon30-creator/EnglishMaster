using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Domain.Books;

public sealed class Book
{
    private readonly List<BookChapter> chapters = [];

    private Book()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Subtitle = string.Empty;
        Summary = string.Empty;
        Description = string.Empty;
        AuthorName = string.Empty;
        Edition = string.Empty;
        Version = string.Empty;
    }

    private Book(
        Guid id,
        string? title,
        string? subtitle,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? coverMediaId,
        Guid? courseId,
        string? authorName,
        string? edition,
        string? version,
        int estimatedPages,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            title,
            subtitle,
            summary,
            description,
            cefrLevel,
            categoryId,
            coverMediaId,
            courseId,
            authorName,
            edition,
            version,
            estimatedPages,
            sortOrder,
            isPublished,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Subtitle { get; private set; } = string.Empty;

    public string Summary { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public CefrLevel? CefrLevel { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Guid? CoverMediaId { get; private set; }

    public Guid? CourseId { get; private set; }

    public string AuthorName { get; private set; } = string.Empty;

    public string Edition { get; private set; } = string.Empty;

    public string Version { get; private set; } = string.Empty;

    public int EstimatedPages { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<BookChapter> Chapters => chapters.AsReadOnly();

    public bool IsPublished { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Book Create(
        string? title,
        string? subtitle,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? coverMediaId,
        Guid? courseId,
        string? authorName,
        string? edition,
        string? version,
        int estimatedPages,
        int sortOrder,
        DateTimeOffset now)
    {
        return new Book(
            Guid.NewGuid(),
            title,
            subtitle,
            summary,
            description,
            cefrLevel,
            categoryId,
            coverMediaId,
            courseId,
            authorName,
            edition,
            version,
            estimatedPages,
            sortOrder,
            isPublished: false,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? title,
        string? subtitle,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? coverMediaId,
        Guid? courseId,
        string? authorName,
        string? edition,
        string? version,
        int estimatedPages,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            title,
            subtitle,
            summary,
            description,
            cefrLevel,
            categoryId,
            coverMediaId,
            courseId,
            authorName,
            edition,
            version,
            estimatedPages,
            sortOrder,
            isPublished,
            isActive,
            now);
    }

    public void Publish(DateTimeOffset now)
    {
        IsPublished = true;
        UpdatedAt = now;
    }

    public void Unpublish(DateTimeOffset now)
    {
        IsPublished = false;
        UpdatedAt = now;
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

    public static string GenerateSlug(string? title)
    {
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), BookFieldLimits.Title);
    }

    private void Apply(
        string? title,
        string? subtitle,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? coverMediaId,
        Guid? courseId,
        string? authorName,
        string? edition,
        string? version,
        int estimatedPages,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Title = BookDomainGuard.RequiredText(title, nameof(Title), nameof(title), BookFieldLimits.Title);
        Slug = GenerateSlug(Title);
        Subtitle = BookDomainGuard.OptionalText(subtitle, nameof(Subtitle), nameof(subtitle), BookFieldLimits.Subtitle);
        Summary = BookDomainGuard.OptionalText(summary, nameof(Summary), nameof(summary), BookFieldLimits.Summary);
        Description = BookDomainGuard.OptionalText(description, nameof(Description), nameof(description), BookFieldLimits.Description);
        CefrLevel = cefrLevel;
        CategoryId = BookDomainGuard.OptionalId(categoryId, nameof(categoryId));
        CoverMediaId = BookDomainGuard.OptionalId(coverMediaId, nameof(coverMediaId));
        CourseId = BookDomainGuard.OptionalId(courseId, nameof(courseId));
        AuthorName = BookDomainGuard.OptionalText(authorName, nameof(AuthorName), nameof(authorName), BookFieldLimits.AuthorName);
        Edition = BookDomainGuard.OptionalText(edition, nameof(Edition), nameof(edition), BookFieldLimits.Edition);
        Version = BookDomainGuard.OptionalText(version, nameof(Version), nameof(version), BookFieldLimits.Version);
        EstimatedPages = BookDomainGuard.NonNegative(estimatedPages, nameof(estimatedPages));
        SortOrder = BookDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

        if (isPublished)
        {
            Publish(updatedAt);
        }
        else
        {
            Unpublish(updatedAt);
        }

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
