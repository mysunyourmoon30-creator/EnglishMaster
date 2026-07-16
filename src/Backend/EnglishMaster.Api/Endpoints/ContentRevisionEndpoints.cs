using System.Security.Claims;
using EnglishMaster.Application.Features.ContentRevisionRestores.Commands;
using EnglishMaster.Application.Features.ContentRevisionRestores.Queries;
using EnglishMaster.Application.Features.ContentRevisions.Commands;
using EnglishMaster.Application.Features.ContentRevisions.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.ContentRevisions;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class ContentRevisionEndpoints
{
    public static IEndpointRouteBuilder MapContentRevisionEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var revisions = endpoints.MapGroup("/api/v1/content-revisions")
            .WithTags("Content Revisions");
        revisions.MapGet("", SearchRevisionsAsync).RequireAuthorization(Permissions.ContentRevisionsRead);
        revisions.MapPost("", CreateRevisionAsync).RequireAuthorization(Permissions.ContentRevisionsManage);
        revisions.MapGet("/{id:guid}", GetRevisionAsync).RequireAuthorization(Permissions.ContentRevisionsRead);
        revisions.MapGet("/{contentType}/{contentId:guid}", GetRevisionsForContentAsync).RequireAuthorization(Permissions.ContentRevisionsRead);
        revisions.MapGet("/{contentType}/{contentId:guid}/latest", GetLatestRevisionAsync).RequireAuthorization(Permissions.ContentRevisionsRead);

        var restores = endpoints.MapGroup("/api/v1/content-revision-restore-requests")
            .WithTags("Content Revision Restores");
        restores.MapGet("", SearchRestoreRequestsAsync).RequireAuthorization(Permissions.ContentRevisionsRead);
        restores.MapGet("/{id:guid}", GetRestoreRequestAsync).RequireAuthorization(Permissions.ContentRevisionsRead);
        restores.MapPost("", CreateRestoreRequestAsync).RequireAuthorization(Permissions.ContentRevisionsRestoreRequest);
        restores.MapPost("/{id:guid}/approve", ApproveRestoreRequestAsync).RequireAuthorization(Permissions.ContentRevisionsRestoreApprove);
        restores.MapPost("/{id:guid}/reject", RejectRestoreRequestAsync).RequireAuthorization(Permissions.ContentRevisionsRestoreApprove);
        restores.MapPost("/{id:guid}/complete", CompleteRestoreRequestAsync).RequireAuthorization(Permissions.ContentRevisionsManage);

        return endpoints;
    }

    private static async Task<IResult> SearchRevisionsAsync(
        ContentRevisionQueryHandler handler,
        string? contentType,
        Guid? contentId,
        string? eventType,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchContentRevisionsQuery(contentType, contentId, eventType, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> CreateRevisionAsync(
        CreateContentRevisionRequest request,
        ClaimsPrincipal user,
        ContentRevisionCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.CreateAsync(
            new CreateContentRevisionCommand(request.ContentType, request.ContentId, request.EventType, request.Title, request.Summary, user.Identity?.Name, request.ChangeReason, request.SnapshotJson, request.DiffJson),
            cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/content-revisions/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> GetRevisionAsync(Guid id, ContentRevisionQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAsync(new GetContentRevisionByIdQuery(id), cancellationToken));

    private static async Task<IResult> GetRevisionsForContentAsync(string contentType, Guid contentId, ContentRevisionQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchContentRevisionsQuery(contentType, contentId, null, 1, 100), cancellationToken));

    private static async Task<IResult> GetLatestRevisionAsync(string contentType, Guid contentId, ContentRevisionQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetLatestAsync(new GetLatestContentRevisionQuery(contentType, contentId), cancellationToken));

    private static async Task<IResult> SearchRestoreRequestsAsync(ContentRevisionRestoreQueryHandler handler, string? status, int? pageNumber, int? pageSize, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchContentRevisionRestoreRequestsQuery(status, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetRestoreRequestAsync(Guid id, ContentRevisionRestoreQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAsync(new GetContentRevisionRestoreRequestByIdQuery(id), cancellationToken));

    private static async Task<IResult> CreateRestoreRequestAsync(
        CreateContentRevisionRestoreRequestRequest request,
        ClaimsPrincipal user,
        ContentRevisionRestoreCommandHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.CreateAsync(new CreateContentRevisionRestoreRequestCommand(request.ContentRevisionId, user.Identity?.Name ?? "Unknown", request.Reason), cancellationToken));

    private static async Task<IResult> ApproveRestoreRequestAsync(Guid id, ReviewContentRevisionRestoreRequestRequest request, ClaimsPrincipal user, ContentRevisionRestoreCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.ApproveAsync(new ApproveContentRevisionRestoreRequestCommand(id, user.Identity?.Name ?? "Unknown", request.ReviewNote), cancellationToken));

    private static async Task<IResult> RejectRestoreRequestAsync(Guid id, ReviewContentRevisionRestoreRequestRequest request, ClaimsPrincipal user, ContentRevisionRestoreCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RejectAsync(new RejectContentRevisionRestoreRequestCommand(id, user.Identity?.Name ?? "Unknown", request.ReviewNote), cancellationToken));

    private static async Task<IResult> CompleteRestoreRequestAsync(Guid id, ContentRevisionRestoreCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.CompleteAsync(new CompleteContentRevisionRestoreRequestCommand(id), cancellationToken));

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
}
