using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Certificates;

namespace EnglishMaster.Web.Services.Certificates;

public sealed class CertificateTemplateFormModel
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string BodyTemplate { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public static CertificateTemplateFormModel FromDto(CertificateTemplateDto template)
    {
        return new CertificateTemplateFormModel
        {
            Code = template.Code,
            Name = template.Name,
            Description = template.Description,
            BodyTemplate = template.BodyTemplate,
            IsActive = template.IsActive
        };
    }

    public CreateCertificateTemplateRequest ToCreateRequest()
    {
        return new CreateCertificateTemplateRequest(Code, Name, Description, BodyTemplate);
    }

    public UpdateCertificateTemplateRequest ToUpdateRequest()
    {
        return new UpdateCertificateTemplateRequest(Name, Description, BodyTemplate, IsActive);
    }
}
