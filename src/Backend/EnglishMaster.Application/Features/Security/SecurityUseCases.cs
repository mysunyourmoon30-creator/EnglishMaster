using EnglishMaster.Contracts.Security;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Security;

public sealed record LoginCommand(string Email, string Password);
public sealed record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword);
public sealed record GetCurrentUserQuery(Guid UserId);
public sealed record CreateUserCommand(string Email, string UserName, string DisplayName, string Password, IReadOnlyCollection<Guid> RoleIds);
public sealed record UpdateUserCommand(Guid Id, string Email, string UserName, string DisplayName, bool IsActive);
public sealed record DeactivateUserCommand(Guid Id);
public sealed record GetUserByIdQuery(Guid Id);
public sealed record SearchUsersQuery(string? Search, bool? IsActive, int? PageNumber, int? PageSize);
public sealed record AssignRoleToUserCommand(Guid UserId, Guid RoleId);
public sealed record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId);
public sealed record CreateRoleCommand(string Name, string Description);
public sealed record UpdateRoleCommand(Guid Id, string Name, string Description, bool IsActive);
public sealed record DeleteRoleCommand(Guid Id);
public sealed record GetRoleByIdQuery(Guid Id);
public sealed record SearchRolesQuery(string? Search, bool? IsActive, int? PageNumber, int? PageSize);
public sealed record AssignPermissionToRoleCommand(Guid RoleId, string PermissionKey);
public sealed record RemovePermissionFromRoleCommand(Guid RoleId, string PermissionKey);
public sealed record GetRolePermissionsQuery(Guid RoleId);
public sealed record GetPermissionsQuery;
public sealed record CheckUserPermissionQuery(Guid UserId, string PermissionKey);

public sealed class LoginCommandHandler(ISecurityService securityService)
{
    public async Task<Result<CurrentUserDto>> HandleAsync(LoginCommand command, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.LoginAsync(command.Email, command.Password, cancellationToken), SecurityUseCaseMapper.ToContract);
}

public sealed class GetCurrentUserQueryHandler(ISecurityService securityService)
{
    public async Task<Result<CurrentUserDto>> HandleAsync(GetCurrentUserQuery query, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.GetCurrentUserAsync(query.UserId, cancellationToken), SecurityUseCaseMapper.ToContract);
}

public sealed class ChangePasswordCommandHandler(ISecurityService securityService)
{
    public Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken) =>
        securityService.ChangePasswordAsync(command.UserId, command.CurrentPassword, command.NewPassword, cancellationToken);
}

public sealed class UserCommandQueryHandlers(ISecurityService securityService)
{
    public async Task<Result<UserDto>> CreateAsync(CreateUserCommand command, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.CreateUserAsync(command.Email, command.UserName, command.DisplayName, command.Password, command.RoleIds, cancellationToken), SecurityUseCaseMapper.ToContract);

    public async Task<Result<UserDto>> UpdateAsync(UpdateUserCommand command, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.UpdateUserAsync(command.Id, command.Email, command.UserName, command.DisplayName, command.IsActive, cancellationToken), SecurityUseCaseMapper.ToContract);

    public Task<Result> DeactivateAsync(DeactivateUserCommand command, CancellationToken cancellationToken) =>
        securityService.DeactivateUserAsync(command.Id, cancellationToken);

    public async Task<Result<UserDto>> GetAsync(GetUserByIdQuery query, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.GetUserByIdAsync(query.Id, cancellationToken), SecurityUseCaseMapper.ToContract);

    public async Task<Result<UserSearchResponse>> SearchAsync(SearchUsersQuery query, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.SearchUsersAsync(query.Search, query.IsActive, query.PageNumber, query.PageSize, cancellationToken), SecurityUseCaseMapper.ToContract);

    public Task<Result> AssignRoleAsync(AssignRoleToUserCommand command, CancellationToken cancellationToken) =>
        securityService.AssignRoleToUserAsync(command.UserId, command.RoleId, cancellationToken);

    public Task<Result> RemoveRoleAsync(RemoveRoleFromUserCommand command, CancellationToken cancellationToken) =>
        securityService.RemoveRoleFromUserAsync(command.UserId, command.RoleId, cancellationToken);
}

public sealed class RoleCommandQueryHandlers(ISecurityService securityService)
{
    public async Task<Result<RoleDto>> CreateAsync(CreateRoleCommand command, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.CreateRoleAsync(command.Name, command.Description, cancellationToken), SecurityUseCaseMapper.ToContract);

