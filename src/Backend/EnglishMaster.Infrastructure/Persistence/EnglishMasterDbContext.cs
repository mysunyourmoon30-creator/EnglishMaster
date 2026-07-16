using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.BulkOperations;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.ContentQuality;
using EnglishMaster.Domain.ContentRevisions;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.ImportJobs;
using EnglishMaster.Domain.Learning;
using EnglishMaster.Domain.LearningGoals;
using EnglishMaster.Domain.LearningReports;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Notifications;
using EnglishMaster.Domain.Motivation;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Domain.Practice;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Security;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using Microsoft.EntityFrameworkCore;
using MediaEntity = EnglishMaster.Domain.Media.Media;

namespace EnglishMaster.Infrastructure.Persistence;

public sealed class EnglishMasterDbContext : DbContext
{
    public EnglishMasterDbContext(DbContextOptions<EnglishMasterDbContext> options)
        : base(options)
    {
    }

    public DbSet<Word> Words => Set<Word>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<MediaEntity> Media => Set<MediaEntity>();

    public DbSet<WordTag> WordTags => Set<WordTag>();

    public DbSet<Pronunciation> Pronunciations => Set<Pronunciation>();

    public DbSet<MinimalPair> MinimalPairs => Set<MinimalPair>();

    public DbSet<GrammarTopic> GrammarTopics => Set<GrammarTopic>();

    public DbSet<GrammarRule> GrammarRules => Set<GrammarRule>();

    public DbSet<GrammarExample> GrammarExamples => Set<GrammarExample>();

    public DbSet<GrammarRuleWord> GrammarRuleWords => Set<GrammarRuleWord>();

    public DbSet<Lesson> Lessons => Set<Lesson>();

    public DbSet<LessonSection> LessonSections => Set<LessonSection>();

    public DbSet<LessonWord> LessonWords => Set<LessonWord>();

    public DbSet<LessonGrammarRule> LessonGrammarRules => Set<LessonGrammarRule>();

    public DbSet<Course> Courses => Set<Course>();

    public DbSet<CourseLesson> CourseLessons => Set<CourseLesson>();

    public DbSet<Book> Books => Set<Book>();

    public DbSet<BookChapter> BookChapters => Set<BookChapter>();

    public DbSet<BookChapterLesson> BookChapterLessons => Set<BookChapterLesson>();

    public DbSet<Quiz> Quizzes => Set<Quiz>();

    public DbSet<QuizQuestion> QuizQuestions => Set<QuizQuestion>();

    public DbSet<QuizChoice> QuizChoices => Set<QuizChoice>();

    public DbSet<Notification> Notifications => Set<Notification>();

    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();

    public DbSet<ContentQualityRule> ContentQualityRules => Set<ContentQualityRule>();

    public DbSet<ContentQualityCheck> ContentQualityChecks => Set<ContentQualityCheck>();

    public DbSet<ContentQualityFinding> ContentQualityFindings => Set<ContentQualityFinding>();

    public DbSet<ContentRevision> ContentRevisions => Set<ContentRevision>();

    public DbSet<ContentRevisionRestoreRequest> ContentRevisionRestoreRequests => Set<ContentRevisionRestoreRequest>();

    public DbSet<BulkOperation> BulkOperations => Set<BulkOperation>();

    public DbSet<BulkOperationItem> BulkOperationItems => Set<BulkOperationItem>();

    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();

    public DbSet<ImportJobRow> ImportJobRows => Set<ImportJobRow>();

    public DbSet<ImportValidationError> ImportValidationErrors => Set<ImportValidationError>();

    public DbSet<StudentProfile> StudentProfiles => Set<StudentProfile>();

    public DbSet<LessonProgress> LessonProgress => Set<LessonProgress>();

    public DbSet<CourseProgress> CourseProgress => Set<CourseProgress>();

    public DbSet<BookProgress> BookProgress => Set<BookProgress>();

    public DbSet<QuizAttempt> QuizAttempts => Set<QuizAttempt>();

    public DbSet<PracticeItem> PracticeItems => Set<PracticeItem>();

    public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();

    public DbSet<PracticeSessionItem> PracticeSessionItems => Set<PracticeSessionItem>();

    public DbSet<LearningGoal> LearningGoals => Set<LearningGoal>();

    public DbSet<DailyStudyPlan> DailyStudyPlans => Set<DailyStudyPlan>();

    public DbSet<DailyStudyPlanItem> DailyStudyPlanItems => Set<DailyStudyPlanItem>();

    public DbSet<LearningActivityLog> LearningActivityLogs => Set<LearningActivityLog>();

    public DbSet<StudentStreak> StudentStreaks => Set<StudentStreak>();

    public DbSet<AchievementDefinition> AchievementDefinitions => Set<AchievementDefinition>();

    public DbSet<StudentAchievement> StudentAchievements => Set<StudentAchievement>();

    public DbSet<WeeklyLearningReport> WeeklyLearningReports => Set<WeeklyLearningReport>();

    public DbSet<WeeklyLearningReportInsight> WeeklyLearningReportInsights => Set<WeeklyLearningReportInsight>();

    public DbSet<PublishJob> PublishJobs => Set<PublishJob>();

    public DbSet<PublishTemplate> PublishTemplates => Set<PublishTemplate>();

    public DbSet<PublishedArtifact> PublishedArtifacts => Set<PublishedArtifact>();

    public DbSet<AppUser> AppUsers => Set<AppUser>();

    public DbSet<AppRole> AppRoles => Set<AppRole>();

    public DbSet<AppPermission> AppPermissions => Set<AppPermission>();

    public DbSet<AppUserRole> AppUserRoles => Set<AppUserRole>();

    public DbSet<AppRolePermission> AppRolePermissions => Set<AppRolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EnglishMasterDbContext).Assembly);
    }
}
