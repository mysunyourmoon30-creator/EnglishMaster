using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Domain.Categories;

namespace EnglishMaster.UnitTests.TestDoubles;

internal sealed class FakeCategoryRepository : ICategoryRepository
{
    public List<Category> Categories { get; } = [];

    public int SaveChangesCount { get; private set; }

    public Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        Categories.Add(category);
        return Task.CompletedTask;
    }

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return Task.FromResult(Categories.SingleOrDefault(category => category.Id == id));
    }

    public Task<IReadOnlyCollection<Category>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken)
    {
        var idSet = ids.ToHashSet();
        IReadOnlyCollection<Category> categories = Categories
            .Where(category => idSet.Contains(category.Id))
            .ToArray();

        return Task.FromResult(categories);
    }

    public Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedCategoryId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(Categories.Any(category =>
            category.Slug == slug &&
            (!excludedCategoryId.HasValue || category.Id != excludedCategoryId.Value)));
    }

    public Task<CategorySearchResult> SearchAsync(
        CategorySearchCriteria criteria,
        CancellationToken cancellationToken)
    {
        var query = Categories.AsEnumerable();

        if (criteria.IsActive.HasValue)
        {
            query = query.Where(category => category.IsActive == criteria.IsActive.Value);
        }

        if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
        {
            query = query.Where(category =>
                category.Name.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                category.Description.Contains(criteria.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        return Task.FromResult(new CategorySearchResult(query
            .OrderBy(category => category.SortOrder)
            .ThenBy(category => category.Name)
            .ToArray()));
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        SaveChangesCount++;
        return Task.FromResult(1);
    }
}
