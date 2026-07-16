namespace EnglishMaster.Domain.LearningGoals;

public sealed class DailyStudyPlan
{
    private readonly List<DailyStudyPlanItem> items = [];

    private DailyStudyPlan()
    {
    }

    private DailyStudyPlan(Guid studentProfileId, DateTimeOffset planDate, int targetMinutes, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId == Guid.Empty ? throw new ArgumentException("StudentProfileId is required.", nameof(studentProfileId)) : studentProfileId;
        PlanDate = planDate;
        Status = DailyStudyPlanStatus.Planned;
        TargetMinutes = targetMinutes < 0 ? throw new ArgumentException("TargetMinutes must not be negative.", nameof(targetMinutes)) : targetMinutes;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public DateTimeOffset PlanDate { get; private set; }
    public DailyStudyPlanStatus Status { get; private set; }
    public int TargetMinutes { get; private set; }
    public int CompletedMinutes { get; private set; }
    public int TotalItems { get; private set; }
    public int CompletedItems { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<DailyStudyPlanItem> Items => items.AsReadOnly();

    public static DailyStudyPlan Create(Guid studentProfileId, DateTimeOffset planDate, int targetMinutes, DateTimeOffset now) =>
        new(studentProfileId, planDate, targetMinutes, now);

    public DailyStudyPlanItem AddItem(string itemType, string contentType, Guid? contentId, string title, string url, int estimatedMinutes, int sortOrder, DateTimeOffset now)
    {
        var item = DailyStudyPlanItem.Create(Id, itemType, contentType, contentId, title, url, estimatedMinutes, sortOrder, now);
        items.Add(item);
        Recount(now);
        return item;
    }

    public void Recount(DateTimeOffset now)
    {
        TotalItems = items.Count;
        CompletedItems = items.Count(item => item.Status == DailyStudyPlanItemStatus.Completed);
        CompletedMinutes = items.Where(item => item.Status == DailyStudyPlanItemStatus.Completed).Sum(item => item.EstimatedMinutes);
        UpdatedAt = now;
    }

    public void Complete(DateTimeOffset now)
    {
        if (Status == DailyStudyPlanStatus.Completed)
        {
            throw new InvalidOperationException("Study plan is already completed.");
        }

        Recount(now);
        Status = DailyStudyPlanStatus.Completed;
        UpdatedAt = now;
    }

    public void MarkInProgress(DateTimeOffset now)
    {
        if (Status == DailyStudyPlanStatus.Planned)
        {
            Status = DailyStudyPlanStatus.InProgress;
            UpdatedAt = now;
        }
    }
}
