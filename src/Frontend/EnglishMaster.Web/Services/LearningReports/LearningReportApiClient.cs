using System.Net;
using System.Net.Http.Json;
using System.Globalization;
using EnglishMaster.Contracts.LearningReports;

namespace EnglishMaster.Web.Services.LearningReports;

public sealed class LearningReportApiClient : ILearningReportApiClient
{
    private readonly HttpClient httpClient;

    public LearningReportApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<WeeklyLearningReportDto?> GetCurrentWeekAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/me/learning-reports/current-week", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WeeklyLearningReportDto>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<WeeklyLearningReportDto>> GetHistoryAsync(DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken)
    {
        var query = new List<string> { "pageNumber=1", "pageSize=20" };
        if (fromDate is not null)
        {
            query.Add($"fromDate={Uri.EscapeDataString(fromDate.Value.ToString("O"))}");
        }

        if (toDate is not null)
        {
            query.Add($"toDate={Uri.EscapeDataString(toDate.Value.ToString("O"))}");
        }

        return await GetAsync<IReadOnlyCollection<WeeklyLearningReportDto>>($"api/v1/me/learning-reports?{string.Join('&', query)}", cancellationToken) ?? [];
    }

    public async Task<WeeklyLearningReportDto> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await GetAsync<WeeklyLearningReportDto>($"api/v1/me/learning-reports/{id}", cancellationToken) ?? EmptyReport(id);

    public async Task<WeeklyLearningReportDto?> GetByDateAsync(DateTimeOffset date, CancellationToken cancellationToken)
    {
        var reportDate = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        var response = await httpClient.GetAsync($"api/v1/me/learning-reports/by-date/{Uri.EscapeDataString(reportDate)}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<WeeklyLearningReportDto>(cancellationToken: cancellationToken);
    }

    public async Task<WeeklyLearningReportDto> GenerateCurrentWeekAsync(CancellationToken cancellationToken) =>
        await PostAsync<WeeklyLearningReportDto>("api/v1/me/learning-reports/current-week/generate", cancellationToken) ?? EmptyReport(Guid.Empty);

    public async Task<WeeklyLearningReportDto> RegenerateAsync(Guid id, CancellationToken cancellationToken) =>
        await PostAsync<WeeklyLearningReportDto>($"api/v1/me/learning-reports/{id}/regenerate", cancellationToken) ?? EmptyReport(id);

    public async Task<IReadOnlyCollection<WeeklyLearningReportInsightDto>> GetInsightsAsync(Guid id, CancellationToken cancellationToken) =>
        await GetAsync<IReadOnlyCollection<WeeklyLearningReportInsightDto>>($"api/v1/me/learning-reports/{id}/insights", cancellationToken) ?? [];

    private async Task<T?> GetAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync(path, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    private async Task<T?> PostAsync<T>(string path, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsync(path, null, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    private static WeeklyLearningReportDto EmptyReport(Guid id) =>
        new(id, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, string.Empty, DateTimeOffset.UtcNow, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, string.Empty, []);
}
