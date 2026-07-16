using EnglishMaster.Domain.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("AppUsers");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Email).HasMaxLength(256).IsRequired();
        builder.Property(user => user.NormalizedEmail).HasMaxLength(256).IsRequired();
        builder.Property(user => user.UserName).HasMaxLength(128).IsRequired();
        builder.Property(user => user.NormalizedUserName).HasMaxLength(128).IsRequired();
        builder.Property(user => user.DisplayName).HasMaxLength(256).IsRequired();
        builder.Property(user => user.PasswordHash).HasMaxLength(1024).IsRequired();
        builder.HasIndex(user => user.NormalizedEmail).IsUnique();
        builder.HasIndex(user => user.NormalizedUserName).IsUnique();
    }
}

internal sealed class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        builder.ToTable("AppRoles");
        builder.HasKey(role => role.Id);
        builder.Property(role => role.Name).HasMaxLength(128).IsRequired();
        builder.Property(role => role.Slug).HasMaxLength(128).IsRequired();
        builder.Property(role => role.Description).HasMaxLength(512).IsRequired();
        builder.HasIndex(role => role.Slug).IsUnique();
    }
}

internal sealed class AppPermissionConfiguration : IEntityTypeConfiguration<AppPermission>
{
    public void Configure(EntityTypeBuilder<AppPermission> builder)
    {
        builder.ToTable("AppPermissions");
        builder.HasKey(permission => permission.Key);
        builder.Property(permission => permission.Key).HasMaxLength(128).IsRequired();
        builder.Property(permission => permission.Name).HasMaxLength(128).IsRequired();
        builder.Property(permission => permission.Module).HasMaxLength(128).IsRequired();
        builder.Property(permission => permission.Description).HasMaxLength(512).IsRequired();
        builder.HasIndex(permission => permission.Module);
    }
}

internal sealed class AppUserRoleConfiguration : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        builder.ToTable("AppUserRoles");
        builder.HasKey(userRole => new { userRole.UserId, userRole.RoleId });
        builder.HasOne(userRole => userRole.User)
            .WithMany(user => user.UserRoles)
            .HasForeignKey(userRole => userRole.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(userRole => userRole.Role)
            .WithMany(role => role.UserRoles)
            .HasForeignKey(userRole => userRole.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class AppRolePermissionConfiguration : IEntityTypeConfiguration<AppRolePermission>
{
    public void Configure(EntityTypeBuilder<AppRolePermission> builder)
    {
        builder.ToTable("AppRolePermissions");
        builder.HasKey(rolePermission => new { rolePermission.RoleId, rolePermission.PermissionKey });
        builder.Property(rolePermission => rolePermission.PermissionKey).HasMaxLength(128).IsRequired();
        builder.HasOne(rolePermission => rolePermission.Role)
            .WithMany(role => role.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(rolePermission => rolePermission.Permission)
            .WithMany(permission => permission.RolePermissions)
            .HasForeignKey(rolePermission => rolePermission.PermissionKey)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
