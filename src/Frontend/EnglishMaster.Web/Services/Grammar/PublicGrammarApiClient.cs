using System.Net;
using System.Net.Http.Json;

using EnglishMaster.Contracts.PublicGrammar;

namespace EnglishMaster.Web.Services.Grammar;

public sealed class PublicGrammarApiClient(HttpClient httpClient) : IPublicGrammarApiClient
{
    public async Task<PublicGrammarTopicDetailDto?> GetTopicBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"api/v1/public/grammar/topics/{Uri.EscapeDataString(slug)}",
            cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublicGrammarTopicDetailDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<PublicGrammarRuleDetailDto?> GetRuleBySlugAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"api/v1/public/grammar/rules/{Uri.EscapeDataString(slug)}",
            cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublicGrammarRuleDetailDto>(
            cancellationToken: cancellationToken);
    }
}