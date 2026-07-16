using EnglishMaster.Domain.Motivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class AchievementDefinitionConfiguration : IEntityTypeConfiguration<AchievementDefinition>
{
    public void Configure(EntityTypeBuilder<AchievementDefinition> builder)
    {
        builder.HasKey(definition => definition.Id);
        builder.Property(definition => definition.Code).HasMaxLength(80).IsRequired();
        builder.Property(definition => definition.Name).HasMaxLength(160).IsRequired();
        builder.Property(definition => definition.Description).HasMaxLength(1000);
        builder.Property(definition => definition.AchievementType).HasMaxLength(80).IsRequired();
        builder.Property(definition => definition.IconName).HasMaxLength(80);
        builder.HasIndex(definition => definition.Code).IsUnique();
        builder.HasIndex(definition => definition.AchievementType);
        builder.HasIndex(definition => definition.IsActive);
    }
}
