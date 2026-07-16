using EnglishMaster.Application.Features.ContentQuality.Dtos;
using EnglishMaster.Domain.ContentQuality;

namespace EnglishMaster.Application.Features.ContentQuality;

public interface IContentQualityRepository
{
    Task<ContentQualityRuleDto> AddRuleAsync(ContentQualityRule rule, CancellationToken cancellationToken);

    Task<ContentQualityRule?> GetRuleEntityAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> RuleCodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken);

    Task<ContentQualityRuleDto> SaveRuleAsync(ContentQualityRule rule, CancellationToken cancellationToken);

    Task<ContentQualityRuleSearchResponse> SearchRulesAsync(string? contentType, string? severity, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<ContentQualityCheckDto> AddCheckAsync(ContentQualityCheck check, CancellationToken cancellationToken);

    Task<ContentQualityCheckDto?> GetCheckAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentQualityCheckDto?> GetLatestCheckAsync(string contentType, Guid contentId, CancellationToken cancellationToken);

    Task<ContentQualityCheckSearchResponse> SearchChecksAsync(string? contentType, string? status, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<ContentQualityFindingDto>> GetFindingsAsync(Guid checkId, bool? isResolved, CancellationToken cancellationToken);

    Task<ContentQualityFindingDto?> MarkFindingResolvedAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken);

    Task<ContentQualityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
}

public interface IContentQualityService
{
    Task<ContentQualityCheckDto?> RunAsync(string contentType, Guid contentId, string? checkedBy, CancellationToken cancellationToken);
}

public interface IContentQualityRuleProvider
{
    IReadOnlyCollection<ContentQualityFindingCandidate> Evaluate(string contentType, object content);
}
