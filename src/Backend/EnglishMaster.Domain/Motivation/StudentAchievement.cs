namespace EnglishMaster.Domain.Motivation;

public sealed class StudentAchievement
{
    private StudentAchievement()
    {
    }

    private StudentAchievement(Guid studentProfileId, Guid achievementDefinitionId, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId == Guid.Empty ? throw new ArgumentException("StudentProfileId is required.", nameof(studentProfileId)) : studentProfileId;
        AchievementDefinitionId = achievementDefinitionId == Guid.Empty ? throw new ArgumentException("AchievementDefinitionId is required.", nameof(achievementDefinitionId)) : achievementDefinitionId;
        Status = StudentAchievementStatus.Locked;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public Guid AchievementDefinitionId { get; private set; }
    public StudentAchievementStatus Status { get; private set; }
    public int ProgressValue { get; private set; }
    public DateTimeOffset? EarnedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static StudentAchievement Create(Guid studentProfileId, Guid achievementDefinitionId, DateTimeOffset now) =>
        new(studentProfileId, achievementDefinitionId, now);

    public void ApplyProgress(int progressValue, int targetValue, DateTimeOffset now)
    {
        ProgressValue = Math.Max(0, progressValue);
        if (Status != StudentAchievementStatus.Earned && ProgressValue >= targetValue)
        {
            Status = StudentAchievementStatus.Earned;
            EarnedAt = now;
        }
        else if (Status == StudentAchievementStatus.Locked && ProgressValue > 0)
        {
            Status = StudentAchievementStatus.InProgress;
        }

        UpdatedAt = now;
    }
}
