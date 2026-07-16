using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Books.Queries;

public sealed class GetBookByIdQueryHandler
{
    private readonly IBookRepository bookRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ICourseRepository courseRepository;
    private readonly ILessonRepository lessonRepository;

    public GetBookByIdQueryHandler(
        IBookRepository bookRepository,
        ICategoryRepository categoryRepository,
        IMediaRepository mediaRepository,
        ICourseRepository courseRepository,
        ILessonRepository lessonRepository)
    {
        this.bookRepository = bookRepository;
        this.categoryRepository = categoryRepository;
        this.mediaRepository = mediaRepository;
        this.courseRepository = courseRepository;
        this.lessonRepository = lessonRepository;
    }

    public async Task<Result<BookDto>> HandleAsync(
        GetBookByIdQuery query,
        CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(query.Id, cancellationToken);
        if (book is null)
        {
            return Result<BookDto>.NotFound(nameof(query.Id), "Book was not found.");
        }

        return Result<BookDto>.Success(await BookReadModelBuilder.MapAsync(
            book,
            categoryRepository,
            mediaRepository,
            courseRepository,
            lessonRepository,
            cancellationToken));
    }
}
