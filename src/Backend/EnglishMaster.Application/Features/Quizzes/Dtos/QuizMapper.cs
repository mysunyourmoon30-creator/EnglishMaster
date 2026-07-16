using EnglishMaster.Application.Features.QuizQuestions.Dtos;
using EnglishMaster.Contracts.Quizzes;
using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Quizzes;

namespace EnglishMaster.Application.Features.Quizzes.Dtos;

internal static class QuizMapper
{
    public static QuizDto ToDto(
        Quiz quiz,
        Category? category = null,
        Lesson? lesson = null,
        Course? course = null,
        Book? book = null)
    {
        return new QuizDto(
            quiz.Id,
            quiz.Title,
            quiz.Slug,
            quiz.Summary,
            quiz.Description,
            quiz.CefrLevel?.ToString(),
            quiz.CategoryId,
            category is null ? null : new QuizCategoryDto(category.Id, category.Name, category.Slug),
            quiz.LessonId,
            lesson is null ? null : new QuizLessonDto(lesson.Id, lesson.Title, lesson.Slug),
            quiz.CourseId,
            course is null ? null : new QuizCourseDto(course.Id, course.Title, course.Slug),
            quiz.BookId,
            book is null ? null : new QuizBookDto(book.Id, book.Title, book.Slug),
            quiz.TimeLimitMinutes,
            quiz.PassingScore,
            quiz.SortOrder,
            QuizQuestionMapper.ToDtos(quiz.Questions),
            quiz.IsPublished,
            quiz.IsActive,
            quiz.CreatedAt,
            quiz.UpdatedAt);
    }
}
