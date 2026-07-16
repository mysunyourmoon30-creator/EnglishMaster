using System.Security.Claims;
using EnglishMaster.Application.Features.Motivation.Commands;
using EnglishMaster.Application.Features.Motivation.Queries;
using EnglishMaster.Contracts.Motivation;
using EnglishMaster.Shared.Results;
using AppActivityDto = EnglishMaster.Application.Features.Motivation.Dtos.LearningActivityDto;
using AppSummaryDto = EnglishMaster.Application.Features.Motivation.Dtos.MotivationSummaryDto;
using AppStreakDto = EnglishMaster.Application.Features.Motivation.Dtos.StudentStreakDto;

namespace EnglishMaster.Api.Endpoints;

public static class MotivationEndpoints
{
    public static IEndpointRouteBuilder MapMotivationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var motivation = endpoints.MapGroup("/api/v1/me/motivation").WithTags("Motivation").RequireAuthorization();
        motivation.MapGet("/summary", SummaryAsync);
        motivation.MapGet("/activity", ActivityAsync);
        motivation.MapGet("/activity/recent", RecentActivityAsync);
        motivation.MapPost("/activity", RecordActivityAsync);

        var streak = endpoints.MapGroup("/api/v1/me/streak").WithTags("Streak").RequireAuthorization();
        streak.MapGet("/", StreakAsync);
        streak.MapPost("/update", UpdateStreakAsync);
        return endpoints;
    }

    private static async Task<IResult> SummaryAsync(ClaimsPrincipal user, MotivationQueryHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetSummaryAsync(new GetMyMotivationSummaryQuery(userId), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> ActivityAsync(ClaimsPrincipal user, MotivationQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetActivityAsync(new GetMyLearningActivityQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> RecentActivityAsync(ClaimsPrincipal user, MotivationQueryHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetActivityAsync(new GetMyLearningActivityQuery(userId, 10), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> RecordActivityAsync(ClaimsPrincipal user, MotivationCommandHandler handler, RecordLearningActivityRequest request, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.RecordActivityAsync(new RecordLearningActivityCommand(userId, request.ActivityType, request.ContentType, request.ContentId, request.Title, request.OccurredAt, request.MinutesSpent, request.MetadataJson), cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.ValidationError => Results.ValidationProblem(result.Errors.GroupBy(error => error.Field).ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray())),
            _ => Results.Problem()
        };
    }

    private static async Task<IResult> StreakAsync(ClaimsPrincipal user, MotivationQueryHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetStreakAsync(new GetMyStreakQuery(userId), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> UpdateStreakAsync(ClaimsPrincipal user, MotivationCommandHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.UpdateStreakAsync(new UpdateMyStreakCommand(userId), cancellationToken)) : Results.Unauthorized();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppActivityDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<AppStreakDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(new StudentStreakDto(result.Value!.CurrentStreakDays, result.Value.LongestStreakDays, result.Value.LastActivityDate, result.Value.StreakStartDate)) : Results.Problem();

    private static IResult ToHttpResult(Result<AppSummaryDto> result) =>
        result.Status == ResultStatus.Success
            ? Results.Ok(new MotivationSummaryDto(
                result.Value!.CurrentStreakDays,
                result.Value.LongestStreakDays,
                result.Value.TotalLessonsCompleted,
                result.Value.TotalCoursesCompleted,
                result.Value.TotalBooksCompleted,
                result.Value.TotalQuizAttempts,
                result.Value.TotalQuizPassed,
                result.Value.TotalPracticeSessionsCompleted,
                result.Value.TotalDailyPlansCompleted,
                result.Value.TotalGoalsCompleted,
                result.Value.EarnedAchievementCount,
                result.Value.RecentAchievements.Select(AchievementEndpoints.ToContract).ToArray(),
                result.Value.RecentActivity.Select(ToContract).ToArray()))
            : Results.Problem();

    private static LearningActivityDto ToContract(AppActivityDto activity) =>
        new(activity.Id, activity.ActivityType, activity.ContentType, activity.ContentId, activity.Title, activity.OccurredAt, activity.MinutesSpent, activity.MetadataJson);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
