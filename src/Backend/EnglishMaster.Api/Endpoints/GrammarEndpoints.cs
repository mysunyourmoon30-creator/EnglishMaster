using EnglishMaster.Application.Features.GrammarExamples.Commands;
using EnglishMaster.Application.Features.GrammarExamples.Queries;
using EnglishMaster.Application.Features.GrammarRules.Commands;
using EnglishMaster.Application.Features.GrammarRules.Queries;
using EnglishMaster.Application.Features.GrammarTopics.Commands;
using EnglishMaster.Application.Features.GrammarTopics.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class GrammarEndpoints
{
    public static IEndpointRouteBuilder MapGrammarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var topics = endpoints.MapGroup("/api/v1/grammar-topics")
            .WithTags("Grammar Topics");

        topics.MapGet("", SearchTopicsAsync).RequireAuthorization(Permissions.GrammarRead);
        topics.MapGet("/{id:guid}", GetTopicAsync).RequireAuthorization(Permissions.GrammarRead);
        topics.MapPost("", CreateTopicAsync).RequireAuthorization(Permissions.GrammarCreate);
        topics.MapPut("/{id:guid}", UpdateTopicAsync).RequireAuthorization(Permissions.GrammarUpdate);
        topics.MapDelete("/{id:guid}", DeleteTopicAsync).RequireAuthorization(Permissions.GrammarDelete);
        topics.MapGet("/{topicId:guid}/rules", GetRulesByTopicIdAsync).RequireAuthorization(Permissions.GrammarRead);

        var rules = endpoints.MapGroup("/api/v1/grammar-rules")
            .WithTags("Grammar Rules");

        rules.MapGet("", SearchRulesAsync).RequireAuthorization(Permissions.GrammarRead);
        rules.MapGet("/{id:guid}", GetRuleAsync).RequireAuthorization(Permissions.GrammarRead);
        rules.MapPost("", CreateRuleAsync).RequireAuthorization(Permissions.GrammarCreate);
        rules.MapPut("/{id:guid}", UpdateRuleAsync).RequireAuthorization(Permissions.GrammarUpdate);
        rules.MapDelete("/{id:guid}", DeleteRuleAsync).RequireAuthorization(Permissions.GrammarDelete);
        rules.MapGet("/{grammarRuleId:guid}/examples", GetExamplesByRuleIdAsync).RequireAuthorization(Permissions.GrammarRead);
        rules.MapPost("/{grammarRuleId:guid}/examples", AddExampleAsync).RequireAuthorization(Permissions.GrammarCreate);
        rules.MapPost("/{grammarRuleId:guid}/words/{wordId:guid}", AddRelatedWordAsync).RequireAuthorization(Permissions.GrammarUpdate);
        rules.MapDelete("/{grammarRuleId:guid}/words/{wordId:guid}", RemoveRelatedWordAsync).RequireAuthorization(Permissions.GrammarUpdate);

        var examples = endpoints.MapGroup("/api/v1/grammar-examples")
            .WithTags("Grammar Examples");

        examples.MapGet("", GetExamplesAsync).RequireAuthorization(Permissions.GrammarRead);
        examples.MapGet("/{id:guid}", GetExampleAsync).RequireAuthorization(Permissions.GrammarRead);
        examples.MapPut("/{id:guid}", UpdateExampleAsync).RequireAuthorization(Permissions.GrammarUpdate);
        examples.MapDelete("/{id:guid}", DeleteExampleAsync).RequireAuthorization(Permissions.GrammarDelete);

        return endpoints;
    }

    private static async Task<IResult> SearchTopicsAsync(
        SearchGrammarTopicsQueryHandler handler,
        string? search,
        string? cefrLevel,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchGrammarTopicsQuery(search, cefrLevel, isActive, pageNumber, pageSize),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetTopicAsync(
        Guid id,
        GetGrammarTopicByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetGrammarTopicByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateTopicAsync(
        CreateGrammarTopicRequest request,
        CreateGrammarTopicCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateGrammarTopicCommand(
                request.Title,
                request.Summary,
                request.CefrLevel,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/grammar-topics/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateTopicAsync(
        Guid id,
        UpdateGrammarTopicRequest request,
        UpdateGrammarTopicCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateGrammarTopicCommand(
                id,
                request.Title,
                request.Summary,
                request.CefrLevel,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteTopicAsync(
        Guid id,
        DeleteGrammarTopicCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteGrammarTopicCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> SearchRulesAsync(
        SearchGrammarRulesQueryHandler handler,
        string? search,
        Guid? grammarTopicId,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new SearchGrammarRulesQuery(search, grammarTopicId, isActive, pageNumber, pageSize),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetRuleAsync(
        Guid id,
        GetGrammarRuleByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetGrammarRuleByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetRulesByTopicIdAsync(
        Guid topicId,
        GetGrammarRulesByTopicIdQueryHandler handler,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetGrammarRulesByTopicIdQuery(topicId, isActive, pageNumber, pageSize),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> CreateRuleAsync(
        CreateGrammarRuleRequest request,
        CreateGrammarRuleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new CreateGrammarRuleCommand(
                request.GrammarTopicId,
                request.Title,
                request.RuleText,
                request.ExplanationTh,
                request.ExplanationEn,
                request.StructurePattern,
                request.CommonMistake,
                request.CorrectUsageNote,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/grammar-rules/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateRuleAsync(
        Guid id,
        UpdateGrammarRuleRequest request,
        UpdateGrammarRuleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateGrammarRuleCommand(
                id,
                request.GrammarTopicId,
                request.Title,
                request.RuleText,
                request.ExplanationTh,
                request.ExplanationEn,
                request.StructurePattern,
                request.CommonMistake,
                request.CorrectUsageNote,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteRuleAsync(
        Guid id,
        DeleteGrammarRuleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteGrammarRuleCommand(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddRelatedWordAsync(
        Guid grammarRuleId,
        Guid wordId,
        AddRelatedWordToGrammarRuleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddRelatedWordToGrammarRuleCommand(grammarRuleId, wordId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> RemoveRelatedWordAsync(
        Guid grammarRuleId,
        Guid wordId,
        RemoveRelatedWordFromGrammarRuleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new RemoveRelatedWordFromGrammarRuleCommand(grammarRuleId, wordId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetExamplesByRuleIdAsync(
        Guid grammarRuleId,
        GetGrammarExamplesByRuleIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetGrammarExamplesByRuleIdQuery(grammarRuleId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> GetExamplesAsync(
        Guid grammarRuleId,
        GetGrammarExamplesByRuleIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new GetGrammarExamplesByRuleIdQuery(grammarRuleId),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> AddExampleAsync(
        Guid grammarRuleId,
        CreateGrammarExampleRequest request,
        AddGrammarExampleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new AddGrammarExampleCommand(
                grammarRuleId,
                request.ExampleEn,
                request.TranslationTh,
                request.ExplanationTh,
                request.IsCorrectExample,
                request.SortOrder),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/grammar-examples/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> GetExampleAsync(
        Guid id,
        GetGrammarExampleByIdQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetGrammarExampleByIdQuery(id), cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> UpdateExampleAsync(
        Guid id,
        UpdateGrammarExampleRequest request,
        UpdateGrammarExampleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            new UpdateGrammarExampleCommand(
                id,
                request.ExampleEn,
                request.TranslationTh,
                request.ExplanationTh,
                request.IsCorrectExample,
                request.SortOrder,
                request.IsActive),
            cancellationToken);

        return ToHttpResult(result);
    }

    private static async Task<IResult> DeleteExampleAsync(
        Guid id,
        DeleteGrammarExampleCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteGrammarExampleCommand(id), cancellationToken);

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
