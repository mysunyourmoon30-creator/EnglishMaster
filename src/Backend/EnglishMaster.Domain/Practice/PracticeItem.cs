namespace EnglishMaster.Domain.Practice;

public sealed class PracticeItem
{
    private PracticeItem()
    {
        ContentType = string.Empty;
        PracticeType = string.Empty;
    }

    private PracticeItem(Guid studentProfileId, string contentType, Guid contentId, string practiceType, DateTimeOffset dueAt, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = RequiredId(studentProfileId, nameof(studentProfileId));
        ContentType = RequiredText(contentType, nameof(ContentType), 64);
        ContentId = RequiredId(contentId, nameof(contentId));
        PracticeType = RequiredText(practiceType, nameof(PracticeType), 64);
        Status = PracticeItemStatus.New;
        DueAt = dueAt;
        NextReviewAt = dueAt;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public string ContentType { get; private set; } = string.Empty;
    public Guid ContentId { get; private set; }
    public string PracticeType { get; private set; } = string.Empty;
    public PracticeItemStatus Status { get; private set; }
    public DateTimeOffset DueAt { get; private set; }
    public DateTimeOffset? LastPracticedAt { get; private set; }
    public DateTimeOffset NextReviewAt { get; private set; }
    public int ReviewCount { get; private set; }
    public int CorrectCount { get; private set; }
    public int IncorrectCount { get; private set; }
    public int CurrentIntervalDays { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static PracticeItem Create(Guid studentProfileId, string contentType, Guid contentId, string practiceType, DateTimeOffset dueAt, DateTimeOffset now) =>
        new(studentProfileId, contentType, contentId, practiceType, dueAt, now);

    public void ApplyResult(PracticeResult result, DateTimeOffset now)
    {
        ReviewCount++;
        LastPracticedAt = now;
        if (result == PracticeResult.Again)
        {
            IncorrectCount++;
        }
        else
        {
            CorrectCount++;
        }

        CurrentIntervalDays = NextInterval(result);
        NextReviewAt = now.AddDays(CurrentIntervalDays);
        DueAt = NextReviewAt;
        Status = CorrectCount >= 5 && CurrentIntervalDays >= 30 ? PracticeItemStatus.Mastered : PracticeItemStatus.Reviewing;
        UpdatedAt = now;
    }

    public void Suspend(DateTimeOffset now)
    {
        Status = PracticeItemStatus.Suspended;
        UpdatedAt = now;
    }

    public void Resume(DateTimeOffset now)
    {
        Status = NextReviewAt <= now ? PracticeItemStatus.Due : PracticeItemStatus.Reviewing;
        UpdatedAt = now;
    }

    private int NextInterval(PracticeResult result)
    {
        if (result == PracticeResult.Again)
        {
            return 1;
        }

        if (result == PracticeResult.Hard)
        {
            return Math.Max(2, CurrentIntervalDays == 0 ? 2 : Math.Min(CurrentIntervalDays * 2, 30));
        }

        if (result == PracticeResult.Easy)
        {
            return CurrentIntervalDays switch
            {
                < 7 => 7,
                < 14 => 14,
                _ => 30
            };
        }

        return CurrentIntervalDays switch
        {
            0 => 4,
            < 2 => 2,
            < 4 => 4,
            < 7 => 7,
            < 14 => 14,
            _ => 30
        };
    }

    private static Guid RequiredId(Guid value, string fieldName) =>
        value == Guid.Empty ? throw new ArgumentException($"{fieldName} is required.", fieldName) : value;

    private static string RequiredText(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized.Length > maxLength ? throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName) : normalized;
    }
}

