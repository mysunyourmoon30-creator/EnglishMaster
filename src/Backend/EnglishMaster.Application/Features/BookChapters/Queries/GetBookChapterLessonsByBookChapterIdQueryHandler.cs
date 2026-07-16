using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Queries;

public sealed class GetBookChapterLessonsByBookChapterIdQueryHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly ILessonRepository lessonRepository;

    public GetBookChapterLessonsByBookChapterIdQueryHandler(
        IBookChapterRepository chapterRepository,
        ILessonRepository lessonRepository)
    {
        this.chapterRepository = chapterRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<IReadOnlyCollection<BookChapterLessonDto>>> HandleAsync(
        GetBookChapterLessonsByBookChapterIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.BookChapterId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<BookChapterLessonDto>>.Validation(
                new ValidationError(nameof(query.BookChapterId), $"{nameof(query.BookChapterId)} cannot be empty."));
        }

        var chapter = await chapterRepository.GetByIdAsync(query.BookChapterId, cancellationToken);
        if (chapter is null)
        {
            return Result<IReadOnlyCollection<BookChapterLessonDto>>.NotFound(
                nameof(query.BookChapterId),
                "Book chapter was not found.");
        }

        return Result<IReadOnlyCollection<BookChapterLessonDto>>.Success(
            await BookChapterReadModelBuilder.MapLessonsAsync(
                chapter.Lessons,
                lessonRepository,
                cancellationToken));
    }
}
