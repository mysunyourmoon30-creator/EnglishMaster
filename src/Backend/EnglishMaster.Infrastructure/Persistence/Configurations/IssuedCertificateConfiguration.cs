using EnglishMaster.Domain.Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class IssuedCertificateConfiguration : IEntityTypeConfiguration<IssuedCertificate>
{
    public void Configure(EntityTypeBuilder<IssuedCertificate> builder)
    {
        builder.HasKey(certificate => certificate.Id);
        builder.Property(certificate => certificate.VerificationCode).HasMaxLength(80).IsRequired();
        builder.Property(certificate => certificate.RecipientName).HasMaxLength(200).IsRequired();
        builder.Property(certificate => certificate.CourseTitle).HasMaxLength(200).IsRequired();
        builder.Property(certificate => certificate.TemplateCode).HasMaxLength(64).IsRequired();
        builder.Property(certificate => certificate.RenderedBody).HasMaxLength(8000).IsRequired();
        builder.HasIndex(certificate => certificate.VerificationCode).IsUnique();
        builder.HasIndex(certificate => new { certificate.UserId, certificate.CourseId }).IsUnique();
        builder.HasIndex(certificate => new { certificate.UserId, certificate.IssuedAt });
        builder.HasIndex(certificate => certificate.CourseId);
    }
}
