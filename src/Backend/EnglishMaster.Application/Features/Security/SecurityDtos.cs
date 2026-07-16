namespace EnglishMaster.Application.Features.Security;

public sealed record SecurityCurrentUser(
    Guid Id,
    string Email,
    string UserName,
    string DisplayName,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Permissions);

public sealed record SecurityUser(
    Guid Id,
    string Email,
    string UserName,
    string DisplayName,
    bool IsActive,
    IReadOnlyCollection<SecurityRoleSummary> Roles,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record SecurityRoleSummary(Guid Id, string Name, string Slug);

public sealed record SecurityUserSearchResult(
    IReadOnlyCollection<SecurityUser> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record SecurityRole(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    bool IsSystem,
    bool IsActive,
    IReadOnlyCollection<string> Permissions,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record SecurityRoleSearchResult(
    IReadOnlyCollection<SecurityRole> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public sealed record SecurityPermission(
    string Key,
    string Name,
    string Module,
    string Description);
