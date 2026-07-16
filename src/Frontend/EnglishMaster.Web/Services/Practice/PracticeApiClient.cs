using System.Net.Http.Json;
using EnglishMaster.Contracts.Practice;

namespace EnglishMaster.Web.Services.Practice;

public sealed class PracticeApiClient : IPracticeApiClient
{
    private readonly HttpClient httpClient;

    public PracticeApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<PracticeSummaryDto> GetSummaryAsync(CancellationToken cancellationToken) =>
        await GetAsync<PracticeSummaryDto>("api/v1/me/practice/summary", cancellationToken) ?? new(0, 0, 0, 0);

    public async Task<IReadOnlyCollection<PracticeItemDto>> GetDueAsync(CancellationToken cancellationToken) =>
        await GetAsync<IReadOnlyCollection<PracticeItemDto>>("api/v1/me/practice/due", cancellationToken) ?? [];

    public async Task<GeneratePracticeItemsResponse> GenerateAsync(CancellationToken cancellationToken) =>
        await PostAsync<GeneratePracticeItemsResponse>("api/v1/me/practice/generate", null, cancellationToken) ?? new(0);

    public async Task<PracticeSessionDto> StartSessionAsync(CancellationToken cancellationToken) =>
        await PostAsync<PracticeSessionDto>("api/v1/me/practice/sessions/start", null, cancellationToken) ?? new(Guid.Empty, DateTimeOffset.UtcNow, null, "Started", 0, 0, 0, 0, []);

    public async Task<PracticeSessionDto> GetSessionAsync(Guid id, CancellationToken cancellationToken) =>
        await GetAsync<PracticeSessionDto>($"api/v1/me/practice/sessions/{id}", cancellationToken) ?? new(Guid.Empty, DateTimeOffset.UtcNow, null, "Started", 0, 0, 0, 0, []);

    public async Task<PracticeSessionItemDto> SubmitAsync(Guid itemId, SubmitPracticeSessionItemRequest request, CancellationToken cancellationToken) =>
        await PostAsync<PracticeSessionItemDto>($"api/v1/me/practice/session-items/{itemId}/submit", request, cancellationToken) ?? new(Guid.Empty, Guid.Empty, Guid.Empty, string.Empty, Guid.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, null, null);

    public async Task<PracticeSessionDto> CompleteAsync(Guid id, CancellationToken cancellationToken) =>
        await PostAsync<PracticeSessionDto>($"api/v1/me/practice/sessions/{id}/complete", null, cancellationToken) ?? new(Guid.Empty, DateTimeOffset.UtcNow, null, "Started", 0, 0, 0, 0, []);

    public async Task<IReadOnlyCollection<PracticeSessionDto>> GetHistoryAsync(CancellationToken cancellationToken) =>
        await GetAsync<IReadOnlyCollection<PracticeSessionDto>>("api/v1/me/practice/history", cancellationToken) ?? [];

    private async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(path, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    private async Task<T?> PostAsync<T>(string path, object? request, CancellationToken cancellationToken)
    {
        var response = request is null
            ? await httpClient.PostAsync(path, null, cancellationToken)
            : await httpClient.PostAsJsonAsync(path, request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }
}
