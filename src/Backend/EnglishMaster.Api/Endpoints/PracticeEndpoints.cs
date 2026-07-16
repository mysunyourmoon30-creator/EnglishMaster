using System.Security.Claims;
using EnglishMaster.Application.Features.Practice.Commands;
using EnglishMaster.Application.Features.Practice.Queries;
using EnglishMaster.Contracts.Practice;
using EnglishMaster.Shared.Results;
using AppPracticeItemDto = EnglishMaster.Application.Features.Practice.Dtos.PracticeItemDto;
using AppPracticeSessionDto = EnglishMaster.Application.Features.Practice.Dtos.PracticeSessionDto;
using AppPracticeSessionItemDto = EnglishMaster.Application.Features.Practice.Dtos.PracticeSessionItemDto;
using AppPracticeSummaryDto = EnglishMaster.Application.Features.Practice.Dtos.PracticeSummaryDto;

namespace EnglishMaster.Api.Endpoints;

public static class PracticeEndpoints
{
    public static IEndpointRouteBuilder MapPracticeEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me/practice").WithTags("Practice").RequireAuthorization();
        group.MapGet("/summary", SummaryAsync);
        group.MapGet("/due", DueAsync);
        group.MapPost("/generate", GenerateAsync);
        group.MapPost("/sessions/start", StartSessionAsync);
        group.MapGet("/sessions/{id:guid}", GetSessionAsync);
        group.MapPost("/session-items/{id:guid}/submit", SubmitAsync);
        group.MapPost("/sessions/{id:guid}/complete", CompleteAsync);
        group.MapGet("/history", HistoryAsync);
        group.MapPost("/items/{id:guid}/suspend", SuspendAsync);
        group.MapPost("/items/{id:guid}/resume", ResumeAsync);
        return endpoints;
    }

    private static async Task<IResult> SummaryAsync(ClaimsPrincipal user, PracticeQueryHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetSummaryAsync(new GetPracticeSummaryQuery(userId), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> DueAsync(ClaimsPrincipal user, PracticeQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetDueAsync(new GetDuePracticeItemsQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GenerateAsync(ClaimsPrincipal user, PracticeCommandHandler handler, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GenerateAsync(new GeneratePracticeItemsCommand(userId), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(new GeneratePracticeItemsResponse(result.Value)) : Results.Problem();
    }

    private static async Task<IResult> StartSessionAsync(ClaimsPrincipal user, PracticeCommandHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.StartSessionAsync(new StartPracticeSessionCommand(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetSessionAsync(ClaimsPrincipal user, PracticeQueryHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetSessionAsync(new GetPracticeSessionByIdQuery(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> SubmitAsync(ClaimsPrincipal user, PracticeCommandHandler handler, Guid id, SubmitPracticeSessionItemRequest request, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.SubmitAsync(new SubmitPracticeSessionItemCommand(userId, id, request.UserAnswer, request.Result), cancellationToken);
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.ValidationError => Validation(result),
            _ => Results.NotFound()
        };
    }

    private static async Task<IResult> CompleteAsync(ClaimsPrincipal user, PracticeCommandHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.CompleteAsync(new CompletePracticeSessionCommand(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> HistoryAsync(ClaimsPrincipal user, PracticeQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetHistoryAsync(new GetPracticeHistoryQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> SuspendAsync(ClaimsPrincipal user, PracticeCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await ItemLifecycleAsync(user, handler, id, suspend: true, cancellationToken);

    private static async Task<IResult> ResumeAsync(ClaimsPrincipal user, PracticeCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        await ItemLifecycleAsync(user, handler, id, suspend: false, cancellationToken);

    private static async Task<IResult> ItemLifecycleAsync(ClaimsPrincipal user, PracticeCommandHandler handler, Guid id, bool suspend, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var command = new PracticeItemLifecycleCommand(userId, id);
        var result = suspend ? await handler.SuspendAsync(command, cancellationToken) : await handler.ResumeAsync(command, cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static IResult ToHttpResult(Result<AppPracticeSummaryDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(new PracticeSummaryDto(result.Value!.DueTodayCount, result.Value.NewCount, result.Value.ReviewingCount, result.Value.MasteredCount)) : Results.Problem();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppPracticeItemDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppPracticeSessionDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<AppPracticeSessionDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.Problem();

    private static IResult Validation(Result result) =>
        Results.ValidationProblem(result.Errors.GroupBy(error => error.Field).ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray()));

    private static PracticeItemDto ToContract(AppPracticeItemDto item) =>
        new(item.Id, item.ContentType, item.ContentId, item.PracticeType, item.Status, item.DueAt, item.LastPracticedAt, item.NextReviewAt, item.ReviewCount, item.CorrectCount, item.IncorrectCount, item.CurrentIntervalDays);

    private static PracticeSessionDto ToContract(AppPracticeSessionDto session) =>
        new(session.Id, session.StartedAt, session.CompletedAt, session.Status, session.TotalItems, session.CompletedItems, session.CorrectItems, session.IncorrectItems, session.Items.Select(ToContract).ToArray());

    private static PracticeSessionItemDto ToContract(AppPracticeSessionItemDto item) =>
        new(item.Id, item.PracticeSessionId, item.PracticeItemId, item.ContentType, item.ContentId, item.PracticeType, item.PromptText, item.AnswerText, item.UserAnswer, item.Result, item.IsCorrect, item.PracticedAt);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
