namespace EnglishMaster.Application.Features.PublishTemplates.Queries;

public sealed record GetPublishTemplateByIdQuery(Guid Id);

public sealed record SearchPublishTemplatesQuery(
    string? Format,
    bool? IsDefault,
    bool? IsActive,
    int? PageNumber,
    int? PageSize);
