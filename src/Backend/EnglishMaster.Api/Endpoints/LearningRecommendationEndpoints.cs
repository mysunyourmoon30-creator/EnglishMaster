using System.Security.Claims;
using EnglishMaster.Application.Features.LearningRecommendations.Queries;
using EnglishMaster.Contracts.LearningRecommendations;
using EnglishMaster.Shared.Results;
using AppContinueLearningItemDto = EnglishMaster.Application.Features.LearningRecommendations.Dtos.ContinueLearningItemDto;
using AppLearningRecommendationDto = EnglishMaster.Application.Features.LearningRecommendations.Dtos.LearningRecommendationDto;
using AppLearningRecommendationSummaryDto = EnglishMaster.Application.Features.LearningRecommendations.Dtos.LearningRecommendationSummaryDto;

namespace EnglishMaster.Api.Endpoints;

public static class LearningRecommendationEndpoints
{
    public static IEndpointRouteBuilder MapLearningRecommendationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me/learning")
            .WithTags("Learning Recommendations")
            .RequireAuthorization();

        group.MapGet("/continue", GetContinueAsync);
        group.MapGet("/recommendations", GetSummaryAsync);
        group.MapGet("/recommended-courses", GetCoursesAsync);
        group.MapGet("/recommended-lessons", GetLessonsAsync);
        group.MapGet("/recommended-words", GetWordsAsync);
        group.MapGet("/recommended-grammar", GetGrammarAsync);
        group.MapGet("/recommended-quizzes", GetQuizzesAsync);
        group.MapGet("/review", GetReviewAsync);
        group.MapGet("/courses/{courseId:guid}/next-lesson", GetNextLessonAsync);

        return endpoints;
    }

    private static async Task<IResult> GetContinueAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetContinueLearningAsync(new GetMyContinueLearningQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetSummaryAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, string? cefrLevel, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetSummaryAsync(new GetMyLearningRecommendationsQuery(userId, limit, cefrLevel), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetCoursesAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, string? cefrLevel, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetCoursesAsync(new GetMyRecommendedQuery(userId, limit, cefrLevel), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetLessonsAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, string? cefrLevel, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetLessonsAsync(new GetMyRecommendedQuery(userId, limit, cefrLevel), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetWordsAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, string? cefrLevel, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetWordsAsync(new GetMyRecommendedQuery(userId, limit, cefrLevel), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetGrammarAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, string? cefrLevel, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetGrammarAsync(new GetMyRecommendedQuery(userId, limit, cefrLevel), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetQuizzesAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, string? cefrLevel, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetQuizzesAsync(new GetMyRecommendedQuery(userId, limit, cefrLevel), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetReviewAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetReviewAsync(new GetMyRecommendedQuery(userId, limit, null), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetNextLessonAsync(ClaimsPrincipal user, LearningRecommendationQueryHandler handler, Guid courseId, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetNextLessonForCourseAsync(new GetNextLessonForCourseQuery(userId, courseId), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppContinueLearningItemDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppLearningRecommendationDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<AppLearningRecommendationSummaryDto> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(new LearningRecommendationSummaryDto(
                result.Value!.ContinueLearning.Select(ToContract).ToArray(),
                result.Value.RecommendedCourses.Select(ToContract).ToArray(),
                result.Value.RecommendedLessons.Select(ToContract).ToArray(),
                result.Value.RecommendedWords.Select(ToContract).ToArray(),
                result.Value.RecommendedGrammar.Select(ToContract).ToArray(),
                result.Value.RecommendedQuizzes.Select(ToContract).ToArray(),
                result.Value.ReviewRecommendations.Select(ToContract).ToArray()))
            : Results.Problem();

    private static ContinueLearningItemDto ToContract(AppContinueLearningItemDto item) =>
        new(item.ContentType, item.ContentId, item.Slug, item.Title, item.Summary, item.Url, item.ProgressPercent, item.Status, item.LastAccessedAt, item.RecommendationReason, item.SortOrder);

    private static LearningRecommendationDto ToContract(AppLearningRecommendationDto item) =>
        new(item.ContentType, item.ContentId, item.Slug, item.Title, item.Summary, item.Url, item.CefrLevel, item.CategoryName, item.Tags, item.RecommendationType, item.ReasonCode, item.ReasonText, item.Score, item.SortOrder);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
