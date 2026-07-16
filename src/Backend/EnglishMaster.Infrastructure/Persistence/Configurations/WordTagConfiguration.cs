using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnglishMaster.Infrastructure.Persistence.Configurations;

internal sealed class WordTagConfiguration : IEntityTypeConfiguration<WordTag>
{
    public void Configure(EntityTypeBuilder<WordTag> builder)
    {
        builder.ToTable("WordTags");

        builder.HasKey(wordTag => new { wordTag.WordId, wordTag.TagId });

        builder.HasIndex(wordTag => wordTag.TagId);

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(wordTag => wordTag.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
