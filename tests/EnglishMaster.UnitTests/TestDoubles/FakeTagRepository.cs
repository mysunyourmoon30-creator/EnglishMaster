using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Domain.Tags;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeTagRepository : ITagRepository
{
    public List<Tag> Tags { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        Tags.Add(tag);
        return Task.CompletedTask;
    }

    public Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Tags.SingleOrDefault(tag => tag.Id == id));
    }

    public Task<IReadOnlyCollection<Tag>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idSet = ids.ToHashSet();
        IReadOnlyCollection<Tag> tags = Tags
            .Where(tag => idSet.Contains(tag.Id))
            .ToArray();

        return Task.FromResult(tags);
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedTagId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Tags.Any(tag =>
            tag.Slug == slug &&
            (!excludedTagId.HasValue || tag.Id != excludedTagId.Value)));
    }

    public Task<TagSearchResult> SearchAsync(
        TagSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Tags.AsEnumerable();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(tag => tag.IsActive == criteria.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(tag =>
                tag.Name.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                tag.Description.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(new TagSearchResult(query
            .OrderBy(tag => tag.Name)
            .ToArray()));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
