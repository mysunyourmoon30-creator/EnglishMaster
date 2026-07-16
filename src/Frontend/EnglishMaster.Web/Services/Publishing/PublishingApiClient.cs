using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Publishing;

namespace EnglishMaster.Web.Services.Publishing;

public sealed class PublishingApiClient : IPublishingApiClient
{
    private readonly HttpClient httpClient;

    public PublishingApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PublishJobSearchResponse> SearchJobsAsync(PublishJobSearchRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildJobSearchEndpoint(request), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<PublishJobSearchResponse>(cancellationToken: cancellationToken)
            ?? new PublishJobSearchResponse([], request.PageNumber, request.PageSize, 0, 0, false, false);
    }

    public async Task<PublishJobDto?> GetJobAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/publish-jobs/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublishJobDto>(cancellationToken: cancellationToken);
    }

    public async Task<PublishJobDto> CreateJobAsync(CreatePublishJobRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/publish-jobs", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<PublishJobDto>(response, cancellationToken);
    }

    public async Task<PublishJobDto> RunJobAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/publish-jobs/{id}/run", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<PublishJobDto>(response, cancellationToken);
    }

    public async Task<PublishJobDto> CancelJobAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/publish-jobs/{id}/cancel", content: null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<PublishJobDto>(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PublishedArtifactDto>> GetArtifactsByJobAsync(Guid publishJobId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/publish-jobs/{publishJobId}/artifacts", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<PublishedArtifactDto>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<PublishTemplateSearchResponse> SearchTemplatesAsync(string? format, bool? isDefault, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var parameters = new List<string>();
        Add(parameters, "format", format);
        Add(parameters, "isDefault", isDefault?.ToString().ToLowerInvariant());
        Add(parameters, "isActive", isActive?.ToString().ToLowerInvariant());
        Add(parameters, "pageNumber", pageNumber.ToString());
        Add(parameters, "pageSize", pageSize.ToString());
        var endpoint = parameters.Count == 0 ? "api/v1/publish-templates" : $"api/v1/publish-templates?{string.Join("&", parameters)}";
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublishTemplateSearchResponse>(cancellationToken: cancellationToken)
            ?? new PublishTemplateSearchResponse([], pageNumber, pageSize, 0, 0, false, false);
    }

    public async Task<PublishTemplateDto?> GetTemplateAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/publish-templates/{id}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublishTemplateDto>(cancellationToken: cancellationToken);
    }

    public async Task<PublishTemplateDto> CreateTemplateAsync(CreatePublishTemplateRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/publish-templates", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<PublishTemplateDto>(response, cancellationToken);
    }

    public async Task<PublishTemplateDto> UpdateTemplateAsync(Guid id, UpdatePublishTemplateRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/publish-templates/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<PublishTemplateDto>(response, cancellationToken);
    }

    public async Task DeleteTemplateAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.DeleteAsync($"api/v1/publish-templates/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
    }

    public async Task<PublishedArtifactSearchResponse> SearchArtifactsAsync(Guid? publishJobId, string? sourceType, Guid? sourceId, string? format, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var parameters = new List<string>();
        Add(parameters, "publishJobId", publishJobId?.ToString());
        Add(parameters, "sourceType", sourceType);
        Add(parameters, "sourceId", sourceId?.ToString());
        Add(parameters, "format", format);
        Add(parameters, "pageNumber", pageNumber.ToString());
        Add(parameters, "pageSize", pageSize.ToString());
        var endpoint = parameters.Count == 0 ? "api/v1/published-artifacts" : $"api/v1/published-artifacts?{string.Join("&", parameters)}";
        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<PublishedArtifactSearchResponse>(cancellationToken: cancellationToken)
            ?? new PublishedArtifactSearchResponse([], pageNumber, pageSize, 0, 0, false, false);
    }

    private static string BuildJobSearchEndpoint(PublishJobSearchRequest request)
    {
        var parameters = new List<string>();
        Add(parameters, "sourceType", request.SourceType);
        Add(parameters, "sourceId", request.SourceId?.ToString());
        Add(parameters, "format", request.Format);
        Add(parameters, "status", request.Status);
        Add(parameters, "pageNumber", request.PageNumber.ToString());
        Add(parameters, "pageSize", request.PageSize.ToString());
        Add(parameters, "sortBy", request.SortBy);
        Add(parameters, "sortDirection", request.SortDirection);
        return parameters.Count == 0 ? "api/v1/publish-jobs" : $"api/v1/publish-jobs?{string.Join("&", parameters)}";
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
