using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Lessons;

namespace EnglishMaster.Web.Services.Lessons;

public sealed class LessonFormModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Summary { get; set; }

    [StringLength(4000)]
    public string? Description { get; set; }

    public string? CefrLevel { get; set; }

    public Guid? CategoryId { get; set; }

    public Guid? ThumbnailMediaId { get; set; }

    [Range(0, int.MaxValue)]
    public int EstimatedMinutes { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsPublished { get; set; }

    public bool IsActive { get; set; } = true;

    public static LessonFormModel FromDto(LessonDto lesson)
    {
        return new LessonFormModel
        {
            Title = lesson.Title,
            Summary = lesson.Summary,
            Description = lesson.Description,
            CefrLevel = lesson.CefrLevel,
            CategoryId = lesson.CategoryId,
            ThumbnailMediaId = lesson.ThumbnailMediaId,
            EstimatedMinutes = lesson.EstimatedMinutes,
            SortOrder = lesson.SortOrder,
            IsPublished = lesson.IsPublished,
            IsActive = lesson.IsActive
        };
    }

    public CreateLessonRequest ToCreateRequest()
    {
        return new CreateLessonRequest(
            Title,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            ThumbnailMediaId,
            EstimatedMinutes,
            SortOrder);
    }

    public UpdateLessonRequest ToUpdateRequest()
    {
        return new UpdateLessonRequest(
            Title,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            ThumbnailMediaId,
            EstimatedMinutes,
            SortOrder,
            IsPublished,
            IsActive);
    }
}
