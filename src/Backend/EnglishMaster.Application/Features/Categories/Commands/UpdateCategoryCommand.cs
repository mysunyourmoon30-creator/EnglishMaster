namespace EnglishMaster.Application.Features.Categories.Commands;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    int SortOrder,
    bool IsActive);
