using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class QuizQuestionConfiguration : IEntityTypeConfiguration<QuizQuestion>
{
    public void Configure(EntityTypeBuilder<QuizQuestion> builder)
    {
        builder.ToTable("QuizQuestions");

        builder.HasKey(question => question.Id);

        builder.Property(question => question.QuestionText)
            .HasMaxLength(QuizQuestionFieldLimits.QuestionText)
            .IsRequired();

        builder.Property(question => question.QuestionType)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(question => question.ExplanationTh)
            .HasMaxLength(QuizQuestionFieldLimits.ExplanationTh)
            .IsRequired();

        builder.Property(question => question.ExplanationEn)
            .HasMaxLength(QuizQuestionFieldLimits.ExplanationEn)
            .IsRequired();

        builder.Property(question => question.Points)
            .IsRequired();

        builder.Property(question => question.SortOrder)
            .IsRequired();

        builder.Property(question => question.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(question => question.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(question => question.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(question => question.QuizId);
        builder.HasIndex(question => question.QuestionType);
        builder.HasIndex(question => question.SortOrder);

        // ClientSetNull avoids SQL Server error 1785 (multiple cascade paths): Word is also
        // reachable via QuizQuestion -> Pronunciation -> Word (Pronunciation.WordId cascades).
        builder.HasOne<Word>()
            .WithMany()
            .HasForeignKey(question => question.WordId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        builder.HasOne<GrammarRule>()
            .WithMany()
            .HasForeignKey(question => question.GrammarRuleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<Pronunciation>()
            .WithMany()
            .HasForeignKey(question => question.PronunciationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(question => question.Choices)
            .WithOne()
            .HasForeignKey(choice => choice.QuizQuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(question => question.Choices)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
