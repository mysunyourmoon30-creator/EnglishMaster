namespace EnglishMaster.Domain.Learning;

public sealed class BookProgress
{
    private BookProgress()
    {
    }

    private BookProgress(Guid userId, Guid bookId, int progressPercent, LearningProgressStatus status, DateTimeOffset lastAccessedAt, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        UserId = userId == Guid.Empty ? throw new ArgumentException("UserId is required.", nameof(userId)) : userId;
        BookId = bookId == Guid.Empty ? throw new ArgumentException("BookId is required.", nameof(bookId)) : bookId;
        ProgressPercent = Math.Clamp(progressPercent, 0, 100);
        Status = status;
        LastAccessedAt = lastAccessedAt;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid BookId { get; private set; }
    public int ProgressPercent { get; private set; }
    public LearningProgressStatus Status { get; private set; }
    public DateTimeOffset LastAccessedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static BookProgress Create(Guid userId, Guid bookId, int progressPercent, LearningProgressStatus status, DateTimeOffset lastAccessedAt, DateTimeOffset now) =>
        new(userId, bookId, progressPercent, status, lastAccessedAt, now);
}

