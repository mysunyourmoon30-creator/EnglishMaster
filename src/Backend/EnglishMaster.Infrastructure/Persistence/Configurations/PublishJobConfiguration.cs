using EnglishMaster.Domain.Publishing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class PublishJobConfiguration : IEntityTypeConfiguration<PublishJob>
{
    public void Configure(EntityTypeBuilder<PublishJob> builder)
    {
        builder.ToTable("PublishJobs");

        builder.HasKey(job => job.Id);

        builder.Property(job => job.SourceType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(job => job.SourceId)
            .IsRequired();

        builder.Property(job => job.Format)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(job => job.Status)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(job => job.Title)
            .HasMaxLength(PublishingFieldLimits.Title)
            .IsRequired();

        builder.Property(job => job.OutputFileName)
            .HasMaxLength(PublishingFieldLimits.OutputFileName)
            .IsRequired();

        builder.Property(job => job.OutputPath)
            .HasMaxLength(PublishingFieldLimits.OutputPath)
            .IsRequired();

        builder.Property(job => job.ErrorMessage)
            .HasMaxLength(PublishingFieldLimits.ErrorMessage)
            .IsRequired();

        builder.Property(job => job.RequestedBy)
            .HasMaxLength(PublishingFieldLimits.RequestedBy)
            .IsRequired();

        builder.Property(job => job.StartedAt)
            .HasColumnType("datetimeoffset");

        builder.Property(job => job.CompletedAt)
            .HasColumnType("datetimeoffset");

        builder.Property(job => job.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(job => job.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(job => job.SourceType);
        builder.HasIndex(job => job.SourceId);
        builder.HasIndex(job => job.Format);
        builder.HasIndex(job => job.Status);
        builder.HasIndex(job => job.CreatedAt);

        builder.HasMany(job => job.Artifacts)
            .WithOne()
            .HasForeignKey(artifact => artifact.PublishJobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(job => job.Artifacts)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
