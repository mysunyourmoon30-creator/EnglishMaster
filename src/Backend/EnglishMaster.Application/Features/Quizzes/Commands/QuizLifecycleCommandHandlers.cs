using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Commands;

public abstract class QuizLifecycleCommandHandler<TCommand>
{
    private readonly IQuizRepository quizRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly ICourseRepository courseRepository;
    private readonly IBookRepository bookRepository;
    private readonly TimeProvider timeProvider;

    protected QuizLifecycleCommandHandler(
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

    protected async Task<Result<QuizDto>> HandleLifecycleAsync(
        Guid id,
        Action<Quiz, DateTimeOffset> apply,
        CancellationToken cancellationToken)
    {
        if (id == Guid.Empty)
        {
            return Result<QuizDto>.Validation(new ValidationError(nameof(id), $"{nameof(id)} cannot be empty."));
        }

        var quiz = await quizRepository.GetByIdAsync(id, cancellationToken);
        if (quiz is null)
        {
            return Result<QuizDto>.NotFound(nameof(id), "Quiz was not found.");
        }

        apply(quiz, timeProvider.GetUtcNow());
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

public sealed class PublishQuizCommandHandler : QuizLifecycleCommandHandler<PublishQuizCommand>
{
    public PublishQuizCommandHandler(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IBookRepository bookRepository, TimeProvider timeProvider)
        : base(quizRepository, categoryRepository, lessonRepository, courseRepository, bookRepository, timeProvider)
    {
    }

    public Task<Result<QuizDto>> HandleAsync(PublishQuizCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (quiz, now) => quiz.Publish(now), cancellationToken);
    }
}

public sealed class UnpublishQuizCommandHandler : QuizLifecycleCommandHandler<UnpublishQuizCommand>
{
    public UnpublishQuizCommandHandler(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IBookRepository bookRepository, TimeProvider timeProvider)
        : base(quizRepository, categoryRepository, lessonRepository, courseRepository, bookRepository, timeProvider)
    {
    }

    public Task<Result<QuizDto>> HandleAsync(UnpublishQuizCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (quiz, now) => quiz.Unpublish(now), cancellationToken);
    }
}

public sealed class ActivateQuizCommandHandler : QuizLifecycleCommandHandler<ActivateQuizCommand>
{
    public ActivateQuizCommandHandler(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IBookRepository bookRepository, TimeProvider timeProvider)
        : base(quizRepository, categoryRepository, lessonRepository, courseRepository, bookRepository, timeProvider)
    {
    }

    public Task<Result<QuizDto>> HandleAsync(ActivateQuizCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (quiz, now) => quiz.Activate(now), cancellationToken);
    }
}

public sealed class DeactivateQuizCommandHandler : QuizLifecycleCommandHandler<DeactivateQuizCommand>
{
    public DeactivateQuizCommandHandler(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IBookRepository bookRepository, TimeProvider timeProvider)
        : base(quizRepository, categoryRepository, lessonRepository, courseRepository, bookRepository, timeProvider)
    {
    }

    public Task<Result<QuizDto>> HandleAsync(DeactivateQuizCommand command, CancellationToken cancellationToken)
    {
        return HandleLifecycleAsync(command.Id, static (quiz, now) => quiz.Deactivate(now), cancellationToken);
    }
}
