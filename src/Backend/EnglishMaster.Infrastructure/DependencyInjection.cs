using EnglishMaster.Application.Features.Analytics;
using EnglishMaster.Application.Features.BookChapters;
using EnglishMaster.Application.Features.Books;
using EnglishMaster.Application.Features.BulkOperations;
using EnglishMaster.Application.Features.Categories;
using EnglishMaster.Application.Features.Certificates;
using EnglishMaster.Application.Features.ContentQuality;
using EnglishMaster.Application.Features.ContentRevisions;
using EnglishMaster.Application.Features.Courses;
using EnglishMaster.Application.Features.GrammarExamples;
using EnglishMaster.Application.Features.GrammarRules;
using EnglishMaster.Application.Features.GrammarTopics;
using EnglishMaster.Application.Features.ImportExport;
using EnglishMaster.Application.Features.ImportJobs;
using EnglishMaster.Application.Features.LessonSections;
using EnglishMaster.Application.Features.Lessons;
using EnglishMaster.Application.Features.LearningRecommendations;
using EnglishMaster.Application.Features.LearningGoals;
using EnglishMaster.Application.Features.LearningReports;
using EnglishMaster.Application.Features.DailyStudyPlans;
using EnglishMaster.Application.Features.Media;
using EnglishMaster.Application.Features.Motivation;
using EnglishMaster.Application.Features.MinimalPairs;
using EnglishMaster.Application.Features.EmailMessages;
using EnglishMaster.Application.Features.Notifications;
using EnglishMaster.Application.Features.Pronunciations;
using EnglishMaster.Application.Features.PublishedArtifacts;
using EnglishMaster.Application.Features.PublishJobs;
using EnglishMaster.Application.Features.Publishing;
using EnglishMaster.Application.Features.Practice;
using EnglishMaster.Application.Features.PublishTemplates;
using EnglishMaster.Application.Features.PublicSearch;
using EnglishMaster.Application.Features.QuizChoices;
using EnglishMaster.Application.Features.QuizQuestions;
using EnglishMaster.Application.Features.Quizzes;
using EnglishMaster.Application.Features.Reports;
using EnglishMaster.Application.Features.Security;
using EnglishMaster.Application.Features.Tags;
using EnglishMaster.Application.Features.Words;
using EnglishMaster.Infrastructure.Analytics;
using EnglishMaster.Infrastructure.Books;
using EnglishMaster.Infrastructure.BulkOperations;
using EnglishMaster.Infrastructure.Categories;
using EnglishMaster.Infrastructure.Certificates;
using EnglishMaster.Infrastructure.ContentQuality;
using EnglishMaster.Infrastructure.ContentRevisions;
using EnglishMaster.Infrastructure.Courses;
using EnglishMaster.Infrastructure.Grammar;
using EnglishMaster.Infrastructure.ImportExport;
using EnglishMaster.Infrastructure.ImportJobs;
using EnglishMaster.Infrastructure.Lessons;
using EnglishMaster.Infrastructure.LearningRecommendations;
using EnglishMaster.Infrastructure.LearningGoals;
using EnglishMaster.Infrastructure.LearningReports;
using EnglishMaster.Infrastructure.Media;
using EnglishMaster.Infrastructure.Motivation;
using EnglishMaster.Infrastructure.Notifications;
using EnglishMaster.Infrastructure.Persistence;
using EnglishMaster.Infrastructure.Publishing;
using EnglishMaster.Infrastructure.Practice;
using EnglishMaster.Infrastructure.PublicSearch;
using EnglishMaster.Infrastructure.Pronunciations;
using EnglishMaster.Infrastructure.Quizzes;
using EnglishMaster.Infrastructure.Reports;
using EnglishMaster.Infrastructure.Security;
using EnglishMaster.Infrastructure.Tags;
using EnglishMaster.Infrastructure.Words;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EnglishMaster.Infrastructure;

