using EnglishMaster.Application.Features.MinimalPairs;
using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeMinimalPairRepository : IMinimalPairRepository
{
    public List<MinimalPair> MinimalPairs { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(MinimalPair minimalPair, CancellationToken cancellationToken)
    {
        MinimalPairs.Add(minimalPair);
        return Task.CompletedTask;
    }

    public Task<MinimalPair?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(MinimalPairs.SingleOrDefault(minimalPair => minimalPair.Id == id));
    }

    public Task<IReadOnlyCollection<MinimalPair>> GetByPronunciationIdAsync(
        Guid pronunciationId,
        CancellationToken cancellationToken)
    {
        IReadOnlyCollection<MinimalPair> minimalPairs = MinimalPairs
            .Where(minimalPair => minimalPair.PronunciationId == pronunciationId)
            .OrderBy(minimalPair => minimalPair.SortOrder)
            .ToArray();

        return Task.FromResult(minimalPairs);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
