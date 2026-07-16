using EnglishMaster.Application.Features.PublishJobs;
using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Publishing;

internal sealed class EfPublishJobRepository : IPublishJobRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfPublishJobRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(PublishJob publishJob, CancellationToken cancellationToken)
    {
        await dbContext.PublishJobs.AddAsync(publishJob, cancellationToken);
    }

    public async Task<PublishJob?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.PublishJobs
            .Include(job => job.Artifacts)
            .FirstOrDefaultAsync(job => job.Id == id, cancellationToken);
    }

    public async Task<PublishJobSearchResult> SearchAsync(PublishJobSearchCriteria criteria, CancellationToken cancellationToken)
    {
        IQueryable<PublishJob> query = dbContext.PublishJobs.AsNoTracking();

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

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new PublishJobSearchResult([], totalCount);
        }

        var items = await ApplySorting(query, criteria)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PublishJobSearchResult(items, totalCount);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken)
    {
        return dbContext.PublishJobs.AsNoTracking().CountAsync(cancellationToken);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    private static IQueryable<PublishJob> ApplySorting(IQueryable<PublishJob> query, PublishJobSearchCriteria criteria)
    {
        return (criteria.SortBy, criteria.SortDirection) switch
        {
            (PublishJobSortBy.Title, PublishSortDirection.Desc) => query.OrderByDescending(job => job.Title).ThenByDescending(job => job.CreatedAt),
            (PublishJobSortBy.Title, _) => query.OrderBy(job => job.Title).ThenByDescending(job => job.CreatedAt),
            (PublishJobSortBy.Status, PublishSortDirection.Desc) => query.OrderByDescending(job => job.Status).ThenByDescending(job => job.CreatedAt),
            (PublishJobSortBy.Status, _) => query.OrderBy(job => job.Status).ThenByDescending(job => job.CreatedAt),
            (PublishJobSortBy.CreatedAt, PublishSortDirection.Asc) => query.OrderBy(job => job.CreatedAt).ThenBy(job => job.Title),
            _ => query.OrderByDescending(job => job.CreatedAt).ThenBy(job => job.Title)
        };
    }
}
