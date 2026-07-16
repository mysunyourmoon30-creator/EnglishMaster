using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Categories;

namespace EnglishMaster.Web.Services.Categories;

public sealed class CategoryFormModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static CategoryFormModel FromDto(CategoryDto category)
    {
        return new CategoryFormModel
        {
            Name = category.Name,
            Description = category.Description,
            SortOrder = category.SortOrder,
            IsActive = category.IsActive
        };
    }

    public CreateCategoryRequest ToCreateRequest()
    {
        return new CreateCategoryRequest(Name, Description, SortOrder);
    }

    public UpdateCategoryRequest ToUpdateRequest()
    {
        return new UpdateCategoryRequest(Name, Description, SortOrder, IsActive);
    }
}
