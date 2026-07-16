using EnglishMaster.Application.Features.ContentRevisions;
using EnglishMaster.Application.Features.ContentRevisions.Dtos;
using EnglishMaster.Domain.ContentRevisions;

namespace EnglishMaster.Infrastructure.ContentRevisions;

public sealed class ContentRevisionService : IContentRevisionService
{
    private readonly IContentRevisionRepository repository;
    private readonly IContentSnapshotSerializer snapshotSerializer;
    private readonly TimeProvider timeProvider;

    public ContentRevisionService(IContentRevisionRepository repository, IContentSnapshotSerializer snapshotSerializer, TimeProvider timeProvider)
    {
        this.repository = repository;
        this.snapshotSerializer = snapshotSerializer;
        this.timeProvider = timeProvider;
    }

    public async Task<ContentRevisionDto> CreateAsync(string contentType, Guid contentId, string eventType, string? title, string? summary, string? changedBy, string? changeReason, string snapshotJson, string? diffJson, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<ContentRevisionEventType>(eventType, ignoreCase: true, out var parsedEvent))
        {
            throw new ArgumentException("EventType is invalid.", nameof(eventType));
        }

        var normalizedType = Normalize(contentType);
        var nextRevision = await repository.GetNextRevisionNumberAsync(normalizedType, contentId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var revision = ContentRevision.Create(
            normalizedType,
            contentId,
            nextRevision,
            parsedEvent,
            title,
            summary,
            changedBy,
            now,
            changeReason,
            snapshotSerializer.SanitizeSnapshot(snapshotJson),
            string.IsNullOrWhiteSpace(diffJson) ? null : snapshotSerializer.SanitizeSnapshot(diffJson),
            now);
        return await repository.AddRevisionAsync(revision, cancellationToken);
    }

    private static string Normalize(string value) => value.Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase).Trim().ToLowerInvariant();
}
