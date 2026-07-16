using System.Net.Http.Json;
using EnglishMaster.Contracts.Achievements;
using EnglishMaster.Contracts.Motivation;

namespace EnglishMaster.Web.Services.Motivation;

public sealed class MotivationApiClient : IMotivationApiClient
{
    private readonly HttpClient httpClient;

    public MotivationApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<MotivationSummaryDto> GetSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/motivation/summary", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<MotivationSummaryDto>(cancellationToken: cancellationToken) ?? new(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, [], []);
    }

    public Task<IReadOnlyCollection<LearningActivityDto>> GetActivityAsync(CancellationToken cancellationToken) =>
        GetCollectionAsync<LearningActivityDto>("api/v1/me/motivation/activity", cancellationToken);

    public async Task<StudentStreakDto> GetStreakAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/streak", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<StudentStreakDto>(cancellationToken: cancellationToken) ?? new(0, 0, null, null);
    }

    public Task<IReadOnlyCollection<StudentAchievementDto>> GetAchievementsAsync(CancellationToken cancellationToken) =>
        GetCollectionAsync<StudentAchievementDto>("api/v1/me/achievements", cancellationToken);

    public Task<IReadOnlyCollection<StudentAchievementDto>> GetEarnedAchievementsAsync(CancellationToken cancellationToken) =>
        GetCollectionAsync<StudentAchievementDto>("api/v1/me/achievements/earned", cancellationToken);

    public async Task<IReadOnlyCollection<StudentAchievementDto>> EvaluateAchievementsAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync("api/v1/me/achievements/evaluate", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<StudentAchievementDto>>(cancellationToken: cancellationToken) ?? [];
    }

    private async Task<IReadOnlyCollection<T>> GetCollectionAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(path, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<T>>(cancellationToken: cancellationToken) ?? [];
    }
}
