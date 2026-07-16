using EnglishMaster.Domain.ContentRevisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class ContentRevisionConfiguration : IEntityTypeConfiguration<ContentRevision>
{
    public void Configure(EntityTypeBuilder<ContentRevision> builder)
    {
        builder.HasKey(revision => revision.Id);
        builder.Property(revision => revision.ContentType).HasMaxLength(64).IsRequired();
        builder.Property(revision => revision.EventType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(revision => revision.Title).HasMaxLength(256).IsRequired();
        builder.Property(revision => revision.Summary).HasMaxLength(1000).IsRequired();
        builder.Property(revision => revision.ChangedBy).HasMaxLength(256).IsRequired();
        builder.Property(revision => revision.ChangeReason).HasMaxLength(1000).IsRequired();
        builder.Property(revision => revision.SnapshotJson).HasMaxLength(8000).IsRequired();
        builder.Property(revision => revision.DiffJson).HasMaxLength(8000).IsRequired();
        builder.HasIndex(revision => new { revision.ContentType, revision.ContentId });
        builder.HasIndex(revision => revision.RevisionNumber);
        builder.HasIndex(revision => revision.EventType);
        builder.HasIndex(revision => revision.ChangedBy);
        builder.HasIndex(revision => revision.ChangedAt);
    }
}
