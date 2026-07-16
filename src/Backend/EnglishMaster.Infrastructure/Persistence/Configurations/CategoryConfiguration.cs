using EnglishMaster.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(category => category.Id);

        builder.Property(category => category.Name)
            .HasMaxLength(CategoryFieldLimits.Name)
            .IsRequired();

        builder.Property(category => category.Slug)
            .HasMaxLength(CategoryFieldLimits.Slug)
            .IsRequired();

        builder.Property(category => category.Description)
            .HasMaxLength(CategoryFieldLimits.Description)
            .IsRequired();

        builder.Property(category => category.SortOrder)
            .IsRequired();

        builder.Property(category => category.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(category => category.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(category => category.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(category => category.Name);
        builder.HasIndex(category => category.Slug)
            .IsUnique();
        builder.HasIndex(category => category.IsActive);
    }
}
