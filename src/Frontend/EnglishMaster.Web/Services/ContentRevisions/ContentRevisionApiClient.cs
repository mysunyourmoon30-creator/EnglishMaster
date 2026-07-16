using System.Net.Http.Json;
using EnglishMaster.Contracts.ContentRevisions;

namespace EnglishMaster.Web.Services.ContentRevisions;

public sealed class ContentRevisionApiClient : IContentRevisionApiClient
{
    private readonly HttpClient httpClient;

    public ContentRevisionApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ContentRevisionSearchResponse> SearchRevisionsAsync(string? contentType, Guid? contentId, string? eventType, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/content-revisions", ("contentType", contentType), ("contentId", contentId?.ToString()), ("eventType", eventType)), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ContentRevisionSearchResponse>(cancellationToken: cancellationToken) ??
            new ContentRevisionSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<ContentRevisionDto> GetRevisionAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/content-revisions/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionDto>(response, cancellationToken);
    }

    public async Task<ContentRevisionSearchResponse> GetRevisionsForContentAsync(string contentType, Guid contentId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/content-revisions/{Uri.EscapeDataString(contentType)}/{contentId}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionSearchResponse>(response, cancellationToken);
    }

    public async Task<ContentRevisionRestoreRequestSearchResponse> SearchRestoreRequestsAsync(string? status, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/content-revision-restore-requests", ("status", status)), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ContentRevisionRestoreRequestSearchResponse>(cancellationToken: cancellationToken) ??
            new ContentRevisionRestoreRequestSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<ContentRevisionRestoreRequestDto> GetRestoreRequestAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/content-revision-restore-requests/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionRestoreRequestDto>(response, cancellationToken);
    }

    public async Task<ContentRevisionRestoreRequestDto> CreateRestoreRequestAsync(CreateContentRevisionRestoreRequestRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/content-revision-restore-requests", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionRestoreRequestDto>(response, cancellationToken);
    }

    public async Task<ContentRevisionRestoreRequestDto> ApproveRestoreRequestAsync(Guid id, string? reviewNote, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"api/v1/content-revision-restore-requests/{id}/approve", new ReviewContentRevisionRestoreRequestRequest(reviewNote), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionRestoreRequestDto>(response, cancellationToken);
    }

    public async Task<ContentRevisionRestoreRequestDto> RejectRestoreRequestAsync(Guid id, string? reviewNote, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"api/v1/content-revision-restore-requests/{id}/reject", new ReviewContentRevisionRestoreRequestRequest(reviewNote), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionRestoreRequestDto>(response, cancellationToken);
    }

    public async Task<ContentRevisionRestoreRequestDto> CompleteRestoreRequestAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/content-revision-restore-requests/{id}/complete", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentRevisionRestoreRequestDto>(response, cancellationToken);
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
