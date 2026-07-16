using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Categories;

internal sealed class EfCategoryRepository : ICategoryRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfCategoryRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await dbContext.Categories.AddAsync(category, cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .FirstOrDefaultAsync(category => category.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Category>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idSet = ids.Distinct().ToArray();
        return await dbContext.Categories.AsNoTracking()
            .Where(category => idSet.Contains(category.Id))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedCategoryId,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Categories.AsNoTracking()
            .Where(category => category.Slug == slug);

        if (excludedCategoryId.HasValue)
        {
            query = query.Where(category => category.Id != excludedCategoryId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<CategorySearchResult> SearchAsync(
        CategorySearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Categories.AsNoTracking();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(category => category.IsActive == criteria.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            var searchTerm = criteria.SearchTerm.Trim().ToLower();
            query = query.Where(category =>
                category.Name.ToLower().Contains(searchTerm) ||
                category.Slug.ToLower().Contains(searchTerm) ||
                category.Description.ToLower().Contains(searchTerm));
        }

        var items = await query
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToArrayAsync(cancellationToken);

        return new CategorySearchResult(items);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
