using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Pronunciations;

namespace EnglishMaster.Web.Services.Pronunciations;

public sealed class PronunciationFormModel
{
    [Required]
    public Guid? WordId { get; set; }

    [StringLength(100)]
    public string? IpaUk { get; set; }

    [StringLength(100)]
    public string? IpaUs { get; set; }

    [StringLength(200)]
    public string? ThaiReading { get; set; }

    [StringLength(300)]
    public string? Syllables { get; set; }

    [StringLength(200)]
    public string? StressPattern { get; set; }

    [StringLength(1000)]
    public string? MouthPosition { get; set; }

    [StringLength(1000)]
    public string? TonguePosition { get; set; }

    [StringLength(1000)]
    public string? CommonMistake { get; set; }

    [StringLength(1000)]
    public string? PracticeNote { get; set; }

    public Guid? AudioSlowMediaId { get; set; }

    public Guid? AudioNormalMediaId { get; set; }

    public Guid? MouthImageMediaId { get; set; }

    public bool IsActive { get; set; } = true;

    public static PronunciationFormModel FromDto(PronunciationDto pronunciation)
    {
        return new PronunciationFormModel
        {
            WordId = pronunciation.WordId,
            IpaUk = pronunciation.IpaUk,
            IpaUs = pronunciation.IpaUs,
            ThaiReading = pronunciation.ThaiReading,
            Syllables = pronunciation.Syllables,
            StressPattern = pronunciation.StressPattern,
            MouthPosition = pronunciation.MouthPosition,
            TonguePosition = pronunciation.TonguePosition,
            CommonMistake = pronunciation.CommonMistake,
            PracticeNote = pronunciation.PracticeNote,
            AudioSlowMediaId = pronunciation.AudioSlowMediaId,
            AudioNormalMediaId = pronunciation.AudioNormalMediaId,
            MouthImageMediaId = pronunciation.MouthImageMediaId,
            IsActive = pronunciation.IsActive
        };
    }

    public CreatePronunciationRequest ToCreateRequest()
    {
        return new CreatePronunciationRequest(
            WordId ?? Guid.Empty,
            IpaUk,
            IpaUs,
            ThaiReading,
            Syllables,
            StressPattern,
            MouthPosition,
            TonguePosition,
            CommonMistake,
            PracticeNote,
            AudioSlowMediaId,
            AudioNormalMediaId,
            MouthImageMediaId);
    }

    public UpdatePronunciationRequest ToUpdateRequest()
    {
        return new UpdatePronunciationRequest(
            WordId ?? Guid.Empty,
            IpaUk,
            IpaUs,
            ThaiReading,
            Syllables,
            StressPattern,
            MouthPosition,
            TonguePosition,
            CommonMistake,
            PracticeNote,
            AudioSlowMediaId,
            AudioNormalMediaId,
            MouthImageMediaId,
            IsActive);
    }
}
