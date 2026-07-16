using EnglishMaster.Application.Features.Reports.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Api.Endpoints;

public static class ReportEndpoints
{
    public static IEndpointRouteBuilder MapReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var reports = endpoints.MapGroup("/api/v1/reports")
            .WithTags("Reports")
            .RequireAuthorization(Permissions.ReportsRead);

        reports.MapGet("/admin-dashboard", GetAdminDashboardAsync);
        reports.MapGet("/content-status", GetContentStatusAsync);
        reports.MapGet("/learning-progress", GetLearningProgressAsync);
        reports.MapGet("/quiz-analytics", GetQuizAnalyticsAsync);
        reports.MapGet("/recent-activity", GetRecentActivityAsync);

        return endpoints;
    }

    private static async Task<IResult> GetAdminDashboardAsync(
        GetAdminDashboardSummaryQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.HandleAsync(new GetAdminDashboardSummaryQuery(), cancellationToken));

    private static async Task<IResult> GetContentStatusAsync(
        GetContentStatusSummaryQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.HandleAsync(new GetContentStatusSummaryQuery(), cancellationToken));

    private static async Task<IResult> GetLearningProgressAsync(
        GetLearningProgressSummaryQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.HandleAsync(new GetLearningProgressSummaryQuery(), cancellationToken));

    private static async Task<IResult> GetQuizAnalyticsAsync(
        GetQuizAnalyticsSummaryQueryHandler handler,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.HandleAsync(new GetQuizAnalyticsSummaryQuery(), cancellationToken));

    private static async Task<IResult> GetRecentActivityAsync(
        GetRecentActivitySummaryQueryHandler handler,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.HandleAsync(new GetRecentActivitySummaryQuery(pageSize ?? 20), cancellationToken));

    private static IResult ToHttpResult<T>(Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.Ok(result.Value),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

    private static Dictionary<string, string[]> ToValidationDictionary(IEnumerable<ValidationError> errors)
    {
        return errors
            .GroupBy(error => error.Field)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.Message).ToArray());
    }
}
