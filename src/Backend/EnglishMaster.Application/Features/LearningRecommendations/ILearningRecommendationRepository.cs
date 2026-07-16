using EnglishMaster.Application.Features.LearningRecommendations.Dtos;

namespace EnglishMaster.Application.Features.LearningRecommendations;

public interface ILearningRecommendationRepository
{
    Task<IReadOnlyCollection<ContinueLearningItemDto>> GetContinueLearningAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<LearningRecommendationSummaryDto> GetSummaryAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedCoursesAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedLessonsAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedWordsAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedGrammarAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedQuizzesAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<LearningRecommendationDto>> GetReviewRecommendationsAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<LearningRecommendationDto?> GetNextLessonForCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken);
}

