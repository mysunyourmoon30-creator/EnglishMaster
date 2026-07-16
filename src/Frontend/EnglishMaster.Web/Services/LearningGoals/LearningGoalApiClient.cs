using System.Net.Http.Json;
using EnglishMaster.Contracts.LearningGoals;

namespace EnglishMaster.Web.Services.LearningGoals;

public sealed class LearningGoalApiClient : ILearningGoalApiClient
{
    private readonly HttpClient httpClient;

    public LearningGoalApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<LearningGoalDto>> GetGoalsAsync(CancellationToken cancellationToken) =>
        await GetCollectionAsync("api/v1/me/learning-goals", cancellationToken);

    public async Task<IReadOnlyCollection<LearningGoalDto>> GetActiveGoalsAsync(CancellationToken cancellationToken) =>
        await GetCollectionAsync("api/v1/me/learning-goals/active", cancellationToken);

    public async Task<LearningGoalSummaryDto> GetSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/learning-goals/summary", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<LearningGoalSummaryDto>(cancellationToken: cancellationToken) ?? new(0, 0, 0, 0);
    }

    public async Task<LearningGoalDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/me/learning-goals/{id}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<LearningGoalDto>(cancellationToken: cancellationToken))!;
    }

    public async Task<LearningGoalDto> CreateAsync(CreateLearningGoalRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("api/v1/me/learning-goals", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<LearningGoalDto>(cancellationToken: cancellationToken))!;
    }

    public async Task<LearningGoalDto> UpdateAsync(Guid id, UpdateLearningGoalRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PutAsJsonAsync($"api/v1/me/learning-goals/{id}", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<LearningGoalDto>(cancellationToken: cancellationToken))!;
    }

    public Task<LearningGoalDto> PauseAsync(Guid id, CancellationToken cancellationToken) => PostLifecycleAsync(id, "pause", cancellationToken);
    public Task<LearningGoalDto> ResumeAsync(Guid id, CancellationToken cancellationToken) => PostLifecycleAsync(id, "resume", cancellationToken);
    public Task<LearningGoalDto> CompleteAsync(Guid id, CancellationToken cancellationToken) => PostLifecycleAsync(id, "complete", cancellationToken);
    public Task<LearningGoalDto> CancelAsync(Guid id, CancellationToken cancellationToken) => PostLifecycleAsync(id, "cancel", cancellationToken);

    private async Task<LearningGoalDto> PostLifecycleAsync(Guid id, string action, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/me/learning-goals/{id}/{action}", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<LearningGoalDto>(cancellationToken: cancellationToken))!;
    }

    private async Task<IReadOnlyCollection<LearningGoalDto>> GetCollectionAsync(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(path, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<LearningGoalDto>>(cancellationToken: cancellationToken) ?? [];
    }
}
