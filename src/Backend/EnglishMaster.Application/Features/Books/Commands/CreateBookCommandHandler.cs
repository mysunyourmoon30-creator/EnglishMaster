using EnglishMaster.Application.Features.Books.Dtos;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Contracts.Books;
using EnglishMaster.Domain.Books;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Books.Commands;

public sealed class CreateBookCommandHandler
{
    private readonly IBookRepository bookRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly IMediaRepository mediaRepository;
    private readonly ICourseRepository courseRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly TimeProvider timeProvider;

    public CreateBookCommandHandler(
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
        CreateBookCommand command,
        CancellationToken cancellationToken)
    {
        var validation = BookInputValidator.Validate(
            command.Title,
            command.Subtitle,
            command.Summary,
            command.Description,
            command.CefrLevel,
            command.CategoryId,
            command.CoverMediaId,
            command.CourseId,
            command.AuthorName,
            command.Edition,
            command.Version,
            command.EstimatedPages,
            command.SortOrder,
            isPublished: false,
            isActive: true);

        if (!validation.IsSuccess)
        {
            return Result<BookDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceValidation = await BookReferenceValidator.ValidateReferencesAsync(
            categoryRepository,
            mediaRepository,
            courseRepository,
            input,
            cancellationToken);
        if (referenceValidation.Errors.Count > 0)
        {
            return Result<BookDto>.Validation([.. referenceValidation.Errors]);
        }

        if (await bookRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<BookDto>.Validation(
                new ValidationError(nameof(command.Title), "A book with this title already exists."));
        }

        var book = Book.Create(
            input.Title,
            input.Subtitle,
            input.Summary,
            input.Description,
            input.CefrLevel,
            input.CategoryId,
            input.CoverMediaId,
            input.CourseId,
            input.AuthorName,
            input.Edition,
            input.Version,
            input.EstimatedPages,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await bookRepository.AddAsync(book, cancellationToken);
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
