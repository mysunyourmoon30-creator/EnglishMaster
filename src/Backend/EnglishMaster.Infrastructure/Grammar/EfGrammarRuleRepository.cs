using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.GrammarRules.Dtos;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Grammar;

internal sealed class EfGrammarRuleRepository : IGrammarRuleRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfGrammarRuleRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(GrammarRule grammarRule, CancellationToken cancellationToken)
    {
        await dbContext.GrammarRules.AddAsync(grammarRule, cancellationToken);
    }

    public async Task<GrammarRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.GrammarRules
            .Include(grammarRule => grammarRule.RelatedWords)
            .FirstOrDefaultAsync(grammarRule => grammarRule.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GrammarRule>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToArray();
        return await dbContext.GrammarRules
            .AsNoTracking()
            .Where(grammarRule => normalizedIds.Contains(grammarRule.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedGrammarRuleId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.GrammarRules.AsNoTracking()
            .Where(grammarRule => grammarRule.Slug == slug);

        if (excludedGrammarRuleId.HasValue)
        {
            query = query.Where(grammarRule => grammarRule.Id != excludedGrammarRuleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<GrammarRuleSearchResult> SearchAsync(
        GrammarRuleSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<GrammarRule> query = dbContext.GrammarRules.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(grammarRule => grammarRule.IsActive == criteria.IsActive.Value);
        }

        if (criteria.GrammarTopicId.HasValue)
        {
            query = query.Where(grammarRule => grammarRule.GrammarTopicId == criteria.GrammarTopicId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(grammarRule =>
                grammarRule.Title.ToLower().Contains(searchTerm) ||
                grammarRule.Slug.ToLower().Contains(searchTerm) ||
                grammarRule.RuleText.ToLower().Contains(searchTerm) ||
                grammarRule.StructurePattern.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new GrammarRuleSearchResult([], totalCount);
        }

        var items = await query
            .OrderBy(grammarRule => grammarRule.SortOrder)
            .ThenBy(grammarRule => grammarRule.Title)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new GrammarRuleSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
