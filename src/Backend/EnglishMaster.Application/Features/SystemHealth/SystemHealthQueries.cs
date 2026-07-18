using EnglishMaster.Application.Features.EmailMessages;
using EnglishMaster.Application.Features.ImportJobs;
using EnglishMaster.Application.Features.PublishJobs;
using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.SystemHealth;

public sealed record GetSystemHealthQuery;

public sealed record SystemHealthDto(
    bool DatabaseHealthy,
    int FailedEmailCount,
    int FailedPublishJobCount,
    int FailedImportJobCount,
    DateTimeOffset CheckedAt);

public sealed class SystemHealthQueryHandler
{
    private readonly IDatabaseHealthChecker databaseHealthChecker;
    private readonly IEmailMessageRepository emailMessageRepository;
    private readonly IPublishJobRepository publishJobRepository;
    private readonly IImportJobRepository importJobRepository;
    private readonly TimeProvider timeProvider;

    public SystemHealthQueryHandler(
        IDatabaseHealthChecker databaseHealthChecker,
        IEmailMessageRepository emailMessageRepository,
        IPublishJobRepository publishJobRepository,
        IImportJobRepository importJobRepository,
        TimeProvider timeProvider)
    {
        this.databaseHealthChecker = databaseHealthChecker;
        this.emailMessageRepository = emailMessageRepository;
        this.publishJobRepository = publishJobRepository;
        this.importJobRepository = importJobRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<SystemHealthDto>> GetAsync(GetSystemHealthQuery query, CancellationToken cancellationToken)
    {
        var databaseHealthy = await databaseHealthChecker.CanConnectAsync(cancellationToken);
        var failedEmailCount = (await emailMessageRepository.SearchAsync("Failed", null, 1, 1, cancellationToken)).TotalCount;
        var failedPublishJobResult = await publishJobRepository.SearchAsync(
            new PublishJobSearchCriteria(null, null, null, PublishJobStatus.Failed, 1, 1, PublishJobSortBy.CreatedAt, PublishSortDirection.Desc),
            cancellationToken);
        var failedImportJobResult = await importJobRepository.SearchAsync(null, null, "Failed", 1, 1, cancellationToken);

        var dto = new SystemHealthDto(
            databaseHealthy,
            failedEmailCount,
            failedPublishJobResult.TotalCount,
            failedImportJobResult.TotalCount,
            timeProvider.GetUtcNow());

        return Result<SystemHealthDto>.Success(dto);
    }
}
