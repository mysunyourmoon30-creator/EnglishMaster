using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.Grammar;

public sealed class GrammarEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public GrammarEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GrammarEndpointsSupportTopicRuleRelatedWordsExamplesAndSoftDelete()
    {
        var topicResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-topics",
            new CreateGrammarTopicRequest(
                "Present Simple Integration",
                "Daily routines",
                "A1",
                0));
        Assert.Equal(HttpStatusCode.Created, topicResponse.StatusCode);
        var topic = await topicResponse.Content.ReadFromJsonAsync<GrammarTopicDto>();
        Assert.NotNull(topic);
        Assert.Equal("present-simple-integration", topic.Slug);

        var topicSearch = await client.GetFromJsonAsync<GrammarTopicSearchResponse>(
            "/api/v1/grammar-topics?cefrLevel=A1&search=Present");
        Assert.Contains(topicSearch!.Items, item => item.Id == topic.Id);

        var ruleResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-rules",
            new CreateGrammarRuleRequest(
                topic.Id,
                "Positive Sentences Integration",
                "Subject + base verb",
                "ใช้กับกิจวัตร",
                "Used for routines",
                "S + V1",
                "Missing s for he/she/it",
                "Add s for third person singular",
                0));
        Assert.Equal(HttpStatusCode.Created, ruleResponse.StatusCode);
        var rule = await ruleResponse.Content.ReadFromJsonAsync<GrammarRuleDto>();
        Assert.NotNull(rule);
        Assert.Equal(topic.Id, rule.GrammarTopicId);

        var byTopic = await client.GetFromJsonAsync<GrammarRuleSearchResponse>(
            $"/api/v1/grammar-topics/{topic.Id}/rules");
        Assert.Contains(byTopic!.Items, item => item.Id == rule.Id);

        var word = await CreateWordAsync("grammar walk integration");
        var relatedWordResponse = await client.PostAsync(
            $"/api/v1/grammar-rules/{rule.Id}/words/{word.Id}",
            content: null);
        Assert.Equal(HttpStatusCode.OK, relatedWordResponse.StatusCode);
        var ruleWithWord = await relatedWordResponse.Content.ReadFromJsonAsync<GrammarRuleDto>();
        Assert.Contains(ruleWithWord!.RelatedWords, item => item.Id == word.Id);

        var exampleResponse = await client.PostAsJsonAsync(
            $"/api/v1/grammar-rules/{rule.Id}/examples",
            new CreateGrammarExampleRequest(
                "She walks to school.",
                "เธอเดินไปโรงเรียน",
                "Add s after she.",
                true,
                0));
        Assert.Equal(HttpStatusCode.Created, exampleResponse.StatusCode);
        var example = await exampleResponse.Content.ReadFromJsonAsync<GrammarExampleDto>();
        Assert.NotNull(example);
        Assert.Equal(rule.Id, example.GrammarRuleId);

        var examples = await client.GetFromJsonAsync<IReadOnlyCollection<GrammarExampleDto>>(
            $"/api/v1/grammar-rules/{rule.Id}/examples");
        Assert.Contains(examples!, item => item.Id == example.Id);

        var examplesFromBaseRoute = await client.GetFromJsonAsync<IReadOnlyCollection<GrammarExampleDto>>(
            $"/api/v1/grammar-examples?grammarRuleId={rule.Id}");
        Assert.Contains(examplesFromBaseRoute!, item => item.Id == example.Id);

        var updateExampleResponse = await client.PutAsJsonAsync(
            $"/api/v1/grammar-examples/{example.Id}",
            new UpdateGrammarExampleRequest(
                "He plays football.",
                "เขาเล่นฟุตบอล",
                "Add s after he.",
                true,
                1,
                true));
        Assert.Equal(HttpStatusCode.OK, updateExampleResponse.StatusCode);
        var updatedExample = await updateExampleResponse.Content.ReadFromJsonAsync<GrammarExampleDto>();
        Assert.Equal("He plays football.", updatedExample!.ExampleEn);

        var removeWordResponse = await client.DeleteAsync(
            $"/api/v1/grammar-rules/{rule.Id}/words/{word.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeWordResponse.StatusCode);

        var updatedRuleResponse = await client.PutAsJsonAsync(
            $"/api/v1/grammar-rules/{rule.Id}",
            new UpdateGrammarRuleRequest(
                topic.Id,
                "Positive Sentences Integration Updated",
                "Subject + base verb",
                "ใช้กับกิจวัตร",
                "Used for routines",
                "S + V1",
                "Missing s for he/she/it",
                "Add s for third person singular",
                1,
                true));
        Assert.Equal(HttpStatusCode.OK, updatedRuleResponse.StatusCode);
        var updatedRule = await updatedRuleResponse.Content.ReadFromJsonAsync<GrammarRuleDto>();
        Assert.Equal("Positive Sentences Integration Updated", updatedRule!.Title);

        var deleteExampleResponse = await client.DeleteAsync($"/api/v1/grammar-examples/{example.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteExampleResponse.StatusCode);

        var deleteRuleResponse = await client.DeleteAsync($"/api/v1/grammar-rules/{rule.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteRuleResponse.StatusCode);

        var activeRuleSearch = await client.GetFromJsonAsync<GrammarRuleSearchResponse>(
            "/api/v1/grammar-rules?search=Updated");
        Assert.DoesNotContain(activeRuleSearch!.Items, item => item.Id == rule.Id);

        var inactiveRuleSearch = await client.GetFromJsonAsync<GrammarRuleSearchResponse>(
            "/api/v1/grammar-rules?search=Updated&isActive=false");
        Assert.Contains(inactiveRuleSearch!.Items, item => item.Id == rule.Id);

        var deleteTopicResponse = await client.DeleteAsync($"/api/v1/grammar-topics/{topic.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteTopicResponse.StatusCode);
    }

    [Fact]
    public async Task CreateRuleReturnsValidationProblemWhenRuleTextIsMissing()
    {
        var topicResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-topics",
            new CreateGrammarTopicRequest("Validation Topic", null, "A1", 0));
        var topic = await topicResponse.Content.ReadFromJsonAsync<GrammarTopicDto>();

        var response = await client.PostAsJsonAsync(
            "/api/v1/grammar-rules",
            new CreateGrammarRuleRequest(topic!.Id, "Missing Rule Text", " ", null, null, null, null, null, 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateRuleReturnsValidationProblemWhenTitleAlreadyExists()
    {
        var topicResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-topics",
            new CreateGrammarTopicRequest("Duplicate Rule Topic", null, "A1", 0));
        var topic = await topicResponse.Content.ReadFromJsonAsync<GrammarTopicDto>();

        var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-rules",
            new CreateGrammarRuleRequest(
                topic!.Id, "Duplicate Rule Title", "Subject + base verb", null, null, null, null, null, 0));
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-rules",
            new CreateGrammarRuleRequest(
                topic.Id, "Duplicate Rule Title", "Subject + base verb", null, null, null, null, null, 1));

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    private async Task<WordDto> CreateWordAsync(string text)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(
                text,
                null,
                null,
                null,
                "Thai meaning",
                text,
                "Verb",
                "A1",
                null,
                null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<WordDto>())!;
    }
}
