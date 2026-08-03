using EnglishMaster.Application.Features.PublicGrammar.Queries;
using EnglishMaster.Shared.Results;

using AppPublicGrammarExampleDto = EnglishMaster.Application.Features.PublicGrammar.Dtos.PublicGrammarExampleDto;
using AppPublicGrammarRelatedWordDto = EnglishMaster.Application.Features.PublicGrammar.Dtos.PublicGrammarRelatedWordDto;
using AppPublicGrammarRuleDetailDto = EnglishMaster.Application.Features.PublicGrammar.Dtos.PublicGrammarRuleDetailDto;
using AppPublicGrammarRuleSummaryDto = EnglishMaster.Application.Features.PublicGrammar.Dtos.PublicGrammarRuleSummaryDto;
using AppPublicGrammarTopicDetailDto = EnglishMaster.Application.Features.PublicGrammar.Dtos.PublicGrammarTopicDetailDto;
using AppPublicGrammarTopicSummaryDto = EnglishMaster.Application.Features.PublicGrammar.Dtos.PublicGrammarTopicSummaryDto;
using ContractPublicGrammarExampleDto = EnglishMaster.Contracts.PublicGrammar.PublicGrammarExampleDto;
using ContractPublicGrammarRelatedWordDto = EnglishMaster.Contracts.PublicGrammar.PublicGrammarRelatedWordDto;
using ContractPublicGrammarRuleDetailDto = EnglishMaster.Contracts.PublicGrammar.PublicGrammarRuleDetailDto;
using ContractPublicGrammarRuleSummaryDto = EnglishMaster.Contracts.PublicGrammar.PublicGrammarRuleSummaryDto;
using ContractPublicGrammarTopicDetailDto = EnglishMaster.Contracts.PublicGrammar.PublicGrammarTopicDetailDto;
using ContractPublicGrammarTopicSummaryDto = EnglishMaster.Contracts.PublicGrammar.PublicGrammarTopicSummaryDto;

namespace EnglishMaster.Api.Endpoints;

public static class PublicGrammarEndpoints
{
    public static IEndpointRouteBuilder MapPublicGrammarEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/public/grammar")
            .WithTags("Public Grammar")
            .AllowAnonymous();

        group.MapGet("/topics/{slug}", GetTopicAsync);
        group.MapGet("/rules/{slug}", GetRuleAsync);

        return endpoints;
    }

    private static async Task<IResult> GetTopicAsync(
        string slug,
        PublicGrammarQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetTopicAsync(
            new GetPublicGrammarTopicBySlugQuery(slug),
            cancellationToken));

    private static async Task<IResult> GetRuleAsync(
        string slug,
        PublicGrammarQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetRuleAsync(
            new GetPublicGrammarRuleBySlugQuery(slug),
            cancellationToken));

    private static IResult ToHttpResult(Result<AppPublicGrammarTopicDetailDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static IResult ToHttpResult(Result<AppPublicGrammarRuleDetailDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };

    private static ContractPublicGrammarTopicDetailDto ToContract(AppPublicGrammarTopicDetailDto topic) =>
        new(
            topic.Title,
            topic.Slug,
            topic.Summary,
            topic.CefrLevel,
            topic.Rules.Select(ToContract).ToArray());

    private static ContractPublicGrammarRuleSummaryDto ToContract(AppPublicGrammarRuleSummaryDto rule) =>
        new(rule.Title, rule.Slug, rule.RuleText, rule.StructurePattern);

    private static ContractPublicGrammarRuleDetailDto ToContract(AppPublicGrammarRuleDetailDto rule) =>
        new(
            rule.Title,
            rule.Slug,
            rule.RuleText,
            rule.ExplanationTh,
            rule.ExplanationEn,
            rule.StructurePattern,
            rule.CommonMistake,
            rule.CorrectUsageNote,
            ToContract(rule.Topic),
            rule.Examples.Select(ToContract).ToArray(),
            rule.RelatedWords.Select(ToContract).ToArray());

    private static ContractPublicGrammarTopicSummaryDto ToContract(AppPublicGrammarTopicSummaryDto topic) =>
        new(topic.Title, topic.Slug, topic.CefrLevel);

    private static ContractPublicGrammarExampleDto ToContract(AppPublicGrammarExampleDto example) =>
        new(example.ExampleEn, example.TranslationTh, example.ExplanationTh, example.IsCorrectExample);

    private static ContractPublicGrammarRelatedWordDto ToContract(AppPublicGrammarRelatedWordDto word) =>
        new(word.Text, word.Slug, word.MeaningTh);

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors) =>
        errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());
}