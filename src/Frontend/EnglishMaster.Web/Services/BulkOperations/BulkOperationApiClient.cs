using System.Net.Http.Json;
using EnglishMaster.Contracts.BulkOperations;

namespace EnglishMaster.Web.Services.BulkOperations;

public sealed class BulkOperationApiClient : IBulkOperationApiClient
{
    private readonly HttpClient httpClient;

    public BulkOperationApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<BulkOperationSearchResponse> SearchAsync(string? operationType, string? contentType, string? status, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/bulk-operations", ("operationType", operationType), ("contentType", contentType), ("status", status)), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<BulkOperationSearchResponse>(cancellationToken: cancellationToken) ??
            new BulkOperationSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<BulkOperationDto> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/bulk-operations/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<BulkOperationDto>(response, cancellationToken);
    }

    public async Task<IReadOnlyCollection<BulkOperationItemDto>> GetItemsAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/bulk-operations/{id}/items", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<BulkOperationItemDto>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<BulkOperationDto> CreateAsync(CreateBulkOperationRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/bulk-operations", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<BulkOperationDto>(response, cancellationToken);
    }

    public async Task<BulkOperationDto> RunAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/bulk-operations/{id}/run", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<BulkOperationDto>(response, cancellationToken);
    }

    public async Task<BulkOperationDto> CancelAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/bulk-operations/{id}/cancel", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<BulkOperationDto>(response, cancellationToken);
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
