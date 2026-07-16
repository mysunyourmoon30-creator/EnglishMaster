using System.Security.Claims;
using EnglishMaster.Application.Features.Analytics;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Shared.Results;
using ContractAdminAnalyticsOverviewDto = EnglishMaster.Contracts.Analytics.AdminAnalyticsOverviewDto;
using ContractStudentAnalyticsOverviewDto = EnglishMaster.Contracts.Analytics.StudentAnalyticsOverviewDto;
using AppAdminAnalyticsOverviewDto = EnglishMaster.Application.Features.Analytics.AdminAnalyticsOverviewDto;
using AppStudentAnalyticsOverviewDto = EnglishMaster.Application.Features.Analytics.StudentAnalyticsOverviewDto;

namespace EnglishMaster.Api.Endpoints;

public static class AnalyticsEndpoints
{
    public static IEndpointRouteBuilder MapAnalyticsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var admin = endpoints.MapGroup("/api/v1/admin/analytics")
            .WithTags("Admin Analytics")
            .RequireAuthorization(Permissions.ReportsRead);
        admin.MapGet("/overview", GetAdminOverviewAsync);

        var mine = endpoints.MapGroup("/api/v1/me/analytics")
            .WithTags("Analytics")
            .RequireAuthorization();
        mine.MapGet("/overview", GetStudentOverviewAsync);

        return endpoints;
    }

    private static async Task<IResult> GetAdminOverviewAsync(AnalyticsQueryHandler handler, DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAdminOverviewAsync(new GetAdminAnalyticsOverviewQuery(fromDate, toDate), cancellationToken));

    private static async Task<IResult> GetStudentOverviewAsync(ClaimsPrincipal user, AnalyticsQueryHandler handler, DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        return ToHttpResult(await handler.GetStudentOverviewAsync(new GetStudentAnalyticsOverviewQuery(userId, fromDate, toDate), cancellationToken));
    }

    private static IResult ToHttpResult(Result<AppAdminAnalyticsOverviewDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.Problem();

    private static IResult ToHttpResult(Result<AppStudentAnalyticsOverviewDto> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.Problem();

    private static ContractAdminAnalyticsOverviewDto ToContract(AppAdminAnalyticsOverviewDto overview) =>
        new(overview.ActiveLearners, overview.StudyMinutes, overview.LearningActivities, overview.CourseCompletions, overview.QuizAttempts, overview.AverageQuizScore, overview.PracticeSessionsCompleted, overview.CertificatesIssued);

    private static ContractStudentAnalyticsOverviewDto ToContract(AppStudentAnalyticsOverviewDto overview) =>
        new(overview.StudyMinutes, overview.LearningActivities, overview.LessonsCompleted, overview.CoursesCompleted, overview.BooksCompleted, overview.QuizAttempts, overview.AverageQuizScore, overview.PracticeSessionsCompleted, overview.CertificatesEarned, overview.CurrentStreakDays, overview.LongestStreakDays);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
