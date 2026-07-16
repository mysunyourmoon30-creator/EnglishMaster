using EnglishMaster.Domain.Publishing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class PublishedArtifactConfiguration : IEntityTypeConfiguration<PublishedArtifact>
{
    public void Configure(EntityTypeBuilder<PublishedArtifact> builder)
    {
        builder.ToTable("PublishedArtifacts");

        builder.HasKey(artifact => artifact.Id);

        builder.Property(artifact => artifact.PublishJobId)
            .IsRequired();

        builder.Property(artifact => artifact.SourceType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(artifact => artifact.SourceId)
            .IsRequired();

        builder.Property(artifact => artifact.Format)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(artifact => artifact.FileName)
            .HasMaxLength(PublishingFieldLimits.ArtifactFileName)
            .IsRequired();

        builder.Property(artifact => artifact.FilePath)
            .HasMaxLength(PublishingFieldLimits.ArtifactFilePath)
            .IsRequired();

        builder.Property(artifact => artifact.PublicUrl)
            .HasMaxLength(PublishingFieldLimits.ArtifactPublicUrl)
            .IsRequired();

        builder.Property(artifact => artifact.FileSize)
            .IsRequired();

        builder.Property(artifact => artifact.ContentType)
            .HasMaxLength(PublishingFieldLimits.ArtifactContentType)
            .IsRequired();

        builder.Property(artifact => artifact.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(artifact => artifact.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(artifact => artifact.PublishJobId);
        builder.HasIndex(artifact => artifact.SourceType);
        builder.HasIndex(artifact => artifact.SourceId);
        builder.HasIndex(artifact => artifact.Format);
    }
}
