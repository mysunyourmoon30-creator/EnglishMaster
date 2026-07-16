namespace EnglishMaster.Domain.LearningGoals;

public sealed class LearningGoal
{
    private LearningGoal()
    {
        GoalType = string.Empty;
        Title = string.Empty;
        Description = string.Empty;
        Unit = string.Empty;
    }

    private LearningGoal(Guid studentProfileId, string goalType, string title, string? description, int targetValue, string? unit, DateTimeOffset? targetDate, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = RequiredId(studentProfileId, nameof(studentProfileId));
        GoalType = RequiredText(goalType, nameof(GoalType), 64);
        Title = RequiredText(title, nameof(Title), 160);
        Description = description?.Trim() ?? string.Empty;
        TargetValue = NonNegative(targetValue, nameof(TargetValue));
        Unit = unit?.Trim() ?? string.Empty;
        TargetDate = targetDate;
        Status = LearningGoalStatus.Active;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public string GoalType { get; private set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public int TargetValue { get; private set; }
    public int CurrentValue { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public DateTimeOffset? TargetDate { get; private set; }
    public LearningGoalStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static LearningGoal Create(Guid studentProfileId, string goalType, string title, string? description, int targetValue, string? unit, DateTimeOffset? targetDate, DateTimeOffset now) =>
        new(studentProfileId, goalType, title, description, targetValue, unit, targetDate, now);

    public void Update(string title, string? description, int targetValue, int currentValue, string? unit, DateTimeOffset? targetDate, DateTimeOffset now)
    {
        EnsureEditable();
        Title = RequiredText(title, nameof(Title), 160);
        Description = description?.Trim() ?? string.Empty;
        TargetValue = NonNegative(targetValue, nameof(TargetValue));
        CurrentValue = Math.Min(NonNegative(currentValue, nameof(CurrentValue)), TargetValue);
        Unit = unit?.Trim() ?? string.Empty;
        TargetDate = targetDate;
        UpdatedAt = now;
    }

    public void Pause(DateTimeOffset now)
    {
        if (Status != LearningGoalStatus.Active)
        {
            throw new InvalidOperationException("Only active goals can be paused.");
        }

        Status = LearningGoalStatus.Paused;
        UpdatedAt = now;
    }

    public void Resume(DateTimeOffset now)
    {
        if (Status != LearningGoalStatus.Paused)
        {
            throw new InvalidOperationException("Only paused goals can be resumed.");
        }

        Status = LearningGoalStatus.Active;
        UpdatedAt = now;
    }

    public void Complete(DateTimeOffset now)
    {
        if (Status != LearningGoalStatus.Active)
        {
            throw new InvalidOperationException("Only active goals can be completed.");
        }

        CurrentValue = TargetValue;
        Status = LearningGoalStatus.Completed;
        UpdatedAt = now;
    }

    public void Cancel(DateTimeOffset now)
    {
        if (Status is LearningGoalStatus.Completed or LearningGoalStatus.Cancelled)
        {
            throw new InvalidOperationException("Completed or cancelled goals cannot be cancelled again.");
        }

        Status = LearningGoalStatus.Cancelled;
        UpdatedAt = now;
    }

    private void EnsureEditable()
    {
        if (Status is LearningGoalStatus.Completed or LearningGoalStatus.Cancelled)
        {
            throw new InvalidOperationException("Completed or cancelled goals cannot be modified.");
        }
    }

    private static Guid RequiredId(Guid value, string fieldName) =>
        value == Guid.Empty ? throw new ArgumentException($"{fieldName} is required.", fieldName) : value;

    private static int NonNegative(int value, string fieldName) =>
        value < 0 ? throw new ArgumentException($"{fieldName} must not be negative.", fieldName) : value;

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
