using System.Net.Http.Json;
using EnglishMaster.Contracts.StudentProgress;

namespace EnglishMaster.Web.Services.StudentProgress;

public sealed class StudentProgressApiClient : IStudentProgressApiClient
{
    private readonly HttpClient httpClient;

    public StudentProgressApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<StudentProgressSummaryDto> GetSummaryAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/me/progress?limit={limit}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<StudentProgressSummaryDto>(cancellationToken: cancellationToken)
            ?? new StudentProgressSummaryDto(0, 0, 0, [], [], []);
    }
}
