using EnglishMaster.Domain.Grammar;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class GrammarTopicConfiguration : IEntityTypeConfiguration<GrammarTopic>
{
    public void Configure(EntityTypeBuilder<GrammarTopic> builder)
    {
        builder.ToTable("GrammarTopics");

        builder.HasKey(grammarTopic => grammarTopic.Id);

        builder.Property(grammarTopic => grammarTopic.Title)
            .HasMaxLength(GrammarTopicFieldLimits.Title)
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.Slug)
            .HasMaxLength(GrammarTopicFieldLimits.Slug)
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.Summary)
            .HasMaxLength(GrammarTopicFieldLimits.Summary)
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.CefrLevel)
            .HasConversion<string>()
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.SortOrder)
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(grammarTopic => grammarTopic.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(grammarTopic => grammarTopic.Title);
        builder.HasIndex(grammarTopic => grammarTopic.Slug)
            .IsUnique();
        builder.HasIndex(grammarTopic => grammarTopic.CefrLevel);
        builder.HasIndex(grammarTopic => grammarTopic.IsActive);

        builder.HasMany(grammarTopic => grammarTopic.Rules)
            .WithOne()
            .HasForeignKey(grammarRule => grammarRule.GrammarTopicId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(grammarTopic => grammarTopic.Rules)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
