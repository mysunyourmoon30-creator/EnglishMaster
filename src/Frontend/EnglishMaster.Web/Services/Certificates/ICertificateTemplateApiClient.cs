using EnglishMaster.Contracts.Certificates;

namespace EnglishMaster.Web.Services.Certificates;

public interface ICertificateTemplateApiClient
{
    Task<CertificateTemplateSearchResponse> SearchAsync(
        string? search,
        bool? isActive,
        CancellationToken cancellationToken);

    Task<CertificateTemplateDto?> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<CertificateTemplateDto> CreateAsync(CreateCertificateTemplateRequest request, CancellationToken cancellationToken);

    Task<CertificateTemplateDto> UpdateAsync(Guid id, UpdateCertificateTemplateRequest request, CancellationToken cancellationToken);

    Task ActivateAsync(Guid id, CancellationToken cancellationToken);

    Task DeactivateAsync(Guid id, CancellationToken cancellationToken);
}
