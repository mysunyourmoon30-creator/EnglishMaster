using EnglishMaster.Contracts.Analytics;

namespace EnglishMaster.Web.Services.Analytics;

public sealed class AnalyticsApiClient : IAnalyticsApiClient
{
    private readonly HttpClient httpClient;

    public AnalyticsApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<AdminAnalyticsOverviewDto> GetAdminOverviewAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/admin/analytics/overview", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<AdminAnalyticsOverviewDto>(response, cancellationToken);
    }

    public async Task<StudentAnalyticsOverviewDto> GetStudentOverviewAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/analytics/overview", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<StudentAnalyticsOverviewDto>(response, cancellationToken);
    }
}
