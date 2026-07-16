using EnglishMaster.Application.Features.Words;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Words;

internal sealed class EfWordRepository : IWordRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfWordRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Word word, CancellationToken cancellationToken)
    {
        await dbContext.Words.AddAsync(word, cancellationToken);
    }

    public async Task<Word?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Words
            .Include(word => word.Tags)
            .FirstOrDefaultAsync(word => word.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Word>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToArray();
        return await dbContext.Words
            .AsNoTracking()
            .Include(word => word.Tags)
            .Where(word => normalizedIds.Contains(word.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedWordId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Words.AsNoTracking()
            .Where(word => word.Slug == slug);

        if (excludedWordId.HasValue)
        {
            query = query.Where(word => word.Id != excludedWordId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<WordSearchResult> SearchAsync(
        WordSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Word> query = dbContext.Words.AsNoTracking()
            .Include(word => word.Tags);

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(word => word.IsActive == criteria.IsActive.Value);
        }

        if (criteria.PartOfSpeech.HasValue)
        {
            query = query.Where(word => word.PartOfSpeech == criteria.PartOfSpeech.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(word => word.CefrLevel == criteria.CefrLevel.Value);
        }

        if (criteria.CategoryId.HasValue)
        {
            query = query.Where(word => word.CategoryId == criteria.CategoryId.Value);
        }

        if (criteria.TagId.HasValue)
        {
            query = query.Where(word => word.Tags.Any(tag => tag.TagId == criteria.TagId.Value));
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(word =>
                word.Text.ToLower().Contains(searchTerm) ||
                word.Slug.ToLower().Contains(searchTerm) ||
                word.MeaningTh.ToLower().Contains(searchTerm) ||
                word.MeaningEn.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new WordSearchResult([], totalCount);
        }

        var sortedQuery = ApplySorting(query, criteria);
        var items = await sortedQuery
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToListAsync(cancellationToken);

        return new WordSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<Word> ApplySorting(
        IQueryable<Word> query,
        WordSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (WordSortBy.CreatedAt, WordSortDirection.Desc) => query
                .OrderByDescending(word => word.CreatedAt)
                .ThenBy(word => word.Text),
            (WordSortBy.CreatedAt, _) => query
                .OrderBy(word => word.CreatedAt)
                .ThenBy(word => word.Text),
            (WordSortBy.Text, WordSortDirection.Desc) => query
                .OrderByDescending(word => word.Text)
                .ThenBy(word => word.Id),
            _ => query
                .OrderBy(word => word.Text)
                .ThenBy(word => word.Id)
        };
    }
}
