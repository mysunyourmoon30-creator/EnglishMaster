using System.Net.Http.Json;
using EnglishMaster.Contracts.ImportJobs;

namespace EnglishMaster.Web.Services.ImportJobs;

public sealed class ImportJobApiClient : IImportJobApiClient
{
    private readonly HttpClient httpClient;

    public ImportJobApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<ImportJobSearchResponse> SearchAsync(string? importType, string? format, string? status, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(BuildEndpoint("api/v1/import-jobs", ("importType", importType), ("format", format), ("status", status)), cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ImportJobSearchResponse>(cancellationToken: cancellationToken) ?? new ImportJobSearchResponse([], 1, 20, 0, 0, false, false);
    }

    public async Task<ImportJobDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
        await ReadAsync<ImportJobDto>($"api/v1/import-jobs/{id}", cancellationToken);

    public async Task<IReadOnlyCollection<ImportJobRowDto>> GetRowsAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/import-jobs/{id}/rows", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ImportJobRowDto>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<IReadOnlyCollection<ImportValidationErrorDto>> GetErrorsAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/import-jobs/{id}/errors", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ImportValidationErrorDto>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<ImportJobDto> UploadAsync(UploadImportJobRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/import-jobs/upload", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ImportJobDto>(response, cancellationToken);
    }

    public Task<ImportJobDto> ValidateAsync(Guid id, CancellationToken cancellationToken) => PostAsync($"api/v1/import-jobs/{id}/validate", cancellationToken);
    public Task<ImportJobDto> ConfirmAsync(Guid id, CancellationToken cancellationToken) => PostAsync($"api/v1/import-jobs/{id}/confirm", cancellationToken);
    public Task<ImportJobDto> RunAsync(Guid id, CancellationToken cancellationToken) => PostAsync($"api/v1/import-jobs/{id}/run", cancellationToken);
    public Task<ImportJobDto> CancelAsync(Guid id, CancellationToken cancellationToken) => PostAsync($"api/v1/import-jobs/{id}/cancel", cancellationToken);
    public Task<ImportJobDto> RollbackAsync(Guid id, CancellationToken cancellationToken) => PostAsync($"api/v1/import-jobs/{id}/rollback", cancellationToken);

    private async Task<T> ReadAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(path, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<T>(response, cancellationToken);
    }

    private async Task<ImportJobDto> PostAsync(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(path, null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ImportJobDto>(response, cancellationToken);
    }

    private static string BuildEndpoint(string path, params (string Key, string? Value)[] parameters)
    {
        var query = parameters.Where(parameter => !string.IsNullOrWhiteSpace(parameter.Value)).Select(parameter => $"{parameter.Key}={Uri.EscapeDataString(parameter.Value!)}").ToArray();
        return query.Length == 0 ? path : $"{path}?{string.Join("&", query)}";
    }
}
