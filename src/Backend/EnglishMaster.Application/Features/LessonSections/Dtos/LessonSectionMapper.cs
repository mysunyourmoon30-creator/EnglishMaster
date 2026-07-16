using EnglishMaster.Application.Features.Lessons.Dtos;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Domain.Lessons;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Application.Features.LessonSections.Dtos;

internal static class LessonSectionMapper
{
    public static LessonSectionDto ToDto(LessonSection lessonSection, MediaEntity? media = null)
    {
        return new LessonSectionDto(
            lessonSection.Id,
            lessonSection.LessonId,
            lessonSection.Title,
            lessonSection.ContentMarkdown,
            lessonSection.SectionType.ToString(),
            lessonSection.MediaId,
            media is null ? null : LessonMapper.ToMediaDto(media),
            lessonSection.SortOrder,
            lessonSection.IsActive,
            lessonSection.CreatedAt,
            lessonSection.UpdatedAt);
    }
}
