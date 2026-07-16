using EnglishMaster.Domain.Publishing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class PublishTemplateConfiguration : IEntityTypeConfiguration<PublishTemplate>
{
    public void Configure(EntityTypeBuilder<PublishTemplate> builder)
    {
        builder.ToTable("PublishTemplates");

        builder.HasKey(template => template.Id);

        builder.Property(template => template.Name)
            .HasMaxLength(PublishingFieldLimits.TemplateName)
            .IsRequired();

        builder.Property(template => template.Slug)
            .HasMaxLength(PublishingFieldLimits.TemplateSlug)
            .IsRequired();

        builder.Property(template => template.Description)
            .HasMaxLength(PublishingFieldLimits.TemplateDescription)
            .IsRequired();

        builder.Property(template => template.Format)
            .HasConversion<string>()
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(template => template.TemplateContent)
            .HasMaxLength(PublishingFieldLimits.TemplateContent)
            .IsRequired();

        builder.Property(template => template.IsDefault)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(template => template.IsActive)
            .HasDefaultValue(true)
            .IsRequired();

        builder.Property(template => template.CreatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.Property(template => template.UpdatedAt)
            .HasColumnType("datetimeoffset")
            .IsRequired();

        builder.HasIndex(template => template.Slug)
            .IsUnique();
        builder.HasIndex(template => template.Format);
        builder.HasIndex(template => template.IsDefault);
    }
}
