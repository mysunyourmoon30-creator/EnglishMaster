using EnglishMaster.Application.Features.Practice;
using EnglishMaster.Application.Features.Practice.Dtos;
using EnglishMaster.Application.Features.Practice.Queries;

namespace EnglishMaster.UnitTests.Practice;

public sealed class PracticeQueryHandlerTests
{
    [Theory]
    [InlineData(null, 10)]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(20, 20)]
    [InlineData(100, 20)]
    public async Task GetDailyVocabularyAsync_ClampsLimit(int? requestedLimit, int expectedLimit)
    {
        var repository = new CapturingPracticeRepository();
        var handler = new PracticeQueryHandler(repository);

        var result = await handler.GetDailyVocabularyAsync(
            new GetDailyVocabularyQuery(Guid.NewGuid(), requestedLimit),
            CancellationToken.None);

        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
        Assert.Equal(expectedLimit, repository.LastDailyLimit);
    }

    private sealed class CapturingPracticeRepository : IPracticeRepository
    {
        public int LastDailyLimit { get; private set; }

        public Task<IReadOnlyCollection<DailyVocabularyItemDto>> GetDailyVocabularyAsync(Guid userId, int limit, CancellationToken cancellationToken)
        {
            LastDailyLimit = limit;
            return Task.FromResult<IReadOnlyCollection<DailyVocabularyItemDto>>([]);
        }

        public Task<PracticeItemDto> CreatePracticeItemAsync(Guid userId, string contentType, Guid contentId, string practiceType, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<int> GeneratePracticeItemsAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<PracticeItemDto>> GetDuePracticeItemsAsync(Guid userId, int limit, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeSessionDto> StartPracticeSessionAsync(Guid userId, int limit, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeSessionDto?> GetPracticeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeSessionItemDto?> SubmitPracticeSessionItemAsync(Guid userId, Guid sessionItemId, string? userAnswer, string result, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeSessionDto?> CompletePracticeSessionAsync(Guid userId, Guid sessionId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<IReadOnlyCollection<PracticeSessionDto>> GetPracticeHistoryAsync(Guid userId, int limit, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeSummaryDto> GetPracticeSummaryAsync(Guid userId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeItemDto?> SuspendPracticeItemAsync(Guid userId, Guid practiceItemId, CancellationToken cancellationToken) => throw new NotSupportedException();
        public Task<PracticeItemDto?> ResumePracticeItemAsync(Guid userId, Guid practiceItemId, CancellationToken cancellationToken) => throw new NotSupportedException();
    }
}
