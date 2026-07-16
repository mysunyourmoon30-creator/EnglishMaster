using EnglishMaster.Domain.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(tag => tag.Id);

        builder.Property(tag => tag.Name)
            .HasMaxLength(TagFieldLimits.Name)
            .IsRequired();

        builder.Property(tag => tag.Slug)
            .HasMaxLength(TagFieldLimits.Slug)
            .IsRequired();

        builder.Property(tag => tag.Description)
            .HasMaxLength(TagFieldLimits.Description)
            .IsRequired();

        builder.Property(tag => tag.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(tag => tag.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(tag => tag.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(tag => tag.Name);
        builder.HasIndex(tag => tag.Slug)
            .IsUnique();
        builder.HasIndex(tag => tag.IsActive);
    }
}
