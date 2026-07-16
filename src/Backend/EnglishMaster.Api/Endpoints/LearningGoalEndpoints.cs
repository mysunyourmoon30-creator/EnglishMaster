using System.Security.Claims;
using EnglishMaster.Application.Features.LearningGoals.Commands;
using EnglishMaster.Application.Features.LearningGoals.Queries;
using EnglishMaster.Contracts.LearningGoals;
using EnglishMaster.Shared.Results;
using AppGoalDto = EnglishMaster.Application.Features.LearningGoals.Dtos.LearningGoalDto;
using AppGoalSummaryDto = EnglishMaster.Application.Features.LearningGoals.Dtos.LearningGoalSummaryDto;

namespace EnglishMaster.Api.Endpoints;

public static class LearningGoalEndpoints
{
    public static IEndpointRouteBuilder MapLearningGoalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me/learning-goals").WithTags("Learning Goals").RequireAuthorization();
        group.MapGet("/", GetGoalsAsync);
        group.MapGet("/active", GetActiveAsync);
        group.MapGet("/summary", GetSummaryAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapPost("/", CreateAsync);
        group.MapPut("/{id:guid}", UpdateAsync);
        group.MapPost("/{id:guid}/pause", PauseAsync);
        group.MapPost("/{id:guid}/resume", ResumeAsync);
        group.MapPost("/{id:guid}/complete", CompleteAsync);
        group.MapPost("/{id:guid}/cancel", CancelAsync);
        return endpoints;
    }

    private static async Task<IResult> GetGoalsAsync(ClaimsPrincipal user, LearningGoalQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetGoalsAsync(new GetMyLearningGoalsQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetActiveAsync(ClaimsPrincipal user, LearningGoalQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetActiveGoalsAsync(new GetMyActiveLearningGoalsQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetSummaryAsync(ClaimsPrincipal user, LearningGoalQueryHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetSummaryAsync(new GetLearningGoalSummaryQuery(userId), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetByIdAsync(ClaimsPrincipal user, LearningGoalQueryHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetByIdAsync(new GetLearningGoalByIdQuery(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> CreateAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, CreateLearningGoalRequest request, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.CreateAsync(new CreateLearningGoalCommand(userId, request.GoalType, request.Title, request.Description, request.TargetValue, request.Unit, request.TargetDate), cancellationToken);
        return ToMutationResult(result);
    }

    private static async Task<IResult> UpdateAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, Guid id, UpdateLearningGoalRequest request, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.UpdateAsync(new UpdateLearningGoalCommand(userId, id, request.Title, request.Description, request.TargetValue, request.CurrentValue, request.Unit, request.TargetDate), cancellationToken);
        return ToMutationResult(result);
    }

    private static async Task<IResult> PauseAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await LifecycleAsync(user, handler, id, "pause", cancellationToken);

    private static async Task<IResult> ResumeAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await LifecycleAsync(user, handler, id, "resume", cancellationToken);

    private static async Task<IResult> CompleteAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await LifecycleAsync(user, handler, id, "complete", cancellationToken);

    private static async Task<IResult> CancelAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await LifecycleAsync(user, handler, id, "cancel", cancellationToken);

    private static async Task<IResult> LifecycleAsync(ClaimsPrincipal user, LearningGoalCommandHandler handler, Guid id, string action, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new LearningGoalLifecycleCommand(userId, id);
        var result = action switch
        {
            "pause" => await handler.PauseAsync(command, cancellationToken),
            "resume" => await handler.ResumeAsync(command, cancellationToken),
            "complete" => await handler.CompleteAsync(command, cancellationToken),
            _ => await handler.CancelAsync(command, cancellationToken)
        };
        return ToMutationResult(result);
    }

    private static IResult ToMutationResult(Result<AppGoalDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.ValidationError => Results.ValidationProblem(result.Errors.GroupBy(error => error.Field).ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray())),
            _ => Results.NotFound()
        };

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppGoalDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<AppGoalSummaryDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(new LearningGoalSummaryDto(result.Value!.ActiveCount, result.Value.CompletedCount, result.Value.PausedCount, result.Value.CancelledCount)) : Results.Problem();

    private static LearningGoalDto ToContract(AppGoalDto goal) =>
        new(goal.Id, goal.GoalType, goal.Title, goal.Description, goal.TargetValue, goal.CurrentValue, goal.Unit, goal.TargetDate, goal.Status, goal.CreatedAt, goal.UpdatedAt);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
