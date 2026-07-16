using EnglishMaster.Application.Features.PublishJobs;
using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakePublishJobRepository : IPublishJobRepository
{
    public List<PublishJob> PublishJobs { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(PublishJob publishJob, CancellationToken cancellationToken)
    {
        PublishJobs.Add(publishJob);
        return Task.CompletedTask;
    }

    public Task<PublishJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(PublishJobs.SingleOrDefault(job => job.Id == id));
    }

    public Task<PublishJobSearchResult> SearchAsync(PublishJobSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var query = PublishJobs.AsEnumerable();

        if (criteria.SourceType.HasValue)
        {
            query = query.Where(job => job.SourceType == criteria.SourceType.Value);
        }

        if (criteria.SourceId.HasValue)
        {
            query = query.Where(job => job.SourceId == criteria.SourceId.Value);
        }

        if (criteria.Format.HasValue)
        {
            query = query.Where(job => job.Format == criteria.Format.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(job => job.Status == criteria.Status.Value);
        }

        query = criteria.SortDirection == PublishSortDirection.Desc
            ? query.OrderByDescending(job => job.CreatedAt)
            : query.OrderBy(job => job.CreatedAt);

        var filtered = query.ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new PublishJobSearchResult(items, filtered.Length));
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(PublishJobs.Count);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
