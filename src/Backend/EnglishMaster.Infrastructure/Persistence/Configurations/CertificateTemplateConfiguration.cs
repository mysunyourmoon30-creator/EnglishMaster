using EnglishMaster.Domain.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class CertificateTemplateConfiguration : IEntityTypeConfiguration<CertificateTemplate>
{
    public void Configure(EntityTypeBuilder<CertificateTemplate> builder)
    {
        builder.HasKey(template => template.Id);
        builder.Property(template => template.Code).HasMaxLength(64).IsRequired();
        builder.Property(template => template.Name).HasMaxLength(160).IsRequired();
        builder.Property(template => template.Description).HasMaxLength(1000).IsRequired();
        builder.Property(template => template.BodyTemplate).HasMaxLength(8000).IsRequired();
        builder.HasIndex(template => template.Code).IsUnique();
        builder.HasIndex(template => template.IsActive);
    }
}
