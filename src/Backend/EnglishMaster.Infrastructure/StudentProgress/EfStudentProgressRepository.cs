using EnglishMaster.Application.Features.StudentProgress;
using EnglishMaster.Application.Features.StudentProgress.Dtos;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.StudentProgress;

public sealed class EfStudentProgressRepository : IStudentProgressRepository
{
    private readonly EnglishMasterDbContext dbContext;

    public EfStudentProgressRepository(EnglishMasterDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<StudentProgressSummaryDto> GetSummaryAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken)
    {
        var lessonStatuses = dbContext.LessonProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId)
            .Join(
                dbContext.Lessons.AsNoTracking().Where(lesson => lesson.IsActive && lesson.IsPublished),
                progress => progress.LessonId,
                lesson => lesson.Id,
                (progress, _) => progress.Status);

        var courseStatuses = dbContext.CourseProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId)
            .Join(
                dbContext.Courses.AsNoTracking().Where(course => course.IsActive && course.IsPublished),
                progress => progress.CourseId,
                course => course.Id,
                (progress, _) => progress.Status);

        var bookStatuses = dbContext.BookProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId)
            .Join(
                dbContext.Books.AsNoTracking().Where(book => book.IsActive && book.IsPublished),
                progress => progress.BookId,
                book => book.Id,
                (progress, _) => progress.Status);

        var counts = await lessonStatuses
            .Concat(courseStatuses)
            .Concat(bookStatuses)
            .GroupBy(_ => 1)
            .Select(group => new
            {
                Total = group.Count(),
                InProgress = group.Count(status => status == LearningProgressStatus.InProgress),
                Completed = group.Count(status => status == LearningProgressStatus.Completed)
            })
            .SingleOrDefaultAsync(cancellationToken);

        var lessons = await dbContext.LessonProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId)
            .Join(
                dbContext.Lessons.AsNoTracking().Where(lesson => lesson.IsActive && lesson.IsPublished),
                progress => progress.LessonId,
                lesson => lesson.Id,
                (progress, lesson) => new { Progress = progress, Content = lesson })
            .OrderByDescending(item => item.Progress.LastAccessedAt)
            .ThenBy(item => item.Content.Title)
            .ThenBy(item => item.Progress.Id)
            .Take(limit)
            .Select(item => new StudentProgressItemDto(
                "lesson",
                item.Content.Id,
                item.Content.Slug,
                item.Content.Title,
                item.Content.Summary,
                $"/lessons/{item.Content.Slug}",
                item.Progress.ProgressPercent,
                item.Progress.Status.ToString(),
                item.Progress.LastAccessedAt))
            .ToArrayAsync(cancellationToken);

        var courses = await dbContext.CourseProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId)
            .Join(
                dbContext.Courses.AsNoTracking().Where(course => course.IsActive && course.IsPublished),
                progress => progress.CourseId,
                course => course.Id,
                (progress, course) => new { Progress = progress, Content = course })
            .OrderByDescending(item => item.Progress.LastAccessedAt)
            .ThenBy(item => item.Content.Title)
            .ThenBy(item => item.Progress.Id)
            .Take(limit)
            .Select(item => new StudentProgressItemDto(
                "course",
                item.Content.Id,
                item.Content.Slug,
                item.Content.Title,
                item.Content.Summary,
                $"/courses/{item.Content.Slug}",
                item.Progress.ProgressPercent,
                item.Progress.Status.ToString(),
                item.Progress.LastAccessedAt))
            .ToArrayAsync(cancellationToken);

        var books = await dbContext.BookProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId)
            .Join(
                dbContext.Books.AsNoTracking().Where(book => book.IsActive && book.IsPublished),
                progress => progress.BookId,
                book => book.Id,
                (progress, book) => new { Progress = progress, Content = book })
            .OrderByDescending(item => item.Progress.LastAccessedAt)
            .ThenBy(item => item.Content.Title)
            .ThenBy(item => item.Progress.Id)
            .Take(limit)
            .Select(item => new StudentProgressItemDto(
                "book",
                item.Content.Id,
                item.Content.Slug,
                item.Content.Title,
                item.Content.Summary,
                $"/books/{item.Content.Slug}",
                item.Progress.ProgressPercent,
                item.Progress.Status.ToString(),
                item.Progress.LastAccessedAt))
            .ToArrayAsync(cancellationToken);

        return new StudentProgressSummaryDto(
            counts?.Total ?? 0,
            counts?.InProgress ?? 0,
            counts?.Completed ?? 0,
            lessons,
            courses,
            books);
    }
}
