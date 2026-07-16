using EnglishMaster.Application.Features.Motivation;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.Motivation;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AchievementDefinitionDto = EnglishMaster.Application.Features.Achievements.Dtos.AchievementDefinitionDto;
using LearningActivityDto = EnglishMaster.Application.Features.Motivation.Dtos.LearningActivityDto;
using MotivationSummaryDto = EnglishMaster.Application.Features.Motivation.Dtos.MotivationSummaryDto;
using StudentAchievementDto = EnglishMaster.Application.Features.Achievements.Dtos.StudentAchievementDto;
using StudentStreakDto = EnglishMaster.Application.Features.Motivation.Dtos.StudentStreakDto;

namespace EnglishMaster.Infrastructure.Motivation;

public sealed class EfMotivationRepository : IMotivationRepository
{
    private static readonly (string Code, string Name, string Type, int Target, string Icon, int Sort)[] Defaults =
    [
        ("first-lesson-completed", "First Lesson Completed", "FirstLessonCompleted", 1, "book-open", 1),
        ("first-quiz-completed", "First Quiz Completed", "FirstQuizCompleted", 1, "check-circle", 2),
        ("first-quiz-passed", "First Quiz Passed", "FirstQuizPassed", 1, "award", 3),
        ("first-practice-session", "First Practice Session", "FirstPracticeSession", 1, "repeat", 4),
        ("three-day-streak", "Three Day Streak", "ThreeDayStreak", 3, "flame", 5),
        ("seven-day-streak", "Seven Day Streak", "SevenDayStreak", 7, "flame", 6),
        ("fourteen-day-streak", "Fourteen Day Streak", "FourteenDayStreak", 14, "flame", 7),
        ("thirty-day-streak", "Thirty Day Streak", "ThirtyDayStreak", 30, "flame", 8),
        ("ten-lessons-completed", "Ten Lessons Completed", "TenLessonsCompleted", 10, "library", 9),
        ("first-course-completed", "First Course Completed", "FirstCourseCompleted", 1, "graduation-cap", 10),
        ("first-book-completed", "First Book Completed", "FirstBookCompleted", 1, "book", 11),
        ("daily-plan-completed", "Daily Plan Completed", "DailyPlanCompleted", 1, "calendar-check", 12),
        ("practice-items-completed", "Practice Items Completed", "PracticeItemsCompleted", 10, "target", 13)
    ];

    private readonly EnglishMasterDbContext dbContext;
    private readonly TimeProvider timeProvider;

    public EfMotivationRepository(EnglishMasterDbContext dbContext, TimeProvider timeProvider)
    {
        this.dbContext = dbContext;
        this.timeProvider = timeProvider;
    }

    public async Task<LearningActivityDto> RecordActivityAsync(Guid userId, string activityType, string? contentType, Guid? contentId, string? title, DateTimeOffset occurredAt, int minutesSpent, string? metadataJson, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var activity = LearningActivityLog.Create(profile.Id, activityType, contentType, contentId, title, occurredAt, minutesSpent, metadataJson, now);
        dbContext.LearningActivityLogs.Add(activity);
        var streak = await GetOrCreateStreakAsync(profile.Id, cancellationToken);
        streak.ApplyActivity(occurredAt, now);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(activity);
    }

    public async Task<IReadOnlyCollection<LearningActivityDto>> GetActivityAsync(Guid userId, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var activities = await dbContext.LearningActivityLogs.AsNoTracking()
            .Where(activity => activity.StudentProfileId == profile.Id)
            .OrderByDescending(activity => activity.OccurredAt)
            .Take(limit)
            .ToArrayAsync(cancellationToken);
        return activities.Select(ToDto).ToArray();
    }

    public async Task<StudentStreakDto> UpdateStreakAsync(Guid userId, DateTimeOffset occurredAt, CancellationToken cancellationToken)
    {
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var streak = await GetOrCreateStreakAsync(profile.Id, cancellationToken);
        streak.ApplyActivity(occurredAt, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(streak);
    }

    public async Task<StudentStreakDto> GetStreakAsync(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return new StudentStreakDto(0, 0, null, null);
        }

        var streak = await dbContext.StudentStreaks.AsNoTracking().FirstOrDefaultAsync(streak => streak.StudentProfileId == profile.Id, cancellationToken);
        return streak is null ? new StudentStreakDto(0, 0, null, null) : ToDto(streak);
    }

    public async Task<MotivationSummaryDto> GetSummaryAsync(Guid userId, CancellationToken cancellationToken)
    {
        await EvaluateAchievementsAsync(userId, cancellationToken);
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return new MotivationSummaryDto(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, [], []);
        }

