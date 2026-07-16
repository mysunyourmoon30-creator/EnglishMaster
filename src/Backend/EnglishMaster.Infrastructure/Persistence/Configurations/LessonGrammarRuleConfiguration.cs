using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class LessonGrammarRuleConfiguration : IEntityTypeConfiguration<LessonGrammarRule>
{
    public void Configure(EntityTypeBuilder<LessonGrammarRule> builder)
    {
        builder.ToTable("LessonGrammarRules");

        builder.HasKey(lessonGrammarRule => new
        {
            lessonGrammarRule.LessonId,
            lessonGrammarRule.GrammarRuleId
        });

        builder.Property(lessonGrammarRule => lessonGrammarRule.LessonId)
            .IsRequired();

        builder.Property(lessonGrammarRule => lessonGrammarRule.GrammarRuleId)
            .IsRequired();

        builder.Property(lessonGrammarRule => lessonGrammarRule.SortOrder)
            .IsRequired();

        builder.HasIndex(lessonGrammarRule => lessonGrammarRule.GrammarRuleId);

        builder.HasOne<GrammarRule>()
            .WithMany()
            .HasForeignKey(lessonGrammarRule => lessonGrammarRule.GrammarRuleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
