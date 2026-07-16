using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Contracts.GrammarTopics;

namespace EnglishMaster.Web.Services.Grammar;

public sealed class GrammarApiClient : IGrammarApiClient
{
    private readonly HttpClient httpClient;

    public GrammarApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<GrammarTopicSearchResponse> SearchTopicsAsync(
        GrammarTopicSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildTopicSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GrammarTopicSearchResponse>(
            cancellationToken: cancellationToken) ?? new GrammarTopicSearchResponse(
                [],
                request.PageNumber,
                request.PageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<GrammarTopicDto?> GetTopicAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/grammar-topics/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GrammarTopicDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<GrammarTopicDto> CreateTopicAsync(
        CreateGrammarTopicRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/grammar-topics", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarTopicDto>(response, cancellationToken);
    }

    public async Task<GrammarTopicDto> UpdateTopicAsync(
        Guid id,
        UpdateGrammarTopicRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/grammar-topics/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarTopicDto>(response, cancellationToken);
    }

    public async Task DeleteTopicAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/grammar-topics/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<GrammarRuleSearchResponse> SearchRulesAsync(
        GrammarRuleSearchRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildRuleSearchEndpoint("api/v1/grammar-rules", request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GrammarRuleSearchResponse>(
            cancellationToken: cancellationToken) ?? new GrammarRuleSearchResponse(
                [],
                request.PageNumber,
                request.PageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<GrammarRuleSearchResponse> GetRulesByTopicIdAsync(
        Guid topicId,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var parameters = new List<string>();
        if (isActive.HasValue)
        {
            Add(parameters, "isActive", isActive.Value ? "true" : "false");
        }

        Add(parameters, "pageNumber", pageNumber.ToString());
        Add(parameters, "pageSize", pageSize.ToString());
        var endpoint = $"api/v1/grammar-topics/{topicId}/rules?{string.Join("&", parameters)}";

        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GrammarRuleSearchResponse>(
            cancellationToken: cancellationToken) ?? new GrammarRuleSearchResponse(
                [],
                pageNumber,
                pageSize,
                0,
                0,
                false,
                false);
    }

    public async Task<GrammarRuleDto?> GetRuleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/grammar-rules/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GrammarRuleDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<GrammarRuleDto> CreateRuleAsync(
        CreateGrammarRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/grammar-rules", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarRuleDto>(response, cancellationToken);
    }

    public async Task<GrammarRuleDto> UpdateRuleAsync(
        Guid id,
        UpdateGrammarRuleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/grammar-rules/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarRuleDto>(response, cancellationToken);
    }

    public async Task DeleteRuleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/grammar-rules/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<GrammarRuleDto> AddRelatedWordAsync(
        Guid grammarRuleId,
        Guid wordId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(
            $"api/v1/grammar-rules/{grammarRuleId}/words/{wordId}",
            content: null,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarRuleDto>(response, cancellationToken);
    }

    public async Task RemoveRelatedWordAsync(
        Guid grammarRuleId,
        Guid wordId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync(
            $"api/v1/grammar-rules/{grammarRuleId}/words/{wordId}",
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<GrammarExampleDto?> GetExampleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/grammar-examples/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<GrammarExampleDto>(
            cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<GrammarExampleDto>> GetExamplesByRuleIdAsync(
        Guid grammarRuleId,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(
            $"api/v1/grammar-rules/{grammarRuleId}/examples",
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<GrammarExampleDto>>(
            cancellationToken: cancellationToken) ?? [];
    }

    public async Task<GrammarExampleDto> AddExampleAsync(
        Guid grammarRuleId,
        CreateGrammarExampleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync(
            $"api/v1/grammar-rules/{grammarRuleId}/examples",
            request,
            cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarExampleDto>(response, cancellationToken);
    }

    public async Task<GrammarExampleDto> UpdateExampleAsync(
        Guid id,
        UpdateGrammarExampleRequest request,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/grammar-examples/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<GrammarExampleDto>(response, cancellationToken);
    }

    public async Task DeleteExampleAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/grammar-examples/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    private static string BuildTopicSearchEndpoint(GrammarTopicSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        Add(parameters, "cefrLevel", request.CefrLevel);
        if (request.IsActive.HasValue)
        {
            Add(parameters, "isActive", request.IsActive.Value ? "true" : "false");
        }

        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());

        return parameters.Count == 0
            ? "api/v1/grammar-topics"
            : $"api/v1/grammar-topics?{string.Join("&", parameters)}";
    }

    private static string BuildRuleSearchEndpoint(
        string baseEndpoint,
        GrammarRuleSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "search", request.Search);
        if (request.GrammarTopicId.HasValue)
        {
            Add(parameters, "grammarTopicId", request.GrammarTopicId.Value.ToString());
        }

        if (request.IsActive.HasValue)
        {
            Add(parameters, "isActive", request.IsActive.Value ? "true" : "false");
        }

        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());

        return parameters.Count == 0
            ? baseEndpoint
            : $"{baseEndpoint}?{string.Join("&", parameters)}";
    }

    private static void Add(ICollection<string> parameters, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        parameters.Add($"{name}={Uri.EscapeDataString(value.Trim())}");
    }
}
