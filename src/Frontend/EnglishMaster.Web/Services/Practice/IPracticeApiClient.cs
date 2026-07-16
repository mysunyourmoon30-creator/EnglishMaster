using EnglishMaster.Contracts.Practice;

namespace EnglishMaster.Web.Services.Practice;

public interface IPracticeApiClient
{
    Task<PracticeSummaryDto> GetSummaryAsync(CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PracticeItemDto>> GetDueAsync(CancellationToken cancellationToken);
    Task<GeneratePracticeItemsResponse> GenerateAsync(CancellationToken cancellationToken);
    Task<PracticeSessionDto> StartSessionAsync(CancellationToken cancellationToken);
    Task<PracticeSessionDto> GetSessionAsync(Guid id, CancellationToken cancellationToken);
    Task<PracticeSessionItemDto> SubmitAsync(Guid itemId, SubmitPracticeSessionItemRequest request, CancellationToken cancellationToken);
    Task<PracticeSessionDto> CompleteAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PracticeSessionDto>> GetHistoryAsync(CancellationToken cancellationToken);
}

