namespace EnglishMaster.Application.Features.Categories.Queries;

public sealed record SearchCategoriesQuery(
    string? Search,
    bool? IsActive);
