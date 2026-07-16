using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.Application.Features.MinimalPairs;

public interface IMinimalPairRepository
{
    Task AddAsync(MinimalPair minimalPair, CancellationToken cancellationToken);

    Task<MinimalPair?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<MinimalPair>> GetByPronunciationIdAsync(
        Guid pronunciationId,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
