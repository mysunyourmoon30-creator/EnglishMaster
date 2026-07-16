using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.DailyStudyPlans;
using EnglishMaster.Contracts.LearningGoals;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.LearningGoals;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.LearningGoals;

public sealed class LearningGoalStudyPlanEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task CreateLearningGoal_ReturnsGoal()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var goal = await PostReadAsync<LearningGoalDto>(client, "/api/v1/me/learning-goals", new CreateLearningGoalRequest("DailyStudyMinutes", "Study daily", "Build habit", 30, "minutes", null));

        Assert.NotEqual(Guid.Empty, goal.Id);
        Assert.Equal("Active", goal.Status);
        Assert.Equal(30, goal.TargetValue);
    }

    [Fact]
    public async Task CreateLearningGoal_WithUnsupportedGoalType_ReturnsBadRequest()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.PostAsJsonAsync("/api/v1/me/learning-goals", new CreateLearningGoalRequest("MagicGoal", "Unsupported", null, 1, "items", null));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PauseAndResumeLearningGoal_UpdatesStatus()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var goal = await PostReadAsync<LearningGoalDto>(client, "/api/v1/me/learning-goals", new CreateLearningGoalRequest("WeeklyStudyMinutes", "Study weekly", null, 120, "minutes", null));

        var paused = await PostReadAsync<LearningGoalDto>(client, $"/api/v1/me/learning-goals/{goal.Id}/pause");
        var resumed = await PostReadAsync<LearningGoalDto>(client, $"/api/v1/me/learning-goals/{goal.Id}/resume");

        Assert.Equal("Paused", paused.Status);
        Assert.Equal("Active", resumed.Status);
    }

    [Fact]
    public async Task CompleteLearningGoal_UpdatesStatusAndCurrentValue()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var goal = await PostReadAsync<LearningGoalDto>(client, "/api/v1/me/learning-goals", new CreateLearningGoalRequest("DailyPracticeItems", "Practice", null, 5, "items", null));

        var completed = await PostReadAsync<LearningGoalDto>(client, $"/api/v1/me/learning-goals/{goal.Id}/complete");

        Assert.Equal("Completed", completed.Status);
        Assert.Equal(completed.TargetValue, completed.CurrentValue);
    }

    [Fact]
    public async Task CompletedLearningGoal_CannotBePaused()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var goal = await PostReadAsync<LearningGoalDto>(client, "/api/v1/me/learning-goals", new CreateLearningGoalRequest("DailyPracticeItems", "Practice complete", null, 5, "items", null));
        await PostReadAsync<LearningGoalDto>(client, $"/api/v1/me/learning-goals/{goal.Id}/complete");

        var response = await client.PostAsync($"/api/v1/me/learning-goals/{goal.Id}/pause", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LearningGoalEndpoints_DoNotExposeOtherUsersGoal()
    {
        var otherProfile = StudentProfile.Create(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
        LearningGoal? otherGoal = null;
        await SeedAsync(dbContext =>
        {
            otherGoal = LearningGoal.Create(otherProfile.Id, "DailyStudyMinutes", "Other goal", null, 10, "minutes", null, DateTimeOffset.UtcNow);
            dbContext.StudentProfiles.Add(otherProfile);
            dbContext.LearningGoals.Add(otherGoal);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync($"/api/v1/me/learning-goals/{otherGoal!.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GenerateDailyStudyPlan_CreatesPlan()
    {
        await SeedPublishedLessonAsync("plan-lesson");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var plan = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");

        Assert.NotEqual(Guid.Empty, plan.Id);
        Assert.True(plan.TargetMinutes > 0);
        Assert.True(plan.TotalItems >= 0);
    }

    [Fact]
    public async Task GenerateDailyStudyPlan_ReturnsOnePlanPerStudentPerDay()
    {
        await SeedPublishedLessonAsync("single-plan-lesson");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var first = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");
        var second = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task GenerateDailyStudyPlan_PutsDuePracticeFirst()
    {
        var userId = await GetSuperAdminUserIdAsync();
        await SeedAsync(dbContext =>
        {
            var profile = dbContext.StudentProfiles.OrderBy(profile => profile.CreatedAt).FirstOrDefault(profile => profile.UserId == userId);
            if (profile is null)
            {
                profile = StudentProfile.Create(userId, null, DateTimeOffset.UtcNow);
                dbContext.StudentProfiles.Add(profile);
            }

            dbContext.PracticeItems.Add(PracticeItem.Create(profile.Id, "word", Guid.NewGuid(), "WordFlashcard", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var plan = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");

        Assert.Equal("PracticeDueItems", plan.Items.OrderBy(item => item.SortOrder).First().ItemType);
    }

    [Fact]
    public async Task GenerateDailyStudyPlan_DoesNotIncludeInactiveLesson()
    {
        var inactive = await SeedPublishedLessonAsync("inactive-plan-lesson", active: false);
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var plan = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");

        Assert.DoesNotContain(plan.Items, item => item.ContentId == inactive.Id);
    }

    [Fact]
    public async Task StudyPlanEndpoints_DoNotExposeOtherUsersPlan()
    {
        var otherProfile = StudentProfile.Create(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
        DailyStudyPlan? otherPlan = null;
        await SeedAsync(dbContext =>
        {
            otherPlan = DailyStudyPlan.Create(otherProfile.Id, new DateTimeOffset(DateTime.UtcNow.Date, TimeSpan.Zero), 30, DateTimeOffset.UtcNow);
            dbContext.StudentProfiles.Add(otherProfile);
            dbContext.DailyStudyPlans.Add(otherPlan);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync($"/api/v1/me/study-plan/{otherPlan!.Id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CompleteStudyPlanItem_UpdatesPlanCounts()
    {
        await SeedPublishedLessonAsync("complete-plan-item");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var plan = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");
        var item = plan.Items.First();

        var completedItem = await PostReadAsync<DailyStudyPlanItemDto>(client, $"/api/v1/me/study-plan/items/{item.Id}/complete");
        var today = await client.GetFromJsonAsync<DailyStudyPlanDto>("/api/v1/me/study-plan/today");

        Assert.Equal("Completed", completedItem.Status);
        Assert.True(today!.CompletedItems > 0);
    }

    [Fact]
    public async Task CompleteDailyStudyPlan_UpdatesStatus()
    {
        await SeedPublishedLessonAsync("complete-plan");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        var plan = await PostReadAsync<DailyStudyPlanDto>(client, "/api/v1/me/study-plan/today/generate");

        var completed = await PostReadAsync<DailyStudyPlanDto>(client, $"/api/v1/me/study-plan/{plan.Id}/complete");

        Assert.Equal("Completed", completed.Status);
    }

    private async Task<Lesson> SeedPublishedLessonAsync(string prefix, bool active = true)
    {
        Lesson? lesson = null;
        await SeedAsync(dbContext =>
        {
            lesson = Lesson.Create(Unique(prefix), "summary", "description", CefrLevel.A1, null, null, 10, 1, DateTimeOffset.UtcNow);
            lesson.Publish(DateTimeOffset.UtcNow);
            if (!active)
            {
                lesson.Deactivate(DateTimeOffset.UtcNow);
            }

            dbContext.Lessons.Add(lesson);
            return Task.CompletedTask;
        });
        return lesson!;
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

    private static async Task<T> PostReadAsync<T>(HttpClient client, string path)
    {
        var response = await client.PostAsync(path, null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static async Task<T> PostReadAsync<T>(HttpClient client, string path, object request)
    {
        var response = await client.PostAsJsonAsync(path, request);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