    public async Task<Result<RoleDto>> UpdateAsync(UpdateRoleCommand command, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.UpdateRoleAsync(command.Id, command.Name, command.Description, command.IsActive, cancellationToken), SecurityUseCaseMapper.ToContract);

    public Task<Result> DeleteAsync(DeleteRoleCommand command, CancellationToken cancellationToken) =>
        securityService.DeleteRoleAsync(command.Id, cancellationToken);

    public async Task<Result<RoleDto>> GetAsync(GetRoleByIdQuery query, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.GetRoleByIdAsync(query.Id, cancellationToken), SecurityUseCaseMapper.ToContract);

    public async Task<Result<RoleSearchResponse>> SearchAsync(SearchRolesQuery query, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(await securityService.SearchRolesAsync(query.Search, query.IsActive, query.PageNumber, query.PageSize, cancellationToken), SecurityUseCaseMapper.ToContract);

    public Task<Result> AssignPermissionAsync(AssignPermissionToRoleCommand command, CancellationToken cancellationToken) =>
        securityService.AssignPermissionToRoleAsync(command.RoleId, command.PermissionKey, cancellationToken);

    public Task<Result> RemovePermissionAsync(RemovePermissionFromRoleCommand command, CancellationToken cancellationToken) =>
        securityService.RemovePermissionFromRoleAsync(command.RoleId, command.PermissionKey, cancellationToken);

    public async Task<Result<IReadOnlyCollection<PermissionDto>>> GetPermissionsAsync(GetRolePermissionsQuery query, CancellationToken cancellationToken) =>
        SecurityUseCaseMapper.Map(
            await securityService.GetRolePermissionsAsync(query.RoleId, cancellationToken),
            permissions => (IReadOnlyCollection<PermissionDto>)permissions.Select(SecurityUseCaseMapper.ToContract).ToArray());
}

public sealed class PermissionQueryHandler(ISecurityService securityService)
{
    public async Task<IReadOnlyCollection<PermissionDto>> GetAllAsync(GetPermissionsQuery query, CancellationToken cancellationToken) =>
        (await securityService.GetPermissionsAsync(cancellationToken)).Select(SecurityUseCaseMapper.ToContract).ToArray();

    public Task<bool> CheckAsync(CheckUserPermissionQuery query, CancellationToken cancellationToken) =>
        securityService.UserHasPermissionAsync(query.UserId, query.PermissionKey, cancellationToken);
}

internal static class SecurityUseCaseMapper
{
    public static Result<TOut> Map<TIn, TOut>(Result<TIn> result, Func<TIn, TOut> map) =>
        result.Status switch
        {
            ResultStatus.Success => Result<TOut>.Success(map(result.Value!)),
            ResultStatus.NotFound => Result<TOut>.NotFound(result.Errors.First().Field, result.Errors.First().Message),
            ResultStatus.ValidationError => Result<TOut>.Validation(result.Errors.ToArray()),
            _ => Result<TOut>.Validation(new ValidationError("Result", "Unsupported result status."))
        };

    public static CurrentUserDto ToContract(SecurityCurrentUser user) =>
        new(user.Id, user.Email, user.UserName, user.DisplayName, user.Roles, user.Permissions);

    public static UserDto ToContract(SecurityUser user) =>
        new(user.Id, user.Email, user.UserName, user.DisplayName, user.IsActive, user.Roles.Select(ToContract).ToArray(), user.CreatedAt, user.UpdatedAt);

    public static RoleSummaryDto ToContract(SecurityRoleSummary role) =>
        new(role.Id, role.Name, role.Slug);

    public static UserSearchResponse ToContract(SecurityUserSearchResult response) =>
        new(response.Items.Select(ToContract).ToArray(), response.PageNumber, response.PageSize, response.TotalCount, response.TotalPages, response.HasPreviousPage, response.HasNextPage);

    public static RoleDto ToContract(SecurityRole role) =>
        new(role.Id, role.Name, role.Slug, role.Description, role.IsSystem, role.IsActive, role.Permissions, role.CreatedAt, role.UpdatedAt);

    public static RoleSearchResponse ToContract(SecurityRoleSearchResult response) =>
        new(response.Items.Select(ToContract).ToArray(), response.PageNumber, response.PageSize, response.TotalCount, response.TotalPages, response.HasPreviousPage, response.HasNextPage);

    public static PermissionDto ToContract(SecurityPermission permission) =>
        new(permission.Key, permission.Name, permission.Module, permission.Description);
}
