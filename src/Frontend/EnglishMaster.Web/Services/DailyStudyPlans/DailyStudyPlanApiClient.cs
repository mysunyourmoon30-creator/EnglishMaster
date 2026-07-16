using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.DailyStudyPlans;

namespace EnglishMaster.Web.Services.DailyStudyPlans;

public sealed class DailyStudyPlanApiClient : IDailyStudyPlanApiClient
{
    private readonly HttpClient httpClient;

    public DailyStudyPlanApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<DailyStudyPlanDto?> GetTodayAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/study-plan/today", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<DailyStudyPlanDto>(cancellationToken: cancellationToken);
    }

    public async Task<DailyStudyPlanDto> GenerateTodayAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync("api/v1/me/study-plan/today/generate", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<DailyStudyPlanDto>(cancellationToken: cancellationToken))!;
    }

    public async Task<IReadOnlyCollection<DailyStudyPlanDto>> GetHistoryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/study-plan/history", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<DailyStudyPlanDto>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<DailyStudyPlanSummaryDto> GetSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/study-plan/summary", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<DailyStudyPlanSummaryDto>(cancellationToken: cancellationToken) ?? new(0, 0, 0, 0);
    }

    public Task<DailyStudyPlanItemDto> CompleteItemAsync(Guid id, CancellationToken cancellationToken) => ItemLifecycleAsync(id, "complete", cancellationToken);
    public Task<DailyStudyPlanItemDto> SkipItemAsync(Guid id, CancellationToken cancellationToken) => ItemLifecycleAsync(id, "skip", cancellationToken);

    public async Task<DailyStudyPlanDto> CompletePlanAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/me/study-plan/{id}/complete", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<DailyStudyPlanDto>(cancellationToken: cancellationToken))!;
    }

    private async Task<DailyStudyPlanItemDto> ItemLifecycleAsync(Guid id, string action, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync($"api/v1/me/study-plan/items/{id}/{action}", null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<DailyStudyPlanItemDto>(cancellationToken: cancellationToken))!;
    }
}
