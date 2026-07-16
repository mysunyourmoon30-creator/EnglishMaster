using EnglishMaster.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(notification => notification.Id);
        builder.Property(notification => notification.Title).HasMaxLength(256).IsRequired();
        builder.Property(notification => notification.Message).HasMaxLength(2000).IsRequired();
        builder.Property(notification => notification.ErrorMessage).HasMaxLength(1000);
        builder.Property(notification => notification.Type).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(notification => notification.EventType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(notification => notification.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(notification => notification.RecipientUserId);
        builder.HasIndex(notification => notification.Status);
        builder.HasIndex(notification => notification.EventType);
        builder.HasIndex(notification => notification.CreatedAt);
    }
}
