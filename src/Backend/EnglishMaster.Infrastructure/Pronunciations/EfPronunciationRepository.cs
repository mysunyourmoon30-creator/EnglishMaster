using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Pronunciations;

internal sealed class EfPronunciationRepository : IPronunciationRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfPronunciationRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Pronunciation pronunciation, CancellationToken cancellationToken)
    {
        await dbContext.Pronunciations.AddAsync(pronunciation, cancellationToken);
    }

    public async Task<Pronunciation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Pronunciations
            .Include(pronunciation => pronunciation.MinimalPairs)
            .FirstOrDefaultAsync(pronunciation => pronunciation.Id == id, cancellationToken);
    }

    public async Task<Pronunciation?> GetByWordIdAsync(Guid wordId, CancellationToken cancellationToken)
    {
        return await dbContext.Pronunciations
            .Include(pronunciation => pronunciation.MinimalPairs)
            .FirstOrDefaultAsync(pronunciation => pronunciation.WordId == wordId, cancellationToken);
    }

    public async Task<bool> WordHasPronunciationAsync(
        Guid wordId,
        Guid? excludedPronunciationId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Pronunciations.AsNoTracking()
            .Where(pronunciation => pronunciation.WordId == wordId);

        if (excludedPronunciationId.HasValue)
        {
            query = query.Where(pronunciation => pronunciation.Id != excludedPronunciationId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<PronunciationSearchResult> SearchAsync(
        PronunciationSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<Pronunciation> query = dbContext.Pronunciations.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(pronunciation => pronunciation.IsActive == criteria.IsActive.Value);
        }

        if (criteria.WordId.HasValue)
        {
            query = query.Where(pronunciation => pronunciation.WordId == criteria.WordId.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(pronunciation =>
                pronunciation.IpaUk.ToLower().Contains(searchTerm) ||
                pronunciation.IpaUs.ToLower().Contains(searchTerm) ||
                pronunciation.ThaiReading.ToLower().Contains(searchTerm) ||
                pronunciation.Syllables.ToLower().Contains(searchTerm) ||
                pronunciation.StressPattern.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new PronunciationSearchResult([], totalCount);
        }

        var items = await query
            .OrderByDescending(pronunciation => pronunciation.CreatedAt)
            .ThenBy(pronunciation => pronunciation.IpaUk)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PronunciationSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
