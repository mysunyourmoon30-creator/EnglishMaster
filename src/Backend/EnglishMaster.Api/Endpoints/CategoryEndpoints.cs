using EnglishMaster.Application.Features.Categories.Commands;
using EnglishMaster.Application.Features.Categories.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Categories;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/categories")
            .WithTags("Categories");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.CategoriesRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.CategoriesRead);
        group.MapPost("", CreateAsync).RequireAuthorization(Permissions.CategoriesCreate);
        group.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization(Permissions.CategoriesUpdate);
        group.MapDelete("/{id:guid}", DeleteAsync).RequireAuthorization(Permissions.CategoriesDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        SearchCategoriesQueryHandler handler,
        string? search,
        bool? isActive,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchCategoriesQuery(search, isActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetCategoryByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetCategoryByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateAsync(
        CreateCategoryRequest request,
        CreateCategoryCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateCategoryCommand(
                request.Name,
                request.Description,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/categories/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request,
        UpdateCategoryCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateCategoryCommand(
                id,
                request.Name,
                request.Description,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        DeleteCategoryCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteCategoryCommand(id), cancellationToken);

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
