using System.Security.Claims;
using EnglishMaster.Application.Features.ContentQuality.Commands;
using EnglishMaster.Application.Features.ContentQuality.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.ContentQuality;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class ContentQualityEndpoints
{
    public static IEndpointRouteBuilder MapContentQualityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/content-quality")
            .WithTags("Content Quality");

        group.MapGet("/dashboard", GetDashboardAsync)
            .RequireAuthorization(Permissions.ContentQualityRead);
        group.MapGet("/rules", SearchRulesAsync)
            .RequireAuthorization(Permissions.ContentQualityRead);
        group.MapPost("/rules", CreateRuleAsync)
            .RequireAuthorization(Permissions.ContentQualityManage);
        group.MapPut("/rules/{id:guid}", UpdateRuleAsync)
            .RequireAuthorization(Permissions.ContentQualityManage);
        group.MapPost("/rules/{id:guid}/activate", ActivateRuleAsync)
            .RequireAuthorization(Permissions.ContentQualityManage);
        group.MapPost("/rules/{id:guid}/deactivate", DeactivateRuleAsync)
            .RequireAuthorization(Permissions.ContentQualityManage);

        group.MapPost("/checks/run", RunAsync)
            .RequireAuthorization(Permissions.ContentQualityRun);
        group.MapPost("/checks/{contentType}/{contentId:guid}/run", RunForContentAsync)
            .RequireAuthorization(Permissions.ContentQualityRun);
        group.MapGet("/checks", SearchChecksAsync)
            .RequireAuthorization(Permissions.ContentQualityRead);
        group.MapGet("/checks/{id:guid}", GetCheckAsync)
            .RequireAuthorization(Permissions.ContentQualityRead);
        group.MapGet("/{contentType}/{contentId:guid}/latest", GetLatestCheckAsync)
            .RequireAuthorization(Permissions.ContentQualityRead);
        group.MapGet("/checks/{id:guid}/findings", GetFindingsAsync)
            .RequireAuthorization(Permissions.ContentQualityRead);
        group.MapPost("/findings/{id:guid}/resolve", ResolveFindingAsync)
            .RequireAuthorization(Permissions.ContentQualityRun);

        return endpoints;
    }

    private static async Task<IResult> SearchRulesAsync(
        ContentQualityQueryHandler handler,
        string? contentType,
        string? severity,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchRulesAsync(new SearchContentQualityRulesQuery(contentType, severity, isActive, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> CreateRuleAsync(
        CreateContentQualityRuleRequest request,
        ContentQualityCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.CreateRuleAsync(new CreateContentQualityRuleCommand(request.Code, request.Name, request.Description, request.ContentType, request.Severity), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/content-quality/rules/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateRuleAsync(
        Guid id,
        UpdateContentQualityRuleRequest request,
        ContentQualityCommandHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.UpdateRuleAsync(new UpdateContentQualityRuleCommand(id, request.Code, request.Name, request.Description, request.ContentType, request.Severity), cancellationToken));

    private static async Task<IResult> ActivateRuleAsync(Guid id, ContentQualityCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.ActivateRuleAsync(new ActivateContentQualityRuleCommand(id), cancellationToken));

    private static async Task<IResult> DeactivateRuleAsync(Guid id, ContentQualityCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.DeactivateRuleAsync(new DeactivateContentQualityRuleCommand(id), cancellationToken));

    private static async Task<IResult> RunAsync(
        RunContentQualityCheckRequest request,
        ClaimsPrincipal user,
        ContentQualityCommandHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RunAsync(new RunContentQualityCheckCommand(request.ContentType, request.ContentId, user.Identity?.Name), cancellationToken));

    private static async Task<IResult> RunForContentAsync(
        string contentType,
        Guid contentId,
        ClaimsPrincipal user,
        ContentQualityCommandHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RunAsync(new RunContentQualityCheckCommand(contentType, contentId, user.Identity?.Name), cancellationToken));

    private static async Task<IResult> SearchChecksAsync(
        ContentQualityQueryHandler handler,
        string? contentType,
        string? status,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchChecksAsync(new SearchContentQualityChecksQuery(contentType, status, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetCheckAsync(Guid id, ContentQualityQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetCheckAsync(new GetContentQualityCheckByIdQuery(id), cancellationToken));

    private static async Task<IResult> GetLatestCheckAsync(string contentType, Guid contentId, ContentQualityQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetLatestCheckAsync(new GetLatestContentQualityCheckQuery(NormalizeContentType(contentType), contentId), cancellationToken));

    private static async Task<IResult> GetFindingsAsync(Guid id, bool? isResolved, ContentQualityQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetFindingsAsync(new GetContentQualityFindingsQuery(id, isResolved), cancellationToken));

    private static async Task<IResult> ResolveFindingAsync(Guid id, ContentQualityCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.MarkFindingResolvedAsync(new MarkContentQualityFindingResolvedCommand(id), cancellationToken));

    private static async Task<IResult> GetDashboardAsync(ContentQualityQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetDashboardAsync(cancellationToken));

    private static IResult ToHttpResult<T>(Result<T> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors) =>
        errors
            .GroupBy(error => error.Field)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());

    private static string NormalizeContentType(string value) =>
        value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
