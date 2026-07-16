using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.GrammarTopics;

namespace EnglishMaster.Web.Services.Grammar;

public sealed class GrammarTopicFormModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Summary { get; set; }

    [Required]
    public string CefrLevel { get; set; } = "A1";

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static GrammarTopicFormModel FromDto(GrammarTopicDto topic)
    {
        return new GrammarTopicFormModel
        {
            Title = topic.Title,
            Summary = topic.Summary,
            CefrLevel = topic.CefrLevel,
            SortOrder = topic.SortOrder,
            IsActive = topic.IsActive
        };
    }

    public CreateGrammarTopicRequest ToCreateRequest()
    {
        return new CreateGrammarTopicRequest(Title, Summary, CefrLevel, SortOrder);
    }

    public UpdateGrammarTopicRequest ToUpdateRequest()
    {
        return new UpdateGrammarTopicRequest(Title, Summary, CefrLevel, SortOrder, IsActive);
    }
}
