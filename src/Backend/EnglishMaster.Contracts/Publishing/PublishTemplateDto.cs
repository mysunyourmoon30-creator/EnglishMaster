namespace EnglishMaster.Contracts.Publishing;

public sealed record PublishTemplateDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string Format,
    string TemplateContent,
    bool IsDefault,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
