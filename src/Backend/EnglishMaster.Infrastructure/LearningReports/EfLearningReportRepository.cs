using EnglishMaster.Application.Features.LearningReports;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.LearningGoals;
using EnglishMaster.Domain.LearningReports;
using EnglishMaster.Domain.Motivation;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using ReportDto = EnglishMaster.Application.Features.LearningReports.Dtos.WeeklyLearningReportDto;
using InsightDto = EnglishMaster.Application.Features.LearningReports.Dtos.WeeklyLearningReportInsightDto;

namespace EnglishMaster.Infrastructure.LearningReports;

public sealed class EfLearningReportRepository : ILearningReportRepository
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly TimeProvider timeProvider;

    public EfLearningReportRepository(EnglishMasterDbContext dbContext, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.timeProvider = timeProvider;
    }

    public async Task<ReportDto> GenerateWeeklyReportAsync(Guid userId, DateTimeOffset weekStartDate, bool regenerate, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var start = WeekStart(weekStartDate);
        var end = start.AddDays(6);
        var exclusiveEnd = end.AddDays(1);
        var report = await dbContext.WeeklyLearningReports.Include(report => report.Insights).SingleOrDefaultAsync(report => report.StudentProfileId == profile.Id && report.WeekStartDate == start, cancellationToken);
        if (report is not null && !regenerate)
        {
            return ToDto(report);
        }

        var isNew = report is null;
        report ??= WeeklyLearningReport.Create(profile.Id, start, end, now);

        var activities = await dbContext.LearningActivityLogs.AsNoTracking()
            .Where(activity => activity.StudentProfileId == profile.Id && activity.OccurredAt >= start && activity.OccurredAt < exclusiveEnd)
            .ToArrayAsync(cancellationToken);
        var quizQuery = dbContext.QuizAttempts.AsNoTracking().Where(attempt => attempt.UserId == userId && attempt.AttemptedAt >= start && attempt.AttemptedAt < exclusiveEnd);
        var planQuery = dbContext.DailyStudyPlans.AsNoTracking().Where(plan => plan.StudentProfileId == profile.Id && plan.PlanDate >= start && plan.PlanDate < exclusiveEnd);
        var practiceSessions = await dbContext.PracticeSessions.AsNoTracking().Where(session => session.StudentProfileId == profile.Id && session.CompletedAt >= start && session.CompletedAt < exclusiveEnd).ToArrayAsync(cancellationToken);
        var streak = await dbContext.StudentStreaks.AsNoTracking().FirstOrDefaultAsync(streak => streak.StudentProfileId == profile.Id, cancellationToken);
        var achievementsEarned = await dbContext.StudentAchievements.AsNoTracking().CountAsync(achievement => achievement.StudentProfileId == profile.Id && achievement.Status == StudentAchievementStatus.Earned && achievement.EarnedAt >= start && achievement.EarnedAt < exclusiveEnd, cancellationToken);
        var averageQuizScore = await quizQuery.AnyAsync(cancellationToken) ? await quizQuery.AverageAsync(attempt => attempt.Score, cancellationToken) : 0;
        var totalStudyMinutes = activities.Sum(activity => activity.MinutesSpent);
        var activeStudyDays = activities.Select(activity => DateOnly.FromDateTime(activity.OccurredAt.UtcDateTime)).Distinct().Count();
        var completedDailyPlans = await planQuery.CountAsync(plan => plan.Status == DailyStudyPlanStatus.Completed, cancellationToken);
        var duePractice = await dbContext.PracticeItems.AsNoTracking().CountAsync(item => item.StudentProfileId == profile.Id && item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now, cancellationToken);
        var goalsCompleted = await dbContext.LearningGoals.AsNoTracking().CountAsync(goal => goal.StudentProfileId == profile.Id && goal.Status == LearningGoalStatus.Completed && goal.UpdatedAt >= start && goal.UpdatedAt < exclusiveEnd, cancellationToken);

        var lessonsStarted = activities.Count(activity => activity.ActivityType == "LessonStarted");
        var lessonsCompleted = activities.Count(activity => activity.ActivityType == "LessonCompleted");
        var coursesStarted = activities.Count(activity => activity.ActivityType == "CourseStarted");
        var coursesCompleted = activities.Count(activity => activity.ActivityType == "CourseCompleted");
        var booksStarted = activities.Count(activity => activity.ActivityType == "BookStarted");
        var booksCompleted = activities.Count(activity => activity.ActivityType == "BookCompleted");
        var quizAttempts = await quizQuery.CountAsync(cancellationToken);
        var quizzesPassed = await quizQuery.CountAsync(attempt => attempt.Passed, cancellationToken);

        var summary = totalStudyMinutes == 0 ? "Start with today's study plan." : "Weekly report generated from your learning activity.";
        report.ApplyMetrics(
            totalStudyMinutes,
            activeStudyDays,
            completedDailyPlans,
            lessonsStarted,
            lessonsCompleted,
            coursesStarted,
            coursesCompleted,
            booksStarted,
            booksCompleted,
            practiceSessions.Length,
            practiceSessions.Sum(session => session.CompletedItems),
            quizAttempts,
            quizzesPassed,
            averageQuizScore,
            goalsCompleted,
            achievementsEarned,
            streak?.CurrentStreakDays ?? 0,
            streak?.LongestStreakDays ?? 0,
            summary,
            now);

        var insights = BuildInsights(report, duePractice, now).ToArray();
        if (isNew)
        {
            report.ReplaceInsights(insights, now);
            dbContext.WeeklyLearningReports.Add(report);
        }
        else
        {
            dbContext.WeeklyLearningReportInsights.RemoveRange(report.Insights);
            report.ReplaceInsights(insights, now);
            dbContext.WeeklyLearningReportInsights.AddRange(insights);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(report);
    }

    public async Task<ReportDto?> GetCurrentWeekAsync(Guid userId, CancellationToken cancellationToken) =>
        await GetByDateAsync(userId, timeProvider.GetUtcNow(), cancellationToken);

    public async Task<ReportDto?> GetByIdAsync(Guid userId, Guid reportId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var report = await dbContext.WeeklyLearningReports.AsNoTracking().Include(report => report.Insights).SingleOrDefaultAsync(report => report.Id == reportId && report.StudentProfileId == profile.Id, cancellationToken);
        return report is null ? null : ToDto(report);
    }

    public async Task<ReportDto?> GetByDateAsync(Guid userId, DateTimeOffset date, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var start = WeekStart(date);
        var report = await dbContext.WeeklyLearningReports.AsNoTracking().Include(report => report.Insights).SingleOrDefaultAsync(report => report.StudentProfileId == profile.Id && report.WeekStartDate == start, cancellationToken);
        return report is null ? null : ToDto(report);
    }

    public async Task<IReadOnlyCollection<ReportDto>> GetHistoryAsync(Guid userId, int pageNumber, int pageSize, DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var query = dbContext.WeeklyLearningReports.AsNoTracking().Include(report => report.Insights).Where(report => report.StudentProfileId == profile.Id);
        if (fromDate.HasValue)
        {
            query = query.Where(report => report.WeekStartDate >= WeekStart(fromDate.Value));
        }

        if (toDate.HasValue)
        {
            query = query.Where(report => report.WeekStartDate <= WeekStart(toDate.Value));
        }

        var reports = await query.OrderByDescending(report => report.WeekStartDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToArrayAsync(cancellationToken);
        return reports.Select(ToDto).ToArray();
    }

    public async Task<IReadOnlyCollection<InsightDto>> GetInsightsAsync(Guid userId, Guid reportId, CancellationToken cancellationToken)
    {
        var report = await GetByIdAsync(userId, reportId, cancellationToken);
        return report?.Insights ?? [];
    }

    public async Task<ReportDto?> ArchiveAsync(Guid userId, Guid reportId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var report = await dbContext.WeeklyLearningReports.Include(report => report.Insights).SingleOrDefaultAsync(report => report.Id == reportId && report.StudentProfileId == profile.Id, cancellationToken);
        if (report is null)
        {
            return null;
        }

        report.Archive(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(report);
    }

    private static IEnumerable<WeeklyLearningReportInsight> BuildInsights(WeeklyLearningReport report, int duePractice, DateTimeOffset now)
    {
        var sort = 1;
        if (report.ActiveStudyDays == 0)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "Inactivity", "Warning", "Start with today's study plan.", "Open today's study plan and complete one item.", sort++, now);
        }

        if (report.TotalStudyMinutes < 60)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "StudyTime", "Warning", "Study time was lower than your weekly goal.", "Schedule a short study block this week.", sort++, now);
        }

        if (duePractice > 0)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "Practice", "Warning", "You have practice items due.", "Start a practice session.", sort++, now);
        }

        if (report.QuizAttempts > 0 && report.AverageQuizScore < 70)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "Quiz", "Warning", "Review quizzes with low scores.", "Review weak quizzes before trying a new one.", sort++, now);
        }

        if (report.CurrentStreakDays > 0)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "Streak", "Positive", "Great job keeping your streak.", "Keep one small activity going tomorrow.", sort++, now);
        }

        if (report.AchievementsEarned > 0)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "Achievement", "Positive", "You earned achievements this week.", "Review your achievements page.", sort++, now);
        }

        if (report.GoalsCompleted > 0)
        {
            yield return WeeklyLearningReportInsight.Create(report.Id, "Goal", "Positive", "You completed learning goals this week.", "Set a new goal for next week.", sort++, now);
        }
    }

    private async Task<StudentProfile> GetOrCreateProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = StudentProfile.Create(userId, null, timeProvider.GetUtcNow());
        dbContext.StudentProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<StudentProfile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.StudentProfiles.OrderBy(profile => profile.CreatedAt).FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

    private static DateTimeOffset WeekStart(DateTimeOffset value)
    {
        var date = value.UtcDateTime.Date;
        var diff = ((int)date.DayOfWeek + 6) % 7;
        return new DateTimeOffset(date.AddDays(-diff), TimeSpan.Zero);
    }

    private static ReportDto ToDto(WeeklyLearningReport report) =>
        new(report.Id, report.WeekStartDate, report.WeekEndDate, report.Status.ToString(), report.GeneratedAt, report.TotalStudyMinutes, report.ActiveStudyDays, report.CompletedDailyPlans, report.LessonsStarted, report.LessonsCompleted, report.CoursesStarted, report.CoursesCompleted, report.BooksStarted, report.BooksCompleted, report.PracticeSessionsCompleted, report.PracticeItemsCompleted, report.QuizAttempts, report.QuizzesPassed, report.AverageQuizScore, report.GoalsCompleted, report.AchievementsEarned, report.CurrentStreakDays, report.LongestStreakDays, report.SummaryText, report.Insights.OrderBy(insight => insight.SortOrder).Select(ToDto).ToArray());

    private static InsightDto ToDto(WeeklyLearningReportInsight insight) =>
        new(insight.Id, insight.WeeklyLearningReportId, insight.InsightType, insight.Severity, insight.Message, insight.Recommendation, insight.SortOrder);
}
