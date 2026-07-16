namespace EnglishMaster.Domain.LearningGoals;

public sealed class DailyStudyPlanItem
{
    private DailyStudyPlanItem()
    {
        ItemType = string.Empty;
        ContentType = string.Empty;
        Title = string.Empty;
        Url = string.Empty;
    }

    private DailyStudyPlanItem(Guid dailyStudyPlanId, string itemType, string contentType, Guid? contentId, string title, string url, int estimatedMinutes, int sortOrder, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        DailyStudyPlanId = dailyStudyPlanId == Guid.Empty ? throw new ArgumentException("DailyStudyPlanId is required.", nameof(dailyStudyPlanId)) : dailyStudyPlanId;
        ItemType = RequiredText(itemType, nameof(ItemType), 64);
        ContentType = contentType?.Trim() ?? string.Empty;
        ContentId = contentId;
        Title = RequiredText(title, nameof(Title), 200);
        Url = url?.Trim() ?? string.Empty;
        EstimatedMinutes = estimatedMinutes < 0 ? throw new ArgumentException("EstimatedMinutes must not be negative.", nameof(estimatedMinutes)) : estimatedMinutes;
        SortOrder = sortOrder < 0 ? throw new ArgumentException("SortOrder must not be negative.", nameof(sortOrder)) : sortOrder;
        Status = DailyStudyPlanItemStatus.Pending;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid DailyStudyPlanId { get; private set; }
    public string ItemType { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public Guid? ContentId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    public int EstimatedMinutes { get; private set; }
    public int SortOrder { get; private set; }
    public DailyStudyPlanItemStatus Status { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static DailyStudyPlanItem Create(Guid dailyStudyPlanId, string itemType, string contentType, Guid? contentId, string title, string url, int estimatedMinutes, int sortOrder, DateTimeOffset now) =>
        new(dailyStudyPlanId, itemType, contentType, contentId, title, url, estimatedMinutes, sortOrder, now);

    public void Complete(DateTimeOffset now)
    {
        if (Status == DailyStudyPlanItemStatus.Completed)
        {
            return;
        }

        Status = DailyStudyPlanItemStatus.Completed;
        CompletedAt = now;
        UpdatedAt = now;
    }

    public void Skip(DateTimeOffset now)
    {
        if (Status == DailyStudyPlanItemStatus.Completed)
        {
            throw new InvalidOperationException("Completed plan items cannot be skipped.");
        }

        Status = DailyStudyPlanItemStatus.Skipped;
        UpdatedAt = now;
    }

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
