using System.Net.Http.Json;
using EnglishMaster.Contracts.Diagnostics;

namespace EnglishMaster.Web.Services.Diagnostics;

public sealed class DiagnosticApiClient(HttpClient httpClient) : IDiagnosticApiClient
{
    public async Task<DiagnosticQuizDto> GetQuizAsync(Guid quizId, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/me/diagnostics/quizzes/{quizId}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<DiagnosticQuizDto>(response, cancellationToken);
    }

    public async Task<DiagnosticResultDto> SubmitAsync(Guid quizId, SubmitDiagnosticRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync($"api/v1/me/diagnostics/quizzes/{quizId}/submit", request, cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await ApiClientResponseHandler.ReadRequiredAsync<DiagnosticResultDto>(response, cancellationToken);
    }
}
