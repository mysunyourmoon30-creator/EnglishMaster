using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.ImportExport;

public sealed class ContentExportEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task ExportWords_ReturnsCsvContainingSeededWord()
    {
        var wordText = Unique("Export Word");
        await SeedAsync(dbContext =>
        {
            dbContext.Words.Add(Word.Create(
                wordText, "ipa-uk", "ipa-us", "thai-reading", "meaning-th", "meaning-en",
                PartOfSpeech.Noun, CefrLevel.A1, "example-en", "example-th", DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/export/words?format=csv");
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/csv", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains("words.csv", response.Content.Headers.ContentDisposition?.FileNameStar ?? string.Empty);
        Assert.StartsWith("Id,Text,Slug", body);
        Assert.Contains(wordText, body);
        Assert.Contains("meaning-th", body);
    }

    [Fact]
    public async Task ExportWords_ReturnsJsonArrayContainingSeededWord()
    {
        var wordText = Unique("Export Json Word");
        await SeedAsync(dbContext =>
        {
            dbContext.Words.Add(Word.Create(
                wordText, "ipa-uk", "ipa-us", "thai-reading", "meaning-th", "meaning-en",
                PartOfSpeech.Verb, CefrLevel.B2, "example-en", "example-th", DateTimeOffset.UtcNow));
            return Task.CompletedTask;
        });
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/export/words?format=json");
        var body = await response.Content.ReadAsStringAsync();
        using var document = JsonDocument.Parse(body);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal(JsonValueKind.Array, document.RootElement.ValueKind);
        Assert.Contains(
            document.RootElement.EnumerateArray(),
            element => element.GetProperty("Text").GetString() == wordText
                && element.GetProperty("MeaningTh").GetString() == "meaning-th");
    }

    [Fact]
    public async Task ExportWords_RejectsUnsupportedFormat()
    {
        using var client = factory.CreateClient(new() { HandleCookies = true });
        await LoginAsync(client);

        var response = await client.GetAsync("/api/v1/export/words?format=xml");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task SeedAsync(Func<EnglishMasterDbContext, Task> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        await seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static Task<HttpResponseMessage> LoginAsync(HttpClient client) =>
        client.PostAsJsonAsync("/api/v1/auth/login", new LoginRequest("superadmin@englishmaster.test", "TestPassword1"));

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}
