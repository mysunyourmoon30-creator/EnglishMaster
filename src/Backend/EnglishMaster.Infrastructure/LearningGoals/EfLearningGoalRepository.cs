using EnglishMaster.Application.Features.DailyStudyPlans;
using EnglishMaster.Application.Features.LearningGoals;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.LearningGoals;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using GoalDto = EnglishMaster.Application.Features.LearningGoals.Dtos.LearningGoalDto;
using GoalSummaryDto = EnglishMaster.Application.Features.LearningGoals.Dtos.LearningGoalSummaryDto;
using PlanDto = EnglishMaster.Application.Features.DailyStudyPlans.Dtos.DailyStudyPlanDto;
using PlanItemDto = EnglishMaster.Application.Features.DailyStudyPlans.Dtos.DailyStudyPlanItemDto;
using PlanSummaryDto = EnglishMaster.Application.Features.DailyStudyPlans.Dtos.DailyStudyPlanSummaryDto;

namespace EnglishMaster.Infrastructure.LearningGoals;

public sealed class EfLearningGoalRepository : ILearningGoalRepository, IDailyStudyPlanRepository
{
    private readonly EnglishMasterDbContext dbContext;
    private readonly TimeProvider timeProvider;

    public EfLearningGoalRepository(EnglishMasterDbContext dbContext, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.timeProvider = timeProvider;
    }

    public async Task<GoalDto> CreateAsync(Guid userId, string goalType, string title, string? description, int targetValue, string? unit, DateTimeOffset? targetDate, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var goal = LearningGoal.Create(profile.Id, goalType, title, description, targetValue, unit, targetDate, now);
        dbContext.LearningGoals.Add(goal);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(goal);
    }

    public async Task<GoalDto?> UpdateAsync(Guid userId, Guid goalId, string title, string? description, int targetValue, int currentValue, string? unit, DateTimeOffset? targetDate, CancellationToken cancellationToken)
    {
        var goal = await GoalForUserAsync(userId, goalId, tracking: true, cancellationToken);
        if (goal is null)
        {
            return null;
        }

        goal.Update(title, description, targetValue, currentValue, unit, targetDate, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(goal);
    }

    public async Task<GoalDto?> ChangeStatusAsync(Guid userId, Guid goalId, string action, CancellationToken cancellationToken)
    {
        var goal = await GoalForUserAsync(userId, goalId, tracking: true, cancellationToken);
        if (goal is null)
        {
            return null;
        }

        var now = timeProvider.GetUtcNow();
        try
        {
            switch (action)
            {
                case "pause":
                    goal.Pause(now);
                    break;
                case "resume":
                    goal.Resume(now);
                    break;
                case "complete":
                    goal.Complete(now);
                    break;
                case "cancel":
                    goal.Cancel(now);
                    break;
                default:
                    return null;
            }
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(goal);
    }

    public async Task<GoalDto?> GetByIdAsync(Guid userId, Guid goalId, CancellationToken cancellationToken)
    {
        var goal = await GoalForUserAsync(userId, goalId, tracking: false, cancellationToken);
        return goal is null ? null : ToDto(goal);
    }

    public async Task<IReadOnlyCollection<GoalDto>> GetGoalsAsync(Guid userId, bool activeOnly, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var query = dbContext.LearningGoals.AsNoTracking().Where(goal => goal.StudentProfileId == profile.Id);
        if (activeOnly)
        {
            query = query.Where(goal => goal.Status == LearningGoalStatus.Active);
        }

        var goals = await query.OrderBy(goal => goal.Status).ThenByDescending(goal => goal.UpdatedAt).Take(limit).ToArrayAsync(cancellationToken);
        return goals.Select(ToDto).ToArray();
    }

    public async Task<GoalSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return new GoalSummaryDto(0, 0, 0, 0);
        }

        var query = dbContext.LearningGoals.AsNoTracking().Where(goal => goal.StudentProfileId == profile.Id);
        return new GoalSummaryDto(
            await query.CountAsync(goal => goal.Status == LearningGoalStatus.Active, cancellationToken),
            await query.CountAsync(goal => goal.Status == LearningGoalStatus.Completed, cancellationToken),
            await query.CountAsync(goal => goal.Status == LearningGoalStatus.Paused, cancellationToken),
            await query.CountAsync(goal => goal.Status == LearningGoalStatus.Cancelled, cancellationToken));
    }

