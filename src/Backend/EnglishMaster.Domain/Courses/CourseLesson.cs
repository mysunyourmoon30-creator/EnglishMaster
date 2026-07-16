namespace EnglishMaster.Domain.Courses;

public sealed class CourseLesson
{
    private CourseLesson()
    {
    }

    private CourseLesson(
        Guid id,
        Guid courseId,
        Guid lessonId,
        int sortOrder,
        bool isRequired,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Id = CourseDomainGuard.RequiredId(id, nameof(id));
        CourseId = CourseDomainGuard.RequiredId(courseId, nameof(courseId));
        LessonId = CourseDomainGuard.RequiredId(lessonId, nameof(lessonId));
        SortOrder = CourseDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        IsRequired = isRequired;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }

    public Guid CourseId { get; private set; }

    public Guid LessonId { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsRequired { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public DateTimeOffset UpdatedAt { get; private set; }

    public static CourseLesson Create(
        Guid courseId,
        Guid lessonId,
        int sortOrder,
        bool isRequired,
        DateTimeOffset now)
    {
        return new CourseLesson(
            Guid.NewGuid(),
            courseId,
            lessonId,
            sortOrder,
            isRequired,
            now,
            now);
    }

    public void UpdateRequirement(bool isRequired, DateTimeOffset now)
    {
        IsRequired = isRequired;
        UpdatedAt = now;
    }

    public void Reorder(int sortOrder, DateTimeOffset now)
    {
        SortOrder = CourseDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
        UpdatedAt = now;
    }
}
