using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.GrammarRules.Queries;

public sealed class GetGrammarRulesByTopicIdQueryHandler
{
    private readonly SearchGrammarRulesQueryHandler searchHandler;

    public GetGrammarRulesByTopicIdQueryHandler(SearchGrammarRulesQueryHandler searchHandler)
    {
        this.searchHandler = searchHandler;
    }

    public async Task<Result<GrammarRuleSearchResponse>> HandleAsync(
        GetGrammarRulesByTopicIdQuery query,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        if (query.GrammarTopicId == Guid.Empty)
        {
            errors.Add(new ValidationError(nameof(query.GrammarTopicId), $"{nameof(query.GrammarTopicId)} cannot be empty."));
        }

        var pageNumber = SearchGrammarRulesQueryHandler.NormalizePageNumber(query.PageNumber, errors);
        var pageSize = SearchGrammarRulesQueryHandler.NormalizePageSize(query.PageSize, errors);

        if (errors.Count > 0)
        {
            return Result<GrammarRuleSearchResponse>.Validation([.. errors]);
        }

        return await searchHandler.SearchAsync(
            search: null,
            grammarTopicId: query.GrammarTopicId,
            isActive: query.IsActive ?? true,
            pageNumber,
            pageSize,
            cancellationToken);
    }
}
