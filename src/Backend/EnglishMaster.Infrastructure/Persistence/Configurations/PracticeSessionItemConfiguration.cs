using EnglishMaster.Domain.Practice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class PracticeSessionItemConfiguration : IEntityTypeConfiguration<PracticeSessionItem>
{
    public void Configure(EntityTypeBuilder<PracticeSessionItem> builder)
    {
        builder.HasKey(item => item.Id);
        builder.Property(item => item.ContentType).HasMaxLength(64).IsRequired();
        builder.Property(item => item.PracticeType).HasMaxLength(64).IsRequired();
        builder.Property(item => item.PromptText).HasMaxLength(1000).IsRequired();
        builder.Property(item => item.AnswerText).HasMaxLength(1000).IsRequired();
        builder.Property(item => item.UserAnswer).HasMaxLength(1000).IsRequired();
        builder.Property(item => item.Result).HasConversion<string>().HasMaxLength(32);
        builder.HasIndex(item => item.PracticeSessionId);
        builder.HasIndex(item => item.PracticeItemId);
    }
}

