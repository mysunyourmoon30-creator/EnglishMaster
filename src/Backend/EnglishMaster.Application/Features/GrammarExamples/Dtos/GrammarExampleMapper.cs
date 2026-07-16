using EnglishMaster.Contracts.GrammarExamples;
using EnglishMaster.Domain.Grammar;

namespace EnglishMaster.Application.Features.GrammarExamples.Dtos;

internal static class GrammarExampleMapper
{
    public static GrammarExampleDto ToDto(GrammarExample grammarExample)
    {
        return new GrammarExampleDto(
            grammarExample.Id,
            grammarExample.GrammarRuleId,
            grammarExample.ExampleEn,
            grammarExample.TranslationTh,
            grammarExample.ExplanationTh,
            grammarExample.IsCorrectExample,
            grammarExample.SortOrder,
            grammarExample.IsActive,
            grammarExample.CreatedAt,
            grammarExample.UpdatedAt);
    }
}
