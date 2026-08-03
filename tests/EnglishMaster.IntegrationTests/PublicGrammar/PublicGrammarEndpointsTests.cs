using System.Net;
using System.Net.Http.Json;

using EnglishMaster.Contracts.PublicGrammar;
using EnglishMaster.Domain.Common;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.IntegrationTests.PublicGrammar;

public sealed class PublicGrammarEndpointsTests(EnglishMasterApiFactory factory) : IClassFixture<EnglishMasterApiFactory>
{
    [Fact]
    public async Task TopicBySlug_AllowsAnonymousAndReturnsOnlyActiveRules()
    {
        var topic = GrammarTopic.Create(Unique("Public Topic"), "Published topic", CefrLevel.A2, 10, DateTimeOffset.UtcNow);
        var activeRule = CreateRule(topic.Id, Unique("Active Rule"), "Published rule");
        var inactiveRule = CreateRule(topic.Id, Unique("Inactive Rule"), "Hidden rule");
        inactiveRule.Deactivate(DateTimeOffset.UtcNow);
        await SeedAsync(dbContext =>
        {
            dbContext.GrammarTopics.Add(topic);
            dbContext.GrammarRules.AddRange(activeRule, inactiveRule);
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/public/grammar/topics/{topic.Slug.ToUpperInvariant()}");
        var result = await response.Content.ReadFromJsonAsync<PublicGrammarTopicDetailDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Contains(result.Rules, item => item.Slug == activeRule.Slug);
        Assert.DoesNotContain(result.Rules, item => item.Slug == inactiveRule.Slug);
    }

    [Fact]
    public async Task RuleBySlug_ReturnsOnlyActiveExamplesAndRelatedWords()
    {
        var now = DateTimeOffset.UtcNow;
        var topic = GrammarTopic.Create(Unique("Rule Topic"), "Published topic", CefrLevel.B1, 10, now);
        var rule = CreateRule(topic.Id, Unique("Public Rule"), "Published rule");
        var activeExample = GrammarExample.Create(rule.Id, "This example is visible.", "ตัวอย่างนี้แสดงผล", string.Empty, true, 10, now);
        var inactiveExample = GrammarExample.Create(rule.Id, "This example is hidden.", "ตัวอย่างนี้ไม่แสดงผล", string.Empty, true, 20, now);
        inactiveExample.Deactivate(now);
        var activeWord = CreateWord(Unique("activeword"), now);
        var inactiveWord = CreateWord(Unique("inactiveword"), now);
        inactiveWord.Deactivate(now);
        rule.AddRelatedWord(activeWord.Id, now);
        rule.AddRelatedWord(inactiveWord.Id, now);
        await SeedAsync(dbContext =>
        {
            dbContext.GrammarTopics.Add(topic);
            dbContext.Words.AddRange(activeWord, inactiveWord);
            dbContext.GrammarRules.Add(rule);
            dbContext.GrammarExamples.AddRange(activeExample, inactiveExample);
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/v1/public/grammar/rules/{rule.Slug}");
        var result = await response.Content.ReadFromJsonAsync<PublicGrammarRuleDetailDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(result);
        Assert.Single(result.Examples, item => item.ExampleEn == activeExample.ExampleEn);
        Assert.DoesNotContain(result.Examples, item => item.ExampleEn == inactiveExample.ExampleEn);
        Assert.Single(result.RelatedWords, item => item.Slug == activeWord.Slug);
        Assert.DoesNotContain(result.RelatedWords, item => item.Slug == inactiveWord.Slug);
    }

    [Fact]
    public async Task RuleBySlug_ReturnsNotFoundWhenRuleOrParentTopicIsInactive()
    {
        var now = DateTimeOffset.UtcNow;
        var inactiveTopic = GrammarTopic.Create(Unique("Inactive Topic"), "Hidden topic", CefrLevel.A1, 10, now);
        inactiveTopic.Deactivate(now);
        var ruleUnderInactiveTopic = CreateRule(inactiveTopic.Id, Unique("Orphaned Rule"), "Hidden by parent");
        var activeTopic = GrammarTopic.Create(Unique("Active Topic"), "Published topic", CefrLevel.A1, 20, now);
        var inactiveRule = CreateRule(activeTopic.Id, Unique("Inactive Public Rule"), "Hidden rule");
        inactiveRule.Deactivate(now);
        await SeedAsync(dbContext =>
        {
            dbContext.GrammarTopics.AddRange(inactiveTopic, activeTopic);
            dbContext.GrammarRules.AddRange(ruleUnderInactiveTopic, inactiveRule);
        });
        using var client = factory.CreateClient();

        var parentResponse = await client.GetAsync($"/api/v1/public/grammar/rules/{ruleUnderInactiveTopic.Slug}");
        var ruleResponse = await client.GetAsync($"/api/v1/public/grammar/rules/{inactiveRule.Slug}");

        Assert.Equal(HttpStatusCode.NotFound, parentResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, ruleResponse.StatusCode);
    }

    [Fact]
    public async Task TypedRoutesRemainUnambiguousWhenTopicAndRuleSlugsMatch()
    {
        var title = Unique("Shared Grammar Slug");
        var topic = GrammarTopic.Create(title, "Topic summary", CefrLevel.A1, 10, DateTimeOffset.UtcNow);
        var rule = CreateRule(topic.Id, title, "Rule body");
        await SeedAsync(dbContext =>
        {
            dbContext.GrammarTopics.Add(topic);
            dbContext.GrammarRules.Add(rule);
        });
        using var client = factory.CreateClient();

        var topicResult = await client.GetFromJsonAsync<PublicGrammarTopicDetailDto>(
            $"/api/v1/public/grammar/topics/{topic.Slug}");
        var ruleResult = await client.GetFromJsonAsync<PublicGrammarRuleDetailDto>(
            $"/api/v1/public/grammar/rules/{rule.Slug}");

        Assert.Equal("Topic summary", topicResult!.Summary);
        Assert.Equal("Rule body", ruleResult!.RuleText);
    }

    private async Task SeedAsync(Action<EnglishMasterDbContext> seed)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<EnglishMasterDbContext>();
        seed(dbContext);
        await dbContext.SaveChangesAsync();
    }

    private static GrammarRule CreateRule(Guid topicId, string title, string ruleText) =>
        GrammarRule.Create(
            topicId,
            title,
            ruleText,
            "คำอธิบาย",
            "Explanation",
            "Subject + verb",
            string.Empty,
            string.Empty,
            10,
            DateTimeOffset.UtcNow);

    private static Word CreateWord(string text, DateTimeOffset now) =>
        Word.Create(
            text,
            string.Empty,
            string.Empty,
            string.Empty,
            "ความหมาย",
            "Meaning",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            string.Empty,
            string.Empty,
            now);

    private static string Unique(string prefix) =>
        $"{prefix}-{Guid.NewGuid():N}";
}