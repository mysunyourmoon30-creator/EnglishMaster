namespace EnglishMaster.Application.Features.Categories.Dtos;

internal sealed record CategoryInput(
    string Name,
    string Slug,
    string Description,
    int SortOrder,
    bool IsActive);
