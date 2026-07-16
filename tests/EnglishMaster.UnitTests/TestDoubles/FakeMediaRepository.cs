using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Media.Dtos;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeMediaRepository : IMediaRepository
{
    public List<MediaEntity> Media { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(MediaEntity media, CancellationToken cancellationToken)
    {
        Media.Add(media);
        return Task.CompletedTask;
    }

    public Task<MediaEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Media.SingleOrDefault(media => media.Id == id));
    }

    public Task<IReadOnlyCollection<MediaEntity>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idSet = ids.ToHashSet();
        IReadOnlyCollection<MediaEntity> media = Media
            .Where(item => idSet.Contains(item.Id))
            .ToArray();

        return Task.FromResult(media);
    }

    public Task<MediaSearchResult> SearchAsync(
        MediaSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Media.AsEnumerable();

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
            query = query.Where(media =>
                media.ContentType.Contains(criteria.ContentType, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(media =>
                media.FileName.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                media.OriginalFileName.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                media.AltText.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                media.Description.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        var filtered = query
            .OrderByDescending(media => media.CreatedAt)
            .ThenBy(media => media.FileName)
            .ToArray();
        var items = filtered
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToArray();

        return Task.FromResult(new MediaSearchResult(items, filtered.Length));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
