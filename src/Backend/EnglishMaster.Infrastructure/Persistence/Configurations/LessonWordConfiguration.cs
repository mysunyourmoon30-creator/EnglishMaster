using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class LessonWordConfiguration : IEntityTypeConfiguration<LessonWord>
{
    public void Configure(EntityTypeBuilder<LessonWord> builder)
    {
        builder.ToTable("LessonWords");

        builder.HasKey(lessonWord => new
        {
            lessonWord.LessonId,
            lessonWord.WordId
        });

        builder.Property(lessonWord => lessonWord.LessonId)
            .IsRequired();

        builder.Property(lessonWord => lessonWord.WordId)
            .IsRequired();

        builder.Property(lessonWord => lessonWord.SortOrder)
            .IsRequired();

        builder.HasIndex(lessonWord => lessonWord.WordId);

        builder.HasOne<Word>()
            .WithMany()
            .HasForeignKey(lessonWord => lessonWord.WordId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
