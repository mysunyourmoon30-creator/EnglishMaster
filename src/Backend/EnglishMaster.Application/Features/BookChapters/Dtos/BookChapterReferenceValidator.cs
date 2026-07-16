using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Dtos;

internal static class BookChapterReferenceValidator
{
    public static async Task<IReadOnlyCollection<ValidationError>> ValidateLessonAsync(
        ILessonRepository lessonRepository,
        Guid lessonId,
        CancellationToken cancellationToken)
    {
        var lesson = await lessonRepository.GetByIdAsync(lessonId, cancellationToken);
        if (lesson is null || !lesson.IsActive)
        {
            return [new ValidationError(nameof(lessonId), "Lesson was not found or is inactive.")];
        }

        return [];
    }
}
