namespace EnglishMaster.Application.Features.Tags.Dtos;

internal sealed record TagInput(
    string Name,
    string Slug,
    string Description,
    bool IsActive);
