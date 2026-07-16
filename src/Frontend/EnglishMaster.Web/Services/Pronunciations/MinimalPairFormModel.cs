using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.MinimalPairs;

namespace EnglishMaster.Web.Services.Pronunciations;

public sealed class MinimalPairFormModel
{
    [Required]
    [StringLength(200)]
    public string PairWordText { get; set; } = string.Empty;

    [StringLength(100)]
    public string? PairIpa { get; set; }

    [StringLength(200)]
    public string? PairThaiReading { get; set; }

    [StringLength(1000)]
    public string? DifferenceNote { get; set; }

    public Guid? AudioMediaId { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static MinimalPairFormModel FromDto(MinimalPairDto minimalPair)
    {
        return new MinimalPairFormModel
        {
            PairWordText = minimalPair.PairWordText,
            PairIpa = minimalPair.PairIpa,
            PairThaiReading = minimalPair.PairThaiReading,
            DifferenceNote = minimalPair.DifferenceNote,
            AudioMediaId = minimalPair.AudioMediaId,
            SortOrder = minimalPair.SortOrder,
            IsActive = minimalPair.IsActive
        };
    }

    public CreateMinimalPairRequest ToCreateRequest()
    {
        return new CreateMinimalPairRequest(
            PairWordText,
            PairIpa,
            PairThaiReading,
            DifferenceNote,
            AudioMediaId,
            SortOrder);
    }

    public UpdateMinimalPairRequest ToUpdateRequest()
    {
        return new UpdateMinimalPairRequest(
            PairWordText,
            PairIpa,
            PairThaiReading,
            DifferenceNote,
            AudioMediaId,
            SortOrder,
            IsActive);
    }
}
