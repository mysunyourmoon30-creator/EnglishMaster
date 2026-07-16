namespace EnglishMaster.Domain.Words;

public sealed class WordTag
{
    private WordTag()
    {
    }

    internal WordTag(Guid wordId, Guid tagId)
    {
        if (wordId == Guid.Empty)
        {
            throw new ArgumentException("WordId is required.", nameof(wordId));
        }

        if (tagId == Guid.Empty)
        {
            throw new ArgumentException("TagId is required.", nameof(tagId));
        }

        WordId = wordId;
        TagId = tagId;
    }

    public Guid WordId { get; private set; }

    public Guid TagId { get; private set; }
}
