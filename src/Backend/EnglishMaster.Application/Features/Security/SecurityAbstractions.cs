using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Security;

public interface ISecurityService
{
    Task<Result<SecurityCurrentUser>> LoginAsync(string email, string password, CancellationToken cancellationToken);

    Task<Result<SecurityCurrentUser>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);

    Task<Result<SecurityUser>> CreateUserAsync(string email, string userName, string displayName, string password, IReadOnlyCollection<Guid> roleIds, CancellationToken cancellationToken);

    Task<Result<SecurityUser>> UpdateUserAsync(Guid id, string email, string userName, string displayName, bool isActive, CancellationToken cancellationToken);

    Task<Result> DeactivateUserAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<SecurityUser>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<SecurityUserSearchResult>> SearchUsersAsync(string? search, bool? isActive, int? pageNumber, int? pageSize, CancellationToken cancellationToken);

    Task<Result> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);

    Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken);

    Task<Result<SecurityRole>> CreateRoleAsync(string name, string description, CancellationToken cancellationToken);

    Task<Result<SecurityRole>> UpdateRoleAsync(Guid id, string name, string description, bool isActive, CancellationToken cancellationToken);

    Task<Result> DeleteRoleAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<SecurityRole>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<SecurityRoleSearchResult>> SearchRolesAsync(string? search, bool? isActive, int? pageNumber, int? pageSize, CancellationToken cancellationToken);

    Task<Result> AssignPermissionToRoleAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken);

    Task<Result> RemovePermissionFromRoleAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken);

    Task<Result<IReadOnlyCollection<SecurityPermission>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<SecurityPermission>> GetPermissionsAsync(CancellationToken cancellationToken);

    Task<bool> UserHasPermissionAsync(Guid userId, string permissionKey, CancellationToken cancellationToken);
}