    public async Task<PlanDto> GenerateTodayAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var planDate = NormalizeDate(now);
        var existing = await dbContext.DailyStudyPlans.Include(plan => plan.Items).SingleOrDefaultAsync(plan => plan.StudentProfileId == profile.Id && plan.PlanDate == planDate, cancellationToken);
        if (existing is not null)
        {
            await EnsureDuePracticeBlockAsync(profile.Id, existing, now, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return ToDto(existing);
        }

        var targetMinutes = await TargetMinutesAsync(profile.Id, cancellationToken);
        var plan = DailyStudyPlan.Create(profile.Id, planDate, targetMinutes, now);
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var sortOrder = 1;

        sortOrder = await AddDuePracticeAsync(profile.Id, plan, used, sortOrder, now, cancellationToken);
        sortOrder = await AddContinueLearningAsync(userId, plan, used, sortOrder, now, cancellationToken);
        sortOrder = await AddRecommendedLessonAsync(userId, plan, used, sortOrder, now, cancellationToken);
        await AddWeakQuizAsync(userId, plan, used, sortOrder, now, cancellationToken);

        dbContext.DailyStudyPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(plan);
    }

    public async Task<PlanDto?> GetTodayAsync(Guid userId, CancellationToken cancellationToken) =>
        await GetByDateAsync(userId, NormalizeDate(timeProvider.GetUtcNow()), cancellationToken);

    public async Task<PlanDto?> GetByDateAsync(Guid userId, DateTimeOffset planDate, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var normalized = NormalizeDate(planDate);
        var plan = await dbContext.DailyStudyPlans.AsNoTracking().Include(plan => plan.Items).SingleOrDefaultAsync(plan => plan.StudentProfileId == profile.Id && plan.PlanDate == normalized, cancellationToken);
        return plan is null ? null : ToDto(plan);
    }

    async Task<PlanDto?> IDailyStudyPlanRepository.GetByIdAsync(Guid userId, Guid planId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var plan = await dbContext.DailyStudyPlans.AsNoTracking().Include(plan => plan.Items).SingleOrDefaultAsync(plan => plan.Id == planId && plan.StudentProfileId == profile.Id, cancellationToken);
        return plan is null ? null : ToDto(plan);
    }

