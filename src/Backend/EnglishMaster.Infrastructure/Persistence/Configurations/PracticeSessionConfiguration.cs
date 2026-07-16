using EnglishMaster.Domain.Practice;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSession>
{
    public void Configure(EntityTypeBuilder<PracticeSession> builder)
    {
        builder.HasKey(session => session.Id);
        builder.Property(session => session.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasMany(session => session.Items).WithOne().HasForeignKey(item => item.PracticeSessionId).OnDelete(DeleteBehavior.Cascade);
        builder.Metadata.FindNavigation(nameof(PracticeSession.Items))?.SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.HasIndex(session => new { session.StudentProfileId, session.StartedAt });
        builder.HasIndex(session => new { session.StudentProfileId, session.Status });
    }
}

