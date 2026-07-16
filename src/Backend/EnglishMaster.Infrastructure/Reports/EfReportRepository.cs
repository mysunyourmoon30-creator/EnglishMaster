using EnglishMaster.Application.Features.Reports;
using EnglishMaster.Application.Features.Reports.Dtos;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Reports;

public sealed class EfReportRepository : IReportRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfReportRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<AdminDashboardSummaryDto> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken)
    {
        var overview = await GetOverviewMetricsAsync(cancellationToken);
        var content = await GetContentStatusSummaryAsync(cancellationToken);
        var quiz = await GetQuizAnalyticsSummaryAsync(cancellationToken);
        var activity = await GetRecentActivitySummaryAsync(10, cancellationToken);

        return new AdminDashboardSummaryDto(overview, content, quiz, activity.Items);
    }

    public async Task<ContentStatusSummaryDto> GetContentStatusSummaryAsync(CancellationToken cancellationToken)
    {
        var publishedLessons = await dbContext.Lessons.AsNoTracking()
            .CountAsync(lesson => lesson.IsPublished, cancellationToken);
        var draftLessons = await dbContext.Lessons.AsNoTracking()
            .CountAsync(lesson => !lesson.IsPublished, cancellationToken);
        var publishedCourses = await dbContext.Courses.AsNoTracking()
            .CountAsync(course => course.IsPublished, cancellationToken);
        var publishedBooks = await dbContext.Books.AsNoTracking()
            .CountAsync(book => book.IsPublished, cancellationToken);
        var publishedQuizzes = await dbContext.Quizzes.AsNoTracking()
            .CountAsync(quiz => quiz.IsPublished, cancellationToken);

        return new ContentStatusSummaryDto(
            publishedLessons,
            draftLessons,
            InReviewContent: 0,
            publishedCourses,
            publishedBooks,
            publishedQuizzes);
    }

    public Task<LearningProgressSummaryDto> GetLearningProgressSummaryAsync(CancellationToken cancellationToken)
    {
        var emptyLessons = Array.Empty<LessonActivityDto>();
        return Task.FromResult(new LearningProgressSummaryDto(
            emptyLessons,
            emptyLessons,
            emptyLessons,
            new ProgressMetricDto(0, 0, 0),
            new ProgressMetricDto(0, 0, 0)));
    }

    public Task<QuizAnalyticsSummaryDto> GetQuizAnalyticsSummaryAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new QuizAnalyticsSummaryDto(
            TotalAttempts: 0,
            AverageScore: 0,
            PassedCount: 0,
            FailedCount: 0,
            AverageScoreByQuiz: [],
            RecentAttempts: [],
            TopQuizzesByAttempts: []));
    }

    public async Task<RecentActivitySummaryDto> GetRecentActivitySummaryAsync(int pageSize, CancellationToken cancellationToken)
    {
        var size = Math.Clamp(pageSize, 1, 100);
        var lessons = await dbContext.Lessons.AsNoTracking()
            .OrderByDescending(lesson => lesson.UpdatedAt)
            .Take(size)
            .Select(lesson => new RecentActivityDto("Lesson", lesson.Title, lesson.UpdatedAt))
            .ToListAsync(cancellationToken);
        var courses = await dbContext.Courses.AsNoTracking()
            .OrderByDescending(course => course.UpdatedAt)
            .Take(size)
            .Select(course => new RecentActivityDto("Course", course.Title, course.UpdatedAt))
            .ToListAsync(cancellationToken);
        var books = await dbContext.Books.AsNoTracking()
            .OrderByDescending(book => book.UpdatedAt)
            .Take(size)
            .Select(book => new RecentActivityDto("Book", book.Title, book.UpdatedAt))
            .ToListAsync(cancellationToken);
        var quizzes = await dbContext.Quizzes.AsNoTracking()
            .OrderByDescending(quiz => quiz.UpdatedAt)
            .Take(size)
            .Select(quiz => new RecentActivityDto("Quiz", quiz.Title, quiz.UpdatedAt))
            .ToListAsync(cancellationToken);

        var items = lessons
            .Concat(courses)
            .Concat(books)
            .Concat(quizzes)
            .OrderByDescending(item => item.OccurredAt)
            .Take(size)
            .ToArray();

        return new RecentActivitySummaryDto(items);
    }

    private async Task<OverviewMetricsDto> GetOverviewMetricsAsync(CancellationToken cancellationToken)
    {
        var totalStudents = await dbContext.AppUserRoles.AsNoTracking()
            .CountAsync(userRole => userRole.Role.Name == SecurityRoles.Viewer, cancellationToken);
        var totalActiveStudents = await dbContext.AppUserRoles.AsNoTracking()
            .CountAsync(userRole => userRole.Role.Name == SecurityRoles.Viewer && userRole.User.IsActive, cancellationToken);
        var totalWords = await dbContext.Words.AsNoTracking().CountAsync(cancellationToken);
        var totalLessons = await dbContext.Lessons.AsNoTracking().CountAsync(cancellationToken);
        var totalCourses = await dbContext.Courses.AsNoTracking().CountAsync(cancellationToken);
        var totalBooks = await dbContext.Books.AsNoTracking().CountAsync(cancellationToken);
        var totalQuizzes = await dbContext.Quizzes.AsNoTracking().CountAsync(cancellationToken);

        return new OverviewMetricsDto(
            totalStudents,
            totalActiveStudents,
            totalWords,
            totalLessons,
            totalCourses,
            totalBooks,
            totalQuizzes,
            TotalQuizAttempts: 0,
            AverageQuizScore: 0);
    }
}
