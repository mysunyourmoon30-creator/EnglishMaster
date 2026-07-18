using EnglishMaster.Application.Features.EmailMessages;
using EnglishMaster.Application.Features.ImportJobs;
using EnglishMaster.Application.Features.PublishJobs;
using EnglishMaster.Application.Features.PublishJobs.Dtos;
using EnglishMaster.Application.Features.SystemHealth;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Domain.Publishing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnglishMaster.Infrastructure.Monitoring;

public sealed class SystemHealthWorker : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly SystemHealthWorkerOptions options;
    private readonly ILogger<SystemHealthWorker> logger;
    private readonly Dictionary<string, DateTimeOffset> lastAlertAt = new(StringComparer.Ordinal);
    private int consecutiveDatabaseFailures;

    public SystemHealthWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<SystemHealthWorkerOptions> options,
        ILogger<SystemHealthWorker> logger)
    {
        this.scopeFactory = scopeFactory;
        this.options = options.Value;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled)
        {
            logger.LogInformation("System health worker is disabled.");
            return;
        }

        logger.LogInformation(
            "System health worker starting with a {PollingInterval} polling interval.",
            options.PollingInterval);

        using var timer = new PeriodicTimer(options.PollingInterval);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "System health worker tick failed unexpectedly.");
            }

            try
            {
                await timer.WaitForNextTickAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    internal static bool ShouldAlert(int currentValue, int threshold, DateTimeOffset? lastAlertAt, TimeSpan cooldown, DateTimeOffset now)
    {
        if (currentValue < threshold)
        {
            return false;
        }

        return lastAlertAt is null || now - lastAlertAt.Value >= cooldown;
    }

    private async Task CheckAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var timeProvider = scope.ServiceProvider.GetRequiredService<TimeProvider>();
        var now = timeProvider.GetUtcNow();

        var databaseHealthChecker = scope.ServiceProvider.GetRequiredService<IDatabaseHealthChecker>();
        var databaseHealthy = await CheckDatabaseAsync(databaseHealthChecker, cancellationToken);

        if (string.IsNullOrWhiteSpace(options.AlertRecipientEmail))
        {
            return;
        }

        if (!databaseHealthy)
        {
            if (ShouldAlert(consecutiveDatabaseFailures, options.ConsecutiveFailuresBeforeAlert, GetLastAlert("database"), options.AlertCooldown, now))
            {
                await QueueAlertAsync(
                    scope,
                    "database",
                    "EnglishMaster alert: database connectivity failing",
                    $"The database has failed connectivity checks {consecutiveDatabaseFailures} time(s) in a row as of {now:u}.",
                    now,
                    cancellationToken);
            }
        }

        var emailRepository = scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
        var failedEmailCount = (await emailRepository.SearchAsync("Failed", null, 1, 1, cancellationToken)).TotalCount;
        if (ShouldAlert(failedEmailCount, options.FailedEmailCountThreshold, GetLastAlert("email"), options.AlertCooldown, now))
        {
            await QueueAlertAsync(
                scope,
                "email",
                "EnglishMaster alert: failed email count threshold exceeded",
                $"There are {failedEmailCount} failed email messages, at or above the configured threshold of {options.FailedEmailCountThreshold}.",
                now,
                cancellationToken);
        }

        var publishJobRepository = scope.ServiceProvider.GetRequiredService<IPublishJobRepository>();
        var failedPublishJobResult = await publishJobRepository.SearchAsync(
            new PublishJobSearchCriteria(null, null, null, PublishJobStatus.Failed, 1, 1, PublishJobSortBy.CreatedAt, PublishSortDirection.Desc),
            cancellationToken);
        if (ShouldAlert(failedPublishJobResult.TotalCount, options.FailedPublishJobCountThreshold, GetLastAlert("publishJob"), options.AlertCooldown, now))
        {
            await QueueAlertAsync(
                scope,
                "publishJob",
                "EnglishMaster alert: failed publish job count threshold exceeded",
                $"There are {failedPublishJobResult.TotalCount} failed publish jobs, at or above the configured threshold of {options.FailedPublishJobCountThreshold}.",
                now,
                cancellationToken);
        }

        var importJobRepository = scope.ServiceProvider.GetRequiredService<IImportJobRepository>();
        var failedImportJobResult = await importJobRepository.SearchAsync(null, null, "Failed", 1, 1, cancellationToken);
        if (ShouldAlert(failedImportJobResult.TotalCount, options.FailedImportJobCountThreshold, GetLastAlert("importJob"), options.AlertCooldown, now))
        {
            await QueueAlertAsync(
                scope,
                "importJob",
                "EnglishMaster alert: failed import job count threshold exceeded",
                $"There are {failedImportJobResult.TotalCount} failed import jobs, at or above the configured threshold of {options.FailedImportJobCountThreshold}.",
                now,
                cancellationToken);
        }
    }

    private async Task<bool> CheckDatabaseAsync(IDatabaseHealthChecker databaseHealthChecker, CancellationToken cancellationToken)
    {
        var canConnect = await databaseHealthChecker.CanConnectAsync(cancellationToken);
        consecutiveDatabaseFailures = canConnect ? 0 : consecutiveDatabaseFailures + 1;
        return canConnect;
    }

    private DateTimeOffset? GetLastAlert(string alertKey) =>
        lastAlertAt.TryGetValue(alertKey, out var value) ? value : null;

    private async Task QueueAlertAsync(
        IServiceScope scope,
        string alertKey,
        string subject,
        string body,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var repository = scope.ServiceProvider.GetRequiredService<IEmailMessageRepository>();
        var email = EmailMessage.Queue(options.AlertRecipientEmail, "Operations", subject, body, isHtml: false, now);
        await repository.AddAsync(email, cancellationToken);
        lastAlertAt[alertKey] = now;
        logger.LogWarning("System health worker queued an alert: {Subject}", subject);
    }
}
