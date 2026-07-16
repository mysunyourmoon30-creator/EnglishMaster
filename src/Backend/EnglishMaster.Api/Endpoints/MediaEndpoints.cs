using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Media.Commands;
using EnglishMaster.Application.Features.Media.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Media;
using EnglishMaster.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace EnglishMaster.Api.Endpoints;

public static class MediaEndpoints
{
    public static IEndpointRouteBuilder MapMediaEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/media")
            .WithTags("Media");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.MediaRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.MediaRead);
        group.MapPost("", CreateAsync).RequireAuthorization(Permissions.MediaCreate);
        group.MapPost("/upload", UploadAsync)
            .DisableAntiforgery()
            .WithMetadata(new RequestSizeLimitAttribute(MediaUploadLimits.MaximumFileSizeBytes))
            .RequireAuthorization(Permissions.MediaCreate);
        group.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization(Permissions.MediaUpdate);
        group.MapPost("/{id:guid}/activate", ActivateAsync).RequireAuthorization(Permissions.MediaUpdate);
        group.MapPost("/{id:guid}/deactivate", DeactivateAsync).RequireAuthorization(Permissions.MediaUpdate);
        group.MapDelete("/{id:guid}", DeleteAsync).RequireAuthorization(Permissions.MediaDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        SearchMediaQueryHandler handler,
        string? search,
        string? mediaType,
        string? contentType,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchMediaQuery(search, mediaType, contentType, isActive, pageNumber, pageSize),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetMediaByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetMediaByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateAsync(
        CreateMediaRequest request,
        CreateMediaCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateMediaCommand(
                request.FileName,
                request.OriginalFileName,
                request.FileExtension,
                request.ContentType,
                request.FileSize,
                request.MediaType,
                request.PublicUrl,
                request.AltText,
                request.Description),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/media/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UploadAsync(
        [FromForm] IFormFile? file,
        [FromForm] string? altText,
        [FromForm] string? description,
        UploadMediaCommandHandler handler,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length <= 0)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(file)] = ["File is required."]
            });
        }

        await using var stream = file.OpenReadStream();
        var result = await handler.HandleAsync(
            new UploadMediaCommand(
                stream,
                file.FileName,
                file.ContentType,
                file.Length,
                altText,
                description),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/media/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateMediaRequest request,
        UpdateMediaCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateMediaCommand(
                id,
                request.FileName,
                request.OriginalFileName,
                request.FileExtension,
                request.ContentType,
                request.FileSize,
                request.MediaType,
                request.PublicUrl,
                request.AltText,
                request.Description,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> ActivateAsync(
        Guid id,
        ActivateMediaCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new ActivateMediaCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeactivateAsync(
        Guid id,
        DeactivateMediaCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeactivateMediaCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        DeleteMediaCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteMediaCommand(id), cancellationToken);

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

    private static Dictionary<string, string[]> ToValidationDictionary(
        IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());
    }
}
