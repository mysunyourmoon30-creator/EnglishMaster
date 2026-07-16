namespace EnglishMaster.Domain.Lessons;

public sealed class LessonGrammarRule
{
    private LessonGrammarRule()
    {
    }

    public LessonGrammarRule(Guid lessonId, Guid grammarRuleId, int sortOrder)
    {
        LessonId = LessonDomainGuard.RequiredId(lessonId, nameof(lessonId));
        GrammarRuleId = LessonDomainGuard.RequiredId(grammarRuleId, nameof(grammarRuleId));
        SortOrder = LessonDomainGuard.NonNegative(sortOrder, nameof(sortOrder));
    }

    public Guid LessonId { get; private set; }

    public Guid GrammarRuleId { get; private set; }

    public int SortOrder { get; private set; }
}
