using EnglishMaster.Contracts.ContentQuality;

namespace EnglishMaster.Web.Services.ContentQuality;

public interface IContentQualityApiClient
{
    Task<ContentQualityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);

    Task<ContentQualityCheckSearchResponse> SearchChecksAsync(string? contentType, string? status, CancellationToken cancellationToken);

    Task<ContentQualityCheckDto> GetCheckAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentQualityRuleSearchResponse> SearchRulesAsync(string? contentType, string? severity, bool? isActive, CancellationToken cancellationToken);

    Task<ContentQualityRuleDto> CreateRuleAsync(CreateContentQualityRuleRequest request, CancellationToken cancellationToken);

    Task<ContentQualityRuleDto> UpdateRuleAsync(Guid id, UpdateContentQualityRuleRequest request, CancellationToken cancellationToken);

    Task<ContentQualityCheckDto> RunAsync(string contentType, Guid contentId, CancellationToken cancellationToken);

    Task<ContentQualityFindingDto> ResolveFindingAsync(Guid id, CancellationToken cancellationToken);
}
