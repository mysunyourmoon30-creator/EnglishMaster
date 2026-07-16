using EnglishMaster.Application.Features.BulkOperations;
using EnglishMaster.Application.Features.ContentQuality;
using EnglishMaster.Domain.BulkOperations;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.BulkOperations;

public sealed class BulkOperationRunner : IBulkOperationRunner
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly IContentQualityService qualityService;
    private readonly TimeProvider timeProvider;

    public BulkOperationRunner(EnglishMasterDbContext dbContext, IContentQualityService qualityService, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.qualityService = qualityService;
        this.timeProvider = timeProvider;
    }

    public async Task RunAsync(BulkOperation operation, CancellationToken cancellationToken)
    {
        foreach (var item in operation.Items)
        {
            var now = timeProvider.GetUtcNow();
            try
            {
                var succeeded = await ApplyAsync(operation, item.ContentId, cancellationToken);
                if (succeeded)
                {
                    item.Succeed(now);
                }
                else
                {
                    item.Fail("Content item was not found or action is not supported for this content type.", now);
                }
            }
            catch (InvalidOperationException exception)
            {
                item.Fail(exception.Message, now);
            }
            catch (ArgumentException exception)
            {
                item.Fail(exception.Message, now);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> ApplyAsync(BulkOperation operation, Guid contentId, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        if (operation.OperationType == BulkOperationType.RunQualityCheck)
        {
            return await qualityService.RunAsync(operation.ContentType, contentId, operation.RequestedBy, cancellationToken) is not null;
        }

        if (operation.OperationType == BulkOperationType.AssignCategory)
        {
            return await AssignCategoryAsync(operation.ContentType, contentId, operation.CategoryId, now, cancellationToken);
        }

        if (operation.OperationType is BulkOperationType.AddTags or BulkOperationType.RemoveTags)
        {
            return await UpdateWordTagsAsync(operation, contentId, now, cancellationToken);
        }

        if (operation.OperationType == BulkOperationType.Export)
        {
            return await ContentExistsAsync(operation.ContentType, contentId, cancellationToken);
        }

        return await ApplyStateActionAsync(operation.OperationType, operation.ContentType, contentId, now, cancellationToken);
    }

    private async Task<bool> ApplyStateActionAsync(BulkOperationType action, string contentType, Guid id, DateTimeOffset now, CancellationToken cancellationToken)
    {
        switch (contentType)
        {
            case "word":
                var word = await dbContext.Words.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
                if (word is null)
                {
                    return false;
                }

                ApplyActiveOnly(action, word.Activate, word.Deactivate, now);
                return true;
            case "lesson":
                var lesson = await dbContext.Lessons.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
                if (lesson is null)
                {
                    return false;
                }

                ApplyPublishAndActive(action, lesson.Publish, lesson.Activate, lesson.Deactivate, now);
                return true;
            case "course":
                var course = await dbContext.Courses.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
                if (course is null)
                {
                    return false;
                }

                ApplyPublishAndActive(action, course.Publish, course.Activate, course.Deactivate, now);
                return true;
            case "book":
                var book = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
                if (book is null)
                {
                    return false;
                }

                ApplyPublishAndActive(action, book.Publish, book.Activate, book.Deactivate, now);
                return true;
            case "quiz":
                var quiz = await dbContext.Quizzes.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
                if (quiz is null)
                {
                    return false;
                }

                ApplyPublishAndActive(action, quiz.Publish, quiz.Activate, quiz.Deactivate, now);
                return true;
            default:
                return false;
        }
    }

    private async Task<bool> AssignCategoryAsync(string contentType, Guid id, Guid? categoryId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (contentType != "word" || !categoryId.HasValue)
        {
            return false;
        }

        var word = await dbContext.Words.Include(item => item.Tags).SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (word is null)
        {
            return false;
        }

        word.SetCategory(categoryId, now);
        return true;
    }

    private async Task<bool> UpdateWordTagsAsync(BulkOperation operation, Guid id, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (operation.ContentType != "word")
        {
            return false;
        }

        var word = await dbContext.Words.Include(item => item.Tags).SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (word is null)
        {
            return false;
        }

        var current = word.Tags.Select(tag => tag.TagId).ToHashSet();
        var requested = operation.TagIds.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(Guid.Parse).ToArray();
        if (operation.OperationType == BulkOperationType.AddTags)
        {
            current.UnionWith(requested);
        }
        else
        {
            current.ExceptWith(requested);
        }

        word.SetTags(current, now);
        return true;
    }

    private async Task<bool> ContentExistsAsync(string contentType, Guid id, CancellationToken cancellationToken) =>
        contentType switch
        {
            "word" => await dbContext.Words.AnyAsync(item => item.Id == id, cancellationToken),
            "lesson" => await dbContext.Lessons.AnyAsync(item => item.Id == id, cancellationToken),
            "course" => await dbContext.Courses.AnyAsync(item => item.Id == id, cancellationToken),
            "book" => await dbContext.Books.AnyAsync(item => item.Id == id, cancellationToken),
            "quiz" => await dbContext.Quizzes.AnyAsync(item => item.Id == id, cancellationToken),
            "pronunciation" => await dbContext.Pronunciations.AnyAsync(item => item.Id == id, cancellationToken),
            "grammartopic" => await dbContext.GrammarTopics.AnyAsync(item => item.Id == id, cancellationToken),
            "grammarrule" => await dbContext.GrammarRules.AnyAsync(item => item.Id == id, cancellationToken),
            _ => false
        };

    private static void ApplyActiveOnly(BulkOperationType action, Action<DateTimeOffset> activate, Action<DateTimeOffset> deactivate, DateTimeOffset now)
    {
        if (action is BulkOperationType.Activate or BulkOperationType.SubmitForReview or BulkOperationType.Approve or BulkOperationType.RequestChanges)
        {
            activate(now);
            return;
        }

        if (action is BulkOperationType.Deactivate or BulkOperationType.Archive)
        {
            deactivate(now);
            return;
        }

        throw new InvalidOperationException("Action is not supported for this content type.");
    }

    private static void ApplyPublishAndActive(BulkOperationType action, Action<DateTimeOffset> publish, Action<DateTimeOffset> activate, Action<DateTimeOffset> deactivate, DateTimeOffset now)
    {
        switch (action)
        {
            case BulkOperationType.Publish:
            case BulkOperationType.Approve:
                publish(now);
                break;
            case BulkOperationType.Activate:
            case BulkOperationType.SubmitForReview:
            case BulkOperationType.RequestChanges:
                activate(now);
                break;
            case BulkOperationType.Deactivate:
            case BulkOperationType.Archive:
                deactivate(now);
                break;
            default:
                throw new InvalidOperationException("Action is not supported for this content type.");
        }
    }
}
