using System.Security.Claims;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Contracts.Security;
using EnglishMaster.Shared.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EnglishMaster.Api.Endpoints;

public static class SecurityEndpoints
{
    public const string CookieScheme = "EnglishMaster.Admin";

    public static IEndpointRouteBuilder MapSecurityEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var auth = endpoints.MapGroup("/api/v1/auth").WithTags("Auth");
        auth.MapPost("/login", LoginAsync).AllowAnonymous();
        auth.MapPost("/logout", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(CookieScheme);
            return Results.NoContent();
        }).RequireAuthorization();
        auth.MapGet("/me", MeAsync).RequireAuthorization();

        var users = endpoints.MapGroup("/api/v1/users").WithTags("Users").RequireAuthorization(Permissions.UsersRead);
        users.MapGet("", SearchUsersAsync);
        users.MapGet("/{id:guid}", GetUserAsync);
        users.MapPost("", CreateUserAsync).RequireAuthorization(Permissions.UsersCreate);
        users.MapPut("/{id:guid}", UpdateUserAsync).RequireAuthorization(Permissions.UsersUpdate);
        users.MapPost("/{id:guid}/deactivate", DeactivateUserAsync).RequireAuthorization(Permissions.UsersDelete);
        users.MapPost("/{id:guid}/roles/{roleId:guid}", AssignRoleAsync).RequireAuthorization(Permissions.UsersUpdate);
        users.MapDelete("/{id:guid}/roles/{roleId:guid}", RemoveRoleAsync).RequireAuthorization(Permissions.UsersUpdate);

        var roles = endpoints.MapGroup("/api/v1/roles").WithTags("Roles").RequireAuthorization(Permissions.RolesRead);
        roles.MapGet("", SearchRolesAsync);
        roles.MapGet("/{id:guid}", GetRoleAsync);
        roles.MapPost("", CreateRoleAsync).RequireAuthorization(Permissions.RolesCreate);
        roles.MapPut("/{id:guid}", UpdateRoleAsync).RequireAuthorization(Permissions.RolesUpdate);
        roles.MapDelete("/{id:guid}", DeleteRoleAsync).RequireAuthorization(Permissions.RolesDelete);
        roles.MapGet("/{id:guid}/permissions", GetRolePermissionsAsync);
        roles.MapPost("/{id:guid}/permissions", AssignPermissionAsync).RequireAuthorization(Permissions.PermissionsUpdate);
        roles.MapDelete("/{id:guid}/permissions/{permissionKey}", RemovePermissionAsync).RequireAuthorization(Permissions.PermissionsUpdate);

        endpoints.MapGet("/api/v1/permissions", GetPermissionsAsync)
            .WithTags("Permissions")
            .RequireAuthorization(Permissions.PermissionsRead);

        return endpoints;
    }

    private static async Task<IResult> LoginAsync(
        LoginRequest request,
        LoginCommandHandler handler,
        HttpContext httpContext,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new LoginCommand(request.Email, request.Password), cancellationToken);
        if (result.Status != ResultStatus.Success)
        {
            return ToHttpResult(result);
        }

        await httpContext.SignInAsync(CookieScheme, CreatePrincipal(result.Value!), AuthenticationProperties());
        return Results.Ok(new LoginResponse(result.Value!));
    }

    private static async Task<IResult> MeAsync(
        GetCurrentUserQueryHandler handler,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId(user);
        if (userId is null)
        {
            return Results.Unauthorized();
        }

        var result = await handler.HandleAsync(new GetCurrentUserQuery(userId.Value), cancellationToken);
        return ToHttpResult(result);
    }

    private static async Task<IResult> SearchUsersAsync(
        UserCommandQueryHandlers handler,
        string? search,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchUsersQuery(search, isActive, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetUserAsync(Guid id, UserCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAsync(new GetUserByIdQuery(id), cancellationToken));

    private static async Task<IResult> CreateUserAsync(CreateUserRequest request, UserCommandQueryHandlers handler, CancellationToken cancellationToken)
    {
        var result = await handler.CreateAsync(new CreateUserCommand(request.Email, request.UserName, request.DisplayName, request.Password, request.RoleIds), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/users/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateUserAsync(Guid id, UpdateUserRequest request, UserCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.UpdateAsync(new UpdateUserCommand(id, request.Email, request.UserName, request.DisplayName, request.IsActive), cancellationToken));

    private static async Task<IResult> DeactivateUserAsync(Guid id, UserCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.DeactivateAsync(new DeactivateUserCommand(id), cancellationToken));

    private static async Task<IResult> AssignRoleAsync(Guid id, Guid roleId, UserCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.AssignRoleAsync(new AssignRoleToUserCommand(id, roleId), cancellationToken));

    private static async Task<IResult> RemoveRoleAsync(Guid id, Guid roleId, UserCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RemoveRoleAsync(new RemoveRoleFromUserCommand(id, roleId), cancellationToken));

    private static async Task<IResult> SearchRolesAsync(
        RoleCommandQueryHandlers handler,
        string? search,
        bool? isActive,
        int? pageNumber,
        int? pageSize,
        CancellationToken cancellationToken) =>
        ToHttpResult(await handler.SearchAsync(new SearchRolesQuery(search, isActive, pageNumber, pageSize), cancellationToken));

    private static async Task<IResult> GetRoleAsync(Guid id, RoleCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetAsync(new GetRoleByIdQuery(id), cancellationToken));

    private static async Task<IResult> CreateRoleAsync(CreateRoleRequest request, RoleCommandQueryHandlers handler, CancellationToken cancellationToken)
    {
        var result = await handler.CreateAsync(new CreateRoleCommand(request.Name, request.Description), cancellationToken);
        return result.Status == ResultStatus.Success
            ? Results.Created($"/api/v1/roles/{result.Value!.Id}", result.Value)
            : ToHttpResult(result);
    }

    private static async Task<IResult> UpdateRoleAsync(Guid id, UpdateRoleRequest request, RoleCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.UpdateAsync(new UpdateRoleCommand(id, request.Name, request.Description, request.IsActive), cancellationToken));

    private static async Task<IResult> DeleteRoleAsync(Guid id, RoleCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.DeleteAsync(new DeleteRoleCommand(id), cancellationToken));

    private static async Task<IResult> GetRolePermissionsAsync(Guid id, RoleCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.GetPermissionsAsync(new GetRolePermissionsQuery(id), cancellationToken));

    private static async Task<IResult> AssignPermissionAsync(Guid id, AssignPermissionRequest request, RoleCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.AssignPermissionAsync(new AssignPermissionToRoleCommand(id, request.PermissionKey), cancellationToken));

    private static async Task<IResult> RemovePermissionAsync(Guid id, string permissionKey, RoleCommandQueryHandlers handler, CancellationToken cancellationToken) =>
        ToHttpResult(await handler.RemovePermissionAsync(new RemovePermissionFromRoleCommand(id, permissionKey), cancellationToken));

    private static async Task<IResult> GetPermissionsAsync(PermissionQueryHandler handler, CancellationToken cancellationToken) =>
        Results.Ok(await handler.GetAllAsync(new GetPermissionsQuery(), cancellationToken));

    public static ClaimsPrincipal CreatePrincipal(CurrentUserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));
        claims.AddRange(user.Permissions.Select(permission => new Claim(SecurityPermissionClaimTypes.Permission, permission)));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieScheme));
    }

    private static AuthenticationProperties AuthenticationProperties() =>
        new()
        {
            IsPersistent = true,
            AllowRefresh = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

    private static Guid? GetUserId(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    private static IResult ToHttpResult(Result result)
    {
        return result.Status switch
        {
            ResultStatus.Success => Results.NoContent(),
            ResultStatus.NotFound => Results.NotFound(),
            ResultStatus.ValidationError => Results.ValidationProblem(ToValidationDictionary(result.Errors)),
            _ => Results.Problem()
        };
    }

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
            .ToDictionary(group => group.Key, group => group.Select(error => error.Message).ToArray());
    }
}