    public Task<PlanItemDto?> CompleteItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken) =>
        MutateItemAsync(userId, itemId, item => item.Complete(timeProvider.GetUtcNow()), cancellationToken);

    public Task<PlanItemDto?> SkipItemAsync(Guid userId, Guid itemId, CancellationToken cancellationToken) =>
        MutateItemAsync(userId, itemId, item => item.Skip(timeProvider.GetUtcNow()), cancellationToken);

    public async Task<PlanDto?> CompletePlanAsync(Guid userId, Guid planId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var plan = await dbContext.DailyStudyPlans.Include(plan => plan.Items).SingleOrDefaultAsync(plan => plan.Id == planId && plan.StudentProfileId == profile.Id, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        try
        {
            plan.Complete(timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(plan);
    }

    public async Task<IReadOnlyCollection<PlanDto>> GetHistoryAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var plans = await dbContext.DailyStudyPlans.AsNoTracking().Include(plan => plan.Items).Where(plan => plan.StudentProfileId == profile.Id).OrderByDescending(plan => plan.PlanDate).Take(limit).ToArrayAsync(cancellationToken);
        return plans.Select(ToDto).ToArray();
    }

    async Task<PlanSummaryDto> IDailyStudyPlanRepository.GetSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return new PlanSummaryDto(0, 0, 0, 0);
        }

        var weekStart = NormalizeDate(timeProvider.GetUtcNow()).AddDays(-6);
        var plans = await dbContext.DailyStudyPlans.AsNoTracking().Where(plan => plan.StudentProfileId == profile.Id && plan.PlanDate >= weekStart).ToArrayAsync(cancellationToken);
        return new PlanSummaryDto(
            plans.Length,
            plans.Count(plan => plan.Status == DailyStudyPlanStatus.Completed),
            plans.Sum(plan => plan.CompletedItems),
            plans.Sum(plan => plan.CompletedMinutes));
    }

    private async Task<PlanItemDto?> MutateItemAsync(Guid userId, Guid itemId, Action<DailyStudyPlanItem> action, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var item = await dbContext.DailyStudyPlanItems.SingleOrDefaultAsync(item => item.Id == itemId, cancellationToken);
        if (item is null)
        {
            return null;
        }

        var plan = await dbContext.DailyStudyPlans.Include(plan => plan.Items).SingleOrDefaultAsync(plan => plan.Id == item.DailyStudyPlanId && plan.StudentProfileId == profile.Id, cancellationToken);
        if (plan is null)
        {
            return null;
        }

        try
        {
            action(item);
            plan.MarkInProgress(timeProvider.GetUtcNow());
            plan.Recount(timeProvider.GetUtcNow());
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(item);
    }

    private async Task<int> AddDuePracticeAsync(Guid profileId, DailyStudyPlan plan, ISet<string> used, int sortOrder, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var dueCount = await dbContext.PracticeItems.AsNoTracking()
            .CountAsync(item => item.StudentProfileId == profileId && item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now, cancellationToken);
        if (dueCount <= 0 || !used.Add("practice|due"))
        {
            return sortOrder;
        }

        plan.AddItem("PracticeDueItems", "practice", null, $"Review {dueCount} due practice item(s)", "/learn/practice", 10, sortOrder, now);
        return sortOrder + 1;
    }

    private async Task EnsureDuePracticeBlockAsync(Guid profileId, DailyStudyPlan plan, DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (plan.Items.Any(item => item.ItemType == "PracticeDueItems"))
        {
            return;
        }

        var dueCount = await dbContext.PracticeItems.AsNoTracking()
            .CountAsync(item => item.StudentProfileId == profileId && item.Status != PracticeItemStatus.Suspended && item.NextReviewAt <= now, cancellationToken);
        if (dueCount <= 0)
        {
            return;
        }

        var item = plan.AddItem("PracticeDueItems", "practice", null, $"Review {dueCount} due practice item(s)", "/learn/practice", 10, 0, now);
        dbContext.DailyStudyPlanItems.Add(item);
    }

    private async Task<int> AddContinueLearningAsync(Guid userId, DailyStudyPlan plan, ISet<string> used, int sortOrder, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var lesson = await dbContext.LessonProgress.AsNoTracking()
            .Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.InProgress)
            .OrderByDescending(progress => progress.LastAccessedAt)
            .Join(dbContext.Lessons.AsNoTracking().Where(lesson => lesson.IsActive && lesson.IsPublished), progress => progress.LessonId, lesson => lesson.Id, (progress, lesson) => new { lesson.Id, lesson.Title, lesson.Slug })
            .FirstOrDefaultAsync(cancellationToken);
        if (lesson is null || !used.Add("lesson|" + lesson.Id))
        {
            return sortOrder;
        }

        plan.AddItem("ContinueLesson", "lesson", lesson.Id, lesson.Title, $"/lessons/{lesson.Slug}", 15, sortOrder, now);
        return sortOrder + 1;
    }

    private async Task<int> AddRecommendedLessonAsync(Guid userId, DailyStudyPlan plan, ISet<string> used, int sortOrder, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var completed = await dbContext.LessonProgress.AsNoTracking().Where(progress => progress.UserId == userId && progress.Status == LearningProgressStatus.Completed).Select(progress => progress.LessonId).ToArrayAsync(cancellationToken);
        var lesson = await dbContext.Lessons.AsNoTracking()
            .Where(lesson => lesson.IsActive && lesson.IsPublished && !completed.Contains(lesson.Id))
            .OrderByDescending(lesson => lesson.UpdatedAt)
            .Select(lesson => new { lesson.Id, lesson.Title, lesson.Slug })
            .FirstOrDefaultAsync(cancellationToken);
        if (lesson is null || !used.Add("lesson|" + lesson.Id))
        {
            return sortOrder;
        }

        plan.AddItem("RecommendedLesson", "lesson", lesson.Id, lesson.Title, $"/lessons/{lesson.Slug}", 15, sortOrder, now);
        return sortOrder + 1;
    }

    private async Task<int> AddWeakQuizAsync(Guid userId, DailyStudyPlan plan, ISet<string> used, int sortOrder, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var quiz = await dbContext.QuizAttempts.AsNoTracking()
            .Where(attempt => attempt.UserId == userId && !attempt.Passed)
            .OrderByDescending(attempt => attempt.AttemptedAt)
            .Join(dbContext.Quizzes.AsNoTracking().Where(quiz => quiz.IsActive && quiz.IsPublished), attempt => attempt.QuizId, quiz => quiz.Id, (attempt, quiz) => new { quiz.Id, quiz.Title, quiz.Slug })
            .FirstOrDefaultAsync(cancellationToken);
        if (quiz is null || !used.Add("quiz|" + quiz.Id))
        {
            return sortOrder;
        }

        plan.AddItem("ReviewWeakQuiz", "quiz", quiz.Id, quiz.Title, $"/quizzes/{quiz.Slug}", 10, sortOrder, now);
        return sortOrder + 1;
    }

    private async Task<int> TargetMinutesAsync(Guid profileId, CancellationToken cancellationToken) =>
        await dbContext.LearningGoals.AsNoTracking()
            .Where(goal => goal.StudentProfileId == profileId && goal.Status == LearningGoalStatus.Active && goal.GoalType == "DailyStudyMinutes")
            .OrderBy(goal => goal.CreatedAt)
            .Select(goal => goal.TargetValue)
            .FirstOrDefaultAsync(cancellationToken) is var target && target > 0 ? target : 30;

    private async Task<LearningGoal?> GoalForUserAsync(Guid userId, Guid goalId, bool tracking, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return null;
        }

        var query = tracking ? dbContext.LearningGoals : dbContext.LearningGoals.AsNoTracking();
        return await query.SingleOrDefaultAsync(goal => goal.Id == goalId && goal.StudentProfileId == profile.Id, cancellationToken);
    }

    private async Task<StudentProfile> GetOrCreateProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is not null)
        {
            return profile;
        }

        profile = StudentProfile.Create(userId, null, timeProvider.GetUtcNow());
        dbContext.StudentProfiles.Add(profile);
        await dbContext.SaveChangesAsync(cancellationToken);
        return profile;
    }

    private async Task<StudentProfile?> GetProfileAsync(Guid userId, CancellationToken cancellationToken) =>
        await dbContext.StudentProfiles.OrderBy(profile => profile.CreatedAt).FirstOrDefaultAsync(profile => profile.UserId == userId, cancellationToken);

    private static DateTimeOffset NormalizeDate(DateTimeOffset value) =>
        new(value.UtcDateTime.Date, TimeSpan.Zero);

    private static GoalDto ToDto(LearningGoal goal) =>
        new(goal.Id, goal.GoalType, goal.Title, goal.Description, goal.TargetValue, goal.CurrentValue, goal.Unit, goal.TargetDate, goal.Status.ToString(), goal.CreatedAt, goal.UpdatedAt);

    private static PlanDto ToDto(DailyStudyPlan plan) =>
        new(plan.Id, plan.PlanDate, plan.Status.ToString(), plan.TargetMinutes, plan.CompletedMinutes, plan.TotalItems, plan.CompletedItems, plan.Items.OrderBy(item => item.SortOrder).Select(ToDto).ToArray());

    private static PlanItemDto ToDto(DailyStudyPlanItem item) =>
        new(item.Id, item.DailyStudyPlanId, item.ItemType, item.ContentType, item.ContentId, item.Title, item.Url, item.EstimatedMinutes, item.SortOrder, item.Status.ToString(), item.CompletedAt);
}
