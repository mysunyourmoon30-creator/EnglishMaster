using EnglishMaster.Domain.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class StudentProfileConfiguration : IEntityTypeConfiguration<StudentProfile>
{
    public void Configure(EntityTypeBuilder<StudentProfile> builder)
    {
        builder.HasKey(profile => profile.Id);
        builder.Property(profile => profile.CurrentCefrLevel).HasConversion<string>().HasMaxLength(16);
        builder.HasIndex(profile => profile.UserId).IsUnique();
    }
}

