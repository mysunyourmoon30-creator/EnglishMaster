using EnglishMaster.Domain.Categories;

namespace EnglishMaster.Application.Features.Categories.Dtos;

public sealed record CategorySearchCriteria(
    string? SearchTerm,
    bool? IsActive);

public sealed record CategorySearchResult(
    IReadOnlyCollection<Category> Items);
