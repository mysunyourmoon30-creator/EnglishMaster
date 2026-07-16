using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Achievements;
using EnglishMaster.Contracts.Motivation;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Motivation;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Motivation;

public sealed class MotivationEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task RecordLearningActivity_AddsActivity()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();

        var activity = await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Intro lesson", DateTimeOffset.UtcNow));
        var activities = await client.GetFromJsonAsync<IReadOnlyCollection<LearningActivityDto>>("/api/v1/me/motivation/activity");

        Assert.Equal("LessonCompleted", activity.ActivityType);
        Assert.Contains(activities!, item => item.Id == activity.Id);
    }

    [Fact]
    public async Task SameDayActivity_DoesNotIncreaseStreakTwice()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        var today = DateTimeOffset.UtcNow;

        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "One", today));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("QuizAttempted", "Two", today.AddHours(1)));
        var streak = await client.GetFromJsonAsync<StudentStreakDto>("/api/v1/me/streak");

        Assert.Equal(1, streak!.CurrentStreakDays);
    }

    [Fact]
    public async Task ConsecutiveDayActivity_IncreasesStreak()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        var today = DateTimeOffset.UtcNow;

        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Yesterday", today.AddDays(-1)));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Today", today));
        var streak = await client.GetFromJsonAsync<StudentStreakDto>("/api/v1/me/streak");

        Assert.Equal(2, streak!.CurrentStreakDays);
    }

    [Fact]
    public async Task MissedDayActivity_ResetsCurrentStreak()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        var today = DateTimeOffset.UtcNow;

        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Old", today.AddDays(-3)));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Today", today));
        var streak = await client.GetFromJsonAsync<StudentStreakDto>("/api/v1/me/streak");

        Assert.Equal(1, streak!.CurrentStreakDays);
        Assert.Equal(1, streak.LongestStreakDays);
    }

    [Fact]
    public async Task OlderActivity_DoesNotReduceExistingStreak()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        var today = DateTimeOffset.UtcNow;

        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Yesterday", today.AddDays(-1)));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Today", today));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Older", today.AddDays(-3)));
        var streak = await client.GetFromJsonAsync<StudentStreakDto>("/api/v1/me/streak");

        Assert.Equal(2, streak!.CurrentStreakDays);
    }

    [Fact]
    public async Task RecordLearningActivity_RemovesSensitiveMetadata()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        var request = new RecordLearningActivityRequest("LessonCompleted", "lesson", Guid.NewGuid(), "Sensitive", DateTimeOffset.UtcNow, 5, "{\"authorization\":\"Bearer abc\"}");

        var activity = await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", request);

        Assert.Equal(string.Empty, activity.MetadataJson);
    }

    [Fact]
    public async Task AchievementDefinition_CodeIsUnique()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var request = new AchievementDefinitionRequest(Unique("unique-achievement"), "Unique Achievement", "description", "FirstLessonCompleted", 1, "award", 50);

        var first = await client.PostAsJsonAsync("/api/v1/admin/achievement-definitions", request);
        var second = await client.PostAsJsonAsync("/api/v1/admin/achievement-definitions", request);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task FirstLessonCompletedAchievement_IsEarned()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        await client.PostAsync("/api/v1/admin/achievement-definitions/seed-defaults", null);
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Lesson", DateTimeOffset.UtcNow));

        var achievements = await PostReadAsync<IReadOnlyCollection<StudentAchievementDto>>(client, "/api/v1/me/achievements/evaluate");

        Assert.Contains(achievements, item => item.AchievementType == "FirstLessonCompleted" && item.Status == "Earned");
    }

    [Fact]
    public async Task ThreeDayStreakAchievement_IsEarned()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();
        await client.PostAsync("/api/v1/admin/achievement-definitions/seed-defaults", null);
        var today = DateTimeOffset.UtcNow;
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Day 1", today.AddDays(-2)));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Day 2", today.AddDays(-1)));
        await PostReadAsync<LearningActivityDto>(client, "/api/v1/me/motivation/activity", Activity("LessonCompleted", "Day 3", today));

        var achievements = await PostReadAsync<IReadOnlyCollection<StudentAchievementDto>>(client, "/api/v1/me/achievements/evaluate");

        Assert.Contains(achievements, item => item.AchievementType == "ThreeDayStreak" && item.Status == "Earned");
    }

    [Fact]
    public async Task MotivationEndpoints_DoNotExposeOtherStudentsActivity()
    {
        var otherProfile = StudentProfile.Create(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
        var contentId = Guid.NewGuid();
        await SeedAsync(dbContext =>
        {
            dbContext.StudentProfiles.Add(otherProfile);
            dbContext.LearningActivityLogs.Add(LearningActivityLog.Create(otherProfile.Id, "LessonCompleted", "lesson", contentId, "Other activity", DateTimeOffset.UtcNow, 5, null, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await ResetMotivationAsync();

        var activities = await client.GetFromJsonAsync<IReadOnlyCollection<LearningActivityDto>>("/api/v1/me/motivation/activity");

        Assert.DoesNotContain(activities!, item => item.ContentId == contentId);
    }

    private async Task ResetMotivationAsync()
    {
        var userId = await GetSuperAdminUserIdAsync();
        await SeedAsync(dbContext =>
        {
            var profiles = dbContext.StudentProfiles.Where(profile => profile.UserId == userId).Select(profile => profile.Id).ToArray();
            dbContext.LearningActivityLogs.RemoveRange(dbContext.LearningActivityLogs.Where(activity => profiles.Contains(activity.StudentProfileId)));
            dbContext.StudentStreaks.RemoveRange(dbContext.StudentStreaks.Where(streak => profiles.Contains(streak.StudentProfileId)));
            dbContext.StudentAchievements.RemoveRange(dbContext.StudentAchievements.Where(achievement => profiles.Contains(achievement.StudentProfileId)));
            return Task.CompletedTask;
        });
    }

    private static RecordLearningActivityRequest Activity(string type, string title, DateTimeOffset occurredAt) =>
        new(type, "lesson", Guid.NewGuid(), title, occurredAt, 5, "{\"safe\":true}");

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

    private static async Task<T> PostReadAsync<T>(HttpClient client, string path, object request)
    {
        var response = await client.PostAsJsonAsync(path, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task<T> PostReadAsync<T>(HttpClient client, string path)
    {
        var response = await client.PostAsync(path, null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
