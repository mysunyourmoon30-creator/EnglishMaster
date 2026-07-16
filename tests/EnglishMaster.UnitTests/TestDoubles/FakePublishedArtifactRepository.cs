using EnglishMaster.Application.Features.PublishedArtifacts;
using EnglishMaster.Application.Features.PublishedArtifacts.Dtos;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakePublishedArtifactRepository : IPublishedArtifactRepository
{
    public List<PublishedArtifact> Artifacts { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(PublishedArtifact artifact, CancellationToken cancellationToken)
    {
        Artifacts.Add(artifact);
        return Task.CompletedTask;
    }

    public Task<PublishedArtifact?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Artifacts.SingleOrDefault(artifact => artifact.Id == id));
    }

    public Task<IReadOnlyCollection<PublishedArtifact>> GetByPublishJobIdAsync(Guid publishJobId, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyCollection<PublishedArtifact>>(
            Artifacts.Where(artifact => artifact.PublishJobId == publishJobId).ToArray());
    }

    public Task<PublishedArtifactSearchResult> SearchAsync(PublishedArtifactSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var query = Artifacts.AsEnumerable();

        if (criteria.PublishJobId.HasValue)
        {
            query = query.Where(artifact => artifact.PublishJobId == criteria.PublishJobId.Value);
        }

        if (criteria.SourceType.HasValue)
        {
            query = query.Where(artifact => artifact.SourceType == criteria.SourceType.Value);
        }

        if (criteria.SourceId.HasValue)
        {
            query = query.Where(artifact => artifact.SourceId == criteria.SourceId.Value);
        }

        if (criteria.Format.HasValue)
        {
            query = query.Where(artifact => artifact.Format == criteria.Format.Value);
        }

        var filtered = query.ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new PublishedArtifactSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
