using EnglishMaster.Domain.BulkOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class BulkOperationConfiguration : IEntityTypeConfiguration<BulkOperation>
{
    public void Configure(EntityTypeBuilder<BulkOperation> builder)
    {
        builder.ToTable("BulkOperations");
        builder.HasKey(operation => operation.Id);
        builder.Property(operation => operation.OperationType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(operation => operation.ContentType).HasMaxLength(64).IsRequired();
        builder.Property(operation => operation.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(operation => operation.RequestedBy).HasMaxLength(256);
        builder.Property(operation => operation.ErrorMessage).HasMaxLength(1000);
        builder.Property(operation => operation.Note).HasMaxLength(1000);
        builder.Property(operation => operation.TagIds).HasMaxLength(2000);
        builder.Property(operation => operation.ExportFormat).HasMaxLength(32);
        builder.HasMany(operation => operation.Items)
            .WithOne()
            .HasForeignKey(item => item.BulkOperationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(operation => operation.OperationType);
        builder.HasIndex(operation => operation.ContentType);
        builder.HasIndex(operation => operation.Status);
        builder.HasIndex(operation => operation.RequestedBy);
        builder.HasIndex(operation => operation.RequestedAt);
    }
}
