namespace EnglishMaster.Contracts.Categories;

public sealed record CreateCategoryRequest(
    string Name,
    string? Description,
    int SortOrder = 0);
