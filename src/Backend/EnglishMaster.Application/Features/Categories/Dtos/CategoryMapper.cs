using EnglishMaster.Contracts.Categories;
using EnglishMaster.Domain.Categories;

namespace EnglishMaster.Application.Features.Categories.Dtos;

internal static class CategoryMapper
{
    public static CategoryDto ToDto(Category category)
    {
        return new CategoryDto(
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.SortOrder,
            category.IsActive,
            category.CreatedAt,
            category.UpdatedAt);
    }
}
