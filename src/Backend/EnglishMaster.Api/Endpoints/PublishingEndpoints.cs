using EnglishMaster.Application.Features.PublishedArtifacts.Queries;
using EnglishMaster.Application.Features.PublishJobs.Commands;
using EnglishMaster.Application.Features.PublishJobs.Queries;
using EnglishMaster.Application.Features.PublishTemplates.Commands;
using EnglishMaster.Application.Features.PublishTemplates.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class PublishingEndpoints
{
    public static IEndpointRouteBuilder MapPublishingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var publishJobs = endpoints.MapGroup("/api/v1/publish-jobs")
            .WithTags("Publish Jobs");

        publishJobs.MapGet("", SearchPublishJobsAsync).RequireAuthorization(Permissions.PublishingRead);
        publishJobs.MapGet("/{id:guid}", GetPublishJobAsync).RequireAuthorization(Permissions.PublishingRead);
        publishJobs.MapPost("", CreatePublishJobAsync).RequireAuthorization(Permissions.PublishingCreate);
        publishJobs.MapPost("/{id:guid}/run", RunPublishJobAsync).RequireAuthorization(Permissions.PublishingRun);
        publishJobs.MapPost("/{id:guid}/cancel", CancelPublishJobAsync).RequireAuthorization(Permissions.PublishingUpdate);
        publishJobs.MapGet("/{publishJobId:guid}/artifacts", GetArtifactsByPublishJobIdAsync).RequireAuthorization(Permissions.PublishingRead);

        var templates = endpoints.MapGroup("/api/v1/publish-templates")
            .WithTags("Publish Templates");

        templates.MapGet("", SearchPublishTemplatesAsync).RequireAuthorization(Permissions.PublishingRead);
        templates.MapGet("/{id:guid}", GetPublishTemplateAsync).RequireAuthorization(Permissions.PublishingRead);
        templates.MapPost("", CreatePublishTemplateAsync).RequireAuthorization(Permissions.PublishingCreate);
        templates.MapPut("/{id:guid}", UpdatePublishTemplateAsync).RequireAuthorization(Permissions.PublishingUpdate);
        templates.MapDelete("/{id:guid}", DeletePublishTemplateAsync).RequireAuthorization(Permissions.PublishingDelete);

        var artifacts = endpoints.MapGroup("/api/v1/published-artifacts")
            .WithTags("Published Artifacts");

        artifacts.MapGet("", SearchPublishedArtifactsAsync).RequireAuthorization(Permissions.PublishingRead);
        artifacts.MapGet("/{id:guid}", GetPublishedArtifactAsync).RequireAuthorization(Permissions.PublishingRead);

        return endpoints;
    }

    private static async Task<IResult> SearchPublishJobsAsync(
        SearchPublishJobsQueryHandler handler,
        string? sourceType,
        Guid? sourceId,
        string? format,
        string? status,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new SearchPublishJobsQuery(sourceType, sourceId, format, status, pageNumber, pageSize, sortBy, sortDirection), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> GetPublishJobAsync(
        Guid id,
        GetPublishJobByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetPublishJobByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> CreatePublishJobAsync(
        CreatePublishJobRequest request,
        CreatePublishJobCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CreatePublishJobCommand(request.SourceType, request.SourceId, request.Format, request.Title, request.RequestedBy), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/publish-jobs/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> RunPublishJobAsync(
        Guid id,
        RunPublishJobCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new RunPublishJobCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> CancelPublishJobAsync(
        Guid id,
        CancelPublishJobCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CancelPublishJobCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> SearchPublishTemplatesAsync(
        SearchPublishTemplatesQueryHandler handler,
        string? format,
        bool? isDefault,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new SearchPublishTemplatesQuery(format, isDefault, isActive, pageNumber, pageSize), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> GetPublishTemplateAsync(
        Guid id,
        GetPublishTemplateByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetPublishTemplateByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> CreatePublishTemplateAsync(
        CreatePublishTemplateRequest request,
        CreatePublishTemplateCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new CreatePublishTemplateCommand(request.Name, request.Description, request.Format, request.TemplateContent, request.IsDefault), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/publish-templates/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdatePublishTemplateAsync(
        Guid id,
        UpdatePublishTemplateRequest request,
        UpdatePublishTemplateCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new UpdatePublishTemplateCommand(id, request.Name, request.Description, request.Format, request.TemplateContent, request.IsDefault, request.IsActive), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> DeletePublishTemplateAsync(
        Guid id,
        DeletePublishTemplateCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeletePublishTemplateCommand(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> SearchPublishedArtifactsAsync(
        SearchPublishedArtifactsQueryHandler handler,
        Guid? publishJobId,
        string? sourceType,
        Guid? sourceId,
        string? format,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new SearchPublishedArtifactsQuery(publishJobId, sourceType, sourceId, format, pageNumber, pageSize), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> GetPublishedArtifactAsync(
        Guid id,
        GetPublishedArtifactByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetPublishedArtifactByIdQuery(id), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> GetArtifactsByPublishJobIdAsync(
        Guid publishJobId,
        GetArtifactsByPublishJobIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetArtifactsByPublishJobIdQuery(publishJobId), cancellationToken);
        return ToHttpResult(result);
    }

    private static IResult ToHttpResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.NoContent(),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());
    }
}
