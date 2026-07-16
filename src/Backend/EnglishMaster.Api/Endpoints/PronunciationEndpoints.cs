using EnglishMaster.Application.Features.MinimalPairs.Commands;
using EnglishMaster.Application.Features.MinimalPairs.Queries;
using EnglishMaster.Application.Features.Pronunciations.Commands;
using EnglishMaster.Application.Features.Pronunciations.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.MinimalPairs;
using EnglishMaster.Contracts.Pronunciations;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class PronunciationEndpoints
{
    public static IEndpointRouteBuilder MapPronunciationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/pronunciations")
            .WithTags("Pronunciations");

        group.MapGet("", SearchAsync).RequireAuthorization(Permissions.PronunciationRead);
        group.MapGet("/{id:guid}", GetAsync).RequireAuthorization(Permissions.PronunciationRead);
        group.MapPost("", CreateAsync).RequireAuthorization(Permissions.PronunciationCreate);
        group.MapPut("/{id:guid}", UpdateAsync).RequireAuthorization(Permissions.PronunciationUpdate);
        group.MapDelete("/{id:guid}", DeleteAsync).RequireAuthorization(Permissions.PronunciationDelete);
        group.MapGet("/{pronunciationId:guid}/minimal-pairs", GetMinimalPairsAsync).RequireAuthorization(Permissions.PronunciationRead);
        group.MapPost("/{pronunciationId:guid}/minimal-pairs", AddMinimalPairAsync).RequireAuthorization(Permissions.PronunciationCreate);

        endpoints.MapGet(
            "/api/v1/words/{wordId:guid}/pronunciation",
            GetByWordIdAsync)
            .WithTags("Pronunciations")
            .RequireAuthorization(Permissions.PronunciationRead);

        var minimalPairs = endpoints.MapGroup("/api/v1/minimal-pairs")
            .WithTags("Minimal Pairs");

        minimalPairs.MapGet("/{id:guid}", GetMinimalPairAsync).RequireAuthorization(Permissions.PronunciationRead);
        minimalPairs.MapPut("/{id:guid}", UpdateMinimalPairAsync).RequireAuthorization(Permissions.PronunciationUpdate);
        minimalPairs.MapDelete("/{id:guid}", DeleteMinimalPairAsync).RequireAuthorization(Permissions.PronunciationDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchAsync(
        SearchPronunciationsQueryHandler handler,
        string? search,
        Guid? wordId,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchPronunciationsQuery(search, wordId, isActive, pageNumber, pageSize),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetAsync(
        Guid id,
        GetPronunciationByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetPronunciationByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetByWordIdAsync(
        Guid wordId,
        GetPronunciationByWordIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetPronunciationByWordIdQuery(wordId), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateAsync(
        CreatePronunciationRequest request,
        CreatePronunciationCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreatePronunciationCommand(
                request.WordId,
                request.IpaUk,
                request.IpaUs,
                request.ThaiReading,
                request.Syllables,
                request.StressPattern,
                request.MouthPosition,
                request.TonguePosition,
                request.CommonMistake,
                request.PracticeNote,
                request.AudioSlowMediaId,
                request.AudioNormalMediaId,
                request.MouthImageMediaId),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/pronunciations/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateAsync(
        Guid id,
        UpdatePronunciationRequest request,
        UpdatePronunciationCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdatePronunciationCommand(
                id,
                request.WordId,
                request.IpaUk,
                request.IpaUs,
                request.ThaiReading,
                request.Syllables,
                request.StressPattern,
                request.MouthPosition,
                request.TonguePosition,
                request.CommonMistake,
                request.PracticeNote,
                request.AudioSlowMediaId,
                request.AudioNormalMediaId,
                request.MouthImageMediaId,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteAsync(
        Guid id,
        DeletePronunciationCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeletePronunciationCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetMinimalPairsAsync(
        Guid pronunciationId,
        GetMinimalPairsByPronunciationIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetMinimalPairsByPronunciationIdQuery(pronunciationId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddMinimalPairAsync(
        Guid pronunciationId,
        CreateMinimalPairRequest request,
        AddMinimalPairCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddMinimalPairCommand(
                pronunciationId,
                request.PairWordText,
                request.PairIpa,
                request.PairThaiReading,
                request.DifferenceNote,
                request.AudioMediaId,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/minimal-pairs/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> GetMinimalPairAsync(
        Guid id,
        GetMinimalPairByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetMinimalPairByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UpdateMinimalPairAsync(
        Guid id,
        UpdateMinimalPairRequest request,
        UpdateMinimalPairCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateMinimalPairCommand(
                id,
                request.PairWordText,
                request.PairIpa,
                request.PairThaiReading,
                request.DifferenceNote,
                request.AudioMediaId,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteMinimalPairAsync(
        Guid id,
        DeleteMinimalPairCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteMinimalPairCommand(id), cancellationToken);

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
