using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.PublicSearch;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.PublicSearch;

public sealed class PublicSearchEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task SearchPublicContent_ReturnsActiveWordByText()
    {
        var marker = Unique("searchword");
        await SeedAsync(dbContext =>
        {
            dbContext.Words.Add(Word.Create(marker, string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.A1, string.Empty, string.Empty, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={marker}");

        Assert.Contains(result!.Items, item => item.ContentType == "word" && item.Title == marker);
    }

    [Fact]
    public async Task SearchPublicContent_DoesNotReturnInactiveWord()
    {
        var marker = Unique("inactiveword");
        await SeedAsync(dbContext =>
        {
            var word = Word.Create(marker, string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.A1, string.Empty, string.Empty, DateTimeOffset.UtcNow);
            word.Deactivate(DateTimeOffset.UtcNow);
            dbContext.Words.Add(word);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={marker}");

        Assert.DoesNotContain(result!.Items, item => item.Title == marker);
    }

    [Fact]
    public async Task SearchPublicContent_ReturnsGrammarTitle()
    {
        var marker = Unique("Grammar Topic");
        await SeedAsync(dbContext =>
        {
            dbContext.GrammarTopics.Add(GrammarTopic.Create(marker, "summary", CefrLevel.B1, 1, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={Uri.EscapeDataString(marker)}");

        Assert.Contains(result!.Items, item => item.ContentType == "grammar" && item.Title == marker);
    }

    [Fact]
    public async Task SearchPublicContent_ReturnsPublishedLessonAndExcludesDraft()
    {
        var publishedTitle = Unique("Published Lesson");
        var draftTitle = Unique("Draft Lesson");
        await SeedAsync(dbContext =>
        {
            var published = Lesson.Create(publishedTitle, "summary", "description", CefrLevel.A2, null, null, 10, 1, DateTimeOffset.UtcNow);
            published.Publish(DateTimeOffset.UtcNow);
            var draft = Lesson.Create(draftTitle, "summary", "description", CefrLevel.A2, null, null, 10, 1, DateTimeOffset.UtcNow);
            dbContext.Lessons.AddRange(published, draft);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var publishedResult = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={Uri.EscapeDataString(publishedTitle)}");
        var draftResult = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={Uri.EscapeDataString(draftTitle)}");

        Assert.Contains(publishedResult!.Items, item => item.ContentType == "lesson" && item.Title == publishedTitle);
        Assert.DoesNotContain(draftResult!.Items, item => item.Title == draftTitle);
    }

    [Fact]
    public async Task SearchPublicContent_FiltersByContentTypeAndCefr()
    {
        var wordTitle = Unique("filterword");
        var lessonTitle = Unique("Filter Lesson");
        await SeedAsync(dbContext =>
        {
            dbContext.Words.Add(Word.Create(wordTitle, string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.C1, string.Empty, string.Empty, DateTimeOffset.UtcNow));
            var lesson = Lesson.Create(lessonTitle, wordTitle, "description", CefrLevel.A1, null, null, 10, 1, DateTimeOffset.UtcNow);
            lesson.Publish(DateTimeOffset.UtcNow);
            dbContext.Lessons.Add(lesson);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={wordTitle}&contentType=word&cefrLevel=C1");

        Assert.All(result!.Items, item => Assert.Equal("word", item.ContentType));
        Assert.Contains(result.Items, item => item.Title == wordTitle && item.CefrLevel == "C1");
    }

    [Fact]
    public async Task SearchPublicContent_SupportsTypeAndCefrAliases()
    {
        var wordTitle = Unique("aliasword");
        await SeedAsync(dbContext =>
        {
            dbContext.Words.Add(Word.Create(wordTitle, string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.B2, string.Empty, string.Empty, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={wordTitle}&type=words&cefr=B2");

        Assert.Contains(result!.Items, item => item.ContentType == "word" && item.Title == wordTitle && item.CefrLevel == "B2");
    }

    [Fact]
    public async Task SearchPublicContent_FiltersByCategoryAndTag()
    {
        var marker = Unique("taggedword");
        var categoryName = Unique("Category");
        var tagName = Unique("Tag");
        Guid categoryId = Guid.Empty;
        Guid tagId = Guid.Empty;
        await SeedAsync(dbContext =>
        {
            var category = Category.Create(categoryName, string.Empty, 1, DateTimeOffset.UtcNow);
            var tag = Tag.Create(tagName, string.Empty, DateTimeOffset.UtcNow);
            categoryId = category.Id;
            tagId = tag.Id;
            var word = Word.Create(marker, string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.A1, string.Empty, string.Empty, category.Id, [tag.Id], DateTimeOffset.UtcNow);
            dbContext.Categories.Add(category);
            dbContext.Tags.Add(tag);
            dbContext.Words.Add(word);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={marker}&categoryId={categoryId}&tagId={tagId}");

        var item = Assert.Single(result!.Items, item => item.Title == marker);
        Assert.Equal(categoryName, item.CategoryName);
        Assert.Contains(tagName, item.Tags);
    }

    [Fact]
    public async Task SearchPublicContent_AppliesPagination()
    {
        var marker = Unique("pageword");
        await SeedAsync(dbContext =>
        {
            for (var index = 0; index < 3; index++)
            {
                dbContext.Words.Add(Word.Create($"{marker}-{index}", string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.A1, string.Empty, string.Empty, DateTimeOffset.UtcNow.AddMinutes(index)));
            }

            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var result = await client.GetFromJsonAsync<PublicSearchResponse>($"/api/v1/public/search?q={marker}&pageNumber=1&pageSize=2");

        Assert.Equal(2, result!.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public async Task SearchPublicContent_HandlesSpecialCharactersSafely()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/public/search?q=%25%5B%5D%27%22&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task PublicSearchSuggestions_ReturnsActiveWordSuggestions()
    {
        var marker = Unique("suggestword");
        await SeedAsync(dbContext =>
        {
            dbContext.Words.Add(Word.Create(marker, string.Empty, string.Empty, string.Empty, "meaning", "meaning", PartOfSpeech.Noun, CefrLevel.A1, string.Empty, string.Empty, DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var suggestions = await client.GetFromJsonAsync<PublicSearchSuggestionsResponse>($"/api/v1/public/search/suggestions?q={marker}");

        Assert.Contains(marker, suggestions!.Suggestions);
    }

    [Fact]
    public async Task SearchPublicContent_DoesNotExposeQuizAnswers()
    {
        var marker = Unique("Quiz Title");
        await SeedAsync(dbContext =>
        {
            var quiz = Quiz.Create(marker, "Safe summary", "description", CefrLevel.B2, null, null, null, null, 10, 70, 1, DateTimeOffset.UtcNow);
            quiz.Publish(DateTimeOffset.UtcNow);
            dbContext.Quizzes.Add(quiz);
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/public/search?q={Uri.EscapeDataString(marker)}");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("IsCorrect", body, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Correct", body, StringComparison.OrdinalIgnoreCase);
    }

    private async Task SeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
