using EnglishMaster.Domain.Notifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class EmailMessageConfiguration : IEntityTypeConfiguration<EmailMessage>
{
    public void Configure(EntityTypeBuilder<EmailMessage> builder)
    {
        builder.ToTable("EmailMessages");
        builder.HasKey(email => email.Id);
        builder.Property(email => email.ToEmail).HasMaxLength(256).IsRequired();
        builder.Property(email => email.ToName).HasMaxLength(256);
        builder.Property(email => email.Subject).HasMaxLength(256).IsRequired();
        builder.Property(email => email.Body).HasMaxLength(8000).IsRequired();
        builder.Property(email => email.ErrorMessage).HasMaxLength(1000);
        builder.Property(email => email.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(email => email.ToEmail);
        builder.HasIndex(email => email.Status);
        builder.HasIndex(email => email.CreatedAt);
    }
}
