namespace EnglishMaster.Domain.Learning;

public sealed class CourseProgress
{
    private CourseProgress()
    {
    }

    private CourseProgress(Guid userId, Guid courseId, int progressPercent, LearningProgressStatus status, DateTimeOffset lastAccessedAt, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        UserId = userId == Guid.Empty ? throw new ArgumentException("UserId is required.", nameof(userId)) : userId;
        CourseId = courseId == Guid.Empty ? throw new ArgumentException("CourseId is required.", nameof(courseId)) : courseId;
        ProgressPercent = Math.Clamp(progressPercent, 0, 100);
        Status = status;
        LastAccessedAt = lastAccessedAt;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid CourseId { get; private set; }
    public int ProgressPercent { get; private set; }
    public LearningProgressStatus Status { get; private set; }
    public DateTimeOffset LastAccessedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static CourseProgress Create(Guid userId, Guid courseId, int progressPercent, LearningProgressStatus status, DateTimeOffset lastAccessedAt, DateTimeOffset now) =>
        new(userId, courseId, progressPercent, status, lastAccessedAt, now);
}

