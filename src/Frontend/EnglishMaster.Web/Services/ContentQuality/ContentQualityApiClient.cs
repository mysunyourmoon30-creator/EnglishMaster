using System.Net.Http.Json;
using EnglishMaster.Contracts.ContentQuality;

namespace EnglishMaster.Web.Services.ContentQuality;

public sealed class ContentQualityApiClient : IContentQualityApiClient
{
    private readonly HttpClient httpClient;

    public ContentQualityApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ContentQualityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/content-quality/dashboard", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentQualityDashboardDto>(response, cancellationToken);
    }

    public async Task<ContentQualityCheckSearchResponse> SearchChecksAsync(string? contentType, string? status, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/content-quality/checks", ("contentType", contentType), ("status", status)), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ContentQualityCheckSearchResponse>(cancellationToken: cancellationToken) ??
            new ContentQualityCheckSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<ContentQualityCheckDto> GetCheckAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/content-quality/checks/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentQualityCheckDto>(response, cancellationToken);
    }

    public async Task<ContentQualityRuleSearchResponse> SearchRulesAsync(string? contentType, string? severity, bool? isActive, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/content-quality/rules", ("contentType", contentType), ("severity", severity), ("isActive", isActive?.ToString())), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ContentQualityRuleSearchResponse>(cancellationToken: cancellationToken) ??
            new ContentQualityRuleSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<ContentQualityRuleDto> CreateRuleAsync(CreateContentQualityRuleRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/content-quality/rules", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentQualityRuleDto>(response, cancellationToken);
    }

    public async Task<ContentQualityRuleDto> UpdateRuleAsync(Guid id, UpdateContentQualityRuleRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/content-quality/rules/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentQualityRuleDto>(response, cancellationToken);
    }

    public async Task<ContentQualityCheckDto> RunAsync(string contentType, Guid contentId, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/content-quality/checks/{Uri.EscapeDataString(contentType)}/{contentId}/run", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentQualityCheckDto>(response, cancellationToken);
    }

    public async Task<ContentQualityFindingDto> ResolveFindingAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/content-quality/findings/{id}/resolve", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentQualityFindingDto>(response, cancellationToken);
    }

    private static string BuildEndpoint(string path, params (string Key, string? Value)[] parameters)
    {
        var query = parameters
            .Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value))
            .Select(parameter => $"{parameter.Key}={Uri.EscapeDataString(parameter.Value!)}")
            .ToArray();
        return query.Length == 0 ? path : $"{path}?{string.Join("&", query)}";
    }
}
