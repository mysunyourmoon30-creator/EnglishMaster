using System.ComponentModel.DataAnnotations;
using EnglishMaster.Contracts.LessonSections;

namespace EnglishMaster.Web.Services.Lessons;

public sealed class LessonSectionFormModel
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(8000)]
    public string? ContentMarkdown { get; set; }

    [Required]
    public string SectionType { get; set; } = "Text";

    public Guid? MediaId { get; set; }

    [Range(0, int.MaxValue)]
    public int SortOrder { get; set; }

    public bool IsActive { get; set; } = true;

    public static LessonSectionFormModel FromDto(LessonSectionDto section)
    {
        return new LessonSectionFormModel
        {
            Title = section.Title,
            ContentMarkdown = section.ContentMarkdown,
            SectionType = section.SectionType,
            MediaId = section.MediaId,
            SortOrder = section.SortOrder,
            IsActive = section.IsActive
        };
    }

    public CreateLessonSectionRequest ToCreateRequest()
    {
        return new CreateLessonSectionRequest(Title, ContentMarkdown, SectionType, MediaId, SortOrder);
    }

    public UpdateLessonSectionRequest ToUpdateRequest()
    {
        return new UpdateLessonSectionRequest(Title, ContentMarkdown, SectionType, MediaId, SortOrder, IsActive);
    }
}
