using EnglishMaster.Application.Features.LearningRecommendations;
using EnglishMaster.Application.Features.LearningRecommendations.Dtos;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.LearningRecommendations;

public sealed class EfLearningRecommendationRepository : ILearningRecommendationRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfLearningRecommendationRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ContinueLearningItemDto>> GetContinueLearningAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var lessons = await dbContext.LessonProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.InProgress)
            .Join(dbContext.Lessons.AsNoTracking().Where(lesson => lesson.IsActive && lesson.IsPublished), progress => progress.LessonId, lesson => lesson.Id, (progress, lesson) => new ContinueLearningItemDto("lesson", lesson.Id, lesson.Slug, lesson.Title, lesson.Summary, $"/lessons/{lesson.Slug}", progress.ProgressPercent, progress.Status.ToString(), progress.LastAccessedAt, "Continue where you left off", 1))
            .ToArrayAsync(cancellationToken);
        var courses = await dbContext.CourseProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.InProgress)
            .Join(dbContext.Courses.AsNoTracking().Where(course => course.IsActive && course.IsPublished), progress => progress.CourseId, course => course.Id, (progress, course) => new ContinueLearningItemDto("course", course.Id, course.Slug, course.Title, course.Summary, $"/courses/{course.Slug}", progress.ProgressPercent, progress.Status.ToString(), progress.LastAccessedAt, "Continue where you left off", 2))
            .ToArrayAsync(cancellationToken);
        var books = await dbContext.BookProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.InProgress)
            .Join(dbContext.Books.AsNoTracking().Where(book => book.IsActive && book.IsPublished), progress => progress.BookId, book => book.Id, (progress, book) => new ContinueLearningItemDto("book", book.Id, book.Slug, book.Title, book.Summary, $"/books/{book.Slug}", progress.ProgressPercent, progress.Status.ToString(), progress.LastAccessedAt, "Continue where you left off", 3))
            .ToArrayAsync(cancellationToken);
        return lessons.Concat(courses).Concat(books).OrderByDescending(item => item.LastAccessedAt).Take(limit).ToArray();
    }

    public async Task<LearningRecommendationSummaryDto> GetSummaryAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken) =>
        new(
            await GetContinueLearningAsync(userId, limit, cancellationToken),
            await GetRecommendedCoursesAsync(userId, limit, cefrLevel, cancellationToken),
            await GetRecommendedLessonsAsync(userId, limit, cefrLevel, cancellationToken),
            await GetRecommendedWordsAsync(userId, limit, cefrLevel, cancellationToken),
            await GetRecommendedGrammarAsync(userId, limit, cefrLevel, cancellationToken),
            await GetRecommendedQuizzesAsync(userId, limit, cefrLevel, cancellationToken),
            await GetReviewRecommendationsAsync(userId, limit, cancellationToken));

    public async Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedCoursesAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken)
    {
        var cefr = await ResolveCefrAsync(userId, cefrLevel, cancellationToken);
        var completed = await dbContext.CourseProgress.AsNoTracking().Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed).Select(progress => progress.CourseId).ToArrayAsync(cancellationToken);
        var categories = await CategoryNamesAsync(cancellationToken);
        var courses = await dbContext.Courses.AsNoTracking()
            .Where(course => course.IsActive && course.IsPublished && !completed.Contains(course.Id))
            .Where(course => cefr == null || course.CefrLevel == cefr)
            .OrderByDescending(course => course.UpdatedAt)
            .ThenBy(course => course.Title)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return courses.Select((course, index) => Recommendation("course", course.Id, course.Slug, course.Title, course.Summary, $"/courses/{course.Slug}", course.CefrLevel?.ToString(), CategoryName(categories, course.CategoryId), "RecommendedCourse", cefr is null ? "NewContentIfAvailable" : "SameCefrLevel", cefr is null ? "New content you may like" : "Matches your CEFR level", 80 - index, index + 1)).ToArray();
    }

    public async Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedLessonsAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken)
    {
        var cefr = await ResolveCefrAsync(userId, cefrLevel, cancellationToken);
        var completed = await dbContext.LessonProgress.AsNoTracking().Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed).Select(progress => progress.LessonId).ToArrayAsync(cancellationToken);
        var categories = await CategoryNamesAsync(cancellationToken);
        var lessons = await dbContext.Lessons.AsNoTracking()
            .Where(lesson => lesson.IsActive && lesson.IsPublished && !completed.Contains(lesson.Id))
            .Where(lesson => cefr == null || lesson.CefrLevel == cefr)
            .OrderByDescending(lesson => lesson.UpdatedAt)
            .ThenBy(lesson => lesson.Title)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return lessons.Select((lesson, index) => Recommendation("lesson", lesson.Id, lesson.Slug, lesson.Title, lesson.Summary, $"/lessons/{lesson.Slug}", lesson.CefrLevel?.ToString(), CategoryName(categories, lesson.CategoryId), "RecommendedLesson", cefr is null ? "NotStarted" : "SameCefrLevel", cefr is null ? "Not started yet" : "Matches your CEFR level", 75 - index, index + 1)).ToArray();
    }

    public async Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedWordsAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken)
    {
        var cefr = await ResolveCefrAsync(userId, cefrLevel, cancellationToken);
        var categories = await CategoryNamesAsync(cancellationToken);
        var words = await dbContext.Words.AsNoTracking()
            .Where(word => word.IsActive)
            .Where(word => cefr == null || word.CefrLevel == cefr)
            .OrderByDescending(word => word.UpdatedAt)
            .ThenBy(word => word.Text)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return words.Select((word, index) => Recommendation("word", word.Id, word.Slug, word.Text, word.MeaningTh, $"/dictionary/{word.Slug}", word.CefrLevel.ToString(), CategoryName(categories, word.CategoryId), "RecommendedWords", cefr is null ? "NewContentIfAvailable" : "SameCefrLevel", cefr is null ? "New vocabulary to learn" : "Matches your CEFR level", 70 - index, index + 1)).ToArray();
    }

    public async Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedGrammarAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken)
    {
        var cefr = await ResolveCefrAsync(userId, cefrLevel, cancellationToken);
        var topics = await dbContext.GrammarTopics.AsNoTracking()
            .Where(topic => topic.IsActive)
            .Where(topic => cefr == null || topic.CefrLevel == cefr)
            .OrderByDescending(topic => topic.UpdatedAt)
            .ThenBy(topic => topic.Title)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return topics.Select((topic, index) => Recommendation("grammar", topic.Id, topic.Slug, topic.Title, topic.Summary, $"/grammar/{topic.Slug}", topic.CefrLevel.ToString(), null, "RecommendedGrammar", cefr is null ? "NewContentIfAvailable" : "SameCefrLevel", cefr is null ? "Grammar topic to review" : "Matches your CEFR level", 65 - index, index + 1)).ToArray();
    }

    public async Task<IReadOnlyCollection<LearningRecommendationDto>> GetRecommendedQuizzesAsync(Guid userId, int limit, string? cefrLevel, CancellationToken cancellationToken)
    {
        var cefr = await ResolveCefrAsync(userId, cefrLevel, cancellationToken);
        var attempted = await dbContext.QuizAttempts.AsNoTracking().Where(attempt => attempt.UserId == userId).Select(attempt => attempt.QuizId).Distinct().ToArrayAsync(cancellationToken);
        var categories = await CategoryNamesAsync(cancellationToken);
        var quizzes = await dbContext.Quizzes.AsNoTracking()
            .Where(quiz => quiz.IsActive && quiz.IsPublished && !attempted.Contains(quiz.Id))
            .Where(quiz => cefr == null || quiz.CefrLevel == cefr)
            .OrderByDescending(quiz => quiz.UpdatedAt)
            .ThenBy(quiz => quiz.Title)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return quizzes.Select((quiz, index) => Recommendation("quiz", quiz.Id, quiz.Slug, quiz.Title, quiz.Summary, $"/quizzes/{quiz.Slug}", quiz.CefrLevel?.ToString(), CategoryName(categories, quiz.CategoryId), "RecommendedQuiz", "NotStarted", "Quiz you have not tried yet", 60 - index, index + 1)).ToArray();
    }

    public async Task<IReadOnlyCollection<LearningRecommendationDto>> GetReviewRecommendationsAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var categories = await CategoryNamesAsync(cancellationToken);
        var lowAttempts = await dbContext.QuizAttempts.AsNoTracking()
            .Where(attempt => attempt.UserId == userId && !attempt.Passed)
            .OrderByDescending(attempt => attempt.AttemptedAt)
            .Take(limit)
            .Join(dbContext.Quizzes.AsNoTracking().Where(quiz => quiz.IsActive && quiz.IsPublished), attempt => attempt.QuizId, quiz => quiz.Id, (attempt, quiz) => new { attempt, quiz })
            .ToArrayAsync(cancellationToken);
        return lowAttempts.Select((item, index) => Recommendation("quiz", item.quiz.Id, item.quiz.Slug, item.quiz.Title, item.quiz.Summary, $"/quizzes/{item.quiz.Slug}", item.quiz.CefrLevel?.ToString(), CategoryName(categories, item.quiz.CategoryId), "ReviewWeakQuiz", "LowQuizScore", "Recommended after low quiz score", 90 - index, index + 1)).ToArray();
    }

    public async Task<LearningRecommendationDto?> GetNextLessonForCourseAsync(Guid userId, Guid courseId, CancellationToken cancellationToken)
    {
        var completed = await dbContext.LessonProgress.AsNoTracking().Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed).Select(progress => progress.LessonId).ToArrayAsync(cancellationToken);
        var next = await dbContext.CourseLessons.AsNoTracking()
            .Where(courseLesson => courseLesson.CourseId == courseId)
            .OrderBy(courseLesson => courseLesson.SortOrder)
            .Join(dbContext.Lessons.AsNoTracking().Where(lesson => lesson.IsActive && lesson.IsPublished), courseLesson => courseLesson.LessonId, lesson => lesson.Id, (courseLesson, lesson) => new { courseLesson, lesson })
            .FirstOrDefaultAsync(item => !completed.Contains(item.lesson.Id), cancellationToken);
        return next is null ? null : Recommendation("lesson", next.lesson.Id, next.lesson.Slug, next.lesson.Title, next.lesson.Summary, $"/lessons/{next.lesson.Slug}", next.lesson.CefrLevel?.ToString(), null, "NextLessonInCourse", "NextInCourse", "Next lesson in your course", 100, next.courseLesson.SortOrder);
    }

    private async Task<CefrLevel?> ResolveCefrAsync(Guid userId, string? cefrLevel, CancellationToken cancellationToken)
    {
        if (Enum.TryParse<CefrLevel>(cefrLevel, ignoreCase: true, out var parsed))
        {
            return parsed;
        }

        return await dbContext.StudentProfiles.AsNoTracking().Where(profile => profile.UserId == userId).Select(profile => profile.CurrentCefrLevel).FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<Dictionary<Guid, string>> CategoryNamesAsync(CancellationToken cancellationToken) =>
        await dbContext.Categories.AsNoTracking().ToDictionaryAsync(category => category.Id, category => category.Name, cancellationToken);

    private static string? CategoryName(IReadOnlyDictionary<Guid, string> categories, Guid? categoryId) =>
        categoryId.HasValue && categories.TryGetValue(categoryId.Value, out var name) ? name : null;

    private static LearningRecommendationDto Recommendation(string contentType, Guid id, string slug, string title, string summary, string url, string? cefrLevel, string? categoryName, string type, string reason, string reasonText, decimal score, int sortOrder) =>
        new(contentType, id, slug, title, summary, url, cefrLevel, categoryName, [], type, reason, reasonText, score, sortOrder);
}

