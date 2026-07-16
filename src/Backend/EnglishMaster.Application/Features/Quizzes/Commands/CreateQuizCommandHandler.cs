using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Commands;

public sealed class CreateQuizCommandHandler
{
    private readonly IQuizRepository quizRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly ICourseRepository courseRepository;
    private readonly IBookRepository bookRepository;
    private readonly TimeProvider timeProvider;

    public CreateQuizCommandHandler(
        IQuizRepository quizRepository,
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IBookRepository bookRepository,
        TimeProvider timeProvider)
    {
        this.quizRepository = quizRepository;
        this.categoryRepository = categoryRepository;
        this.lessonRepository = lessonRepository;
        this.courseRepository = courseRepository;
        this.bookRepository = bookRepository;
        this.timeProvider = timeProvider;
    }

    public async Task<Result<QuizDto>> HandleAsync(CreateQuizCommand command, CancellationToken cancellationToken)
    {
        var validation = QuizInputValidator.Validate(
            command.Title,
            command.Summary,
            command.Description,
            command.CefrLevel,
            command.CategoryId,
            command.LessonId,
            command.CourseId,
            command.BookId,
            command.TimeLimitMinutes,
            command.PassingScore,
            command.SortOrder,
            isPublished: false,
            isActive: true);
        if (!validation.IsSuccess)
        {
            return Result<QuizDto>.Validation([.. validation.Errors]);
        }

        var input = validation.Value!;
        var referenceErrors = await QuizReferenceValidator.ValidateReferencesAsync(
            categoryRepository,
            lessonRepository,
            courseRepository,
            bookRepository,
            input,
            cancellationToken);
        if (referenceErrors.Count > 0)
        {
            return Result<QuizDto>.Validation([.. referenceErrors]);
        }

        if (await quizRepository.SlugExistsAsync(input.Slug, null, cancellationToken))
        {
            return Result<QuizDto>.Validation(new ValidationError(nameof(command.Title), "A quiz with this title already exists."));
        }

        var quiz = Quiz.Create(
            input.Title,
            input.Summary,
            input.Description,
            input.CefrLevel,
            input.CategoryId,
            input.LessonId,
            input.CourseId,
            input.BookId,
            input.TimeLimitMinutes,
            input.PassingScore,
            input.SortOrder,
            timeProvider.GetUtcNow());

        await quizRepository.AddAsync(quiz, cancellationToken);
        await quizRepository.SaveChangesAsync(cancellationToken);

        return Result<QuizDto>.Success(await QuizReadModelBuilder.MapAsync(
            quiz,
            categoryRepository,
            lessonRepository,
            courseRepository,
            bookRepository,
            cancellationToken));
    }
}
