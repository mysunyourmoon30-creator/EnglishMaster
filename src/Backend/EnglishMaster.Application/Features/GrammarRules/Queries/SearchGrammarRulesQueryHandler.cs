using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Queries;

public sealed class SearchGrammarRulesQueryHandler
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaximumPageSize = 100;
    private const int MaximumSearchLength = GrammarRuleFieldLimits.Title;

    private readonly IGrammarRuleRepository grammarRuleRepository;
    private readonly IGrammarTopicRepository grammarTopicRepository;
    private readonly IWordRepository wordRepository;

    public SearchGrammarRulesQueryHandler(
        IGrammarRuleRepository grammarRuleRepository,
        IGrammarTopicRepository grammarTopicRepository,
        IWordRepository wordRepository)
    {
        this.grammarRuleRepository = grammarRuleRepository;
        this.grammarTopicRepository = grammarTopicRepository;
        this.wordRepository = wordRepository;
    }

    public async Task<Result<GrammarRuleSearchResponse>> HandleAsync(
        SearchGrammarRulesQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        var search = NormalizeSearch(query.Search, errors);
        ValidateOptionalId(query.GrammarTopicId, nameof(query.GrammarTopicId), errors);
        var pageNumber = NormalizePageNumber(query.PageNumber, errors);
        var pageSize = NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<GrammarRuleSearchResponse>.Validation([.. errors]);
        }

        return await SearchAsync(
            search,
            query.GrammarTopicId,
            query.IsActive ?? true,
            pageNumber,
            pageSize,
            cancellationToken);
    }

    internal async Task<Result<GrammarRuleSearchResponse>> SearchAsync(
        string? search,
        Guid? grammarTopicId,
        bool? isActive,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var result = await grammarRuleRepository.SearchAsync(
            new GrammarRuleSearchCriteria(search, grammarTopicId, isActive, pageNumber, pageSize),
            cancellationToken);
        var items = await GrammarRuleReadModelBuilder.MapAsync(
            result.Items,
            grammarTopicRepository,
            wordRepository,
            cancellationToken);
        var totalPages = result.TotalCount == 0
            ? 0
            : (int)Math.Ceiling(result.TotalCount / (double)pageSize);

        return Result<GrammarRuleSearchResponse>.Success(new GrammarRuleSearchResponse(
            items,
            pageNumber,
            pageSize,
            result.TotalCount,
            totalPages,
            pageNumber > 1 && totalPages > 0,
            pageNumber < totalPages));
    }

    internal static int NormalizePageNumber(
        int? value,
        ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageNumber;
        }

        if (value.Value < 1)
        {
            errors.Add(new ValidationError(nameof(SearchGrammarRulesQuery.PageNumber), $"{nameof(SearchGrammarRulesQuery.PageNumber)} must be greater than or equal to 1."));
            return DefaultPageNumber;
        }

        return value.Value;
    }

    internal static int NormalizePageSize(
        int? value,
        ICollection<ValidationError> errors)
    {
        if (!value.HasValue)
        {
            return DefaultPageSize;
        }

        if (value.Value < 1 || value.Value > MaximumPageSize)
        {
            errors.Add(new ValidationError(nameof(SearchGrammarRulesQuery.PageSize), $"{nameof(SearchGrammarRulesQuery.PageSize)} must be between 1 and {MaximumPageSize}."));
            return DefaultPageSize;
        }

        return value.Value;
    }

    private static string? NormalizeSearch(
        string? value,
        ICollection<ValidationError> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim();
        if (normalized.Length > MaximumSearchLength)
        {
            errors.Add(new ValidationError(nameof(SearchGrammarRulesQuery.Search), $"{nameof(SearchGrammarRulesQuery.Search)} must be {MaximumSearchLength} characters or fewer."));
        }

        return normalized;
    }

    private static void ValidateOptionalId(
        Guid? value,
        string field,
        ICollection<ValidationError> errors)
    {
        if (value == Guid.Empty)
        {
            errors.Add(new ValidationError(field, $"{field} cannot be empty."));
        }
    }
}
