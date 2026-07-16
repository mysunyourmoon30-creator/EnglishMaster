using EnglishMaster.Application.Features.Words;
using EnglishMaster.Application.Features.Words.Dtos;
using EnglishMaster.Domain.Words;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeWordRepository : IWordRepository
{
    public List<Word> Words { get; } = [];

    public Task AddAsync(Word word, CancellationToken cancellationToken)
    {
        Words.Add(word);
        return Task.CompletedTask;
    }

    public Task<Word?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Words.SingleOrDefault(word => word.Id == id));
    }

    public Task<IReadOnlyCollection<Word>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToHashSet();
        return Task.FromResult<IReadOnlyCollection<Word>>(
            Words.Where(word => normalizedIds.Contains(word.Id)).ToArray());
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedWordId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Words.Any(word =>
            word.Slug == slug &&
            (!excludedWordId.HasValue || word.Id != excludedWordId.Value)));
    }

    public Task<WordSearchResult> SearchAsync(
        WordSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Words.AsEnumerable();
        if (criteria.IsActive.HasValue)
        {
            query = query.Where(word => word.IsActive == criteria.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(word =>
                word.Text.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                word.MeaningTh.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                word.MeaningEn.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderBy(word => word.Text)
            .ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new WordSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1);
    }
}
