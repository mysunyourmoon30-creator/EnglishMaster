using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.Domain.Courses;

public sealed class Course
{
    private readonly List<CourseLesson> lessons = [];

    private Course()
    {
        Title = string.Empty;
        Slug = string.Empty;
        Summary = string.Empty;
        Description = string.Empty;
    }

    private Course(
        Guid id,
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
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
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes,
            sortOrder,
            isPublished,
            isActive,
            updatedAt);
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Slug { get; private set; } = string.Empty;

    public string Summary { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public CefrLevel? CefrLevel { get; private set; }

    public Guid? CategoryId { get; private set; }

    public Guid? ThumbnailMediaId { get; private set; }

    public int EstimatedMinutes { get; private set; }

    public int SortOrder { get; private set; }

    public IReadOnlyCollection<CourseLesson> Lessons => lessons.AsReadOnly();

    public bool IsPublished { get; private set; }

    public bool IsActive { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static Course Create(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        DateTimeOffset now)
    {
        return new Course(
            Guid.NewGuid(),
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes,
            sortOrder,
            isPublished: false,
            isActive: true,
            createdAt: now,
            updatedAt: now);
    }

    public void Update(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset now)
    {
        Apply(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId,
            estimatedMinutes,
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

    public CourseLesson AddLesson(
        Guid lessonId,
        int sortOrder,
        bool isRequired,
        DateTimeOffset now)
    {
        CourseDomainGuard.RequiredId(lessonId, nameof(lessonId));
        var existing = lessons.SingleOrDefault(item => item.LessonId == lessonId);
        if (existing is not null)
        {
            existing.UpdateRequirement(isRequired, now);
            return existing;
        }

        var courseLesson = CourseLesson.Create(Id, lessonId, sortOrder, isRequired, now);
        lessons.Add(courseLesson);
        UpdatedAt = now;
        return courseLesson;
    }

    public bool RemoveLesson(Guid lessonId, DateTimeOffset now)
    {
        CourseDomainGuard.RequiredId(lessonId, nameof(lessonId));
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
        return SlugGenerator.Generate(title, nameof(Title), nameof(title), CourseFieldLimits.Title);
    }

    private void Apply(
        string? title,
        string? summary,
        string? description,
        CefrLevel? cefrLevel,
        Guid? categoryId,
        Guid? thumbnailMediaId,
        int estimatedMinutes,
        int sortOrder,
        bool isPublished,
        bool isActive,
        DateTimeOffset updatedAt)
    {
        Title = CourseDomainGuard.RequiredText(title, nameof(Title), CourseFieldLimits.Title);
        Slug = GenerateSlug(Title);
        Summary = CourseDomainGuard.OptionalText(summary, nameof(Summary), CourseFieldLimits.Summary);
        Description = CourseDomainGuard.OptionalText(description, nameof(Description), CourseFieldLimits.Description);
        CefrLevel = cefrLevel;
        CategoryId = CourseDomainGuard.OptionalId(categoryId, nameof(categoryId));
        ThumbnailMediaId = CourseDomainGuard.OptionalId(thumbnailMediaId, nameof(thumbnailMediaId));
        EstimatedMinutes = CourseDomainGuard.NonNegative(estimatedMinutes, nameof(estimatedMinutes));
        SortOrder = CourseDomainGuard.NonNegative(sortOrder, nameof(sortOrder));

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
