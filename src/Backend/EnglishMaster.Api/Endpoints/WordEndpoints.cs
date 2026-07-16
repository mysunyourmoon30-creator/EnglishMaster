using EnglishMaster.Application.Features.Words.Commands;
using EnglishMaster.Application.Features.Words.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Words;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class WordEndpoints
{
    public static IEndpointRouteBuilder MapWordEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/words")
            .WithTags("Words");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.WordsRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.WordsRead);
        group.MapPost("", CreateAsync).RequireAuthorization(Permissions.WordsCreate);
        group.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization(Permissions.WordsUpdate);
        group.MapDelete("/{id:guid}", DeleteAsync).RequireAuthorization(Permissions.WordsDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        SearchWordsQueryHandler handler,
        string? search,
        string? partOfSpeech,
        string? cefrLevel,
        bool? isActive,
        Guid? categoryId,
        Guid? tagId,
        int? pageNumber,
        int? pageSize,
        string? sortBy,
        string? sortDirection,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchWordsQuery(
                search,
                partOfSpeech,
                cefrLevel,
                isActive,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                categoryId,
                tagId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetWordByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetWordByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateAsync(
        CreateWordRequest request,
        CreateWordCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateWordCommand(
                request.Text,
                request.IpaUk,
                request.IpaUs,
                request.ThaiReading,
                request.MeaningTh,
                request.MeaningEn,
                request.PartOfSpeech,
                request.CefrLevel,
                request.ExampleEn,
                request.ExampleTh,
                request.CategoryId,
                request.TagIds,
                request.ImageMediaId,
                request.AudioMediaId),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/words/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdateWordRequest request,
        UpdateWordCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateWordCommand(
                id,
                request.Text,
                request.IpaUk,
                request.IpaUs,
                request.ThaiReading,
                request.MeaningTh,
                request.MeaningEn,
                request.PartOfSpeech,
                request.CefrLevel,
                request.ExampleEn,
                request.ExampleTh,
                request.IsActive,
                request.CategoryId,
                request.TagIds,
                request.ImageMediaId,
                request.AudioMediaId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        DeleteWordCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteWordCommand(id), cancellationToken);

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
