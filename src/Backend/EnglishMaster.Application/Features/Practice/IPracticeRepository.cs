using EnglishMaster.Application.Features.Practice.Dtos;

namespace EnglishMaster.Application.Features.Practice;

public interface IPracticeRepository
{
    Task<PracticeItemDto> CreatePracticeItemAsync(Guid userId, string contentType, Guid contentId, string practiceType, CancellationToken cancellationToken);
    Task<int> GeneratePracticeItemsAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PracticeItemDto>> GetDuePracticeItemsAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<PracticeSessionDto> StartPracticeSessionAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<PracticeSessionDto?> GetPracticeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken);
    Task<PracticeSessionItemDto?> SubmitPracticeSessionItemAsync(Guid userId, Guid sessionItemId, string? userAnswer, string result, CancellationToken cancellationToken);
    Task<PracticeSessionDto?> CompletePracticeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<PracticeSessionDto>> GetPracticeHistoryAsync(Guid userId, int limit, CancellationToken cancellationToken);
    Task<PracticeSummaryDto> GetPracticeSummaryAsync(Guid userId, CancellationToken cancellationToken);
    Task<PracticeItemDto?> SuspendPracticeItemAsync(Guid userId, Guid practiceItemId, CancellationToken cancellationToken);
    Task<PracticeItemDto?> ResumePracticeItemAsync(Guid userId, Guid practiceItemId, CancellationToken cancellationToken);
}

