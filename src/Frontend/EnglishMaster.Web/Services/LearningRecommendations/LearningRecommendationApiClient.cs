using System.Net.Http.Json;
using EnglishMaster.Contracts.LearningRecommendations;

namespace EnglishMaster.Web.Services.LearningRecommendations;

public sealed class LearningRecommendationApiClient : ILearningRecommendationApiClient
{
    private readonly HttpClient httpClient;

    public LearningRecommendationApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<ContinueLearningItemDto>> GetContinueLearningAsync(int limit, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/me/learning/continue?limit={limit}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<IReadOnlyCollection<ContinueLearningItemDto>>(cancellationToken: cancellationToken) ?? [];
    }

    public async Task<LearningRecommendationSummaryDto> GetRecommendationsAsync(int limit, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"api/v1/me/learning/recommendations?limit={limit}", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);
        return await response.Content.ReadFromJsonAsync<LearningRecommendationSummaryDto>(cancellationToken: cancellationToken)
            ?? new LearningRecommendationSummaryDto([], [], [], [], [], [], []);
    }
}

