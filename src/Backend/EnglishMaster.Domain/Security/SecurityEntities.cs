using EnglishMaster.Domain.Common;

namespace EnglishMaster.Domain.Security;

public sealed class AppUser
{
    private readonly List<AppUserRole> userRoles = [];

    private AppUser()
    {
        Email = string.Empty;
        UserName = string.Empty;
        DisplayName = string.Empty;
        PasswordHash = string.Empty;
    }

    private AppUser(Guid id, string email, string userName, string displayName, string passwordHash, DateTimeOffset now)
    {
        Id = id;
        PasswordHash = Required(passwordHash, nameof(PasswordHash), 1024);
        CreatedAt = now;
        Update(email, userName, displayName, true, now);
    }

    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string NormalizedEmail { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public string NormalizedUserName { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<AppUserRole> UserRoles => userRoles.AsReadOnly();

    public static AppUser Create(string email, string userName, string displayName, string passwordHash, DateTimeOffset now) =>
        new(Guid.NewGuid(), email, userName, displayName, passwordHash, now);

    public void Update(string email, string userName, string displayName, bool isActive, DateTimeOffset now)
    {
        Email = Required(email, nameof(Email), 256);
        NormalizedEmail = Email.ToUpperInvariant();
        UserName = Required(userName, nameof(UserName), 128);
        NormalizedUserName = UserName.ToUpperInvariant();
        DisplayName = Required(displayName, nameof(DisplayName), 256);
        IsActive = isActive;
        UpdatedAt = now;
    }

    public void SetPasswordHash(string passwordHash, DateTimeOffset now)
    {
        PasswordHash = Required(passwordHash, nameof(PasswordHash), 1024);
        UpdatedAt = now;
    }

    public void Deactivate(DateTimeOffset now)
    {
        IsActive = false;
        UpdatedAt = now;
    }

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}

public sealed class AppRole
{
    private readonly List<AppUserRole> userRoles = [];
    private readonly List<AppRolePermission> rolePermissions = [];

    private AppRole()
    {
        Name = string.Empty;
        Slug = string.Empty;
        Description = string.Empty;
    }

    private AppRole(Guid id, string name, string description, bool isSystem, DateTimeOffset now)
    {
        Id = id;
        IsSystem = isSystem;
        CreatedAt = now;
        Update(name, description, true, now);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsSystem { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<AppUserRole> UserRoles => userRoles.AsReadOnly();
    public IReadOnlyCollection<AppRolePermission> RolePermissions => rolePermissions.AsReadOnly();

    public static AppRole Create(string name, string description, bool isSystem, DateTimeOffset now) =>
        new(Guid.NewGuid(), name, description, isSystem, now);

    public void Update(string name, string description, bool isActive, DateTimeOffset now)
    {
        Name = Required(name, nameof(Name), 128);
        Slug = SlugGenerator.Generate(Name, nameof(Name), nameof(name), 128);
        Description = Optional(description, nameof(Description), 512);
        IsActive = isActive;
        UpdatedAt = now;
    }

    private static string Required(string? value, string fieldName, int maxLength)
    {
        var normalized = Optional(value, fieldName, maxLength);
        if (normalized.Length == 0)
        {
            throw new ArgumentException($"{fieldName} is required.", fieldName);
        }

        return normalized;
    }

    private static string Optional(string? value, string fieldName, int maxLength)
    {
        var normalized = value?.Trim() ?? string.Empty;
        if (normalized.Length > maxLength)
        {
            throw new ArgumentException($"{fieldName} must be {maxLength} characters or fewer.", fieldName);
        }

        return normalized;
    }
}

public sealed class AppPermission
{
    private readonly List<AppRolePermission> rolePermissions = [];

    private AppPermission()
    {
        Key = string.Empty;
        Name = string.Empty;
        Module = string.Empty;
        Description = string.Empty;
    }

    private AppPermission(string key, string name, string module, string description)
    {
        Key = key;
        Name = name;
        Module = module;
        Description = description;
    }

    public string Key { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Module { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public IReadOnlyCollection<AppRolePermission> RolePermissions => rolePermissions.AsReadOnly();

    public static AppPermission Create(string key, string name, string module, string description) =>
        new(key, name, module, description);
}

public sealed class AppUserRole
{
    private AppUserRole()
    {
    }

    private AppUserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }

    public Guid UserId { get; private set; }
    public AppUser User { get; private set; } = null!;
    public Guid RoleId { get; private set; }
    public AppRole Role { get; private set; } = null!;

    public static AppUserRole Create(Guid userId, Guid roleId) => new(userId, roleId);
}

public sealed class AppRolePermission
{
    private AppRolePermission()
    {
        PermissionKey = string.Empty;
    }

    private AppRolePermission(Guid roleId, string permissionKey)
    {
        RoleId = roleId;
        PermissionKey = permissionKey;
    }

    public Guid RoleId { get; private set; }
    public AppRole Role { get; private set; } = null!;
    public string PermissionKey { get; private set; } = string.Empty;
    public AppPermission Permission { get; private set; } = null!;

    public static AppRolePermission Create(Guid roleId, string permissionKey) => new(roleId, permissionKey);
}
