using EnglishMaster.Application.Features.SystemHealth;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace EnglishMaster.Infrastructure.Monitoring;

internal sealed class EfDatabaseHealthChecker : IDatabaseHealthChecker
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly ILogger<EfDatabaseHealthChecker> logger;

    public EfDatabaseHealthChecker(EnglishMasterDbContext dbContext, ILogger<EfDatabaseHealthChecker> logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    public async Task<bool> CanConnectAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.Database.CanConnectAsync(cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            logger.LogWarning(exception, "Database health check failed.");
            return false;
        }
    }
}
