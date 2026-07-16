using EnglishMaster.Domain.Grammar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class GrammarExampleConfiguration : IEntityTypeConfiguration<GrammarExample>
{
    public void Configure(EntityTypeBuilder<GrammarExample> builder)
    {
        builder.ToTable("GrammarExamples");

        builder.HasKey(grammarExample => grammarExample.Id);

        builder.Property(grammarExample => grammarExample.GrammarRuleId)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.ExampleEn)
            .HasMaxLength(GrammarExampleFieldLimits.ExampleEn)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.TranslationTh)
            .HasMaxLength(GrammarExampleFieldLimits.TranslationTh)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.ExplanationTh)
            .HasMaxLength(GrammarExampleFieldLimits.ExplanationTh)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.IsCorrectExample)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.SortOrder)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(grammarExample => grammarExample.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(grammarExample => grammarExample.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(grammarExample => grammarExample.GrammarRuleId);
        builder.HasIndex(grammarExample => grammarExample.IsActive);
    }
}
