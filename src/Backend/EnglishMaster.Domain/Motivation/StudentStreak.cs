namespace EnglishMaster.Domain.Motivation;

public sealed class StudentStreak
{
    private StudentStreak()
    {
    }

    private StudentStreak(Guid studentProfileId, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId == Guid.Empty ? throw new ArgumentException("StudentProfileId is required.", nameof(studentProfileId)) : studentProfileId;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public int CurrentStreakDays { get; private set; }
    public int LongestStreakDays { get; private set; }
    public DateTimeOffset? LastActivityDate { get; private set; }
    public DateTimeOffset? StreakStartDate { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public static StudentStreak Create(Guid studentProfileId, DateTimeOffset now) => new(studentProfileId, now);

    public void ApplyActivity(DateTimeOffset activityAt, DateTimeOffset now)
    {
        var activityDate = DateOnly.FromDateTime(activityAt.UtcDateTime);
        if (LastActivityDate.HasValue)
        {
            var lastDate = DateOnly.FromDateTime(LastActivityDate.Value.UtcDateTime);
            var diff = activityDate.DayNumber - lastDate.DayNumber;
            if (diff < 0)
            {
                UpdatedAt = now;
                return;
            }

            if (diff == 0)
            {
                UpdatedAt = now;
                return;
            }

            if (diff == 1)
            {
                CurrentStreakDays++;
            }
            else
            {
                CurrentStreakDays = 1;
                StreakStartDate = new DateTimeOffset(activityAt.UtcDateTime.Date, TimeSpan.Zero);
            }
        }
        else
        {
            CurrentStreakDays = 1;
            StreakStartDate = new DateTimeOffset(activityAt.UtcDateTime.Date, TimeSpan.Zero);
        }

        LastActivityDate = new DateTimeOffset(activityAt.UtcDateTime.Date, TimeSpan.Zero);
        LongestStreakDays = Math.Max(LongestStreakDays, CurrentStreakDays);
        UpdatedAt = now;
    }
}
