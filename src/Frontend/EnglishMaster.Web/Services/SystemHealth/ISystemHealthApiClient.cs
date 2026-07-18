using EnglishMaster.Contracts.SystemHealth;

namespace EnglishMaster.Web.Services.SystemHealth;

public interface ISystemHealthApiClient
{
    Task<SystemHealthResponse> GetAsync(CancellationToken cancellationToken);
}
