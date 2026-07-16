namespace EnglishMaster.Contracts.Security;

public sealed record LoginRequest(string Email, string Password);

public sealed record LoginResponse(CurrentUserDto User);

public sealed record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public sealed record CurrentUserDto(
    Guid Id,
    string Email,
    string UserName,
    string DisplayName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);

public sealed record UserDto(
    Guid Id,
    string Email,
    string UserName,
    string DisplayName,
    bool IsActive,
    IReadOnlyCollection<RoleSummaryDto> Roles,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateUserRequest(
    string Email,
    string UserName,
    string DisplayName,
    string Password,
    IReadOnlyCollection<Guid> RoleIds);

public sealed record UpdateUserRequest(
    string Email,
    string UserName,
    string DisplayName,
    bool IsActive);

public sealed record UserSearchResponse(
    IReadOnlyCollection<UserDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record RoleDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    bool IsSystem,
    bool IsActive,
    IReadOnlyCollection<string> Permissions,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record RoleSummaryDto(Guid Id, string Name, string Slug);

public sealed record CreateRoleRequest(string Name, string Description);

public sealed record UpdateRoleRequest(string Name, string Description, bool IsActive);

public sealed record RoleSearchResponse(
    IReadOnlyCollection<RoleDto> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record PermissionDto(
    string Key,
    string Name,
    string Module,
    string Description);

public sealed record AssignPermissionRequest(string PermissionKey);
