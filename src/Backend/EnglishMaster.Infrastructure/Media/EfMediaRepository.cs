using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Media.Dtos;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Media;

internal sealed class EfMediaRepository : IMediaRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfMediaRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(MediaEntity media, CancellationToken cancellationToken)
    {
        await dbContext.Media.AddAsync(media, cancellationToken);
    }

    public async Task<MediaEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Media
            .FirstOrDefaultAsync(media => media.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<MediaEntity>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idArray = ids
            .Where(id => id != Guid.Empty)
            .Distinct()
            .ToArray();

        if (idArray.Length == 0)
        {
            return [];
        }

        return await dbContext.Media
            .AsNoTracking()
            .Where(media => idArray.Contains(media.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<MediaSearchResult> SearchAsync(
        MediaSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        IQueryable<MediaEntity> query = dbContext.Media.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(media => media.IsActive == criteria.IsActive.Value);
        }

        if (criteria.MediaType.HasValue)
        {
            query = query.Where(media => media.MediaType == criteria.MediaType.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.ContentType))
        {
            var contentType = criteria.ContentType.Trim().ToLower();
            query = query.Where(media => media.ContentType.ToLower().Contains(contentType));
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(media =>
                media.FileName.ToLower().Contains(searchTerm) ||
                media.OriginalFileName.ToLower().Contains(searchTerm) ||
                media.AltText.ToLower().Contains(searchTerm) ||
                media.Description.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var skip = (long)(criteria.PageNumber - 1) * criteria.PageSize;
        if (skip > int.MaxValue)
        {
            return new MediaSearchResult([], totalCount);
        }

        var items = await query
            .OrderByDescending(media => media.CreatedAt)
            .ThenBy(media => media.FileName)
            .Skip((int)skip)
            .Take(criteria.PageSize)
            .ToArrayAsync(cancellationToken);

        return new MediaSearchResult(items, totalCount);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
