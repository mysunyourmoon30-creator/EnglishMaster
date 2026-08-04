using System.Security.Claims;
using EnglishMaster.Application.Features.StudentProgress.Queries;
using EnglishMaster.Contracts.StudentProgress;
using EnglishMaster.Shared.Results;
using AppStudentProgressItemDto = EnglishMaster.Application.Features.StudentProgress.Dtos.StudentProgressItemDto;
using AppStudentProgressSummaryDto = EnglishMaster.Application.Features.StudentProgress.Dtos.StudentProgressSummaryDto;

namespace EnglishMaster.Api.Endpoints;

public static class StudentProgressEndpoints
{
    public static IEndpointRouteBuilder MapStudentProgressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/v1/me")
            .WithTags("Student Progress")
            .RequireAuthorization();

        group.MapGet("/progress", GetProgressAsync);
        return endpoints;
    }

    private static async Task<IResult> GetProgressAsync(
        ClaimsPrincipal user,
        StudentProgressQueryHandler handler,
        int? limit,
        CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.HandleAsync(
            new GetMyStudentProgressQuery(userId, limit),
            cancellationToken);

        return result.Status == ResultStatus.Success
            ? Results.Ok(ToContract(result.Value!))
            : Results.Problem();
    }

    private static StudentProgressSummaryDto ToContract(AppStudentProgressSummaryDto summary) =>
        new(
            summary.TotalTrackedItems,
            summary.InProgressCount,
            summary.CompletedCount,
            summary.Lessons.Select(ToContract).ToArray(),
            summary.Courses.Select(ToContract).ToArray(),
            summary.Books.Select(ToContract).ToArray());

    private static StudentProgressItemDto ToContract(AppStudentProgressItemDto item) =>
        new(
            item.ContentType,
            item.ContentId,
            item.Slug,
            item.Title,
            item.Summary,
            item.Url,
            item.ProgressPercent,
            item.Status,
            item.LastAccessedAt);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
