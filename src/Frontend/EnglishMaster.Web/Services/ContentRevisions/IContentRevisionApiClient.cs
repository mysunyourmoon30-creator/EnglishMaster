using EnglishMaster.Contracts.ContentRevisions;

namespace EnglishMaster.Web.Services.ContentRevisions;

public interface IContentRevisionApiClient
{
    Task<ContentRevisionSearchResponse> SearchRevisionsAsync(string? contentType, Guid? contentId, string? eventType, CancellationToken cancellationToken);

    Task<ContentRevisionDto> GetRevisionAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentRevisionSearchResponse> GetRevisionsForContentAsync(string contentType, Guid contentId, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestSearchResponse> SearchRestoreRequestsAsync(string? status, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> GetRestoreRequestAsync(Guid id, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> CreateRestoreRequestAsync(CreateContentRevisionRestoreRequestRequest request, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> ApproveRestoreRequestAsync(Guid id, string? reviewNote, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> RejectRestoreRequestAsync(Guid id, string? reviewNote, CancellationToken cancellationToken);

    Task<ContentRevisionRestoreRequestDto> CompleteRestoreRequestAsync(Guid id, CancellationToken cancellationToken);
}
