using System.Security.Claims;
using EnglishMaster.Application.Features.DailyStudyPlans.Commands;
using EnglishMaster.Application.Features.DailyStudyPlans.Queries;
using EnglishMaster.Contracts.DailyStudyPlans;
using EnglishMaster.Shared.Results;
using AppPlanDto = EnglishMaster.Application.Features.DailyStudyPlans.Dtos.DailyStudyPlanDto;
using AppPlanItemDto = EnglishMaster.Application.Features.DailyStudyPlans.Dtos.DailyStudyPlanItemDto;
using AppPlanSummaryDto = EnglishMaster.Application.Features.DailyStudyPlans.Dtos.DailyStudyPlanSummaryDto;

namespace EnglishMaster.Api.Endpoints;

public static class DailyStudyPlanEndpoints
{
    public static IEndpointRouteBuilder MapDailyStudyPlanEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me/study-plan").WithTags("Daily Study Plan").RequireAuthorization();
        group.MapGet("/today", GetTodayAsync);
        group.MapPost("/today/generate", GenerateTodayAsync);
        group.MapGet("/date/{date}", GetByDateAsync);
        group.MapGet("/history", GetHistoryAsync);
        group.MapGet("/summary", GetSummaryAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapPost("/items/{id:guid}/complete", CompleteItemAsync);
        group.MapPost("/items/{id:guid}/skip", SkipItemAsync);
        group.MapPost("/{id:guid}/complete", CompletePlanAsync);
        return endpoints;
    }

    private static async Task<IResult> GetTodayAsync(ClaimsPrincipal user, DailyStudyPlanQueryHandler handler, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetTodayAsync(new GetTodayStudyPlanQuery(userId), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GenerateTodayAsync(ClaimsPrincipal user, DailyStudyPlanCommandHandler handler, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GenerateTodayAsync(new GenerateDailyStudyPlanCommand(userId), cancellationToken);
        return Results.Ok(ToContract(result.Value!));
    }

    private static async Task<IResult> GetByDateAsync(ClaimsPrincipal user, DailyStudyPlanQueryHandler handler, string date, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        if (!DateTimeOffset.TryParse(date, out var parsed))
        {
            return Results.BadRequest();
        }

        var result = await handler.GetByDateAsync(new GetStudyPlanByDateQuery(userId, parsed), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GetByIdAsync(ClaimsPrincipal user, DailyStudyPlanQueryHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetByIdAsync(new GetStudyPlanByIdQuery(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> CompleteItemAsync(ClaimsPrincipal user, DailyStudyPlanCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await ItemLifecycleAsync(user, handler, id, complete: true, cancellationToken);

    private static async Task<IResult> SkipItemAsync(ClaimsPrincipal user, DailyStudyPlanCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await ItemLifecycleAsync(user, handler, id, complete: false, cancellationToken);

    private static async Task<IResult> ItemLifecycleAsync(ClaimsPrincipal user, DailyStudyPlanCommandHandler handler, Guid id, bool complete, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new DailyStudyPlanItemLifecycleCommand(userId, id);
        var result = complete ? await handler.CompleteItemAsync(command, cancellationToken) : await handler.SkipItemAsync(command, cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> CompletePlanAsync(ClaimsPrincipal user, DailyStudyPlanCommandHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.CompletePlanAsync(new CompleteDailyStudyPlanCommand(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GetHistoryAsync(ClaimsPrincipal user, DailyStudyPlanQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetHistoryAsync(new GetMyStudyPlanHistoryQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetSummaryAsync(ClaimsPrincipal user, DailyStudyPlanQueryHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetSummaryAsync(new GetStudyPlanSummaryQuery(userId), cancellationToken)) : Results.Unauthorized();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppPlanDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<AppPlanSummaryDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(new DailyStudyPlanSummaryDto(result.Value!.PlansThisWeek, result.Value.CompletedPlans, result.Value.CompletedItems, result.Value.CompletedMinutes)) : Results.Problem();

    private static DailyStudyPlanDto ToContract(AppPlanDto plan) =>
        new(plan.Id, plan.PlanDate, plan.Status, plan.TargetMinutes, plan.CompletedMinutes, plan.TotalItems, plan.CompletedItems, plan.Items.Select(ToContract).ToArray());

    private static DailyStudyPlanItemDto ToContract(AppPlanItemDto item) =>
        new(item.Id, item.DailyStudyPlanId, item.ItemType, item.ContentType, item.ContentId, item.Title, item.Url, item.EstimatedMinutes, item.SortOrder, item.Status, item.CompletedAt);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
