namespace EnglishMaster.Application.Features.SystemHealth;

public interface IDatabaseHealthChecker
{
    Task<bool> CanConnectAsync(CancellationToken cancellationToken);
}
