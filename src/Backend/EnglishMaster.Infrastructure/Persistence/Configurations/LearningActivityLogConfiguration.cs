using EnglishMaster.Domain.Motivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class LearningActivityLogConfiguration : IEntityTypeConfiguration<LearningActivityLog>
{
    public void Configure(EntityTypeBuilder<LearningActivityLog> builder)
    {
        builder.HasKey(activity => activity.Id);
        builder.Property(activity => activity.ActivityType).HasMaxLength(64).IsRequired();
        builder.Property(activity => activity.ContentType).HasMaxLength(64);
        builder.Property(activity => activity.Title).HasMaxLength(200);
        builder.Property(activity => activity.MetadataJson).HasMaxLength(2000);
        builder.HasIndex(activity => activity.StudentProfileId);
        builder.HasIndex(activity => activity.ActivityType);
        builder.HasIndex(activity => activity.OccurredAt);
    }
}
