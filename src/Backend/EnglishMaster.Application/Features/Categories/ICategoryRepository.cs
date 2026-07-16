using EnglishMaster.Application.Features.Categories.Dtos;
using EnglishMaster.Domain.Categories;

namespace EnglishMaster.Application.Features.Categories;

public interface ICategoryRepository
{
    Task AddAsync(Category category, CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Category>> GetByIdsAsync(
        IEnumerable<Guid> ids,
        CancellationToken cancellationToken);

    Task<bool> SlugExistsAsync(
        string slug,
        Guid? excludedCategoryId,
        CancellationToken cancellationToken);

    Task<CategorySearchResult> SearchAsync(
        CategorySearchCriteria criteria,
        CancellationToken cancellationToken);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
