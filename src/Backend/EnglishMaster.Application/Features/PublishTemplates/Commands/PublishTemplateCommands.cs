namespace EnglishMaster.Application.Features.PublishTemplates.Commands;

public sealed record CreatePublishTemplateCommand(
    string? Name,
    string? Description,
    string? Format,
    string? TemplateContent,
    bool IsDefault);

public sealed record UpdatePublishTemplateCommand(
    Guid Id,
    string? Name,
    string? Description,
    string? Format,
    string? TemplateContent,
    bool IsDefault,
    bool IsActive);

public sealed record DeletePublishTemplateCommand(Guid Id);

public sealed record ActivatePublishTemplateCommand(Guid Id);

public sealed record DeactivatePublishTemplateCommand(Guid Id);
