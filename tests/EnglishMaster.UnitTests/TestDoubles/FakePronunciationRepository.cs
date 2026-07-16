using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.Pronunciations.Dtos;
using EnglishMaster.Domain.Pronunciations;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakePronunciationRepository : IPronunciationRepository
{
    public List<Pronunciation> Pronunciations { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Pronunciation pronunciation, CancellationToken cancellationToken)
    {
        Pronunciations.Add(pronunciation);
        return Task.CompletedTask;
    }

    public Task<Pronunciation?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Pronunciations.SingleOrDefault(pronunciation => pronunciation.Id == id));
    }

    public Task<Pronunciation?> GetByWordIdAsync(Guid wordId, CancellationToken cancellationToken)
    {
        return Task.FromResult(Pronunciations.SingleOrDefault(pronunciation => pronunciation.WordId == wordId));
    }

    public Task<bool> WordHasPronunciationAsync(
        Guid wordId,
        Guid? excludedPronunciationId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Pronunciations.Any(pronunciation =>
            pronunciation.WordId == wordId &&
            (!excludedPronunciationId.HasValue || pronunciation.Id != excludedPronunciationId.Value)));
    }

    public Task<PronunciationSearchResult> SearchAsync(
        PronunciationSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Pronunciations.AsEnumerable();
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
            query = query.Where(pronunciation =>
                pronunciation.IpaUk.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                pronunciation.IpaUs.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                pronunciation.ThaiReading.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderByDescending(pronunciation => pronunciation.CreatedAt)
            .ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new PronunciationSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
