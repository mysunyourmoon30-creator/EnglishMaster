namespace EnglishMaster.Contracts.Publishing;

public sealed record UpdatePublishTemplateRequest(
    string Name,
    string? Description,
    string Format,
    string? TemplateContent,
    bool IsDefault,
    bool IsActive);
