using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.Application.Features.GrammarRules;

public interface IGrammarRuleRepository
{
    Task AddAsync(GrammarRule grammarRule, CancellationToken cancellationToken);

    Task<GrammarRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<GrammarRule>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedGrammarRuleId,
        CancellationToken cancellationToken);

    Task<GrammarRuleSearchResult> SearchAsync(
        GrammarRuleSearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
