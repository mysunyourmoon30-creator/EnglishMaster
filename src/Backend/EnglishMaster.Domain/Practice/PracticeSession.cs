namespace EnglishMaster.Domain.Practice;

public sealed class PracticeSession
{
    private readonly List<PracticeSessionItem> items = [];

    private PracticeSession()
    {
    }

    private PracticeSession(Guid studentProfileId, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId == Guid.Empty ? throw new ArgumentException("StudentProfileId is required.", nameof(studentProfileId)) : studentProfileId;
        StartedAt = now;
        Status = PracticeSessionStatus.Started;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public DateTimeOffset StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public PracticeSessionStatus Status { get; private set; }
    public int TotalItems { get; private set; }
    public int CompletedItems { get; private set; }
    public int CorrectItems { get; private set; }
    public int IncorrectItems { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<PracticeSessionItem> Items => items.AsReadOnly();

    public static PracticeSession Create(Guid studentProfileId, DateTimeOffset now) =>
        new(studentProfileId, now);

    public void AddItem(PracticeItem item, string promptText, string answerText, DateTimeOffset now)
    {
        items.Add(PracticeSessionItem.Create(Id, item.Id, item.ContentType, item.ContentId, item.PracticeType, promptText, answerText, now));
        TotalItems = items.Count;
        UpdatedAt = now;
    }

    public void Recount(DateTimeOffset now)
    {
        CompletedItems = items.Count(item => item.PracticedAt.HasValue);
        CorrectItems = items.Count(item => item.IsCorrect == true);
        IncorrectItems = items.Count(item => item.IsCorrect == false);
        UpdatedAt = now;
    }

    public void Complete(DateTimeOffset now)
    {
        if (Status != PracticeSessionStatus.Started)
        {
            throw new InvalidOperationException("Only started practice sessions can be completed.");
        }

        Recount(now);
        Status = PracticeSessionStatus.Completed;
        CompletedAt = now;
        UpdatedAt = now;
    }
}
