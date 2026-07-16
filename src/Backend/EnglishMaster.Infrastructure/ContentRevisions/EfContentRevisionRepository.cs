using EnglishMaster.Application.Features.ContentRevisionRestores.Dtos;
using EnglishMaster.Application.Features.ContentRevisions;
using EnglishMaster.Application.Features.ContentRevisions.Dtos;
using EnglishMaster.Domain.ContentRevisions;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.ContentRevisions;

public sealed class EfContentRevisionRepository : IContentRevisionRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfContentRevisionRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<int> GetNextRevisionNumberAsync(string contentType, Guid contentId, CancellationToken cancellationToken)
    {
        var latest = await dbContext.ContentRevisions
            .Where(revision => revision.ContentType == contentType && revision.ContentId == contentId)
            .MaxAsync(revision => (int?)revision.RevisionNumber, cancellationToken);
        return (latest ?? 0) + 1;
    }

    public async Task<ContentRevisionDto> AddRevisionAsync(ContentRevision revision, CancellationToken cancellationToken)
    {
        dbContext.ContentRevisions.Add(revision);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToRevisionDto(revision);
    }

    public async Task<ContentRevisionDto?> GetRevisionAsync(Guid id, CancellationToken cancellationToken)
    {
        var revision = await dbContext.ContentRevisions.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return revision is null ? null : ToRevisionDto(revision);
    }

    public async Task<ContentRevisionDto?> GetLatestRevisionAsync(string contentType, Guid contentId, CancellationToken cancellationToken)
    {
        var normalized = Normalize(contentType);
        var revision = await dbContext.ContentRevisions.AsNoTracking()
            .Where(item => item.ContentType == normalized && item.ContentId == contentId)
            .OrderByDescending(item => item.RevisionNumber)
            .FirstOrDefaultAsync(cancellationToken);
        return revision is null ? null : ToRevisionDto(revision);
    }

    public async Task<ContentRevisionSearchResponse> SearchRevisionsAsync(string? contentType, Guid? contentId, string? eventType, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.ContentRevisions.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(contentType))
        {
            var normalized = Normalize(contentType);
            query = query.Where(revision => revision.ContentType == normalized);
        }

        if (contentId.HasValue)
        {
            query = query.Where(revision => revision.ContentId == contentId.Value);
        }

        if (Enum.TryParse<ContentRevisionEventType>(eventType, ignoreCase: true, out var parsedEvent))
        {
            query = query.Where(revision => revision.EventType == parsedEvent);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(revision => revision.ChangedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new ContentRevisionSearchResponse(items.Select(ToRevisionDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    public async Task<ContentRevisionRestoreRequestDto> AddRestoreRequestAsync(ContentRevisionRestoreRequest request, CancellationToken cancellationToken)
    {
        dbContext.ContentRevisionRestoreRequests.Add(request);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToRestoreDto(request);
    }

    public async Task<ContentRevisionRestoreRequestDto?> GetRestoreRequestAsync(Guid id, CancellationToken cancellationToken)
    {
        var request = await dbContext.ContentRevisionRestoreRequests.AsNoTracking().SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return request is null ? null : ToRestoreDto(request);
    }

    public async Task<ContentRevisionRestoreRequest?> GetRestoreRequestEntityAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.ContentRevisionRestoreRequests.SingleOrDefaultAsync(request => request.Id == id, cancellationToken);

    public async Task<ContentRevisionRestoreRequestDto> SaveRestoreRequestAsync(ContentRevisionRestoreRequest request, CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToRestoreDto(request);
    }

    public async Task<ContentRevisionRestoreRequestSearchResponse> SearchRestoreRequestsAsync(string? status, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var query = dbContext.ContentRevisionRestoreRequests.AsNoTracking();
        if (Enum.TryParse<ContentRevisionRestoreStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            query = query.Where(request => request.Status == parsedStatus);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(request => request.RequestedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)pageSize);
        return new ContentRevisionRestoreRequestSearchResponse(items.Select(ToRestoreDto).ToArray(), pageNumber, pageSize, total, totalPages, pageNumber > 1, pageNumber < totalPages);
    }

    private static ContentRevisionDto ToRevisionDto(ContentRevision revision) =>
        new(revision.Id, revision.ContentType, revision.ContentId, revision.RevisionNumber, revision.EventType.ToString(), revision.Title, revision.Summary, revision.ChangedBy, revision.ChangedAt, revision.ChangeReason, revision.SnapshotJson, revision.DiffJson, revision.CreatedAt, revision.UpdatedAt);

    private static ContentRevisionRestoreRequestDto ToRestoreDto(ContentRevisionRestoreRequest request) =>
        new(request.Id, request.ContentRevisionId, request.RequestedBy, request.RequestedAt, request.Reason, request.Status.ToString(), request.ReviewedBy, request.ReviewedAt, request.ReviewNote, request.CreatedAt, request.UpdatedAt);

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
