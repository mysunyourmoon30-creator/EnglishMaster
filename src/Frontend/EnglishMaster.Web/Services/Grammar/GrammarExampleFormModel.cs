using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.GrammarExamples;

namespace EnglishMaster.Web.Services.Grammar;

public sealed class GrammarExampleFormModel
{
    [Required]
    [StringLength(1000)]
    public string ExampleEn { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? TranslationTh { get; set; }

    [StringLength(2000)]
    public string? ExplanationTh { get; set; }

    public bool IsCorrectExample { get; set; } = true;

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static GrammarExampleFormModel FromDto(GrammarExampleDto example)
    {
        return new GrammarExampleFormModel
        {
            ExampleEn = example.ExampleEn,
            TranslationTh = example.TranslationTh,
            ExplanationTh = example.ExplanationTh,
            IsCorrectExample = example.IsCorrectExample,
            SortOrder = example.SortOrder,
            IsActive = example.IsActive
        };
    }

    public CreateGrammarExampleRequest ToCreateRequest()
    {
        return new CreateGrammarExampleRequest(
            ExampleEn,
            TranslationTh,
            ExplanationTh,
            IsCorrectExample,
            SortOrder);
    }

    public UpdateGrammarExampleRequest ToUpdateRequest()
    {
        return new UpdateGrammarExampleRequest(
            ExampleEn,
            TranslationTh,
            ExplanationTh,
            IsCorrectExample,
            SortOrder,
            IsActive);
    }
}
