using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.Web.Services.Words;

public sealed class WordFormModel
{
    [Required]
    [StringLength(200)]
    public string Text { get; set; } = string.Empty;

    [StringLength(100)]
    public string? IpaUk { get; set; }

    [StringLength(100)]
    public string? IpaUs { get; set; }

    [StringLength(200)]
    public string? ThaiReading { get; set; }

    [Required]
    [StringLength(1000)]
    public string MeaningTh { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? MeaningEn { get; set; }

    [Required]
    public string PartOfSpeech { get; set; } = "Noun";

    [Required]
    public string CefrLevel { get; set; } = "A1";

    [StringLength(1000)]
    public string? ExampleEn { get; set; }

    [StringLength(1000)]
    public string? ExampleTh { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? CategoryId { get; set; }

    public List<Guid> TagIds { get; } = [];

    public Guid? ImageMediaId { get; set; }

    public Guid? AudioMediaId { get; set; }

    public static WordFormModel FromDto(WordDto word)
    {
        var model = new WordFormModel
        {
            Text = word.Text,
            IpaUk = word.IpaUk,
            IpaUs = word.IpaUs,
            ThaiReading = word.ThaiReading,
            MeaningTh = word.MeaningTh,
            MeaningEn = word.MeaningEn,
            PartOfSpeech = word.PartOfSpeech,
            CefrLevel = word.CefrLevel,
            ExampleEn = word.ExampleEn,
            ExampleTh = word.ExampleTh,
            IsActive = word.IsActive,
            CategoryId = word.CategoryId,
            ImageMediaId = word.ImageMediaId,
            AudioMediaId = word.AudioMediaId
        };

        model.TagIds.AddRange(word.Tags.Select(tag => tag.Id));

        return model;
    }

    public CreateWordRequest ToCreateRequest()
    {
        return new CreateWordRequest(
            Text,
            IpaUk,
            IpaUs,
            ThaiReading,
            MeaningTh,
            MeaningEn,
            PartOfSpeech,
            CefrLevel,
            ExampleEn,
            ExampleTh,
            CategoryId,
            TagIds,
            ImageMediaId,
            AudioMediaId);
    }

    public UpdateWordRequest ToUpdateRequest()
    {
        return new UpdateWordRequest(
            Text,
            IpaUk,
            IpaUs,
            ThaiReading,
            MeaningTh,
            MeaningEn,
            PartOfSpeech,
            CefrLevel,
            ExampleEn,
            ExampleTh,
            IsActive,
            CategoryId,
            TagIds,
            ImageMediaId,
            AudioMediaId);
    }
}
