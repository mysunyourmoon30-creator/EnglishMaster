namespace EnglishMaster.Contracts.Categories;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    int SortOrder,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
