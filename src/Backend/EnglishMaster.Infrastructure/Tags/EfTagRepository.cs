using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Tags.Dtos;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Tags;

internal sealed class EfTagRepository : ITagRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfTagRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Tag tag, CancellationToken cancellationToken)
    {
        await dbContext.Tags.AddAsync(tag, cancellationToken);
    }

    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Tags
            .FirstOrDefaultAsync(tag => tag.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Tag>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idSet = ids.Distinct().ToArray();
        return await dbContext.Tags.AsNoTracking()
            .Where(tag => idSet.Contains(tag.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedTagId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Tags.AsNoTracking()
            .Where(tag => tag.Slug == slug);

        if (excludedTagId.HasValue)
        {
            query = query.Where(tag => tag.Id != excludedTagId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<TagSearchResult> SearchAsync(
        TagSearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Tags.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(tag => tag.IsActive == criteria.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(tag =>
                tag.Name.ToLower().Contains(searchTerm) ||
                tag.Slug.ToLower().Contains(searchTerm) ||
                tag.Description.ToLower().Contains(searchTerm));
        }

        var items = await query
            .OrderBy(tag => tag.Name)
            .ToArrayAsync(cancellationToken);

        return new TagSearchResult(items);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
