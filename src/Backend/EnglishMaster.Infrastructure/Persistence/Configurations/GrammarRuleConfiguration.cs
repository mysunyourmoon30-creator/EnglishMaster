using EnglishMaster.Domain.Grammar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class GrammarRuleConfiguration : IEntityTypeConfiguration<GrammarRule>
{
    public void Configure(EntityTypeBuilder<GrammarRule> builder)
    {
        builder.ToTable("GrammarRules");

        builder.HasKey(grammarRule => grammarRule.Id);

        builder.Property(grammarRule => grammarRule.GrammarTopicId)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.Title)
            .HasMaxLength(GrammarRuleFieldLimits.Title)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.Slug)
            .HasMaxLength(GrammarRuleFieldLimits.Slug)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.RuleText)
            .HasMaxLength(GrammarRuleFieldLimits.RuleText)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.ExplanationTh)
            .HasMaxLength(GrammarRuleFieldLimits.ExplanationTh)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.ExplanationEn)
            .HasMaxLength(GrammarRuleFieldLimits.ExplanationEn)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.StructurePattern)
            .HasMaxLength(GrammarRuleFieldLimits.StructurePattern)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.CommonMistake)
            .HasMaxLength(GrammarRuleFieldLimits.CommonMistake)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.CorrectUsageNote)
            .HasMaxLength(GrammarRuleFieldLimits.CorrectUsageNote)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.SortOrder)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(grammarRule => grammarRule.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(grammarRule => grammarRule.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(grammarRule => grammarRule.GrammarTopicId);
        builder.HasIndex(grammarRule => grammarRule.Title);
        builder.HasIndex(grammarRule => grammarRule.Slug).IsUnique();
        builder.HasIndex(grammarRule => grammarRule.IsActive);

        builder.HasMany(grammarRule => grammarRule.Examples)
            .WithOne()
            .HasForeignKey(grammarExample => grammarExample.GrammarRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(grammarRule => grammarRule.RelatedWords)
            .WithOne()
            .HasForeignKey(grammarRuleWord => grammarRuleWord.GrammarRuleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(grammarRule => grammarRule.Examples)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(grammarRule => grammarRule.RelatedWords)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
