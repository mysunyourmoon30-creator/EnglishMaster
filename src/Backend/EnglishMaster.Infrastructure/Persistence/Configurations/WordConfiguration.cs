using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class WordConfiguration : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        builder.ToTable("Words");

        builder.HasKey(word => word.Id);

        builder.Property(word => word.Text)
            .HasMaxLength(WordFieldLimits.Text)
            .IsRequired();

        builder.Property(word => word.Slug)
            .HasMaxLength(WordFieldLimits.Slug)
            .IsRequired();

        builder.Property(word => word.IpaUk)
            .HasMaxLength(WordFieldLimits.IpaUk)
            .IsRequired();

        builder.Property(word => word.IpaUs)
            .HasMaxLength(WordFieldLimits.IpaUs)
            .IsRequired();

        builder.Property(word => word.ThaiReading)
            .HasMaxLength(WordFieldLimits.ThaiReading)
            .IsRequired();

        builder.Property(word => word.MeaningTh)
            .HasMaxLength(WordFieldLimits.MeaningTh)
            .IsRequired();

        builder.Property(word => word.MeaningEn)
            .HasMaxLength(WordFieldLimits.MeaningEn)
            .IsRequired();

        builder.Property(word => word.PartOfSpeech)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(word => word.CefrLevel)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(word => word.ExampleEn)
            .HasMaxLength(WordFieldLimits.ExampleEn)
            .IsRequired();

        builder.Property(word => word.ExampleTh)
            .HasMaxLength(WordFieldLimits.ExampleTh)
            .IsRequired();

        builder.Property(word => word.CategoryId);

        builder.Property(word => word.ImageMediaId);

        builder.Property(word => word.AudioMediaId);

        builder.Property(word => word.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(word => word.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(word => word.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(word => word.Text);
        builder.HasIndex(word => word.Slug)
            .IsUnique();
        builder.HasIndex(word => word.CefrLevel);
        builder.HasIndex(word => word.IsActive);
        builder.HasIndex(word => word.CategoryId);
        builder.HasIndex(word => word.ImageMediaId);
        builder.HasIndex(word => word.AudioMediaId);

        builder.HasOne<Category>()
            .WithMany()
            .HasForeignKey(word => word.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // ClientSetNull avoids SQL Server error 1785 (multiple cascade paths): two SetNull FKs
        // from Words to Media would both try to null out on delete, which SQL Server rejects.
        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(word => word.ImageMediaId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne<MediaEntity>()
            .WithMany()
            .HasForeignKey(word => word.AudioMediaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(word => word.Tags)
            .WithOne()
            .HasForeignKey(wordTag => wordTag.WordId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(word => word.Tags)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
