using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Publishing;

namespace EnglishMaster.Web.Services.Publishing;

public sealed class PublishJobFormModel
{
    [Required]
    public string SourceType { get; set; } = "Lesson";

    [Required]
    public string SourceId { get; set; } = string.Empty;

    [Required]
    public string Format { get; set; } = "Html";

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(200)]
    public string? RequestedBy { get; set; }

    public CreatePublishJobRequest ToRequest()
    {
        return new CreatePublishJobRequest(SourceType, Guid.Parse(SourceId), Format, Title, RequestedBy);
    }
}

public sealed class PublishTemplateFormModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public string Format { get; set; } = "Html";

    [StringLength(8000)]
    public string? TemplateContent { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; } = true;

    public static PublishTemplateFormModel FromDto(PublishTemplateDto template)
    {
        return new PublishTemplateFormModel
        {
            Name = template.Name,
            Description = template.Description,
            Format = template.Format,
            TemplateContent = template.TemplateContent,
            IsDefault = template.IsDefault,
            IsActive = template.IsActive
        };
    }

    public CreatePublishTemplateRequest ToCreateRequest()
    {
        return new CreatePublishTemplateRequest(Name, Description, Format, TemplateContent, IsDefault);
    }

    public UpdatePublishTemplateRequest ToUpdateRequest()
    {
        return new UpdatePublishTemplateRequest(Name, Description, Format, TemplateContent, IsDefault, IsActive);
    }
}
