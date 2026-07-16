using EnglishMaster.Application.Features.BookChapters.Dtos;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.BookChapters;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.BookChapters.Queries;

public sealed class GetBookChapterByIdQueryHandler
{
    private readonly IBookChapterRepository chapterRepository;
    private readonly ILessonRepository lessonRepository;

    public GetBookChapterByIdQueryHandler(
        IBookChapterRepository chapterRepository,
        ILessonRepository lessonRepository)
    {
        this.chapterRepository = chapterRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<BookChapterDto>> HandleAsync(
        GetBookChapterByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            return Result<BookChapterDto>.Validation(
                new ValidationError(nameof(query.Id), $"{nameof(query.Id)} cannot be empty."));
        }

        var chapter = await chapterRepository.GetByIdAsync(query.Id, cancellationToken);
        if (chapter is null)
        {
            return Result<BookChapterDto>.NotFound(nameof(query.Id), "Book chapter was not found.");
        }

        return Result<BookChapterDto>.Success(await BookChapterReadModelBuilder.MapAsync(
            chapter,
            lessonRepository,
            cancellationToken));
    }
}
