using EnglishMaster.Application.Features.Analytics;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Analytics;

public sealed class EfAnalyticsRepository : IAnalyticsRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfAnalyticsRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<AdminAnalyticsOverviewDto> GetAdminOverviewAsync(AnalyticsDateRange range, CancellationToken cancellationToken)
    {
        var activityQuery = dbContext.LearningActivityLogs.AsNoTracking()
            .Where(activity => activity.OccurredAt >= range.FromDate && activity.OccurredAt <= range.ToDate);
        var quizQuery = dbContext.QuizAttempts.AsNoTracking()
            .Where(attempt => attempt.AttemptedAt >= range.FromDate && attempt.AttemptedAt <= range.ToDate);

        var activeLearners = await activityQuery.Select(activity => activity.StudentProfileId).Distinct().CountAsync(cancellationToken);
        var studyMinutes = await activityQuery.SumAsync(activity => (int?)activity.MinutesSpent, cancellationToken) ?? 0;
        var learningActivities = await activityQuery.CountAsync(cancellationToken);
        var courseCompletions = await dbContext.CourseProgress.AsNoTracking()
            .CountAsync(progress => progress.Status == LearningProgressStatus.Completed && progress.UpdatedAt >= range.FromDate && progress.UpdatedAt <= range.ToDate, cancellationToken);
        var quizAttempts = await quizQuery.CountAsync(cancellationToken);
        var averageQuizScore = await quizQuery.AverageAsync(attempt => (decimal?)attempt.Score, cancellationToken) ?? 0;
        var practiceSessionsCompleted = await dbContext.PracticeSessions.AsNoTracking()
            .CountAsync(session => session.Status == PracticeSessionStatus.Completed && session.CompletedAt >= range.FromDate && session.CompletedAt <= range.ToDate, cancellationToken);
        var certificatesIssued = await dbContext.IssuedCertificates.AsNoTracking()
            .CountAsync(certificate => certificate.IssuedAt >= range.FromDate && certificate.IssuedAt <= range.ToDate, cancellationToken);

        return new AdminAnalyticsOverviewDto(activeLearners, studyMinutes, learningActivities, courseCompletions, quizAttempts, averageQuizScore, practiceSessionsCompleted, certificatesIssued);
    }

    public async Task<StudentAnalyticsOverviewDto> GetStudentOverviewAsync(Guid userId, AnalyticsDateRange range, CancellationToken cancellationToken)
    {
        var profileId = await dbContext.StudentProfiles.AsNoTracking()
            .Where(profile => profile.UserId == userId)
            .OrderBy(profile => profile.CreatedAt)
            .Select(profile => (Guid?)profile.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!profileId.HasValue)
        {
            return new StudentAnalyticsOverviewDto(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

        var activityQuery = dbContext.LearningActivityLogs.AsNoTracking()
            .Where(activity => activity.StudentProfileId == profileId.Value && activity.OccurredAt >= range.FromDate && activity.OccurredAt <= range.ToDate);
        var quizQuery = dbContext.QuizAttempts.AsNoTracking()
            .Where(attempt => attempt.UserId == userId && attempt.AttemptedAt >= range.FromDate && attempt.AttemptedAt <= range.ToDate);

        var studyMinutes = await activityQuery.SumAsync(activity => (int?)activity.MinutesSpent, cancellationToken) ?? 0;
        var learningActivities = await activityQuery.CountAsync(cancellationToken);
        var lessonsCompleted = await dbContext.LessonProgress.AsNoTracking()
            .CountAsync(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed && progress.UpdatedAt >= range.FromDate && progress.UpdatedAt <= range.ToDate, cancellationToken);
        var coursesCompleted = await dbContext.CourseProgress.AsNoTracking()
            .CountAsync(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed && progress.UpdatedAt >= range.FromDate && progress.UpdatedAt <= range.ToDate, cancellationToken);
        var booksCompleted = await dbContext.BookProgress.AsNoTracking()
            .CountAsync(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed && progress.UpdatedAt >= range.FromDate && progress.UpdatedAt <= range.ToDate, cancellationToken);
        var quizAttempts = await quizQuery.CountAsync(cancellationToken);
        var averageQuizScore = await quizQuery.AverageAsync(attempt => (decimal?)attempt.Score, cancellationToken) ?? 0;
        var practiceSessionsCompleted = await dbContext.PracticeSessions.AsNoTracking()
            .CountAsync(session => session.StudentProfileId == profileId.Value && session.Status == PracticeSessionStatus.Completed && session.CompletedAt >= range.FromDate && session.CompletedAt <= range.ToDate, cancellationToken);
        var certificatesEarned = await dbContext.IssuedCertificates.AsNoTracking()
            .CountAsync(certificate => certificate.UserId == userId && certificate.IssuedAt >= range.FromDate && certificate.IssuedAt <= range.ToDate, cancellationToken);
        var streak = await dbContext.StudentStreaks.AsNoTracking()
            .Where(item => item.StudentProfileId == profileId.Value)
            .Select(item => new { item.CurrentStreakDays, item.LongestStreakDays })
            .SingleOrDefaultAsync(cancellationToken);

        return new StudentAnalyticsOverviewDto(
            studyMinutes,
            learningActivities,
            lessonsCompleted,
            coursesCompleted,
            booksCompleted,
            quizAttempts,
            averageQuizScore,
            practiceSessionsCompleted,
            certificatesEarned,
            streak?.CurrentStreakDays ?? 0,
            streak?.LongestStreakDays ?? 0);
    }
}
