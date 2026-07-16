using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Shared.Results;

namespace EnglishMaster.Application.Features.Quizzes.Dtos;

internal static class QuizReferenceValidator
{
    public static async Task<IReadOnlyCollection<ValidationError>> ValidateReferencesAsync(
        ICategoryRepository categoryRepository,
        ILessonRepository lessonRepository,
        ICourseRepository courseRepository,
        IBookRepository bookRepository,
        QuizInput input,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();

        if (input.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(input.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                errors.Add(new ValidationError(nameof(input.CategoryId), "Category was not found."));
            }
            else if (!category.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.CategoryId), "Category is inactive."));
            }
        }

        if (input.LessonId.HasValue)
        {
            var lesson = await lessonRepository.GetByIdAsync(input.LessonId.Value, cancellationToken);
            if (lesson is null)
            {
                errors.Add(new ValidationError(nameof(input.LessonId), "Lesson was not found."));
            }
            else if (!lesson.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.LessonId), "Lesson is inactive."));
            }
        }

        if (input.CourseId.HasValue)
        {
            var course = await courseRepository.GetByIdAsync(input.CourseId.Value, cancellationToken);
            if (course is null)
            {
                errors.Add(new ValidationError(nameof(input.CourseId), "Course was not found."));
            }
            else if (!course.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.CourseId), "Course is inactive."));
            }
        }

        if (input.BookId.HasValue)
        {
            var book = await bookRepository.GetByIdAsync(input.BookId.Value, cancellationToken);
            if (book is null)
            {
                errors.Add(new ValidationError(nameof(input.BookId), "Book was not found."));
            }
            else if (!book.IsActive)
            {
                errors.Add(new ValidationError(nameof(input.BookId), "Book is inactive."));
            }
        }

        return errors;
    }
}
