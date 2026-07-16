namespace EnglishMaster.Application.Features.Categories.Commands;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    int SortOrder);
