namespace EnglishMaster.Application.Features.LearningRecommendations.Dtos;

public sealed record ContinueLearningItemDto(
    string ContentType,
    Guid ContentId,
    string Slug,
    string Title,
    string Summary,
    string Url,
    int ProgressPercent,
    string Status,
    DateTimeOffset LastAccessedAt,
    string RecommendationReason,
    int SortOrder);

public sealed record LearningRecommendationDto(
    string ContentType,
    Guid ContentId,
    string Slug,
    string Title,
    string Summary,
    string Url,
    string? CefrLevel,
    string? CategoryName,
    IReadOnlyCollection<string> Tags,
    string RecommendationType,
    string ReasonCode,
    string ReasonText,
    decimal Score,
    int SortOrder);

public sealed record LearningRecommendationSummaryDto(
    IReadOnlyCollection<ContinueLearningItemDto> ContinueLearning,
    IReadOnlyCollection<LearningRecommendationDto> RecommendedCourses,
    IReadOnlyCollection<LearningRecommendationDto> RecommendedLessons,
    IReadOnlyCollection<LearningRecommendationDto> RecommendedWords,
    IReadOnlyCollection<LearningRecommendationDto> RecommendedGrammar,
    IReadOnlyCollection<LearningRecommendationDto> RecommendedQuizzes,
    IReadOnlyCollection<LearningRecommendationDto> ReviewRecommendations);

