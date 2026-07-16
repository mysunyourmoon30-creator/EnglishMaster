using EnglishMaster.Domain.Pronunciations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class MinimalPairConfiguration : IEntityTypeConfiguration<MinimalPair>
{
    public void Configure(EntityTypeBuilder<MinimalPair> builder)
    {
        builder.ToTable("MinimalPairs");

        builder.HasKey(minimalPair => minimalPair.Id);

        builder.Property(minimalPair => minimalPair.PronunciationId)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.PairWordText)
            .HasMaxLength(MinimalPairFieldLimits.PairWordText)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.PairIpa)
            .HasMaxLength(MinimalPairFieldLimits.PairIpa)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.PairThaiReading)
            .HasMaxLength(MinimalPairFieldLimits.PairThaiReading)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.DifferenceNote)
            .HasMaxLength(MinimalPairFieldLimits.DifferenceNote)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.AudioMediaId);

        builder.Property(minimalPair => minimalPair.SortOrder)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(minimalPair => minimalPair.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(minimalPair => minimalPair.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(minimalPair => minimalPair.PronunciationId);
        builder.HasIndex(minimalPair => minimalPair.PairWordText);
        builder.HasIndex(minimalPair => minimalPair.AudioMediaId);
        builder.HasIndex(minimalPair => minimalPair.IsActive);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(minimalPair => minimalPair.AudioMediaId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
