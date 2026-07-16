using EnglishMaster.Application.Features.ContentRevisionRestores.Dtos;
using EnglishMaster.Application.Features.ContentRevisions.Dtos;
using EnglishMaster.Domain.ContentRevisions;

namespace EnglishMaster.Application.Features.ContentRevisions;

public interface IContentRevisionRepository
{
    Task<int> GetNextRevisionNumberAsync(string contentType, Guid contentId, CancellationToken cancellationToken);

    Task<ContentRevisionDto> AddRevisionAsync(ContentRevision revision, CancellationToken cancellationToken);

    Task<ContentRevisionDto?> GetRevisionAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentRevisionDto?> GetLatestRevisionAsync(string contentType, Guid contentId, CancellationToken cancellationToken);

    Task<ContentRevisionSearchResponse> SearchRevisionsAsync(string? contentType, Guid? contentId, string? eventType, int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> AddRestoreRequestAsync(ContentRevisionRestoreRequest request, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto?> GetRestoreRequestAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequest?> GetRestoreRequestEntityAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> SaveRestoreRequestAsync(ContentRevisionRestoreRequest request, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestSearchResponse> SearchRestoreRequestsAsync(string? status, int pageNumber, int pageSize, CancellationToken cancellationToken);
}

public interface IContentRevisionService
{
    Task<ContentRevisionDto> CreateAsync(string contentType, Guid contentId, string eventType, string? title, string? summary, string? changedBy, string? changeReason, string snapshotJson, string? diffJson, CancellationToken cancellationToken);
}

public interface IContentSnapshotSerializer
{
    string SanitizeSnapshot(string snapshotJson);
}
