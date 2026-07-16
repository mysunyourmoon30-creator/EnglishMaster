using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeGrammarRuleRepository : IGrammarRuleRepository
{
    public List<GrammarRule> GrammarRules { get; } = [];

    public Task AddAsync(GrammarRule grammarRule, CancellationToken cancellationToken)
    {
        GrammarRules.Add(grammarRule);
        return Task.CompletedTask;
    }

    public Task<GrammarRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(GrammarRules.SingleOrDefault(rule => rule.Id == id));
    }

    public Task<IReadOnlyCollection<GrammarRule>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToHashSet();
        return Task.FromResult<IReadOnlyCollection<GrammarRule>>(
            GrammarRules.Where(rule => normalizedIds.Contains(rule.Id)).ToArray());
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedGrammarRuleId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(GrammarRules.Any(rule =>
            rule.Slug == slug &&
            (!excludedGrammarRuleId.HasValue || rule.Id != excludedGrammarRuleId.Value)));
    }

    public Task<GrammarRuleSearchResult> SearchAsync(
        GrammarRuleSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = GrammarRules.AsEnumerable();
        if (criteria.IsActive.HasValue)
        {
            query = query.Where(rule => rule.IsActive == criteria.IsActive.Value);
        }

        if (criteria.GrammarTopicId.HasValue)
        {
            query = query.Where(rule => rule.GrammarTopicId == criteria.GrammarTopicId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(rule =>
                rule.Title.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                rule.RuleText.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderBy(rule => rule.SortOrder)
            .ThenBy(rule => rule.Title)
            .ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new GrammarRuleSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1);
    }
}