public static class DependencyInjection
{
    private const string DefaultConnectionString =
        "Server=(localdb)\\mssqllocaldb;Database=EnglishMaster;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string? connectionString,
        IConfiguration configuration)
    {
        services.AddDbContext<EnglishMasterDbContext>(options =>
            options.UseSqlServer(string.IsNullOrWhiteSpace(connectionString)
                ? DefaultConnectionString
                : connectionString));

        services.AddScoped<ICategoryRepository, EfCategoryRepository>();
        services.AddScoped<IAnalyticsRepository, EfAnalyticsRepository>();
        services.AddScoped<ICertificateTemplateRepository, EfCertificateTemplateRepository>();
        services.AddScoped<IIssuedCertificateRepository, EfIssuedCertificateRepository>();
        services.AddScoped<ITagRepository, EfTagRepository>();
        services.AddScoped<IMediaRepository, EfMediaRepository>();
        services.AddScoped<IMediaStorageService, LocalMediaStorageService>();
        services.AddScoped<IWordRepository, EfWordRepository>();
        services.AddScoped<IPronunciationRepository, EfPronunciationRepository>();
        services.AddScoped<IMinimalPairRepository, EfMinimalPairRepository>();
        services.AddScoped<IGrammarTopicRepository, EfGrammarTopicRepository>();
        services.AddScoped<IGrammarRuleRepository, EfGrammarRuleRepository>();
        services.AddScoped<IGrammarExampleRepository, EfGrammarExampleRepository>();
        services.AddScoped<ILessonRepository, EfLessonRepository>();
        services.AddScoped<ILearningRecommendationRepository, EfLearningRecommendationRepository>();
        services.AddScoped<EfLearningGoalRepository>();
        services.AddScoped<ILearningGoalRepository>(provider => provider.GetRequiredService<EfLearningGoalRepository>());
        services.AddScoped<IDailyStudyPlanRepository>(provider => provider.GetRequiredService<EfLearningGoalRepository>());
        services.AddScoped<ILessonSectionRepository, EfLessonSectionRepository>();
        services.AddScoped<ICourseRepository, EfCourseRepository>();
        services.AddScoped<IBookRepository, EfBookRepository>();
        services.AddScoped<IBookChapterRepository, EfBookChapterRepository>();
        services.AddScoped<IQuizRepository, EfQuizRepository>();
        services.AddScoped<IQuizQuestionRepository, EfQuizQuestionRepository>();
        services.AddScoped<IQuizChoiceRepository, EfQuizChoiceRepository>();
        services.AddScoped<INotificationRepository, EfNotificationRepository>();
        services.AddScoped<INotificationService, EnglishMaster.Application.Features.Notifications.Commands.NotificationService>();
        services.AddScoped<IEmailMessageRepository, EfEmailMessageRepository>();
        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.AddScoped<DevelopmentEmailSender>();
        services.AddScoped<SmtpEmailSender>();
        services.AddScoped<IEmailSender, ConfiguredEmailSender>();
        services.AddScoped<IEmailProviderStatusService, EmailProviderStatusService>();
        services.Configure<EmailDeliveryWorkerOptions>(configuration.GetSection("EmailDeliveryWorker"));
        services.AddHostedService<EmailDeliveryWorker>();
        services.AddScoped<IContentQualityRepository, EfContentQualityRepository>();
        services.AddScoped<IContentQualityService, ContentQualityService>();
        services.AddScoped<IContentQualityRuleProvider, DefaultContentQualityRuleProvider>();
        services.AddScoped<IContentRevisionRepository, EfContentRevisionRepository>();
        services.AddScoped<IContentRevisionService, ContentRevisionService>();
        services.AddScoped<IContentSnapshotSerializer, ContentSnapshotSerializer>();
        services.AddScoped<IBulkOperationRepository, EfBulkOperationRepository>();
        services.AddScoped<IBulkOperationRunner, BulkOperationRunner>();
        services.AddScoped<IImportJobRepository, EfImportJobRepository>();
        services.AddScoped<IImportParser, ImportParser>();
        services.AddScoped<ImportJobService>();
        services.AddScoped<IImportValidationService>(provider => provider.GetRequiredService<ImportJobService>());
        services.AddScoped<IImportPreviewService>(provider => provider.GetRequiredService<ImportJobService>());
        services.AddScoped<IImportRunService>(provider => provider.GetRequiredService<ImportJobService>());
        services.AddScoped<IImportRollbackService>(provider => provider.GetRequiredService<ImportJobService>());
        services.AddScoped<IReportRepository, EfReportRepository>();
        services.AddScoped<IPublishJobRepository, EfPublishJobRepository>();
        services.AddScoped<IPublishTemplateRepository, EfPublishTemplateRepository>();
        services.AddScoped<IPublishedArtifactRepository, EfPublishedArtifactRepository>();
        services.AddScoped<IPublicSearchRepository, EfPublicSearchRepository>();
        services.AddScoped<IPracticeRepository, EfPracticeRepository>();
        services.AddScoped<IMotivationRepository, EfMotivationRepository>();
        services.AddScoped<ILearningReportRepository, EfLearningReportRepository>();
        services.AddScoped<ContentImportExportService>();
        services.AddScoped<IContentImportService>(provider =>
            provider.GetRequiredService<ContentImportExportService>());
        services.AddScoped<IContentExportService>(provider =>
            provider.GetRequiredService<ContentImportExportService>());
        services.AddScoped<IPublishingService, EnglishMaster.Application.Features.Publishing.PublishingService>();
        services.AddScoped<IPublishContentBuilder, BasicPublishContentBuilder>();
        services.AddScoped<IPublishFileStorage, LocalPublishFileStorage>();
        services.AddScoped<ISecurityService, EfSecurityService>();

        return services;
    }
}
