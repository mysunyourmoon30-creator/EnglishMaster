namespace EnglishMaster.Contracts.Analytics;

public sealed record AdminAnalyticsOverviewDto(
    int ActiveLearners,
    int StudyMinutes,
    int LearningActivities,
    int CourseCompletions,
    int QuizAttempts,
    decimal AverageQuizScore,
    int PracticeSessionsCompleted,
    int CertificatesIssued);

public sealed record StudentAnalyticsOverviewDto(
    int StudyMinutes,
    int LearningActivities,
    int LessonsCompleted,
    int CoursesCompleted,
    int BooksCompleted,
    int QuizAttempts,
    decimal AverageQuizScore,
    int PracticeSessionsCompleted,
    int CertificatesEarned,
    int CurrentStreakDays,
    int LongestStreakDays);
