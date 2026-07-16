using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Books.Commands;

public sealed class ActivateBookCommandHandler
{
    private readonly IBookRepository bookRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ICourseRepository courseRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public ActivateBookCommandHandler(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository,
        TimeProvider timeProvider)
    {
        this.bookRepository = bookRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.courseRepository = courseRepository;
        this.lessonRepository = lessonRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<BookDto>> HandleAsync(
        ActivateBookCommand command,
        CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(command.Id, cancellationToken);
        if (book is null)
        {
            return Result<BookDto>.NotFound(nameof(command.Id), "Book was not found.");
        }

        book.Activate(timeProvider.GetUtcNow());
        await bookRepository.SaveChangesAsync(cancellationToken);

        return Result<BookDto>.Success(await BookReadModelBuilder.MapAsync(
            book,
            categoryRepository,
            mediaRepository,
            courseRepository,
            lessonRepository,
            cancellationToken));
    }
}
