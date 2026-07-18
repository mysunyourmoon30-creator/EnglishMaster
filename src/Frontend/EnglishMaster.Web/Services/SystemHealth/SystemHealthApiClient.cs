using EnglishMaster.Contracts.SystemHealth;
using EnglishMaster.Web.Services;

namespace EnglishMaster.Web.Services.SystemHealth;

public sealed class SystemHealthApiClient : ISystemHealthApiClient
{
    private readonly HttpClient httpClient;

    public SystemHealthApiClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }

    public async Task<SystemHealthResponse> GetAsync(CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync("api/v1/admin/system-health", cancellationToken);
        await ApiClientResponseHandler.EnsureSuccessAsync(response, cancellationToken);

        return await ApiClientResponseHandler.ReadRequiredAsync<SystemHealthResponse>(response, cancellationToken);
    }
}
