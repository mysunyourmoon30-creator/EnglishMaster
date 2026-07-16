using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.Quizzes.Dtos;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Queries;

public sealed class GetQuizByIdQueryHandler
{
    private readonly IQuizRepository quizRepository;
    private readonly ICategoryRepository categoryRepository;
    private readonly ILessonRepository lessonRepository;
    private readonly ICourseRepository courseRepository;
    private readonly IBookRepository bookRepository;

    public GetQuizByIdQueryHandler(IQuizRepository quizRepository, ICategoryRepository categoryRepository, ILessonRepository lessonRepository, ICourseRepository courseRepository, IBookRepository bookRepository)
    {
        this.quizRepository = quizRepository;
        this.categoryRepository = categoryRepository;
        this.lessonRepository = lessonRepository;
        this.courseRepository = courseRepository;
        this.bookRepository = bookRepository;
    }

    public async Task<Result<QuizDto>> HandleAsync(GetQuizByIdQuery query, CancellationToken cancellationToken)
    {
        if (query.Id == Guid.Empty)
        {
            return Result<QuizDto>.Validation(new ValidationError(nameof(query.Id), $"{nameof(query.Id)} cannot be empty."));
        }

        var quiz = await quizRepository.GetByIdAsync(query.Id, cancellationToken);
        if (quiz is null)
        {
            return Result<QuizDto>.NotFound(nameof(query.Id), "Quiz was not found.");
        }

        return Result<QuizDto>.Success(await QuizReadModelBuilder.MapAsync(quiz, categoryRepository, lessonRepository, courseRepository, bookRepository, cancellationToken));
    }
}
