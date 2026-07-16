using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class GrammarRuleWordConfiguration : IEntityTypeConfiguration<GrammarRuleWord>
{
    public void Configure(EntityTypeBuilder<GrammarRuleWord> builder)
    {
        builder.ToTable("GrammarRuleWords");

        builder.HasKey(grammarRuleWord => new
        {
            grammarRuleWord.GrammarRuleId,
            grammarRuleWord.WordId
        });

        builder.Property(grammarRuleWord => grammarRuleWord.GrammarRuleId)
            .IsRequired();

        builder.Property(grammarRuleWord => grammarRuleWord.WordId)
            .IsRequired();

        builder.HasIndex(grammarRuleWord => grammarRuleWord.WordId);

        builder.HasOne<Word>()
            .WithMany()
            .HasForeignKey(grammarRuleWord => grammarRuleWord.WordId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
