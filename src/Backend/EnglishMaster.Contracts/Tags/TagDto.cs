namespace EnglishMaster.Contracts.Tags;

public sealed record TagDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
