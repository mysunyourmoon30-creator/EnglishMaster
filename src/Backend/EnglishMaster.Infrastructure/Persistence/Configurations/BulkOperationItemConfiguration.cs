using EnglishMaster.Domain.BulkOperations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class BulkOperationItemConfiguration : IEntityTypeConfiguration<BulkOperationItem>
{
    public void Configure(EntityTypeBuilder<BulkOperationItem> builder)
    {
        builder.ToTable("BulkOperationItems");
        builder.HasKey(item => item.Id);
        builder.Property(item => item.Status).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(item => item.ErrorMessage).HasMaxLength(1000);

        builder.HasIndex(item => item.BulkOperationId);
        builder.HasIndex(item => item.ContentId);
        builder.HasIndex(item => item.Status);
    }
}
