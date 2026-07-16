using System.Net.Http.Json;
using EnglishMaster.Contracts.Analytics;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Certificates;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Motivation;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Analytics;

public sealed class AnalyticsEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task AdminAnalyticsOverview_ReturnsAggregatesForDateRange()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var at = DateTimeOffset.UtcNow.AddDays(30);
        await SeedAsync(dbContext =>
        {
            var profile = GetOrCreateProfile(dbContext, userId, at);
            var course = Course.Create(Unique("Analytics Course"), "summary", "description", CefrLevel.B1, null, null, 20, 1, at);
            var quiz = Quiz.Create(Unique("Analytics Quiz"), "summary", "description", CefrLevel.B1, null, null, null, null, 10, 80, 1, at);
            var template = CertificateTemplate.Create(Unique("analytics-template").ToLowerInvariant(), "Analytics Template", string.Empty, "Body", at);
            var session = PracticeSession.Create(profile.Id, at);
            session.Complete(at);
            dbContext.Courses.Add(course);
            dbContext.Quizzes.Add(quiz);
            dbContext.CertificateTemplates.Add(template);
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profile.Id, "Study", "course", course.Id, course.Title, at, 42, null, at));
            dbContext.CourseProgress.Add(CourseProgress.Create(userId, course.Id, 100, LearningProgressStatus.Completed, at, at));
            dbContext.QuizAttempts.Add(QuizAttempt.Create(userId, quiz.Id, 88, passed: true, at, at));
            dbContext.PracticeSessions.Add(session);
            dbContext.IssuedCertificates.Add(IssuedCertificate.Create(userId, course.Id, template.Id, Unique("cert").ToLowerInvariant(), "superadmin", course.Title, template.Code, "Body", at));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var overview = await client.GetFromJsonAsync<AdminAnalyticsOverviewDto>($"/api/v1/admin/analytics/overview?fromDate={Escape(at.AddMinutes(-1))}&toDate={Escape(at.AddMinutes(1))}");

        Assert.NotNull(overview);
        Assert.True(overview!.ActiveLearners >= 1);
        Assert.True(overview.StudyMinutes >= 42);
        Assert.True(overview.CourseCompletions >= 1);
        Assert.True(overview.QuizAttempts >= 1);
        Assert.True(overview.PracticeSessionsCompleted >= 1);
        Assert.True(overview.CertificatesIssued >= 1);
    }

    [Fact]
    public async Task StudentAnalyticsOverview_IsScopedToCurrentUser()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var otherUserId = Guid.NewGuid();
        var at = DateTimeOffset.UtcNow.AddDays(40);
        await SeedAsync(dbContext =>
        {
            var profile = GetOrCreateProfile(dbContext, userId, at);
            var otherProfile = StudentProfile.Create(otherUserId, null, at);
            var ownCourse = Course.Create(Unique("Own Analytics Course"), "summary", "description", CefrLevel.B1, null, null, 20, 1, at);
            var otherCourse = Course.Create(Unique("Other Analytics Course"), "summary", "description", CefrLevel.B1, null, null, 20, 2, at);
            var quiz = Quiz.Create(Unique("Own Analytics Quiz"), "summary", "description", CefrLevel.B1, null, null, null, null, 10, 80, 1, at);
            var template = CertificateTemplate.Create(Unique("student-analytics-template").ToLowerInvariant(), "Analytics Template", string.Empty, "Body", at);
            var session = PracticeSession.Create(profile.Id, at);
            session.Complete(at);
            var streak = StudentStreak.Create(profile.Id, at);
            streak.ApplyActivity(at.AddDays(-1), at.AddDays(-1));
            streak.ApplyActivity(at, at);
            dbContext.StudentProfiles.Add(otherProfile);
            dbContext.Courses.AddRange(ownCourse, otherCourse);
            dbContext.Quizzes.Add(quiz);
            dbContext.CertificateTemplates.Add(template);
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profile.Id, "Study", "course", ownCourse.Id, ownCourse.Title, at, 30, null, at));
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(otherProfile.Id, "Study", "course", otherCourse.Id, otherCourse.Title, at, 90, null, at));
            dbContext.CourseProgress.Add(CourseProgress.Create(userId, ownCourse.Id, 100, LearningProgressStatus.Completed, at, at));
            dbContext.CourseProgress.Add(CourseProgress.Create(otherUserId, otherCourse.Id, 100, LearningProgressStatus.Completed, at, at));
            dbContext.QuizAttempts.Add(QuizAttempt.Create(userId, quiz.Id, 70, passed: true, at, at));
            dbContext.PracticeSessions.Add(session);
            dbContext.StudentStreaks.Add(streak);
            dbContext.IssuedCertificates.Add(IssuedCertificate.Create(userId, ownCourse.Id, template.Id, Unique("cert").ToLowerInvariant(), "superadmin", ownCourse.Title, template.Code, "Body", at));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var overview = await client.GetFromJsonAsync<StudentAnalyticsOverviewDto>($"/api/v1/me/analytics/overview?fromDate={Escape(at.AddMinutes(-1))}&toDate={Escape(at.AddMinutes(1))}");

        Assert.NotNull(overview);
        Assert.Equal(30, overview!.StudyMinutes);
        Assert.Equal(1, overview.LearningActivities);
        Assert.Equal(1, overview.CoursesCompleted);
        Assert.Equal(1, overview.QuizAttempts);
        Assert.Equal(1, overview.PracticeSessionsCompleted);
        Assert.Equal(1, overview.CertificatesEarned);
        Assert.Equal(2, overview.CurrentStreakDays);
    }

    private async Task<Guid> GetSuperAdminUserIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        return await dbContext.AppUsers.Where(user => user.Email == "superadmin@englishmaster.test").Select(user => user.Id).SingleAsync();
    }

    private async Task SeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static StudentProfile GetOrCreateProfile(EnglishMasterDbContext dbContext, Guid userId, DateTimeOffset now)
    {
        var profile = dbContext.StudentProfiles.OrderBy(item => item.CreatedAt).FirstOrDefault(item => item.UserId == userId);
        if (profile is not null)
        {
            return profile;
        }

        profile = StudentProfile.Create(userId, null, now);
        dbContext.StudentProfiles.Add(profile);
        return profile;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Escape(DateTimeOffset value) =>
        Uri.EscapeDataString(value.ToString("O"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
