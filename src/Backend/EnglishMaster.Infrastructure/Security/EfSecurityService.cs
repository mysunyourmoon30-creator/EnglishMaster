using EnglishMaster.Application.Features.Security;
using EnglishMaster.Domain.Security;
using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Shared.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Security;

internal sealed class EfSecurityService(
    EnglishMasterDbContext dbContext,
    TimeProvider timeProvider)
    : ISecurityService
{
    private readonly PasswordHasher<AppUser> passwordHasher = new();

    public async Task<Result<SecurityCurrentUser>> LoginAsync(string email, string password, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return Result<SecurityCurrentUser>.Validation(new ValidationError(nameof(email), "Email is required."));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return Result<SecurityCurrentUser>.Validation(new ValidationError(nameof(password), "Password is required."));
        }

        var normalizedEmail = email.Trim().ToUpperInvariant();
        var user = await LoadUserByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result<SecurityCurrentUser>.Validation(new ValidationError(nameof(email), "Invalid email or password."));
        }

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Result<SecurityCurrentUser>.Validation(new ValidationError(nameof(email), "Invalid email or password."));
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.SetPasswordHash(passwordHasher.HashPassword(user, password), timeProvider.GetUtcNow());
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result<SecurityCurrentUser>.Success(ToCurrentUserDto(user));
    }

    public async Task<Result<SecurityCurrentUser>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await LoadUserByIdAsync(userId, cancellationToken);
        return user is null || !user.IsActive
            ? Result<SecurityCurrentUser>.NotFound(nameof(userId), "User was not found.")
            : Result<SecurityCurrentUser>.Success(ToCurrentUserDto(user));
    }

    public async Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await LoadUserByIdAsync(userId, cancellationToken);
        if (user is null || !user.IsActive)
        {
            return Result.NotFound(nameof(userId), "User was not found.");
        }

        var verification = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
        if (verification == PasswordVerificationResult.Failed)
        {
            return Result.Validation(new ValidationError(nameof(currentPassword), "Current password is invalid."));
        }

        var passwordValidation = ValidatePassword(newPassword);
        if (passwordValidation.Count > 0)
        {
            return Result.Validation(passwordValidation.ToArray());
        }

        user.SetPasswordHash(passwordHasher.HashPassword(user, newPassword), timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<SecurityUser>> CreateUserAsync(
        string email,
        string userName,
        string displayName,
        string password,
        IReadOnlyCollection<Guid> roleIds,
        CancellationToken cancellationToken)
    {
        var errors = ValidatePassword(password);
        if (await dbContext.AppUsers.AnyAsync(user => user.NormalizedEmail == email.Trim().ToUpperInvariant(), cancellationToken))
        {
            errors.Add(new ValidationError(nameof(email), "Email is already used."));
        }

        if (await dbContext.AppUsers.AnyAsync(user => user.NormalizedUserName == userName.Trim().ToUpperInvariant(), cancellationToken))
        {
            errors.Add(new ValidationError(nameof(userName), "User name is already used."));
        }

        var roles = roleIds.Count == 0
            ? []
            : await dbContext.AppRoles.Where(role => roleIds.Contains(role.Id) && role.IsActive).ToListAsync(cancellationToken);
        if (roles.Count != roleIds.Distinct().Count())
        {
            errors.Add(new ValidationError(nameof(roleIds), "One or more roles were not found."));
        }

        if (errors.Count > 0)
        {
            return Result<SecurityUser>.Validation(errors.ToArray());
        }

        var now = timeProvider.GetUtcNow();
        var user = AppUser.Create(email, userName, displayName, "pending", now);
        user.SetPasswordHash(passwordHasher.HashPassword(user, password), now);
        dbContext.AppUsers.Add(user);
        foreach (var role in roles)
        {
            dbContext.AppUserRoles.Add(AppUserRole.Create(user.Id, role.Id));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        var saved = await LoadUserByIdAsync(user.Id, cancellationToken);
        return Result<SecurityUser>.Success(ToUserDto(saved!));
    }

    public async Task<Result<SecurityUser>> UpdateUserAsync(Guid id, string email, string userName, string displayName, bool isActive, CancellationToken cancellationToken)
    {
        var user = await LoadUserByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return Result<SecurityUser>.NotFound(nameof(id), "User was not found.");
        }

        var normalizedEmail = email.Trim().ToUpperInvariant();
        var normalizedUserName = userName.Trim().ToUpperInvariant();
        var errors = new List<ValidationError>();
        if (await dbContext.AppUsers.AnyAsync(item => item.Id != id && item.NormalizedEmail == normalizedEmail, cancellationToken))
        {
            errors.Add(new ValidationError(nameof(email), "Email is already used."));
        }

        if (await dbContext.AppUsers.AnyAsync(item => item.Id != id && item.NormalizedUserName == normalizedUserName, cancellationToken))
        {
            errors.Add(new ValidationError(nameof(userName), "User name is already used."));
        }

        if (errors.Count > 0)
        {
            return Result<SecurityUser>.Validation(errors.ToArray());
        }

        user.Update(email, userName, displayName, isActive, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<SecurityUser>.Success(ToUserDto(user));
    }

    public async Task<Result> DeactivateUserAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await dbContext.AppUsers.FindAsync([id], cancellationToken);
        if (user is null)
        {
            return Result.NotFound(nameof(id), "User was not found.");
        }

        user.Deactivate(timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<SecurityUser>> GetUserByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await LoadUserByIdAsync(id, cancellationToken);
        return user is null
            ? Result<SecurityUser>.NotFound(nameof(id), "User was not found.")
            : Result<SecurityUser>.Success(ToUserDto(user));
    }

    public async Task<Result<SecurityUserSearchResult>> SearchUsersAsync(string? search, bool? isActive, int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var page = Math.Max(pageNumber ?? 1, 1);
        var size = Math.Clamp(pageSize ?? 20, 1, 100);
        var query = dbContext.AppUsers
            .Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(user => user.Email.Contains(term) || user.UserName.Contains(term) || user.DisplayName.Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(user => user.IsActive == isActive.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(user => user.Email)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)size);

        return Result<SecurityUserSearchResult>.Success(new SecurityUserSearchResult(
            items.Select(ToUserDto).ToArray(),
            page,
            size,
            total,
            totalPages,
            page > 1,
            page < totalPages));
    }

    public async Task<Result> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        if (!await dbContext.AppUsers.AnyAsync(user => user.Id == userId && user.IsActive, cancellationToken))
        {
            return Result.NotFound(nameof(userId), "User was not found.");
        }

        if (!await dbContext.AppRoles.AnyAsync(role => role.Id == roleId && role.IsActive, cancellationToken))
        {
            return Result.NotFound(nameof(roleId), "Role was not found.");
        }

        if (!await dbContext.AppUserRoles.AnyAsync(item => item.UserId == userId && item.RoleId == roleId, cancellationToken))
        {
            dbContext.AppUserRoles.Add(AppUserRole.Create(userId, roleId));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken)
    {
        var userRole = await dbContext.AppUserRoles.FindAsync([userId, roleId], cancellationToken);
        if (userRole is null)
        {
            return Result.NotFound(nameof(roleId), "User role was not found.");
        }

        dbContext.AppUserRoles.Remove(userRole);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<SecurityRole>> CreateRoleAsync(string name, string description, CancellationToken cancellationToken)
    {
        var slug = GenerateSlug(name);
        if (await dbContext.AppRoles.AnyAsync(role => role.Slug == slug, cancellationToken))
        {
            return Result<SecurityRole>.Validation(new ValidationError(nameof(name), "Role name is already used."));
        }

        var role = AppRole.Create(name, description, isSystem: false, timeProvider.GetUtcNow());
        dbContext.AppRoles.Add(role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<SecurityRole>.Success(ToRoleDto(role));
    }

    public async Task<Result<SecurityRole>> UpdateRoleAsync(Guid id, string name, string description, bool isActive, CancellationToken cancellationToken)
    {
        var role = await LoadRoleByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return Result<SecurityRole>.NotFound(nameof(id), "Role was not found.");
        }

        if (role.IsSystem && !isActive)
        {
            return Result<SecurityRole>.Validation(new ValidationError(nameof(isActive), "System roles cannot be deactivated."));
        }

        role.Update(name, description, isActive, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result<SecurityRole>.Success(ToRoleDto(role));
    }

    public async Task<Result> DeleteRoleAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await LoadRoleByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return Result.NotFound(nameof(id), "Role was not found.");
        }

        if (role.IsSystem)
        {
            return Result.Validation(new ValidationError(nameof(id), "System roles cannot be deleted."));
        }

        dbContext.AppRoles.Remove(role);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<SecurityRole>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await LoadRoleByIdAsync(id, cancellationToken);
        return role is null
            ? Result<SecurityRole>.NotFound(nameof(id), "Role was not found.")
            : Result<SecurityRole>.Success(ToRoleDto(role));
    }

    public async Task<Result<SecurityRoleSearchResult>> SearchRolesAsync(string? search, bool? isActive, int? pageNumber, int? pageSize, CancellationToken cancellationToken)
    {
        var page = Math.Max(pageNumber ?? 1, 1);
        var size = Math.Clamp(pageSize ?? 20, 1, 100);
        var query = dbContext.AppRoles
            .Include(role => role.RolePermissions)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(role => role.Name.Contains(term) || role.Description.Contains(term));
        }

        if (isActive.HasValue)
        {
            query = query.Where(role => role.IsActive == isActive.Value);
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(role => role.Name)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(total / (double)size);

        return Result<SecurityRoleSearchResult>.Success(new SecurityRoleSearchResult(
            items.Select(ToRoleDto).ToArray(),
            page,
            size,
            total,
            totalPages,
            page > 1,
            page < totalPages));
    }

    public async Task<Result> AssignPermissionToRoleAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken)
    {
        if (!Permissions.All.Any(permission => permission.Key == permissionKey))
        {
            return Result.Validation(new ValidationError(nameof(permissionKey), "Permission is not supported."));
        }

        if (!await dbContext.AppRoles.AnyAsync(role => role.Id == roleId && role.IsActive, cancellationToken))
        {
            return Result.NotFound(nameof(roleId), "Role was not found.");
        }

        await EnsurePermissionRowsAsync(cancellationToken);
        if (!await dbContext.AppRolePermissions.AnyAsync(item => item.RoleId == roleId && item.PermissionKey == permissionKey, cancellationToken))
        {
            dbContext.AppRolePermissions.Add(AppRolePermission.Create(roleId, permissionKey));
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success();
    }

    public async Task<Result> RemovePermissionFromRoleAsync(Guid roleId, string permissionKey, CancellationToken cancellationToken)
    {
        var rolePermission = await dbContext.AppRolePermissions.FindAsync([roleId, permissionKey], cancellationToken);
        if (rolePermission is null)
        {
            return Result.NotFound(nameof(permissionKey), "Role permission was not found.");
        }

        dbContext.AppRolePermissions.Remove(rolePermission);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<IReadOnlyCollection<SecurityPermission>>> GetRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken)
    {
        if (!await dbContext.AppRoles.AnyAsync(role => role.Id == roleId, cancellationToken))
        {
            return Result<IReadOnlyCollection<SecurityPermission>>.NotFound(nameof(roleId), "Role was not found.");
        }

        var keys = await dbContext.AppRolePermissions
            .Where(item => item.RoleId == roleId)
            .Select(item => item.PermissionKey)
            .ToListAsync(cancellationToken);
        var permissions = Permissions.All
            .Where(permission => keys.Contains(permission.Key))
            .Select(ToPermissionDto)
            .OrderBy(permission => permission.Module)
            .ThenBy(permission => permission.Key)
            .ToArray();
        return Result<IReadOnlyCollection<SecurityPermission>>.Success(permissions);
    }

    public Task<IReadOnlyCollection<SecurityPermission>> GetPermissionsAsync(CancellationToken cancellationToken)
    {
        IReadOnlyCollection<SecurityPermission> permissions = Permissions.All
            .Select(ToPermissionDto)
            .OrderBy(permission => permission.Module)
            .ThenBy(permission => permission.Key)
            .ToArray();
        return Task.FromResult(permissions);
    }

    public async Task<bool> UserHasPermissionAsync(Guid userId, string permissionKey, CancellationToken cancellationToken)
    {
        return await dbContext.AppUserRoles
            .Where(userRole => userRole.UserId == userId && userRole.User.IsActive && userRole.Role.IsActive)
            .SelectMany(userRole => userRole.Role.RolePermissions)
            .AnyAsync(rolePermission => rolePermission.PermissionKey == permissionKey, cancellationToken);
    }

    internal async Task SeedDefaultsAsync(string? superAdminEmail, string? superAdminPassword, CancellationToken cancellationToken)
    {
        await EnsurePermissionRowsAsync(cancellationToken);
        var roleMap = new Dictionary<string, AppRole>(StringComparer.Ordinal);
        foreach (var roleName in SecurityRoles.All)
        {
            var slug = GenerateSlug(roleName);
            var role = await dbContext.AppRoles.FirstOrDefaultAsync(item => item.Slug == slug, cancellationToken);
            if (role is null)
            {
                role = AppRole.Create(roleName, $"{roleName} role", isSystem: true, timeProvider.GetUtcNow());
                dbContext.AppRoles.Add(role);
            }

            roleMap[roleName] = role;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        await AssignDefaultRolePermissionsAsync(roleMap, cancellationToken);

        if (!string.IsNullOrWhiteSpace(superAdminEmail) && !string.IsNullOrWhiteSpace(superAdminPassword))
        {
            var normalizedEmail = superAdminEmail.Trim().ToUpperInvariant();
            if (!await dbContext.AppUsers.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken))
            {
                var now = timeProvider.GetUtcNow();
                var user = AppUser.Create(superAdminEmail, "superadmin", "Super Admin", "pending", now);
                user.SetPasswordHash(passwordHasher.HashPassword(user, superAdminPassword), now);
                dbContext.AppUsers.Add(user);
                await dbContext.SaveChangesAsync(cancellationToken);
                dbContext.AppUserRoles.Add(AppUserRole.Create(user.Id, roleMap[SecurityRoles.SuperAdmin].Id));
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task EnsurePermissionRowsAsync(CancellationToken cancellationToken)
    {
        var existingKeys = await dbContext.AppPermissions.Select(permission => permission.Key).ToListAsync(cancellationToken);
        foreach (var permission in Permissions.All.Where(permission => !existingKeys.Contains(permission.Key)))
        {
            dbContext.AppPermissions.Add(AppPermission.Create(permission.Key, permission.Name, permission.Module, permission.Description));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task AssignDefaultRolePermissionsAsync(Dictionary<string, AppRole> roles, CancellationToken cancellationToken)
    {
        var allKeys = Permissions.All.Select(permission => permission.Key).ToArray();
        var adminKeys = allKeys.Where(permission => !permission.StartsWith("roles.", StringComparison.Ordinal) && permission != Permissions.UsersCreate && permission != Permissions.UsersDelete && permission != Permissions.PermissionsUpdate).Append(Permissions.UsersRead).Append(Permissions.UsersUpdate).Distinct().ToArray();
        var editorKeys = Permissions.ContentRead.Concat(Permissions.ContentCreateUpdate).Concat([Permissions.PublishingRead, Permissions.NotificationsRead, Permissions.ContentQualityRead, Permissions.ContentQualityRun, Permissions.ContentRevisionsRead, Permissions.ContentRevisionsRestoreRequest, Permissions.BulkOperationsRead, Permissions.BulkOperationsRun, Permissions.ImportRead, Permissions.ImportUpload, Permissions.ImportValidate, Permissions.ImportRun]).Distinct().ToArray();
        var reviewerKeys = Permissions.ContentRead.Concat([Permissions.PublishingRead, Permissions.PublishingRun, Permissions.NotificationsRead, Permissions.ContentQualityRead, Permissions.ContentQualityRun, Permissions.ContentRevisionsRead, Permissions.ContentRevisionsRestoreApprove, Permissions.BulkOperationsRead, Permissions.BulkOperationsRun, Permissions.ImportRead, Permissions.ImportValidate]).Distinct().ToArray();
        var viewerKeys = Permissions.ContentRead.Concat([Permissions.PublishingRead, Permissions.NotificationsRead]).Distinct().ToArray();

        await AssignMissingAsync(roles[SecurityRoles.SuperAdmin], allKeys, cancellationToken);
        await AssignMissingAsync(roles[SecurityRoles.Admin], adminKeys, cancellationToken);
        await AssignMissingAsync(roles[SecurityRoles.ContentEditor], editorKeys, cancellationToken);
        await AssignMissingAsync(roles[SecurityRoles.Reviewer], reviewerKeys, cancellationToken);
        await AssignMissingAsync(roles[SecurityRoles.Viewer], viewerKeys, cancellationToken);
    }

    private async Task AssignMissingAsync(AppRole role, IReadOnlyCollection<string> permissionKeys, CancellationToken cancellationToken)
    {
        var existingKeys = await dbContext.AppRolePermissions
            .Where(item => item.RoleId == role.Id)
            .Select(item => item.PermissionKey)
            .ToListAsync(cancellationToken);
        foreach (var key in permissionKeys.Where(key => !existingKeys.Contains(key)))
        {
            dbContext.AppRolePermissions.Add(AppRolePermission.Create(role.Id, key));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<AppUser?> LoadUserByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) =>
        await dbContext.AppUsers
            .Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role).ThenInclude(role => role.RolePermissions)
            .FirstOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);

    private async Task<AppUser?> LoadUserByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.AppUsers
            .Include(user => user.UserRoles).ThenInclude(userRole => userRole.Role).ThenInclude(role => role.RolePermissions)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    private async Task<AppRole?> LoadRoleByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await dbContext.AppRoles
            .Include(role => role.RolePermissions)
            .FirstOrDefaultAsync(role => role.Id == id, cancellationToken);

    private static SecurityCurrentUser ToCurrentUserDto(AppUser user)
    {
        var activeRoles = user.UserRoles.Select(userRole => userRole.Role).Where(role => role.IsActive).ToArray();
        return new SecurityCurrentUser(
            user.Id,
            user.Email,
            user.UserName,
            user.DisplayName,
            activeRoles.Select(role => role.Name).OrderBy(name => name).ToArray(),
            activeRoles.SelectMany(role => role.RolePermissions.Select(permission => permission.PermissionKey)).Distinct().OrderBy(key => key).ToArray());
    }

    private static SecurityUser ToUserDto(AppUser user) =>
        new(
            user.Id,
            user.Email,
            user.UserName,
            user.DisplayName,
            user.IsActive,
            user.UserRoles.Select(userRole => new SecurityRoleSummary(userRole.Role.Id, userRole.Role.Name, userRole.Role.Slug)).OrderBy(role => role.Name).ToArray(),
            user.CreatedAt,
            user.UpdatedAt);

    private static SecurityRole ToRoleDto(AppRole role) =>
        new(
            role.Id,
            role.Name,
            role.Slug,
            role.Description,
            role.IsSystem,
            role.IsActive,
            role.RolePermissions.Select(permission => permission.PermissionKey).OrderBy(key => key).ToArray(),
            role.CreatedAt,
            role.UpdatedAt);

    private static SecurityPermission ToPermissionDto(PermissionDefinition permission) =>
        new(permission.Key, permission.Name, permission.Module, permission.Description);

    private static string GenerateSlug(string value)
    {
        var slug = new string((value ?? string.Empty)
            .Trim()
            .ToLowerInvariant()
            .Select(character => char.IsLetterOrDigit(character) ? character : '-')
            .ToArray());

        while (slug.Contains("--", StringComparison.Ordinal))
        {
            slug = slug.Replace("--", "-", StringComparison.Ordinal);
        }

        return slug.Trim('-');
    }

    private static List<ValidationError> ValidatePassword(string? password)
    {
        var errors = new List<ValidationError>();
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        {
            errors.Add(new ValidationError(nameof(password), "Password must be at least 8 characters."));
        }

        if (password is not null && !password.Any(char.IsUpper))
        {
            errors.Add(new ValidationError(nameof(password), "Password must contain an uppercase letter."));
        }

        if (password is not null && !password.Any(char.IsLower))
        {
            errors.Add(new ValidationError(nameof(password), "Password must contain a lowercase letter."));
        }

        if (password is not null && !password.Any(char.IsDigit))
        {
            errors.Add(new ValidationError(nameof(password), "Password must contain a digit."));
        }

        return errors;
    }
}
