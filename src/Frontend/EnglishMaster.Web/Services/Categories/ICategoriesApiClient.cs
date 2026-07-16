using EnglishMaster.Contracts.Categories;

namespace EnglishMaster.Web.Services.Categories;

public interface ICategoriesApiClient
{
    Task<CategorySearchResponse> SearchAsync(
        string? search,
        bool? isActive,
        CancellationToken cancellationToken);

    Task<CategoryDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken);

    Task<CategoryDto> UpdateAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
