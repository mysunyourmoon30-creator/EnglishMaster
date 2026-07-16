using EnglishMaster.Application.Features.MinimalPairs;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Pronunciations;

internal sealed class EfMinimalPairRepository : IMinimalPairRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfMinimalPairRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(MinimalPair minimalPair, CancellationToken cancellationToken)
    {
        await dbContext.MinimalPairs.AddAsync(minimalPair, cancellationToken);
    }

    public async Task<MinimalPair?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.MinimalPairs
            .FirstOrDefaultAsync(minimalPair => minimalPair.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MinimalPair>> GetByPronunciationIdAsync(
        Guid pronunciationId,
        CancellationToken cancellationToken)
    {
        return await dbContext.MinimalPairs
            .AsNoTracking()
            .Where(minimalPair => minimalPair.PronunciationId == pronunciationId)
            .OrderBy(minimalPair => minimalPair.SortOrder)
            .ThenBy(minimalPair => minimalPair.PairWordText)
            .ToArrayAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
