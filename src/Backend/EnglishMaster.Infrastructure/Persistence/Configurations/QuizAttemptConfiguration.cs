using EnglishMaster.Domain.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

public sealed class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
{
    public void Configure(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.HasKey(attempt => attempt.Id);
        builder.Property(attempt => attempt.Score).HasPrecision(5, 2);
        builder.HasIndex(attempt => new { attempt.UserId, attempt.QuizId, attempt.AttemptedAt });
        builder.HasIndex(attempt => new { attempt.UserId, attempt.Passed });
    }
}

