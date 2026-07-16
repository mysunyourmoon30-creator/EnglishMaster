namespace EnglishMaster.Domain.Lessons;

public sealed class LessonWord
{
    private LessonWord()
    {
    }

    public LessonWord(Guid lessonId, Guid wordId, int sortOrder)
    {
        LessonId = LessonDomainGuard.RequiredId(lessonId, nameof(lessonId));
        WordId = LessonDomainGuard.RequiredId(wordId, nameof(wordId));
        SortOrder = LessonDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
    }

    public Guid LessonId { get; private set; }

    public Guid WordId { get; private set; }

    public int SortOrder { get; private set; }
}
