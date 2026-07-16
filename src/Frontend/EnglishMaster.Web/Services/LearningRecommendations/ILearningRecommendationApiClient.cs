using EnglishMaster.Contracts.LearningRecommendations;

namespace EnglishMaster.Web.Services.LearningRecommendations;

public interface ILearningRecommendationApiClient
{
    Task<IReadOnlyCollection<ContinueLearningItemDto>> GetContinueLearningAsync(int limit, CancellationToken cancellationToken);
    Task<LearningRecommendationSummaryDto> GetRecommendationsAsync(int limit, CancellationToken cancellationToken);
}

