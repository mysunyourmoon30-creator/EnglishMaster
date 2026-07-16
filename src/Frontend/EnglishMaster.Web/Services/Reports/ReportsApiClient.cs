using System.Net.Http.Json;
using EnglishMaster.Contracts.Reports;

namespace EnglishMaster.Web.Services.Reports;

public sealed class ReportsApiClient : IReportsApiClient
{
    private readonly HttpClient httpClient;

    public ReportsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<AdminDashboardSummaryDto> GetAdminDashboardSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/reports/admin-dashboard", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<AdminDashboardSummaryDto>(response, cancellationToken);
    }

    public async Task<ContentStatusSummaryDto> GetContentStatusSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/reports/content-status", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<ContentStatusSummaryDto>(response, cancellationToken);
    }

    public async Task<LearningProgressSummaryDto> GetLearningProgressSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/reports/learning-progress", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<LearningProgressSummaryDto>(response, cancellationToken);
    }

    public async Task<QuizAnalyticsSummaryDto> GetQuizAnalyticsSummaryAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/reports/quiz-analytics", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<QuizAnalyticsSummaryDto>(response, cancellationToken);
    }

    public async Task<RecentActivitySummaryDto> GetRecentActivitySummaryAsync(int pageSize, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/reports/recent-activity?pageSize={pageSize}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<RecentActivitySummaryDto>(
            cancellationToken: cancellationToken) ?? new RecentActivitySummaryDto([]);
    }
}
