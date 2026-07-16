namespace EnglishMaster.Domain.Grammar;

public sealed class GrammarRuleWord
{
    private GrammarRuleWord()
    {
    }

    public GrammarRuleWord(Guid grammarRuleId, Guid wordId)
    {
        GrammarRuleId = GrammarDomainGuard.RequiredId(grammarRuleId, nameof(grammarRuleId));
        WordId = GrammarDomainGuard.RequiredId(wordId, nameof(wordId));
    }

    public Guid GrammarRuleId { get; private set; }

    public Guid WordId { get; private set; }
}
