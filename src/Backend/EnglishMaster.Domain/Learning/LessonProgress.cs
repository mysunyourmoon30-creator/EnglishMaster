namespace EnglishMaster.Domain.Learning;

public sealed class LessonProgress
{
    private LessonProgress()
    {
    }

    private LessonProgress(Guid userId, Guid lessonId, int progressPercent, LearningProgressStatus status, DateTimeOffset lastAccessedAt, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        UserId = userId == Guid.Empty ? throw new ArgumentException("UserId is required.", nameof(userId)) : userId;
        LessonId = lessonId == Guid.Empty ? throw new ArgumentException("LessonId is required.", nameof(lessonId)) : lessonId;
        ProgressPercent = Math.Clamp(progressPercent, 0, 100);
        Status = status;
        LastAccessedAt = lastAccessedAt;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid LessonId { get; private set; }
    public int ProgressPercent { get; private set; }
    public LearningProgressStatus Status { get; private set; }
    public DateTimeOffset LastAccessedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static LessonProgress Create(Guid userId, Guid lessonId, int progressPercent, LearningProgressStatus status, DateTimeOffset lastAccessedAt, DateTimeOffset now) =>
        new(userId, lessonId, progressPercent, status, lastAccessedAt, now);
}

