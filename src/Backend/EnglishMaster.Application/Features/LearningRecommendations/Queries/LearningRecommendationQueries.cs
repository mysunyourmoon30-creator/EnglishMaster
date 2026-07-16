using EnglishMaster.Application.Features.LearningRecommendations.Dtos;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.LearningRecommendations.Queries;

public sealed record GetMyContinueLearningQuery(Guid UserId, int? Limit);
public sealed record GetMyLearningRecommendationsQuery(Guid UserId, int? Limit, string? CefrLevel);
public sealed record GetMyRecommendedQuery(Guid UserId, int? Limit, string? CefrLevel);
public sealed record GetNextLessonForCourseQuery(Guid UserId, Guid CourseId);

public sealed class LearningRecommendationQueryHandler
{
    private const int MaximumLimit = 20;
    private readonly ILearningRecommendationRepository repository;

    public LearningRecommendationQueryHandler(ILearningRecommendationRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<IReadOnlyCollection<ContinueLearningItemDto>>> GetContinueLearningAsync(GetMyContinueLearningQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<ContinueLearningItemDto>>.Success(await repository.GetContinueLearningAsync(query.UserId, Limit(query.Limit), cancellationToken));

    public async Task<Result<LearningRecommendationSummaryDto>> GetSummaryAsync(GetMyLearningRecommendationsQuery query, CancellationToken cancellationToken) =>
        Result<LearningRecommendationSummaryDto>.Success(await repository.GetSummaryAsync(query.UserId, Limit(query.Limit), query.CefrLevel, cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningRecommendationDto>>> GetCoursesAsync(GetMyRecommendedQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningRecommendationDto>>.Success(await repository.GetRecommendedCoursesAsync(query.UserId, Limit(query.Limit), query.CefrLevel, cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningRecommendationDto>>> GetLessonsAsync(GetMyRecommendedQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningRecommendationDto>>.Success(await repository.GetRecommendedLessonsAsync(query.UserId, Limit(query.Limit), query.CefrLevel, cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningRecommendationDto>>> GetWordsAsync(GetMyRecommendedQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningRecommendationDto>>.Success(await repository.GetRecommendedWordsAsync(query.UserId, Limit(query.Limit), query.CefrLevel, cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningRecommendationDto>>> GetGrammarAsync(GetMyRecommendedQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningRecommendationDto>>.Success(await repository.GetRecommendedGrammarAsync(query.UserId, Limit(query.Limit), query.CefrLevel, cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningRecommendationDto>>> GetQuizzesAsync(GetMyRecommendedQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningRecommendationDto>>.Success(await repository.GetRecommendedQuizzesAsync(query.UserId, Limit(query.Limit), query.CefrLevel, cancellationToken));

    public async Task<Result<IReadOnlyCollection<LearningRecommendationDto>>> GetReviewAsync(GetMyRecommendedQuery query, CancellationToken cancellationToken) =>
        Result<IReadOnlyCollection<LearningRecommendationDto>>.Success(await repository.GetReviewRecommendationsAsync(query.UserId, Limit(query.Limit), cancellationToken));

    public async Task<Result<LearningRecommendationDto>> GetNextLessonForCourseAsync(GetNextLessonForCourseQuery query, CancellationToken cancellationToken)
    {
        var result = await repository.GetNextLessonForCourseAsync(query.UserId, query.CourseId, cancellationToken);
        return result is null ? Result<LearningRecommendationDto>.NotFound(nameof(query.CourseId), "Next lesson was not found.") : Result<LearningRecommendationDto>.Success(result);
    }

    private static int Limit(int? value) =>
        Math.Clamp(value ?? 5, 1, MaximumLimit);
}

