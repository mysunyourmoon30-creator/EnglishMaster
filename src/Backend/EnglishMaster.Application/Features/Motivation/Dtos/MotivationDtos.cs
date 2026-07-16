namespace EnglishMaster.Application.Features.Motivation.Dtos;

public sealed record LearningActivityDto(Guid Id, string ActivityType, string ContentType, Guid? ContentId, string Title, DateTimeOffset OccurredAt, int MinutesSpent, string MetadataJson);
public sealed record StudentStreakDto(int CurrentStreakDays, int LongestStreakDays, DateTimeOffset? LastActivityDate, DateTimeOffset? StreakStartDate);
public sealed record MotivationSummaryDto(
    int CurrentStreakDays,
    int LongestStreakDays,
    int TotalLessonsCompleted,
    int TotalCoursesCompleted,
    int TotalBooksCompleted,
    int TotalQuizAttempts,
    int TotalQuizPassed,
    int TotalPracticeSessionsCompleted,
    int TotalDailyPlansCompleted,
    int TotalGoalsCompleted,
    int EarnedAchievementCount,
    IReadOnlyCollection<EnglishMaster.Application.Features.Achievements.Dtos.StudentAchievementDto> RecentAchievements,
    IReadOnlyCollection<LearningActivityDto> RecentActivity);
