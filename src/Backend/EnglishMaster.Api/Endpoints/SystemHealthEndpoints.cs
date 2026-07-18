using EnglishMaster.Application.Features.Security;
using EnglishMaster.Application.Features.SystemHealth;
using EnglishMaster.Contracts.SystemHealth;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class SystemHealthEndpoints
{
    public static IEndpointRouteBuilder MapSystemHealthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/v1/admin/system-health", GetSystemHealthAsync)
            .WithTags("Admin System Health")
            .RequireAuthorization(Permissions.SystemHealthRead);

        return endpoints;
    }

    private static async Task<IResult> GetSystemHealthAsync(
        SystemHealthQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.GetAsync(new GetSystemHealthQuery(), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Ok(ToContract(result.Value!))
            : Results.Problem();
    }

    private static SystemHealthResponse ToContract(SystemHealthDto dto) =>
        new(dto.DatabaseHealthy, dto.FailedEmailCount, dto.FailedPublishJobCount, dto.FailedImportJobCount, dto.CheckedAt);
}
