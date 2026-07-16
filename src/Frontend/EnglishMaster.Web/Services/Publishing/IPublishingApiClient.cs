using EnglishMaster.Contracts.Publishing;

namespace EnglishMaster.Web.Services.Publishing;

public interface IPublishingApiClient
{
    Task<PublishJobSearchResponse> SearchJobsAsync(PublishJobSearchRequest request, CancellationToken cancellationToken);

    Task<PublishJobDto?> GetJobAsync(Guid id, CancellationToken cancellationToken);

    Task<PublishJobDto> CreateJobAsync(CreatePublishJobRequest request, CancellationToken cancellationToken);

    Task<PublishJobDto> RunJobAsync(Guid id, CancellationToken cancellationToken);

    Task<PublishJobDto> CancelJobAsync(Guid id, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<PublishedArtifactDto>> GetArtifactsByJobAsync(Guid publishJobId, CancellationToken cancellationToken);

    Task<PublishTemplateSearchResponse> SearchTemplatesAsync(string? format, bool? isDefault, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<PublishTemplateDto?> GetTemplateAsync(Guid id, CancellationToken cancellationToken);

    Task<PublishTemplateDto> CreateTemplateAsync(CreatePublishTemplateRequest request, CancellationToken cancellationToken);

    Task<PublishTemplateDto> UpdateTemplateAsync(Guid id, UpdatePublishTemplateRequest request, CancellationToken cancellationToken);

    Task DeleteTemplateAsync(Guid id, CancellationToken cancellationToken);

    Task<PublishedArtifactSearchResponse> SearchArtifactsAsync(Guid? publishJobId, string? sourceType, Guid? sourceId, string? format, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
