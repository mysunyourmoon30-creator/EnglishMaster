namespace EnglishMaster.Domain.LearningReports;

public sealed class WeeklyLearningReport
{
    private readonly List<WeeklyLearningReportInsight> insights = [];

    private WeeklyLearningReport()
    {
        SummaryText = string.Empty;
    }

    private WeeklyLearningReport(Guid studentProfileId, DateTimeOffset weekStartDate, DateTimeOffset weekEndDate, DateTimeOffset now)
    {
        Id = Guid.NewGuid();
        StudentProfileId = studentProfileId == Guid.Empty ? throw new ArgumentException("StudentProfileId is required.", nameof(studentProfileId)) : studentProfileId;
        if (weekEndDate < weekStartDate)
        {
            throw new ArgumentException("WeekEndDate must be after or equal WeekStartDate.", nameof(weekEndDate));
        }

        WeekStartDate = weekStartDate;
        WeekEndDate = weekEndDate;
        Status = WeeklyLearningReportStatus.Generated;
        GeneratedAt = now;
        CreatedAt = now;
        UpdatedAt = now;
        SummaryText = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid StudentProfileId { get; private set; }
    public DateTimeOffset WeekStartDate { get; private set; }
    public DateTimeOffset WeekEndDate { get; private set; }
    public WeeklyLearningReportStatus Status { get; private set; }
    public DateTimeOffset GeneratedAt { get; private set; }
    public int TotalStudyMinutes { get; private set; }
    public int ActiveStudyDays { get; private set; }
    public int CompletedDailyPlans { get; private set; }
    public int LessonsStarted { get; private set; }
    public int LessonsCompleted { get; private set; }
    public int CoursesStarted { get; private set; }
    public int CoursesCompleted { get; private set; }
    public int BooksStarted { get; private set; }
    public int BooksCompleted { get; private set; }
    public int PracticeSessionsCompleted { get; private set; }
    public int PracticeItemsCompleted { get; private set; }
    public int QuizAttempts { get; private set; }
    public int QuizzesPassed { get; private set; }
    public decimal AverageQuizScore { get; private set; }
    public int GoalsCompleted { get; private set; }
    public int AchievementsEarned { get; private set; }
    public int CurrentStreakDays { get; private set; }
    public int LongestStreakDays { get; private set; }
    public string SummaryText { get; private set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<WeeklyLearningReportInsight> Insights => insights.AsReadOnly();

    public static WeeklyLearningReport Create(Guid studentProfileId, DateTimeOffset weekStartDate, DateTimeOffset weekEndDate, DateTimeOffset now) =>
        new(studentProfileId, weekStartDate, weekEndDate, now);

    public void ApplyMetrics(
        int totalStudyMinutes,
        int activeStudyDays,
        int completedDailyPlans,
        int lessonsStarted,
        int lessonsCompleted,
        int coursesStarted,
        int coursesCompleted,
        int booksStarted,
        int booksCompleted,
        int practiceSessionsCompleted,
        int practiceItemsCompleted,
        int quizAttempts,
        int quizzesPassed,
        decimal averageQuizScore,
        int goalsCompleted,
        int achievementsEarned,
        int currentStreakDays,
        int longestStreakDays,
        string summaryText,
        DateTimeOffset now)
    {
        TotalStudyMinutes = NonNegative(totalStudyMinutes);
        ActiveStudyDays = NonNegative(activeStudyDays);
        CompletedDailyPlans = NonNegative(completedDailyPlans);
        LessonsStarted = NonNegative(lessonsStarted);
        LessonsCompleted = NonNegative(lessonsCompleted);
        CoursesStarted = NonNegative(coursesStarted);
        CoursesCompleted = NonNegative(coursesCompleted);
        BooksStarted = NonNegative(booksStarted);
        BooksCompleted = NonNegative(booksCompleted);
        PracticeSessionsCompleted = NonNegative(practiceSessionsCompleted);
        PracticeItemsCompleted = NonNegative(practiceItemsCompleted);
        QuizAttempts = NonNegative(quizAttempts);
        QuizzesPassed = NonNegative(quizzesPassed);
        AverageQuizScore = Math.Clamp(averageQuizScore, 0, 100);
        GoalsCompleted = NonNegative(goalsCompleted);
        AchievementsEarned = NonNegative(achievementsEarned);
        CurrentStreakDays = NonNegative(currentStreakDays);
        LongestStreakDays = NonNegative(longestStreakDays);
        SummaryText = summaryText?.Trim() ?? string.Empty;
        GeneratedAt = now;
        Status = WeeklyLearningReportStatus.Generated;
        UpdatedAt = now;
    }

    public void ReplaceInsights(IEnumerable<WeeklyLearningReportInsight> newInsights, DateTimeOffset now)
    {
        insights.Clear();
        insights.AddRange(newInsights);
        UpdatedAt = now;
    }

    public void Archive(DateTimeOffset now)
    {
        Status = WeeklyLearningReportStatus.Archived;
        UpdatedAt = now;
    }

    private static int NonNegative(int value) => value < 0 ? 0 : value;
}
