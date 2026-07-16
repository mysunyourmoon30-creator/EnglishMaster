namespace EnglishMaster.Application.Features.ContentQuality.Dtos;

public sealed record ContentQualityRuleDto(
    Guid Id,
    string Code,
    string Name,
    string Description,
    string ContentType,
    string Severity,
    bool IsActive,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ContentQualityFindingDto(
    Guid Id,
    Guid ContentQualityCheckId,
    string RuleCode,
    string Severity,
    string Message,
    string FieldName,
    string Recommendation,
    bool IsResolved,
    DateTimeOffset? ResolvedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ContentQualityCheckDto(
    Guid Id,
    string ContentType,
    Guid ContentId,
    string Status,
    DateTimeOffset CheckedAt,
    string CheckedBy,
    int Score,
    IReadOnlyCollection<ContentQualityFindingDto> Findings,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ContentQualityRuleSearchResponse(
    IReadOnlyCollection<ContentQualityRuleDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record ContentQualityCheckSearchResponse(
    IReadOnlyCollection<ContentQualityCheckDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record ContentQualityDashboardDto(
    int TotalChecks,
    int FailedChecks,
    int WarningChecks,
    int CriticalFindings,
    double AverageScore,
    IReadOnlyCollection<ContentQualityFindingDto> RecentFindings);

public sealed record ContentQualityFindingCandidate(
    string RuleCode,
    string Severity,
    string Message,
    string FieldName,
    string Recommendation);
