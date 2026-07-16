namespace EnglishMaster.Contracts.Categories;

public sealed record CategorySearchResponse(
    IReadOnlyCollection<CategoryDto> Items);
