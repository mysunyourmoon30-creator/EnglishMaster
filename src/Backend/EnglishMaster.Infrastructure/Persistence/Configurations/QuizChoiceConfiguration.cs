using EnglishMaster.Domain.Quizzes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class QuizChoiceConfiguration : IEntityTypeConfiguration<QuizChoice>
{
    public void Configure(EntityTypeBuilder<QuizChoice> builder)
    {
        builder.ToTable("QuizChoices");

        builder.HasKey(choice => choice.Id);

        builder.Property(choice => choice.ChoiceText)
            .HasMaxLength(QuizChoiceFieldLimits.ChoiceText)
            .IsRequired();

        builder.Property(choice => choice.IsCorrect)
            .IsRequired();

        builder.Property(choice => choice.ExplanationTh)
            .HasMaxLength(QuizChoiceFieldLimits.ExplanationTh)
            .IsRequired();

        builder.Property(choice => choice.ExplanationEn)
            .HasMaxLength(QuizChoiceFieldLimits.ExplanationEn)
            .IsRequired();

        builder.Property(choice => choice.SortOrder)
            .IsRequired();

        builder.Property(choice => choice.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(choice => choice.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(choice => choice.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(choice => choice.QuizQuestionId);
        builder.HasIndex(choice => choice.SortOrder);
    }
}
