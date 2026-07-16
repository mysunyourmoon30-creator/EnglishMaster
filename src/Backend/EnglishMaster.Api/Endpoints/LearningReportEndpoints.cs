using System.Security.Claims;
using System.Globalization;
using EnglishMaster.Application.Features.LearningReports.Commands;
using EnglishMaster.Application.Features.LearningReports.Queries;
using EnglishMaster.Contracts.LearningReports;
using EnglishMaster.Shared.Results;
using AppInsightDto = EnglishMaster.Application.Features.LearningReports.Dtos.WeeklyLearningReportInsightDto;
using AppReportDto = EnglishMaster.Application.Features.LearningReports.Dtos.WeeklyLearningReportDto;

namespace EnglishMaster.Api.Endpoints;

public static class LearningReportEndpoints
{
    public static IEndpointRouteBuilder MapLearningReportEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me/learning-reports").WithTags("Learning Reports").RequireAuthorization();
        group.MapGet("/current-week", GetCurrentWeekAsync);
        group.MapGet("/", GetHistoryAsync);
        group.MapGet("/{id:guid}", GetByIdAsync);
        group.MapGet("/by-date/{date}", GetByDateAsync);
        group.MapPost("/current-week/generate", GenerateCurrentWeekAsync);
        group.MapPost("/{id:guid}/regenerate", RegenerateAsync);
        group.MapGet("/{id:guid}/insights", GetInsightsAsync);
        return endpoints;
    }

    private static async Task<IResult> GetCurrentWeekAsync(ClaimsPrincipal user, LearningReportQueryHandler handler, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetCurrentWeekAsync(new GetCurrentWeekLearningReportQuery(userId), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GetHistoryAsync(ClaimsPrincipal user, LearningReportQueryHandler handler, int? pageNumber, int? pageSize, DateTimeOffset? fromDate, DateTimeOffset? toDate, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetHistoryAsync(new GetMyLearningReportHistoryQuery(userId, pageNumber, pageSize, fromDate, toDate), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetByIdAsync(ClaimsPrincipal user, LearningReportQueryHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.GetByIdAsync(new GetWeeklyLearningReportByIdQuery(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GetByDateAsync(ClaimsPrincipal user, LearningReportQueryHandler handler, string date, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
        {
            return Results.BadRequest();
        }

        var parsed = new DateTimeOffset(parsedDate.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
        var result = await handler.GetByDateAsync(new GetWeeklyLearningReportByDateQuery(userId, parsed), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GenerateCurrentWeekAsync(ClaimsPrincipal user, LearningReportCommandHandler handler, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId)
            ? Results.Ok(ToContract((await handler.GenerateAsync(new GenerateWeeklyLearningReportCommand(userId, null, false), cancellationToken)).Value!))
            : Results.Unauthorized();

    private static async Task<IResult> RegenerateAsync(ClaimsPrincipal user, LearningReportCommandHandler handler, Guid id, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.RegenerateAsync(new RegenerateWeeklyLearningReportCommand(userId, id), cancellationToken);
        return result.Status == ResultStatus.Success ? Results.Ok(ToContract(result.Value!)) : Results.NotFound();
    }

    private static async Task<IResult> GetInsightsAsync(ClaimsPrincipal user, LearningReportQueryHandler handler, Guid id, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToInsightResult(await handler.GetInsightsAsync(new GetWeeklyLearningReportInsightsQuery(userId, id), cancellationToken)) : Results.Unauthorized();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppReportDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToInsightResult(Result<IReadOnlyCollection<AppInsightDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static WeeklyLearningReportDto ToContract(AppReportDto report) =>
        new(report.Id, report.WeekStartDate, report.WeekEndDate, report.Status, report.GeneratedAt, report.TotalStudyMinutes, report.ActiveStudyDays, report.CompletedDailyPlans, report.LessonsStarted, report.LessonsCompleted, report.CoursesStarted, report.CoursesCompleted, report.BooksStarted, report.BooksCompleted, report.PracticeSessionsCompleted, report.PracticeItemsCompleted, report.QuizAttempts, report.QuizzesPassed, report.AverageQuizScore, report.GoalsCompleted, report.AchievementsEarned, report.CurrentStreakDays, report.LongestStreakDays, report.SummaryText, report.Insights.Select(ToContract).ToArray());

    private static WeeklyLearningReportInsightDto ToContract(AppInsightDto insight) =>
        new(insight.Id, insight.WeeklyLearningReportId, insight.InsightType, insight.Severity, insight.Message, insight.Recommendation, insight.SortOrder);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
