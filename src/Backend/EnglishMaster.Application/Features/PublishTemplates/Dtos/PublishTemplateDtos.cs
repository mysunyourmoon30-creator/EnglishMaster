using EnglishMaster.Contracts.Publishing;
using EnglishMaster.Domain.Publishing;

namespace EnglishMaster.Application.Features.PublishTemplates.Dtos;

public sealed record PublishTemplateSearchCriteria(
    PublishFormat? Format,
    bool? IsDefault,
    bool? IsActive,
    int PageNumber,
    int PageSize);

public sealed record PublishTemplateSearchResult(
    IReadOnlyCollection<PublishTemplate> Items,
    int TotalCount);

internal static class PublishTemplateMapper
{
    public static PublishTemplateDto ToDto(PublishTemplate template)
    {
        return new PublishTemplateDto(
            template.Id,
            template.Name,
            template.Slug,
            template.Description,
            template.Format.ToString(),
            template.TemplateContent,
            template.IsDefault,
            template.IsActive,
            template.CreatedAt,
            template.UpdatedAt);
    }
}
