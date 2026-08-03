using EnglishMaster.Application.Features.PublicGrammar.Dtos;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.PublicGrammar.Queries;

public sealed record GetPublicGrammarTopicBySlugQuery(string? Slug);

public sealed record GetPublicGrammarRuleBySlugQuery(string? Slug);

public sealed class PublicGrammarQueryHandler
{
    private readonly IPublicGrammarRepository repository;

    public PublicGrammarQueryHandler(IPublicGrammarRepository repository)
    {
        this.repository = repository;
    }

    public async Task<Result<PublicGrammarTopicDetailDto>> GetTopicAsync(
        GetPublicGrammarTopicBySlugQuery query,
        CancellationToken cancellationToken)
    {
        var validationError = ValidateSlug(query.Slug, GrammarTopicFieldLimits.Slug);
        if (validationError is not null)
        {
            return Result<PublicGrammarTopicDetailDto>.Validation(validationError);
        }

        var topic = await repository.GetTopicBySlugAsync(
            query.Slug!.Trim().ToLowerInvariant(),
            cancellationToken);
        return topic is null
            ? Result<PublicGrammarTopicDetailDto>.NotFound(nameof(query.Slug), "Published grammar topic was not found.")
            : Result<PublicGrammarTopicDetailDto>.Success(topic);
    }

    public async Task<Result<PublicGrammarRuleDetailDto>> GetRuleAsync(
        GetPublicGrammarRuleBySlugQuery query,
        CancellationToken cancellationToken)
    {
        var validationError = ValidateSlug(query.Slug, GrammarRuleFieldLimits.Slug);
        if (validationError is not null)
        {
            return Result<PublicGrammarRuleDetailDto>.Validation(validationError);
        }

        var rule = await repository.GetRuleBySlugAsync(
            query.Slug!.Trim().ToLowerInvariant(),
            cancellationToken);
        return rule is null
            ? Result<PublicGrammarRuleDetailDto>.NotFound(nameof(query.Slug), "Published grammar rule was not found.")
            : Result<PublicGrammarRuleDetailDto>.Success(rule);
    }

    private static ValidationError? ValidateSlug(string? slug, int maximumLength)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return new ValidationError("Slug", "Slug is required.");
        }

        return slug.Trim().Length > maximumLength
            ? new ValidationError("Slug", $"Slug must be {maximumLength} characters or fewer.")
            : null;
    }
}