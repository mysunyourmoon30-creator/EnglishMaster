using EnglishMaster.Application.Features.ContentQuality;
using EnglishMaster.Application.Features.ContentQuality.Dtos;
using EnglishMaster.Domain.ContentQuality;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.ContentQuality;

public sealed class EfContentQualityRepository : IContentQualityRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfContentQualityRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<ContentQualityRuleDto> AddRuleAsync(ContentQualityRule rule, CancellationToken cancellationToken)
    {
        dbContext.ContentQualityRules.Add(rule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToRuleDto(rule);
    }

    public async Task<ContentQualityRule?> GetRuleEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.ContentQualityRules.SingleOrDefaultAsync(rule => rule.Id == id, cancellationToken);

    public async Task<bool> RuleCodeExistsAsync(string code, Guid? excludingId, CancellationToken cancellationToken) =>
        await dbContext.ContentQualityRules.AnyAsync(rule => rule.Code == code && (!excludingId.HasValue || rule.Id != excludingId.Value), cancellationToken);

    public async Task<ContentQualityRuleDto> SaveRuleAsync(ContentQualityRule rule, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToRuleDto(rule);
    }

    public async Task<ContentQualityRuleSearchResponse> SearchRulesAsync(string? contentType, string? severity, bool? isActive, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.ContentQualityRules.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            query = query.Where(rule => rule.ContentType == contentType.Trim());
        }

        if (Enum.TryParse<ContentQualitySeverity>(severity, ignoreCase: true, out var parsedSeverity))
        {
            query = query.Where(rule => rule.Severity == parsedSeverity);
        }

        if (isActive.HasValue)
        {
            query = query.Where(rule => rule.IsActive == isActive.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var entities = await query.OrderBy(rule => rule.ContentType).ThenBy(rule => rule.Code)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new ContentQualityRuleSearchResponse(entities.Select(ToRuleDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<ContentQualityCheckDto> AddCheckAsync(ContentQualityCheck check, CancellationToken cancellationToken)
    {
        dbContext.ContentQualityChecks.Add(check);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToCheckDto(check);
    }

    public async Task<ContentQualityCheckDto?> GetCheckAsync(Guid id, CancellationToken cancellationToken)
    {
        var check = await dbContext.ContentQualityChecks.AsNoTracking()
            .Include(item => item.Findings)
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return check is null ? null : ToCheckDto(check);
    }

    public async Task<ContentQualityCheckDto?> GetLatestCheckAsync(string contentType, Guid contentId, CancellationToken cancellationToken)
    {
        var check = await dbContext.ContentQualityChecks.AsNoTracking()
            .Include(item => item.Findings)
            .Where(item => item.ContentType == contentType && item.ContentId == contentId)
            .OrderByDescending(item => item.CheckedAt)
            .FirstOrDefaultAsync(cancellationToken);
        return check is null ? null : ToCheckDto(check);
    }

    public async Task<ContentQualityCheckSearchResponse> SearchChecksAsync(string? contentType, string? status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        IQueryable<ContentQualityCheck> query = dbContext.ContentQualityChecks.AsNoTracking().Include(check => check.Findings);
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            query = query.Where(check => check.ContentType == contentType.Trim());
        }

        if (Enum.TryParse<ContentQualityCheckStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(check => check.Status == parsedStatus);
        }

        var total = await query.CountAsync(cancellationToken);
        var entities = await query.OrderByDescending(check => check.CheckedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new ContentQualityCheckSearchResponse(entities.Select(ToCheckDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<IReadOnlyCollection<ContentQualityFindingDto>> GetFindingsAsync(Guid checkId, bool? isResolved, CancellationToken cancellationToken)
    {
        var query = dbContext.ContentQualityFindings.AsNoTracking()
            .Where(finding => finding.ContentQualityCheckId == checkId);
        if (isResolved.HasValue)
        {
            query = query.Where(finding => finding.IsResolved == isResolved.Value);
        }

        var findings = await query.OrderByDescending(finding => finding.Severity).ThenBy(finding => finding.RuleCode).ToArrayAsync(cancellationToken);
        return findings.Select(ToFindingDto).ToArray();
    }

    public async Task<ContentQualityFindingDto?> MarkFindingResolvedAsync(Guid id, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var finding = await dbContext.ContentQualityFindings.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (finding is null)
        {
            return null;
        }

        finding.MarkResolved(now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToFindingDto(finding);
    }

    public async Task<ContentQualityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var total = await dbContext.ContentQualityChecks.CountAsync(cancellationToken);
        var failed = await dbContext.ContentQualityChecks.CountAsync(check => check.Status == ContentQualityCheckStatus.Failed, cancellationToken);
        var warning = await dbContext.ContentQualityChecks.CountAsync(check => check.Status == ContentQualityCheckStatus.Warning, cancellationToken);
        var critical = await dbContext.ContentQualityFindings.CountAsync(finding => finding.Severity == ContentQualitySeverity.Critical && !finding.IsResolved, cancellationToken);
        var average = total == 0 ? 0 : await dbContext.ContentQualityChecks.AverageAsync(check => check.Score, cancellationToken);
        var recent = await dbContext.ContentQualityFindings.AsNoTracking()
            .OrderByDescending(finding => finding.CreatedAt)
            .Take(10)
            .ToArrayAsync(cancellationToken);
        return new ContentQualityDashboardDto(total, failed, warning, critical, average, recent.Select(ToFindingDto).ToArray());
    }

    private static ContentQualityRuleDto ToRuleDto(ContentQualityRule rule) =>
        new(rule.Id, rule.Code, rule.Name, rule.Description, rule.ContentType, rule.Severity.ToString(), rule.IsActive, rule.CreatedAt, rule.UpdatedAt);

    private static ContentQualityCheckDto ToCheckDto(ContentQualityCheck check) =>
        new(check.Id, check.ContentType, check.ContentId, check.Status.ToString(), check.CheckedAt, check.CheckedBy, check.Score, check.Findings.Select(ToFindingDto).ToArray(), check.CreatedAt, check.UpdatedAt);

    private static ContentQualityFindingDto ToFindingDto(ContentQualityFinding finding) =>
        new(finding.Id, finding.ContentQualityCheckId, finding.RuleCode, finding.Severity.ToString(), finding.Message, finding.FieldName, finding.Recommendation, finding.IsResolved, finding.ResolvedAt, finding.CreatedAt, finding.UpdatedAt);
}
