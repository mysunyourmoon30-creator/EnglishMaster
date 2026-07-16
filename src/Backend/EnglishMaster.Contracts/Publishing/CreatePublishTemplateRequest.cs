namespace EnglishMaster.Contracts.Publishing;

public sealed record CreatePublishTemplateRequest(
    string Name,
    string? Description,
    string Format,
    string? TemplateContent,
    bool IsDefault);
