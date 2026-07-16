using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.GrammarTopics.Dtos;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeGrammarTopicRepository : IGrammarTopicRepository
{
    public List<GrammarTopic> GrammarTopics { get; } = [];

    public Task AddAsync(GrammarTopic grammarTopic, CancellationToken cancellationToken)
    {
        GrammarTopics.Add(grammarTopic);
        return Task.CompletedTask;
    }

    public Task<GrammarTopic?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(GrammarTopics.SingleOrDefault(topic => topic.Id == id));
    }

    public Task<IReadOnlyCollection<GrammarTopic>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var normalizedIds = ids.Distinct().ToHashSet();
        return Task.FromResult<IReadOnlyCollection<GrammarTopic>>(
            GrammarTopics.Where(topic => normalizedIds.Contains(topic.Id)).ToArray());
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedGrammarTopicId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(GrammarTopics.Any(topic =>
            topic.Slug == slug &&
            (!excludedGrammarTopicId.HasValue || topic.Id != excludedGrammarTopicId.Value)));
    }

    public Task<GrammarTopicSearchResult> SearchAsync(
        GrammarTopicSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = GrammarTopics.AsEnumerable();
        if (criteria.IsActive.HasValue)
        {
            query = query.Where(topic => topic.IsActive == criteria.IsActive.Value);
        }

        if (criteria.CefrLevel.HasValue)
        {
            query = query.Where(topic => topic.CefrLevel == criteria.CefrLevel.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(topic =>
                topic.Title.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                topic.Summary.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderBy(topic => topic.SortOrder)
            .ThenBy(topic => topic.Title)
            .ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new GrammarTopicSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(1);
    }
}
