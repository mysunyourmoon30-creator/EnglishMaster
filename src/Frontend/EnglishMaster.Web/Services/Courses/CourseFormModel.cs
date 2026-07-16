using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.Courses;

namespace EnglishMaster.Web.Services.Courses;

public sealed class CourseFormModel
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

    public static CourseFormModel FromDto(CourseDto course)
    {
        return new CourseFormModel
        {
            Title = course.Title,
            Summary = course.Summary,
            Description = course.Description,
            CefrLevel = course.CefrLevel,
            CategoryId = course.CategoryId,
            ThumbnailMediaId = course.ThumbnailMediaId,
            EstimatedMinutes = course.EstimatedMinutes,
            SortOrder = course.SortOrder,
            IsPublished = course.IsPublished,
            IsActive = course.IsActive
        };
    }

    public CreateCourseRequest ToCreateRequest()
    {
        return new CreateCourseRequest(
            Title,
            Summary,
            Description,
            CefrLevel,
            CategoryId,
            ThumbnailMediaId,
            EstimatedMinutes,
            SortOrder);
    }

    public UpdateCourseRequest ToUpdateRequest()
    {
        return new UpdateCourseRequest(
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
