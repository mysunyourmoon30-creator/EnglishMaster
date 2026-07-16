using EnglishMaster.Application.Features.Tags.Commands;
using EnglishMaster.Application.Features.Tags.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Tags;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class TagEndpoints
{
    public static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/tags")
            .WithTags("Tags");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.TagsRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.TagsRead);
        group.MapPost("", CreateAsync).RequireAuthorization(Permissions.TagsCreate);
        group.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization(Permissions.TagsUpdate);
        group.MapDelete("/{id:guid}", DeleteAsync).RequireAuthorization(Permissions.TagsDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        SearchTagsQueryHandler handler,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchTagsQuery(search, isActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetTagByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetTagByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateAsync(
        CreateTagRequest request,
        CreateTagCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateTagCommand(
                request.Name,
                request.Description),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/tags/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateTagRequest request,
        UpdateTagCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateTagCommand(
                id,
                request.Name,
                request.Description,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        DeleteTagCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteTagCommand(id), cancellationToken);

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
