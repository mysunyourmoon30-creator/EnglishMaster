using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Grammar;

internal sealed class EfGrammarTopicRepository : IGrammarTopicRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfGrammarTopicRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(GrammarTopic grammarTopic, CancellationToken cancellationToken)
    {
        await dbContext.GrammarTopics.AddAsync(grammarTopic, cancellationToken);
    }

    public async Task<GrammarTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.GrammarTopics
            .FirstOrDefaultAsync(grammarTopic => grammarTopic.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<GrammarTopic>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToArray();
        return await dbContext.GrammarTopics
            .AsNoTracking()
            .Where(grammarTopic => normalizedIds.Contains(grammarTopic.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedGrammarTopicId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.GrammarTopics.AsNoTracking()
            .Where(grammarTopic => grammarTopic.Slug == slug);

        if (excludedGrammarTopicId.HasValue)
        {
            query = query.Where(grammarTopic => grammarTopic.Id != excludedGrammarTopicId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<GrammarTopicSearchResult> SearchAsync(
        GrammarTopicSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<GrammarTopic> query = dbContext.GrammarTopics.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(grammarTopic => grammarTopic.IsActive == criteria.IsActive.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(grammarTopic => grammarTopic.CefrLevel == criteria.CefrLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(grammarTopic =>
                grammarTopic.Title.ToLower().Contains(searchTerm) ||
                grammarTopic.Slug.ToLower().Contains(searchTerm) ||
                grammarTopic.Summary.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new GrammarTopicSearchResult([], totalCount);
        }

        var items = await query
            .OrderBy(grammarTopic => grammarTopic.SortOrder)
            .ThenBy(grammarTopic => grammarTopic.Title)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new GrammarTopicSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
