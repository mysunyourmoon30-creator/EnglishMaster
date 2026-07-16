using EnglishMaster.Application.Features.PublishedArtifacts;
using EnglishMaster.Application.Features.PublishedArtifacts.Dtos;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Publishing;

internal sealed class EfPublishedArtifactRepository : IPublishedArtifactRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfPublishedArtifactRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(PublishedArtifact artifact, CancellationToken cancellationToken)
    {
        await dbContext.PublishedArtifacts.AddAsync(artifact, cancellationToken);
    }

    public Task<PublishedArtifact?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return dbContext.PublishedArtifacts.AsNoTracking().FirstOrDefaultAsync(artifact => artifact.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<PublishedArtifact>> GetByPublishJobIdAsync(Guid publishJobId, CancellationToken cancellationToken)
    {
        return await dbContext.PublishedArtifacts.AsNoTracking()
            .Where(artifact => artifact.PublishJobId == publishJobId)
            .OrderByDescending(artifact => artifact.CreatedAt)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<PublishedArtifactSearchResult> SearchAsync(PublishedArtifactSearchCriteria criteria, CancellationToken cancellationToken)
    {
        IQueryable<PublishedArtifact> query = dbContext.PublishedArtifacts.AsNoTracking();

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

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new PublishedArtifactSearchResult([], totalCount);
        }

        var items = await query
            .OrderByDescending(artifact => artifact.CreatedAt)
            .ThenBy(artifact => artifact.FileName)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new PublishedArtifactSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
