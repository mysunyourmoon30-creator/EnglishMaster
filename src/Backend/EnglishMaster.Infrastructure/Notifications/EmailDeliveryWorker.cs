using EnglishMaster.Application.Features.EmailMessages.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EnglishMaster.Infrastructure.Notifications;

public sealed class EmailDeliveryWorker : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly EmailDeliveryWorkerOptions options;
    private readonly ILogger<EmailDeliveryWorker> logger;

    public EmailDeliveryWorker(
        IServiceScopeFactory scopeFactory,
        IOptions<EmailDeliveryWorkerOptions> options,
        ILogger<EmailDeliveryWorker> logger)
    {
        this.scopeFactory = scopeFactory;
        this.options = options.Value;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled)
        {
            logger.LogInformation("Email delivery worker is disabled.");
            return;
        }

        logger.LogInformation(
            "Email delivery worker starting with a {PollingInterval} polling interval.",
            options.PollingInterval);

        using var timer = new PeriodicTimer(options.PollingInterval);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessQueueAsync(stoppingToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                logger.LogError(exception, "Email delivery worker tick failed unexpectedly.");
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

    private async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<EmailMessageCommandHandler>();
        var result = await handler.ProcessPendingQueueAsync(
            new ProcessPendingEmailQueueCommand(options.MaxItemsPerRun),
            cancellationToken);

        if (result.Value is { Processed: > 0 } value)
        {
            logger.LogInformation(
                "Email delivery worker processed {Processed} message(s): {Sent} sent, {Failed} failed.",
                value.Processed,
                value.Sent,
                value.Failed);
        }
    }
}