        var streak = await GetStreakAsync(userId, cancellationToken);
        var query = dbContext.LearningActivityLogs.AsNoTracking().Where(activity => activity.StudentProfileId == profile.Id);
        var recentAchievements = await GetAchievementsAsync(userId, earnedOnly: true, 5, cancellationToken);
        var recentActivity = await GetActivityAsync(userId, 5, cancellationToken);
        var earnedAchievementCount = await dbContext.StudentAchievements.AsNoTracking().CountAsync(achievement => achievement.StudentProfileId == profile.Id && achievement.Status == StudentAchievementStatus.Earned, cancellationToken);
        return new MotivationSummaryDto(
            streak.CurrentStreakDays,
            streak.LongestStreakDays,
            await query.CountAsync(activity => activity.ActivityType == "LessonCompleted", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "CourseCompleted", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "BookCompleted", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "QuizAttempted" || activity.ActivityType == "QuizPassed", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "QuizPassed", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "PracticeSessionCompleted", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "DailyStudyPlanCompleted", cancellationToken),
            await query.CountAsync(activity => activity.ActivityType == "LearningGoalCompleted", cancellationToken),
            earnedAchievementCount,
            recentAchievements,
            recentActivity);
    }

    public async Task<int> SeedDefaultAchievementDefinitionsAsync(CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var existing = await dbContext.AchievementDefinitions.AsNoTracking().Select(definition => definition.Code).ToArrayAsync(cancellationToken);
        var created = 0;
        foreach (var item in Defaults.Where(item => !existing.Contains(item.Code, StringComparer.OrdinalIgnoreCase)))
        {
            dbContext.AchievementDefinitions.Add(AchievementDefinition.Create(item.Code, item.Name, item.Name, item.Type, item.Target, item.Icon, item.Sort, now));
            created++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return created;
    }

    public async Task<IReadOnlyCollection<AchievementDefinitionDto>> SearchDefinitionsAsync(int limit, CancellationToken cancellationToken)
    {
        var definitions = await dbContext.AchievementDefinitions.AsNoTracking().OrderBy(definition => definition.SortOrder).ThenBy(definition => definition.Name).Take(limit).ToArrayAsync(cancellationToken);
        return definitions.Select(ToDto).ToArray();
    }

    public async Task<AchievementDefinitionDto?> CreateDefinitionAsync(string code, string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, CancellationToken cancellationToken)
    {
        if (await dbContext.AchievementDefinitions.AnyAsync(definition => definition.Code == code, cancellationToken))
        {
            return null;
        }

        var definition = AchievementDefinition.Create(code, name, description, achievementType, targetValue, iconName, sortOrder, timeProvider.GetUtcNow());
        dbContext.AchievementDefinitions.Add(definition);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(definition);
    }

    public async Task<AchievementDefinitionDto?> UpdateDefinitionAsync(Guid id, string name, string? description, string achievementType, int targetValue, string? iconName, int sortOrder, CancellationToken cancellationToken)
    {
        var definition = await dbContext.AchievementDefinitions.FindAsync([id], cancellationToken);
        if (definition is null)
        {
            return null;
        }

        definition.Update(name, description, achievementType, targetValue, iconName, sortOrder, timeProvider.GetUtcNow());
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(definition);
    }

    public async Task<AchievementDefinitionDto?> SetDefinitionActiveAsync(Guid id, bool active, CancellationToken cancellationToken)
    {
        var definition = await dbContext.AchievementDefinitions.FindAsync([id], cancellationToken);
        if (definition is null)
        {
            return null;
        }

        if (active)
        {
            definition.Activate(timeProvider.GetUtcNow());
        }
        else
        {
            definition.Deactivate(timeProvider.GetUtcNow());
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToDto(definition);
    }

    public async Task<IReadOnlyCollection<StudentAchievementDto>> EvaluateAchievementsAsync(Guid userId, CancellationToken cancellationToken)
    {
        await SeedDefaultAchievementDefinitionsAsync(cancellationToken);
        var profile = await GetOrCreateProfileAsync(userId, cancellationToken);
        var now = timeProvider.GetUtcNow();
        var definitions = await dbContext.AchievementDefinitions.Where(definition => definition.IsActive).OrderBy(definition => definition.SortOrder).ToArrayAsync(cancellationToken);
        var achievements = await dbContext.StudentAchievements.Where(achievement => achievement.StudentProfileId == profile.Id).ToArrayAsync(cancellationToken);
        foreach (var definition in definitions)
        {
            var achievement = achievements.SingleOrDefault(item => item.AchievementDefinitionId == definition.Id);
            if (achievement is null)
            {
                achievement = StudentAchievement.Create(profile.Id, definition.Id, now);
                dbContext.StudentAchievements.Add(achievement);
            }

            achievement.ApplyProgress(await ProgressAsync(profile.Id, definition, cancellationToken), definition.TargetValue, now);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return await GetAchievementsAsync(userId, earnedOnly: false, 200, cancellationToken);
    }

    public async Task<IReadOnlyCollection<StudentAchievementDto>> GetAchievementsAsync(Guid userId, bool earnedOnly, int limit, CancellationToken cancellationToken)
    {
        var profile = await GetProfileAsync(userId, cancellationToken);
        if (profile is null)
        {
            return [];
        }

        var query = dbContext.StudentAchievements.AsNoTracking()
            .Where(achievement => achievement.StudentProfileId == profile.Id)
            .Join(dbContext.AchievementDefinitions.AsNoTracking(), achievement => achievement.AchievementDefinitionId, definition => definition.Id, (achievement, definition) => new { achievement, definition });
        if (earnedOnly)
        {
            query = query.Where(item => item.achievement.Status == StudentAchievementStatus.Earned);
        }

        var rows = await query.OrderByDescending(item => item.achievement.EarnedAt).ThenBy(item => item.definition.SortOrder).Take(limit).ToArrayAsync(cancellationToken);
        return rows.Select(item => ToDto(item.achievement, item.definition)).ToArray();
    }

    private async Task<int> ProgressAsync(Guid profileId, AchievementDefinition definition, CancellationToken cancellationToken)
    {
        var query = dbContext.LearningActivityLogs.AsNoTracking().Where(activity => activity.StudentProfileId == profileId);
        return definition.AchievementType switch
        {
            "FirstLessonCompleted" => await query.CountAsync(activity => activity.ActivityType == "LessonCompleted", cancellationToken),
            "FirstQuizCompleted" => await query.CountAsync(activity => activity.ActivityType == "QuizAttempted" || activity.ActivityType == "QuizPassed", cancellationToken),
            "FirstQuizPassed" => await query.CountAsync(activity => activity.ActivityType == "QuizPassed", cancellationToken),
            "FirstPracticeSession" => await query.CountAsync(activity => activity.ActivityType == "PracticeSessionCompleted", cancellationToken),
            "ThreeDayStreak" or "SevenDayStreak" or "FourteenDayStreak" or "ThirtyDayStreak" => await dbContext.StudentStreaks.AsNoTracking().Where(streak => streak.StudentProfileId == profileId).Select(streak => streak.LongestStreakDays).FirstOrDefaultAsync(cancellationToken),
            "TenLessonsCompleted" => await query.CountAsync(activity => activity.ActivityType == "LessonCompleted", cancellationToken),
            "FirstCourseCompleted" => await query.CountAsync(activity => activity.ActivityType == "CourseCompleted", cancellationToken),
            "FirstBookCompleted" => await query.CountAsync(activity => activity.ActivityType == "BookCompleted", cancellationToken),
            "DailyPlanCompleted" => await query.CountAsync(activity => activity.ActivityType == "DailyStudyPlanCompleted", cancellationToken),
            "PracticeItemsCompleted" => await query.CountAsync(activity => activity.ActivityType == "PracticeSessionCompleted", cancellationToken),
            _ => 0
        };
    }

    private async Task<StudentStreak> GetOrCreateStreakAsync(Guid profileId, CancellationToken cancellationToken)
    {
        var streak = await dbContext.StudentStreaks.FirstOrDefaultAsync(streak => streak.StudentProfileId == profileId, cancellationToken);
        if (streak is not null)
        {
            return streak;
        }

        streak = StudentStreak.Create(profileId, timeProvider.GetUtcNow());
        dbContext.StudentStreaks.Add(streak);
        return streak;
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

    private static LearningActivityDto ToDto(LearningActivityLog activity) =>
        new(activity.Id, activity.ActivityType, activity.ContentType, activity.ContentId, activity.Title, activity.OccurredAt, activity.MinutesSpent, activity.MetadataJson);

    private static StudentStreakDto ToDto(StudentStreak streak) =>
        new(streak.CurrentStreakDays, streak.LongestStreakDays, streak.LastActivityDate, streak.StreakStartDate);

    private static AchievementDefinitionDto ToDto(AchievementDefinition definition) =>
        new(definition.Id, definition.Code, definition.Name, definition.Description, definition.AchievementType, definition.TargetValue, definition.IconName, definition.IsActive, definition.SortOrder);

    private static StudentAchievementDto ToDto(StudentAchievement achievement, AchievementDefinition definition) =>
        new(achievement.Id, definition.Id, definition.Code, definition.Name, definition.Description, definition.AchievementType, definition.TargetValue, definition.IconName, achievement.Status.ToString(), achievement.ProgressValue, achievement.EarnedAt);
}
