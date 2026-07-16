using EnglishMaster.Domain.ContentRevisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ContentRevisionRestoreRequestConfiguration : IEntityTypeConfiguration<ContentRevisionRestoreRequest>
{
    public void Configure(EntityTypeBuilder<ContentRevisionRestoreRequest> builder)
    {
        builder.HasKey(request => request.Id);
        builder.Property(request => request.RequestedBy).HasMaxLength(256).IsRequired();
        builder.Property(request => request.Reason).HasMaxLength(1000).IsRequired();
        builder.Property(request => request.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(request => request.ReviewedBy).HasMaxLength(256).IsRequired();
        builder.Property(request => request.ReviewNote).HasMaxLength(1000).IsRequired();
        builder.HasIndex(request => request.ContentRevisionId);
        builder.HasIndex(request => request.Status);
        builder.HasIndex(request => request.RequestedBy);
        builder.HasIndex(request => request.RequestedAt);
    }
}
