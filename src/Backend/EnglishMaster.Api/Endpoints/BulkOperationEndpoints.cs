using System.Security.Claims;
using EnglishMaster.Application.Features.BulkOperations.Commands;
using EnglishMaster.Application.Features.BulkOperations.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.BulkOperations;
using EnglishMaster.Shared.Results;
using AppBulkOperationDto = EnglishMaster.Application.Features.BulkOperations.Dtos.BulkOperationDto;
using AppBulkOperationItemDto = EnglishMaster.Application.Features.BulkOperations.Dtos.BulkOperationItemDto;
using AppBulkOperationSearchResponse = EnglishMaster.Application.Features.BulkOperations.Dtos.BulkOperationSearchResponse;

namespace EnglishMaster.Api.Endpoints;

public static class BulkOperationEndpoints
{
    public static IEndpointRouteBuilder MapBulkOperationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/bulk-operations")
            .WithTags("Bulk Operations");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.BulkOperationsRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.BulkOperationsRead);
        group.MapGet("/{id:guid}/items", GetItemsAsync).RequireAuthorization(Permissions.BulkOperationsRead);
        group.MapPost("", CreateAsync).RequireAuthorization(Permissions.BulkOperationsRun);
        group.MapPost("/{id:guid}/run", RunAsync).RequireAuthorization(Permissions.BulkOperationsRun);
        group.MapPost("/{id:guid}/cancel", CancelAsync).RequireAuthorization(Permissions.BulkOperationsCancel);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        BulkOperationQueryHandler handler,
        string? operationType,
        string? contentType,
        string? status,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchBulkOperationsQuery(operationType, contentType, status, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetAsync(Guid id, BulkOperationQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAsync(new GetBulkOperationByIdQuery(id), cancellationToken));

    private static async Task<IResult> GetItemsAsync(Guid id, BulkOperationQueryHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetItemsAsync(new GetBulkOperationItemsQuery(id), cancellationToken));

    private static async Task<IResult> CreateAsync(CreateBulkOperationRequest request, ClaimsPrincipal user, BulkOperationCommandHandler handler, CancellationToken cancellationToken)
    {
        if (!CanCreateRequestedOperation(request, user))
        {
            return Results.Forbid();
        }

        var result = await handler.CreateAsync(
            new CreateBulkOperationCommand(request.OperationType, request.ContentType, request.ContentIds, user.Identity?.Name ?? "Unknown", request.Note, request.CategoryId, request.TagIds, request.ExportFormat),
            cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/bulk-operations/{result.Value!.Id}", ToContract(result.Value))
            : ToHttpResult(result);
    }

    private static async Task<IResult> RunAsync(Guid id, ClaimsPrincipal user, BulkOperationQueryHandler queryHandler, BulkOperationCommandHandler commandHandler, CancellationToken cancellationToken)
    {
        var operation = await queryHandler.GetAsync(new GetBulkOperationByIdQuery(id), cancellationToken);
        if (operation.Status == ResultStatus.NotFound)
        {
            return Results.NotFound();
        }

        if (!CanRunStoredOperation(operation.Value!.OperationType, user))
        {
            return Results.Forbid();
        }

        return ToHttpResult(await commandHandler.RunAsync(new RunBulkOperationCommand(id), cancellationToken));
    }

    private static async Task<IResult> CancelAsync(Guid id, BulkOperationCommandHandler handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.CancelAsync(new CancelBulkOperationCommand(id), cancellationToken));

    private static IResult ToHttpResult(Result<AppBulkOperationDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static IResult ToHttpResult(Result<AppBulkOperationSearchResponse> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(new BulkOperationSearchResponse(
                result.Value!.Items.Select(ToContract).ToArray(),
                result.Value.PageNumber,
                result.Value.PageSize,
                result.Value.TotalCount,
                result.Value.TotalPages,
                result.Value.HasPreviousPage,
                result.Value.HasNextPage)),
            _ => Results.Problem()
        };

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppBulkOperationItemDto>> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value!.Select(ToContract).ToArray()),
            _ => Results.Problem()
        };

    private static BulkOperationDto ToContract(AppBulkOperationDto operation) =>
        new(
            operation.Id,
            operation.OperationType,
            operation.ContentType,
            operation.Status,
            operation.RequestedBy,
            operation.RequestedAt,
            operation.StartedAt,
            operation.CompletedAt,
            operation.TotalItems,
            operation.SucceededItems,
            operation.FailedItems,
            operation.ErrorMessage,
            operation.Note,
            operation.CategoryId,
            operation.TagIds,
            operation.ExportFormat,
            operation.CreatedAt,
            operation.UpdatedAt);

    private static BulkOperationItemDto ToContract(AppBulkOperationItemDto item) =>
        new(item.Id, item.BulkOperationId, item.ContentId, item.Status, item.ErrorMessage, item.CreatedAt, item.UpdatedAt);

    private static bool CanCreateRequestedOperation(CreateBulkOperationRequest request, ClaimsPrincipal user)
    {
        if (!HasPermission(user, Permissions.BulkOperationsRun))
        {
            return false;
        }

        return CanRunStoredOperation(request.OperationType, user);
    }

    private static bool CanRunStoredOperation(string operationType, ClaimsPrincipal user)
    {
        if (!HasPermission(user, Permissions.BulkOperationsRun))
        {
            return false;
        }

        return operationType.Trim().ToLowerInvariant() switch
        {
            "runqualitycheck" => HasPermission(user, Permissions.ContentQualityRun),
            "publish" => HasPermission(user, Permissions.PublishingRun),
            "approve" => HasPermission(user, Permissions.ContentRevisionsRestoreApprove) || HasPermission(user, Permissions.PublishingRun),
            "archive" => HasAnyContentUpdatePermission(user),
            "assigncategory" => HasPermission(user, Permissions.CategoriesUpdate) || HasPermission(user, Permissions.WordsUpdate),
            "addtags" or "removetags" => HasPermission(user, Permissions.TagsUpdate) || HasPermission(user, Permissions.WordsUpdate),
            "export" => HasPermission(user, Permissions.WordsRead),
            _ => true
        };
    }

    private static bool HasAnyContentUpdatePermission(ClaimsPrincipal user) =>
        Permissions.ContentCreateUpdate.Any(permission => HasPermission(user, permission));

    private static bool HasPermission(ClaimsPrincipal user, string permission) =>
        user.HasClaim(SecurityPermissionClaimTypes.Permission, permission);

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors) =>
        errors
            .GroupBy(error => error.Field)
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
}
