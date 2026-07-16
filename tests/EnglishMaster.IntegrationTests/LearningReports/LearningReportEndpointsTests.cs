using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EnglishMaster.Contracts.LearningReports;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.LearningGoals;
using EnglishMaster.Domain.LearningReports;
using EnglishMaster.Domain.Motivation;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.LearningReports;

public sealed class LearningReportEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task GenerateWeeklyLearningReport_ReturnsReport()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();

        var report = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");

        Assert.NotEqual(Guid.Empty, report.Id);
        Assert.Equal("Generated", report.Status);
        Assert.True(report.WeekEndDate >= report.WeekStartDate);
    }

    [Fact]
    public async Task GenerateWeeklyLearningReport_UsesOneReportPerStudentWeek()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();

        var first = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");
        var second = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(first.WeekStartDate, second.WeekStartDate);
    }

    [Fact]
    public async Task CurrentWeekLearningReport_ReturnsCurrentStudentData()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();
        var profileId = await GetSuperAdminProfileIdAsync();
        var now = DateTimeOffset.UtcNow;
        await SeedAsync(dbContext =>
        {
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profileId, "LessonCompleted", "lesson", Guid.NewGuid(), "Lesson", now, 30, null, now));
            return Task.CompletedTask;
        });

        await client.PostAsync("/api/v1/me/learning-reports/current-week/generate", null);
        var report = await client.GetFromJsonAsync<WeeklyLearningReportDto>("/api/v1/me/learning-reports/current-week");

        Assert.NotNull(report);
        Assert.Equal(30, report!.TotalStudyMinutes);
        Assert.Equal(1, report.LessonsCompleted);
    }

    [Fact]
    public async Task LearningReportHistory_ReturnsGeneratedReports()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();
        var generated = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");

        var history = await client.GetFromJsonAsync<IReadOnlyCollection<WeeklyLearningReportDto>>("/api/v1/me/learning-reports?pageNumber=1&pageSize=10");

        Assert.Contains(history!, report => report.Id == generated.Id);
    }

    [Fact]
    public async Task LearningReportByDate_ReturnsMatchingWeek()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();
        var generated = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");

        var byDate = await client.GetFromJsonAsync<WeeklyLearningReportDto>($"/api/v1/me/learning-reports/by-date/{generated.WeekStartDate:yyyy-MM-dd}");
        var invalid = await client.GetAsync("/api/v1/me/learning-reports/by-date/not-a-date");

        Assert.Equal(generated.Id, byDate!.Id);
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
    }

    [Fact]
    public async Task RegenerateWeeklyLearningReport_RefreshesExistingReport()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();
        var generated = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");
        var profileId = await GetSuperAdminProfileIdAsync();
        var now = DateTimeOffset.UtcNow;
        await SeedAsync(dbContext =>
        {
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profileId, "LessonCompleted", "lesson", Guid.NewGuid(), "Regenerated", now, 35, null, now));
            return Task.CompletedTask;
        });

        var regenerated = await PostReadAsync<WeeklyLearningReportDto>(client, $"/api/v1/me/learning-reports/{generated.Id}/regenerate");

        Assert.Equal(generated.Id, regenerated.Id);
        Assert.Equal(35, regenerated.TotalStudyMinutes);
    }

    [Fact]
    public async Task LearningReportInsightsEndpoint_ReturnsInsights()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();
        var generated = await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");

        var insights = await client.GetFromJsonAsync<IReadOnlyCollection<WeeklyLearningReportInsightDto>>($"/api/v1/me/learning-reports/{generated.Id}/insights");

        Assert.NotEmpty(insights!);
        Assert.Contains(insights!, insight => insight.InsightType == "Inactivity");
    }

    [Fact]
    public async Task LearningReports_DoNotExposeOtherStudentsReport()
    {
        var otherProfile = StudentProfile.Create(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
        var otherReport = WeeklyLearningReport.Create(otherProfile.Id, WeekStart(DateTimeOffset.UtcNow), WeekStart(DateTimeOffset.UtcNow).AddDays(6), DateTimeOffset.UtcNow);
        await SeedAsync(dbContext =>
        {
            dbContext.StudentProfiles.Add(otherProfile);
            dbContext.WeeklyLearningReports.Add(otherReport);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync(resetReportsOnlyForSuperAdmin: true);

        var response = await client.GetAsync($"/api/v1/me/learning-reports/{otherReport.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task WeeklyLearningReport_CalculatesStudyMinutes()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var profileId = GetSuperAdminProfileId(dbContext);
            var now = DateTimeOffset.UtcNow;
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profileId, "LessonStarted", "lesson", Guid.NewGuid(), "One", now, 20, null, now));
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profileId, "QuizAttempted", "quiz", Guid.NewGuid(), "Two", now.AddHours(1), 25, null, now));
            return Task.CompletedTask;
        });

        Assert.Equal(45, report.TotalStudyMinutes);
        Assert.Equal(1, report.ActiveStudyDays);
    }

    [Fact]
    public async Task WeeklyLearningReport_CalculatesQuizAverage()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var userId = GetSuperAdminUserId(dbContext);
            var now = DateTimeOffset.UtcNow;
            dbContext.QuizAttempts.Add(QuizAttempt.Create(userId, Guid.NewGuid(), 50, passed: false, now, now));
            dbContext.QuizAttempts.Add(QuizAttempt.Create(userId, Guid.NewGuid(), 90, passed: true, now.AddHours(1), now));
            return Task.CompletedTask;
        });

        Assert.Equal(2, report.QuizAttempts);
        Assert.Equal(1, report.QuizzesPassed);
        Assert.Equal(70, report.AverageQuizScore);
    }

    [Fact]
    public async Task WeeklyLearningReport_CountsPracticeSessions()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var profileId = GetSuperAdminProfileId(dbContext);
            var now = DateTimeOffset.UtcNow;
            var session = PracticeSession.Create(profileId, now);
            session.Complete(now);
            dbContext.PracticeSessions.Add(session);
            return Task.CompletedTask;
        });

        Assert.Equal(1, report.PracticeSessionsCompleted);
    }

    [Fact]
    public async Task WeeklyLearningReport_CountsEarnedAchievements()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var profileId = GetSuperAdminProfileId(dbContext);
            var now = DateTimeOffset.UtcNow;
            var definition = AchievementDefinition.Create(Unique("weekly-report-achievement"), "Weekly Report Achievement", null, "FirstLessonCompleted", 1, "award", 1, now);
            var achievement = StudentAchievement.Create(profileId, definition.Id, now);
            achievement.ApplyProgress(1, 1, now);
            dbContext.AchievementDefinitions.Add(definition);
            dbContext.StudentAchievements.Add(achievement);
            return Task.CompletedTask;
        });

        Assert.Equal(1, report.AchievementsEarned);
    }

    [Fact]
    public async Task WeeklyLearningReport_AddsLowStudyTimeInsight()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var profileId = GetSuperAdminProfileId(dbContext);
            var now = DateTimeOffset.UtcNow;
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(profileId, "LessonCompleted", "lesson", Guid.NewGuid(), "Short lesson", now, 10, null, now));
            return Task.CompletedTask;
        });

        Assert.Contains(report.Insights, insight => insight.InsightType == "StudyTime" && insight.Severity == "Warning");
    }

    [Fact]
    public async Task WeeklyLearningReport_AddsLowQuizScoreInsight()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var userId = GetSuperAdminUserId(dbContext);
            var now = DateTimeOffset.UtcNow;
            dbContext.QuizAttempts.Add(QuizAttempt.Create(userId, Guid.NewGuid(), 45, passed: false, now, now));
            return Task.CompletedTask;
        });

        Assert.Contains(report.Insights, insight => insight.InsightType == "Quiz" && insight.Severity == "Warning");
    }

    [Fact]
    public async Task WeeklyLearningReport_AddsPracticeDueInsight()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var profileId = GetSuperAdminProfileId(dbContext);
            var now = DateTimeOffset.UtcNow;
            dbContext.PracticeItems.Add(PracticeItem.Create(profileId, "word", Guid.NewGuid(), "WordFlashcard", now.AddDays(-1), now));
            return Task.CompletedTask;
        });

        Assert.Contains(report.Insights, insight => insight.InsightType == "Practice" && insight.Severity == "Warning");
    }

    [Fact]
    public async Task WeeklyLearningReport_AddsStreakPositiveInsight()
    {
        var report = await GenerateWithSeedAsync(dbContext =>
        {
            var profileId = GetSuperAdminProfileId(dbContext);
            var now = DateTimeOffset.UtcNow;
            var streak = StudentStreak.Create(profileId, now);
            streak.ApplyActivity(now, now);
            dbContext.StudentStreaks.Add(streak);
            return Task.CompletedTask;
        });

        Assert.Contains(report.Insights, insight => insight.InsightType == "Streak" && insight.Severity == "Positive");
    }

    [Fact]
    public async Task WeeklyLearningReport_AddsNoActivityInsight()
    {
        var report = await GenerateWithSeedAsync(_ => Task.CompletedTask);

        Assert.Contains(report.Insights, insight => insight.InsightType == "Inactivity" && insight.Severity == "Warning");
    }

    [Fact]
    public async Task WeeklyLearningReport_DoesNotExposeQuizAnswers()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();

        var response = await client.PostAsync("/api/v1/me/learning-reports/current-week/generate", null);
        var json = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("CorrectAnswer", json, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("AnswerText", json, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(JsonSerializer.Deserialize<WeeklyLearningReportDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }));
    }

    private async Task<WeeklyLearningReportDto> GenerateWithSeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetReportsAsync();
        await SeedAsync(seed);
        return await PostReadAsync<WeeklyLearningReportDto>(client, "/api/v1/me/learning-reports/current-week/generate");
    }

    private async Task ResetReportsAsync(bool resetReportsOnlyForSuperAdmin = false)
    {
        var userId = await GetSuperAdminUserIdAsync();
        await SeedAsync(dbContext =>
        {
            var profiles = dbContext.StudentProfiles.Where(profile => profile.UserId == userId).Select(profile => profile.Id).ToArray();
            if (!resetReportsOnlyForSuperAdmin)
            {
                dbContext.LearningActivityLogs.RemoveRange(dbContext.LearningActivityLogs.Where(activity => profiles.Contains(activity.StudentProfileId)));
                dbContext.StudentStreaks.RemoveRange(dbContext.StudentStreaks.Where(streak => profiles.Contains(streak.StudentProfileId)));
                dbContext.StudentAchievements.RemoveRange(dbContext.StudentAchievements.Where(achievement => profiles.Contains(achievement.StudentProfileId)));
                dbContext.LearningGoals.RemoveRange(dbContext.LearningGoals.Where(goal => profiles.Contains(goal.StudentProfileId)));
                dbContext.DailyStudyPlans.RemoveRange(dbContext.DailyStudyPlans.Where(plan => profiles.Contains(plan.StudentProfileId)));
                dbContext.PracticeSessions.RemoveRange(dbContext.PracticeSessions.Where(session => profiles.Contains(session.StudentProfileId)));
                dbContext.PracticeItems.RemoveRange(dbContext.PracticeItems.Where(item => profiles.Contains(item.StudentProfileId)));
                dbContext.QuizAttempts.RemoveRange(dbContext.QuizAttempts.Where(attempt => attempt.UserId == userId));
            }

            dbContext.WeeklyLearningReports.RemoveRange(dbContext.WeeklyLearningReports.Where(report => profiles.Contains(report.StudentProfileId)));
            return Task.CompletedTask;
        });
    }

    private Task<Guid> GetSuperAdminUserIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        return Task.FromResult(GetSuperAdminUserId(dbContext));
    }

    private Task<Guid> GetSuperAdminProfileIdAsync()
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        return Task.FromResult(GetSuperAdminProfileId(dbContext));
    }

    private static Guid GetSuperAdminUserId(EnglishMasterDbContext dbContext) =>
        dbContext.AppUsers.Where(user => user.Email == "superadmin@englishmaster.test").Select(user => user.Id).Single();

    private static Guid GetSuperAdminProfileId(EnglishMasterDbContext dbContext)
    {
        var userId = GetSuperAdminUserId(dbContext);
        var profile = dbContext.StudentProfiles.SingleOrDefault(profile => profile.UserId == userId);
        if (profile is not null)
        {
            return profile.Id;
        }

        profile = StudentProfile.Create(userId, null, DateTimeOffset.UtcNow);
        dbContext.StudentProfiles.Add(profile);
        dbContext.SaveChanges();
        return profile.Id;
    }

    private async Task SeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static async Task<T> PostReadAsync<T>(HttpClient client, string path)
    {
        var response = await client.PostAsync(path, null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static DateTimeOffset WeekStart(DateTimeOffset date)
    {
        var utc = date.UtcDateTime.Date;
        var diff = ((int)utc.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return new DateTimeOffset(utc.AddDays(-diff), TimeSpan.Zero);
    }

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
