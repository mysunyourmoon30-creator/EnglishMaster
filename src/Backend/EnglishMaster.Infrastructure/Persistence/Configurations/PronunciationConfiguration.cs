using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class PronunciationConfiguration : IEntityTypeConfiguration<Pronunciation>
{
    public void Configure(EntityTypeBuilder<Pronunciation> builder)
    {
        builder.ToTable("Pronunciations");

        builder.HasKey(pronunciation => pronunciation.Id);

        builder.Property(pronunciation => pronunciation.WordId)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.IpaUk)
            .HasMaxLength(PronunciationFieldLimits.IpaUk)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.IpaUs)
            .HasMaxLength(PronunciationFieldLimits.IpaUs)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.ThaiReading)
            .HasMaxLength(PronunciationFieldLimits.ThaiReading)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.Syllables)
            .HasMaxLength(PronunciationFieldLimits.Syllables)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.StressPattern)
            .HasMaxLength(PronunciationFieldLimits.StressPattern)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.MouthPosition)
            .HasMaxLength(PronunciationFieldLimits.MouthPosition)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.TonguePosition)
            .HasMaxLength(PronunciationFieldLimits.TonguePosition)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.CommonMistake)
            .HasMaxLength(PronunciationFieldLimits.CommonMistake)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.PracticeNote)
            .HasMaxLength(PronunciationFieldLimits.PracticeNote)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.AudioSlowMediaId);
        builder.Property(pronunciation => pronunciation.AudioNormalMediaId);
        builder.Property(pronunciation => pronunciation.MouthImageMediaId);

        builder.Property(pronunciation => pronunciation.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(pronunciation => pronunciation.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(pronunciation => pronunciation.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(pronunciation => pronunciation.WordId)
            .IsUnique();
        builder.HasIndex(pronunciation => pronunciation.IpaUk);
        builder.HasIndex(pronunciation => pronunciation.IpaUs);
        builder.HasIndex(pronunciation => pronunciation.IsActive);
        builder.HasIndex(pronunciation => pronunciation.AudioSlowMediaId);
        builder.HasIndex(pronunciation => pronunciation.AudioNormalMediaId);
        builder.HasIndex(pronunciation => pronunciation.MouthImageMediaId);

        builder.HasOne<Word>()
            .WithOne()
            .HasForeignKey<Pronunciation>(pronunciation => pronunciation.WordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(pronunciation => pronunciation.AudioSlowMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        // ClientSetNull avoids SQL Server error 1785 (multiple cascade paths): three SetNull FKs
        // from Pronunciations to Media would all try to null out on delete, which SQL Server rejects.
        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(pronunciation => pronunciation.AudioNormalMediaId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(pronunciation => pronunciation.MouthImageMediaId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasMany(pronunciation => pronunciation.MinimalPairs)
            .WithOne()
            .HasForeignKey(minimalPair => minimalPair.PronunciationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(pronunciation => pronunciation.MinimalPairs)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
