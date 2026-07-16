using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.Quizzes.Dtos;

internal static class QuizReadModelBuilder
{
    public static async Task<QuizDto> MapAsync(
        Quiz quiz,
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IBookRepository bookRepository,
        CancellationToken cancellationToken)
    {
        var category = quiz.CategoryId.HasValue
            ? await categoryRepository.GetByIdAsync(quiz.CategoryId.Value, cancellationToken)
            : null;
        var lesson = quiz.LessonId.HasValue
            ? await lessonRepository.GetByIdAsync(quiz.LessonId.Value, cancellationToken)
            : null;
        var course = quiz.CourseId.HasValue
            ? await courseRepository.GetByIdAsync(quiz.CourseId.Value, cancellationToken)
            : null;
        var book = quiz.BookId.HasValue
            ? await bookRepository.GetByIdAsync(quiz.BookId.Value, cancellationToken)
            : null;

        return QuizMapper.ToDto(quiz, category, lesson, course, book);
    }

    public static async Task<IReadOnlyCollection<QuizDto>> MapAsync(
        IReadOnlyCollection<Quiz> quizzes,
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IBookRepository bookRepository,
        CancellationToken cancellationToken)
    {
        var categoryIds = quizzes.Where(quiz => quiz.CategoryId.HasValue).Select(quiz => quiz.CategoryId!.Value).Distinct().ToArray();
        var categories = categoryIds.Length == 0
            ? []
            : await categoryRepository.GetByIdsAsync(categoryIds, cancellationToken);
        var categoryById = categories.ToDictionary(category => category.Id);

        var lessonById = new Dictionary<Guid, Lesson>();
        foreach (var lessonId in quizzes.Where(quiz => quiz.LessonId.HasValue).Select(quiz => quiz.LessonId!.Value).Distinct())
        {
            var lesson = await lessonRepository.GetByIdAsync(lessonId, cancellationToken);
            if (lesson is not null)
            {
                lessonById[lesson.Id] = lesson;
            }
        }

        var courseById = new Dictionary<Guid, Course>();
        foreach (var courseId in quizzes.Where(quiz => quiz.CourseId.HasValue).Select(quiz => quiz.CourseId!.Value).Distinct())
        {
            var course = await courseRepository.GetByIdAsync(courseId, cancellationToken);
            if (course is not null)
            {
                courseById[course.Id] = course;
            }
        }

        var bookById = new Dictionary<Guid, Book>();
        foreach (var bookId in quizzes.Where(quiz => quiz.BookId.HasValue).Select(quiz => quiz.BookId!.Value).Distinct())
        {
            var book = await bookRepository.GetByIdAsync(bookId, cancellationToken);
            if (book is not null)
            {
                bookById[book.Id] = book;
            }
        }

        return quizzes.Select(quiz =>
        {
            Category? category = null;
            if (quiz.CategoryId.HasValue)
            {
                categoryById.TryGetValue(quiz.CategoryId.Value, out category);
            }

            Lesson? lesson = null;
            if (quiz.LessonId.HasValue)
            {
                lessonById.TryGetValue(quiz.LessonId.Value, out lesson);
            }

            Course? course = null;
            if (quiz.CourseId.HasValue)
            {
                courseById.TryGetValue(quiz.CourseId.Value, out course);
            }

            Book? book = null;
            if (quiz.BookId.HasValue)
            {
                bookById.TryGetValue(quiz.BookId.Value, out book);
            }

            return QuizMapper.ToDto(quiz, category, lesson, course, book);
        }).ToArray();
    }
}
