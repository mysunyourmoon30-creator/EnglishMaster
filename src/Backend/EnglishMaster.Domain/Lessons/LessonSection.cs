namespace EnglishMaster.Domain.Lessons;

public sealed class LessonSection
{
    private LessonSection()
    {
        Title = string.Empty;
        ContentMarkdown = string.Empty;
    }

    private LessonSection(
        Guid id,
        Guid lessonId,
        string? title,
        string? contentMarkdown,
        SectionType sectionType,
        Guid? mediaId,
        int sortOrder,
        bool isActive,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = id;
        CreatedAt = createdAt;
        Apply(
            lessonId,
            title,
            contentMarkdown,
            sectionType,
            mediaId,
            sortOrder,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public Guid LessonId { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string ContentMarkdown { get; private set; } = string.Empty;

    public SectionType SectionType { get; private set; }

    public Guid? MediaId { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static LessonSection Create(
        Guid lessonId,
        string? title,
        string? contentMarkdown,
        SectionType sectionType,
        Guid? mediaId,
        int sortOrder,
        DateTimeOffset now)
    {
        return new LessonSection(
            Guid.NewGuid(),
            lessonId,
            title,
            contentMarkdown,
            sectionType,
            mediaId,
            sortOrder,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? title,
        string? contentMarkdown,
        SectionType sectionType,
        Guid? mediaId,
        int sortOrder,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            LessonId,
            title,
            contentMarkdown,
            sectionType,
            mediaId,
            sortOrder,
            isActive,
            now);
    }

    public void Reorder(int sortOrder, DateTimeOffset now)
    {
        SortOrder = LessonDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
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

    private void Apply(
        Guid lessonId,
        string? title,
        string? contentMarkdown,
        SectionType sectionType,
        Guid? mediaId,
        int sortOrder,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        LessonId = LessonDomainGuard.RequiredId(lessonId, nameof(lessonId));
        Title = LessonDomainGuard.RequiredText(title, nameof(Title), LessonSectionFieldLimits.Title);
        ContentMarkdown = LessonDomainGuard.OptionalText(contentMarkdown, nameof(ContentMarkdown), LessonSectionFieldLimits.ContentMarkdown);
        SectionType = sectionType;
        MediaId = LessonDomainGuard.OptionalId(mediaId, nameof(mediaId));
        SortOrder = LessonDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

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
