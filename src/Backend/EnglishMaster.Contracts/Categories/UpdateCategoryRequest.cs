namespace EnglishMaster.Contracts.Categories;

public sealed record UpdateCategoryRequest(
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive);
