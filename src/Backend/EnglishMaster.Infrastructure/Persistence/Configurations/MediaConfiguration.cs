using EnglishMaster.Domain.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class MediaConfiguration : IEntityTypeConfiguration<MediaEntity>
{
    public void Configure(EntityTypeBuilder<MediaEntity> builder)
    {
        builder.ToTable("Media");

        builder.HasKey(media => media.Id);

        builder.Property(media => media.FileName)
            .HasMaxLength(MediaFieldLimits.FileName)
            .IsRequired();

        builder.Property(media => media.OriginalFileName)
            .HasMaxLength(MediaFieldLimits.OriginalFileName)
            .IsRequired();

        builder.Property(media => media.FileExtension)
            .HasMaxLength(MediaFieldLimits.FileExtension)
            .IsRequired();

        builder.Property(media => media.ContentType)
            .HasMaxLength(MediaFieldLimits.ContentType)
            .IsRequired();

        builder.Property(media => media.FileSize)
            .IsRequired();

        builder.Property(media => media.MediaType)
            .HasConversion<string>()
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(media => media.StoragePath)
            .HasMaxLength(MediaFieldLimits.StoragePath)
            .IsRequired();

        builder.Property(media => media.PublicUrl)
            .HasMaxLength(MediaFieldLimits.PublicUrl)
            .IsRequired();

        builder.Property(media => media.AltText)
            .HasMaxLength(MediaFieldLimits.AltText)
            .IsRequired();

        builder.Property(media => media.Description)
            .HasMaxLength(MediaFieldLimits.Description)
            .IsRequired();

        builder.Property(media => media.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(media => media.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(media => media.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(media => media.FileName);
        builder.HasIndex(media => media.MediaType);
        builder.HasIndex(media => media.ContentType);
        builder.HasIndex(media => media.IsActive);
        builder.HasIndex(media => media.CreatedAt);
    }
}
