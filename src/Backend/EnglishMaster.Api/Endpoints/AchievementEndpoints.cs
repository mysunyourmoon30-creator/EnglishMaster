using System.Security.Claims;
using EnglishMaster.Application.Features.Achievements.Commands;
using EnglishMaster.Application.Features.Achievements.Queries;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Achievements;
using EnglishMaster.Shared.Results;
using AppDefinitionDto = EnglishMaster.Application.Features.Achievements.Dtos.AchievementDefinitionDto;
using AppStudentAchievementDto = EnglishMaster.Application.Features.Achievements.Dtos.StudentAchievementDto;

namespace EnglishMaster.Api.Endpoints;

public static class AchievementEndpoints
{
    public static IEndpointRouteBuilder MapAchievementEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var learner = endpoints.MapGroup("/api/v1/me/achievements").WithTags("Achievements").RequireAuthorization();
        learner.MapGet("/", GetMineAsync);
        learner.MapGet("/earned", GetEarnedAsync);
        learner.MapGet("/progress", GetMineAsync);
        learner.MapPost("/evaluate", EvaluateAsync);

        var admin = endpoints.MapGroup("/api/v1/admin/achievement-definitions").WithTags("Achievement Definitions").RequireAuthorization();
        admin.MapGet("/", SearchDefinitionsAsync).RequireAuthorization(Permissions.AchievementsRead);
        admin.MapPost("/", CreateDefinitionAsync).RequireAuthorization(Permissions.AchievementsManage);
        admin.MapPut("/{id:guid}", UpdateDefinitionAsync).RequireAuthorization(Permissions.AchievementsManage);
        admin.MapPost("/{id:guid}/activate", ActivateAsync).RequireAuthorization(Permissions.AchievementsManage);
        admin.MapPost("/{id:guid}/deactivate", DeactivateAsync).RequireAuthorization(Permissions.AchievementsManage);
        admin.MapPost("/seed-defaults", SeedDefaultsAsync).RequireAuthorization(Permissions.AchievementsManage);
        return endpoints;
    }

    private static async Task<IResult> GetMineAsync(ClaimsPrincipal user, AchievementQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetMyAchievementsAsync(new GetMyAchievementsQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> GetEarnedAsync(ClaimsPrincipal user, AchievementQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        TryUserId(user, out var userId) ? ToHttpResult(await handler.GetMyEarnedAchievementsAsync(new GetMyEarnedAchievementsQuery(userId, limit), cancellationToken)) : Results.Unauthorized();

    private static async Task<IResult> EvaluateAsync(ClaimsPrincipal user, AchievementCommandHandler handler, CancellationToken cancellationToken)
    {
        if (!TryUserId(user, out var userId))
        {
            return Results.Unauthorized();
        }

        var result = await handler.EvaluateAsync(new EvaluateMyAchievementsCommand(userId), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> SearchDefinitionsAsync(AchievementQueryHandler handler, int? limit, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchDefinitionsAsync(new SearchAchievementDefinitionsQuery(limit), cancellationToken));

    private static async Task<IResult> SeedDefaultsAsync(AchievementCommandHandler handler, CancellationToken cancellationToken)
    {
        var result = await handler.SeedDefaultsAsync(new SeedDefaultAchievementDefinitionsCommand(), cancellationToken);
        return Results.Ok(new { created = result.Value });
    }

    private static async Task<IResult> CreateDefinitionAsync(AchievementCommandHandler handler, AchievementDefinitionRequest request, CancellationToken cancellationToken)
    {
        var result = await handler.CreateDefinitionAsync(new AchievementDefinitionCommand(request.Code, request.Name, request.Description, request.AchievementType, request.TargetValue, request.IconName, request.SortOrder), cancellationToken);
        return ToMutationResult(result);
    }

    private static async Task<IResult> UpdateDefinitionAsync(AchievementCommandHandler handler, Guid id, AchievementDefinitionRequest request, CancellationToken cancellationToken)
    {
        var result = await handler.UpdateDefinitionAsync(new UpdateAchievementDefinitionCommand(id, request.Name, request.Description, request.AchievementType, request.TargetValue, request.IconName, request.SortOrder), cancellationToken);
        return ToMutationResult(result);
    }

    private static async Task<IResult> ActivateAsync(AchievementCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        ToMutationResult(await handler.ActivateAsync(new AchievementDefinitionLifecycleCommand(id), cancellationToken));

    private static async Task<IResult> DeactivateAsync(AchievementCommandHandler handler, Guid id, CancellationToken cancellationToken) =>
        ToMutationResult(await handler.DeactivateAsync(new AchievementDefinitionLifecycleCommand(id), cancellationToken));

    private static IResult ToMutationResult(Result<AppDefinitionDto> result) =>
        result.Status switch
        {
            ResultStatus.Success => Results.Ok(ToContract(result.Value!)),
            ResultStatus.ValidationError => Results.ValidationProblem(result.Errors.GroupBy(error => error.Field).ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray())),
            _ => Results.NotFound()
        };

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppDefinitionDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static IResult ToHttpResult(Result<IReadOnlyCollection<AppStudentAchievementDto>> result) =>
        result.Status == ResultStatus.Success ? Results.Ok(result.Value!.Select(ToContract).ToArray()) : Results.Problem();

    private static AchievementDefinitionDto ToContract(AppDefinitionDto definition) =>
        new(definition.Id, definition.Code, definition.Name, definition.Description, definition.AchievementType, definition.TargetValue, definition.IconName, definition.IsActive, definition.SortOrder);

    internal static StudentAchievementDto ToContract(AppStudentAchievementDto achievement) =>
        new(achievement.Id, achievement.AchievementDefinitionId, achievement.Code, achievement.Name, achievement.Description, achievement.AchievementType, achievement.TargetValue, achievement.IconName, achievement.Status, achievement.ProgressValue, achievement.EarnedAt);

    private static bool TryUserId(ClaimsPrincipal user, out Guid userId) =>
        Guid.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out userId) && userId != Guid.Empty;
}
