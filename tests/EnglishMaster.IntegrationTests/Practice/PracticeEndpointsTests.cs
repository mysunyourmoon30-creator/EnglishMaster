using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Practice;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.Practice;

public sealed class PracticeEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task GeneratePracticeItems_CreatesWordItemsWithoutDuplicates()
    {
        var word = await SeedWordAsync("practiceword");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var first = await client.PostAsync("/api/v1/me/practice/generate", null);
        var second = await client.PostAsync("/api/v1/me/practice/generate", null);
        var due = await client.GetFromJsonAsync<IReadOnlyCollection<PracticeItemDto>>("/api/v1/me/practice/due");

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, second.StatusCode);
        Assert.Contains(due!, item => item.ContentId == word.Id && item.PracticeType == "WordFlashcard");
        Assert.NotNull(due);
        Assert.Equal(due.Select(item => new { item.ContentId, item.PracticeType }).Distinct().Count(), due.Count);
    }

    [Fact]
    public async Task StartPracticeSession_ReturnsDueItems()
    {
        await SeedWordAsync("sessionword");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);

        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");

        Assert.NotEqual(Guid.Empty, session.Id);
        Assert.True(session.TotalItems > 0);
        Assert.NotEmpty(session.Items);
    }

    [Fact]
    public async Task SubmitPracticeSessionItem_WithGoodResult_SchedulesFourDays()
    {
        await SeedWordAsync("goodword");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");
        var target = session.Items.First();

        var submitted = await client.PostAsJsonAsync($"/api/v1/me/practice/session-items/{target.Id}/submit", new SubmitPracticeSessionItemRequest("answer", "Good"));
        var item = await submitted.Content.ReadFromJsonAsync<PracticeSessionItemDto>();
        var due = await client.GetFromJsonAsync<IReadOnlyCollection<PracticeItemDto>>("/api/v1/me/practice/due?limit=50");

        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);
        Assert.True(item!.IsCorrect);
        Assert.DoesNotContain(due!, dueItem => dueItem.Id == target.PracticeItemId);
        await AssertIntervalAsync(target.PracticeItemId, 4);
    }

    [Fact]
    public async Task SubmitPracticeSessionItem_WithAgainResult_SchedulesTomorrow()
    {
        await SeedWordAsync("againword");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");
        var target = session.Items.First();

        var submitted = await client.PostAsJsonAsync($"/api/v1/me/practice/session-items/{target.Id}/submit", new SubmitPracticeSessionItemRequest("answer", "Again"));

        Assert.Equal(HttpStatusCode.OK, submitted.StatusCode);
        await AssertIntervalAsync(target.PracticeItemId, 1);
    }

    [Fact]
    public async Task SubmitPracticeSessionItem_WithInvalidResult_ReturnsBadRequest()
    {
        await SeedWordAsync("invalidpractice");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");
        var target = session.Items.First();

        var submitted = await client.PostAsJsonAsync($"/api/v1/me/practice/session-items/{target.Id}/submit", new SubmitPracticeSessionItemRequest("answer", "Almost"));

        Assert.Equal(HttpStatusCode.BadRequest, submitted.StatusCode);
    }

    [Fact]
    public async Task SubmitPracticeSessionItem_WhenSubmittedTwice_DoesNotIncrementCountersAgain()
    {
        await SeedWordAsync("doublesubmitpractice");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");
        var target = session.Items.First();

        var first = await client.PostAsJsonAsync($"/api/v1/me/practice/session-items/{target.Id}/submit", new SubmitPracticeSessionItemRequest("answer", "Good"));
        var second = await client.PostAsJsonAsync($"/api/v1/me/practice/session-items/{target.Id}/submit", new SubmitPracticeSessionItemRequest("answer", "Easy"));

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
        await AssertCountersAsync(target.PracticeItemId, reviewCount: 1, correctCount: 1, incorrectCount: 0);
    }

    [Fact]
    public async Task CompletePracticeSession_RecordsCounts()
    {
        await SeedWordAsync("completepractice");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");
        await client.PostAsJsonAsync($"/api/v1/me/practice/session-items/{session.Items.First().Id}/submit", new SubmitPracticeSessionItemRequest("answer", "Good"));

        var completed = await PostReadAsync<PracticeSessionDto>(client, $"/api/v1/me/practice/sessions/{session.Id}/complete");

        Assert.Equal("Completed", completed.Status);
        Assert.True(completed.CompletedItems > 0);
    }

    [Fact]
    public async Task CompletePracticeSession_WhenCompletedTwice_ReturnsNotFound()
    {
        await SeedWordAsync("doublecompletepractice");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var session = await PostReadAsync<PracticeSessionDto>(client, "/api/v1/me/practice/sessions/start");

        var first = await client.PostAsync($"/api/v1/me/practice/sessions/{session.Id}/complete", null);
        var second = await client.PostAsync($"/api/v1/me/practice/sessions/{session.Id}/complete", null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, second.StatusCode);
    }

    [Fact]
    public async Task SuspendPracticeItem_RemovesItemFromDuePractice()
    {
        await SeedWordAsync("suspendpractice");
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);
        await client.PostAsync("/api/v1/me/practice/generate", null);
        var dueBefore = await client.GetFromJsonAsync<IReadOnlyCollection<PracticeItemDto>>("/api/v1/me/practice/due");
        var target = dueBefore!.First();

        var suspended = await client.PostAsync($"/api/v1/me/practice/items/{target.Id}/suspend", null);
        var dueAfter = await client.GetFromJsonAsync<IReadOnlyCollection<PracticeItemDto>>("/api/v1/me/practice/due?limit=50");

        Assert.Equal(HttpStatusCode.OK, suspended.StatusCode);
        Assert.DoesNotContain(dueAfter!, item => item.Id == target.Id);
    }

    [Fact]
    public async Task PracticeEndpoints_DoNotExposeOtherUsersPracticeData()
    {
        var userId = await GetSuperAdminUserIdAsync();
        var otherProfile = StudentProfile.Create(Guid.NewGuid(), null, DateTimeOffset.UtcNow);
        var otherContentId = Guid.NewGuid();
        await SeedAsync(dbContext =>
        {
            dbContext.StudentProfiles.Add(otherProfile);
            dbContext.PracticeItems.Add(PracticeItem.Create(otherProfile.Id, "word", otherContentId, "WordFlashcard", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var due = await client.GetFromJsonAsync<IReadOnlyCollection<PracticeItemDto>>("/api/v1/me/practice/due");

        Assert.DoesNotContain(due!, item => item.ContentId == otherContentId);
        Assert.NotEqual(Guid.Empty, userId);
    }

    private async Task<Word> SeedWordAsync(string prefix)
    {
        Word? word = null;
        await SeedAsync(dbContext =>
        {
            word = Word.Create(Unique(prefix), string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.A1, string.Empty, string.Empty, DateTimeOffset.UtcNow);
            dbContext.Words.Add(word);
            return Task.CompletedTask;
        });
        return word!;
    }

    private async Task AssertIntervalAsync(Guid practiceItemId, int expectedDays)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        var item = await dbContext.PracticeItems.AsNoTracking().SingleAsync(item => item.Id == practiceItemId);
        Assert.Equal(expectedDays, item.CurrentIntervalDays);
    }

    private async Task AssertCountersAsync(Guid practiceItemId, int reviewCount, int correctCount, int incorrectCount)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        var item = await dbContext.PracticeItems.AsNoTracking().SingleAsync(item => item.Id == practiceItemId);
        Assert.Equal(reviewCount, item.ReviewCount);
        Assert.Equal(correctCount, item.CorrectCount);
        Assert.Equal(incorrectCount, item.IncorrectCount);
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

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
