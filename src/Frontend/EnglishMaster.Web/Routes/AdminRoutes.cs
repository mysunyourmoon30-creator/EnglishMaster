namespace EnglishMaster.Web.Routes;

public static class AdminRoutes
{
    public const string Dashboard = "/admin";

    public const string Login = "/login";

    public const string Logout = "/logout";

    public static class Words
    {
        public const string Index = "/admin/words";
        public const string Create = "/admin/words/create";
        public const string DetailTemplate = "/admin/words/{id:guid}";
        public const string EditTemplate = "/admin/words/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/words/{id}";

        public static string Edit(Guid id) => $"/admin/words/{id}/edit";
    }

    public static class Categories
    {
        public const string Index = "/admin/categories";
        public const string Create = "/admin/categories/create";
        public const string DetailTemplate = "/admin/categories/{id:guid}";
        public const string EditTemplate = "/admin/categories/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/categories/{id}";

        public static string Edit(Guid id) => $"/admin/categories/{id}/edit";
    }

    public static class CertificateTemplates
    {
        public const string Index = "/admin/certificate-templates";
        public const string Create = "/admin/certificate-templates/create";
        public const string DetailTemplate = "/admin/certificate-templates/{id:guid}";
        public const string EditTemplate = "/admin/certificate-templates/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/certificate-templates/{id}";

        public static string Edit(Guid id) => $"/admin/certificate-templates/{id}/edit";
    }

    public static class Tags
    {
        public const string Index = "/admin/tags";
        public const string Create = "/admin/tags/create";
        public const string DetailTemplate = "/admin/tags/{id:guid}";
        public const string EditTemplate = "/admin/tags/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/tags/{id}";

        public static string Edit(Guid id) => $"/admin/tags/{id}/edit";
    }

    public static class Media
    {
        public const string Index = "/admin/media";
        public const string Create = "/admin/media/create";
        public const string DetailTemplate = "/admin/media/{id:guid}";
        public const string EditTemplate = "/admin/media/{id:guid}/edit";
        public const string Upload = "/admin/media/upload";

        public static string Detail(Guid id) => $"/admin/media/{id}";

        public static string Edit(Guid id) => $"/admin/media/{id}/edit";
    }

    public static class Pronunciations
    {
        public const string Index = "/admin/pronunciations";
        public const string Create = "/admin/pronunciations/create";
        public const string DetailTemplate = "/admin/pronunciations/{id:guid}";
        public const string EditTemplate = "/admin/pronunciations/{id:guid}/edit";

        public static string CreateForWord(Guid wordId) => $"/admin/pronunciations/create?wordId={wordId}";

        public static string Detail(Guid id) => $"/admin/pronunciations/{id}";

        public static string Edit(Guid id) => $"/admin/pronunciations/{id}/edit";
    }

    public static class GrammarTopics
    {
        public const string Index = "/admin/grammar-topics";
        public const string Create = "/admin/grammar-topics/create";
        public const string DetailTemplate = "/admin/grammar-topics/{id:guid}";
        public const string EditTemplate = "/admin/grammar-topics/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/grammar-topics/{id}";

        public static string Edit(Guid id) => $"/admin/grammar-topics/{id}/edit";
    }

    public static class GrammarRules
    {
        public const string Index = "/admin/grammar-rules";
        public const string Create = "/admin/grammar-rules/create";
        public const string DetailTemplate = "/admin/grammar-rules/{id:guid}";
        public const string EditTemplate = "/admin/grammar-rules/{id:guid}/edit";

        public static string IndexForTopic(Guid topicId) => $"/admin/grammar-rules?topicId={topicId}";

        public static string CreateForTopic(Guid topicId) => $"/admin/grammar-rules/create?topicId={topicId}";

        public static string Detail(Guid id) => $"/admin/grammar-rules/{id}";

        public static string Edit(Guid id) => $"/admin/grammar-rules/{id}/edit";
    }

    public static class GrammarExamples
    {
        public const string EditTemplate = "/admin/grammar-examples/{id:guid}/edit";

        public static string Edit(Guid id) => $"/admin/grammar-examples/{id}/edit";
    }

    public static class Lessons
    {
        public const string Index = "/admin/lessons";
        public const string Create = "/admin/lessons/create";
        public const string DetailTemplate = "/admin/lessons/{id:guid}";
        public const string EditTemplate = "/admin/lessons/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/lessons/{id}";

        public static string Edit(Guid id) => $"/admin/lessons/{id}/edit";
    }

    public static class Courses
    {
        public const string Index = "/admin/courses";
        public const string Create = "/admin/courses/create";
        public const string DetailTemplate = "/admin/courses/{id:guid}";
        public const string EditTemplate = "/admin/courses/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/courses/{id}";

        public static string Edit(Guid id) => $"/admin/courses/{id}/edit";
    }

    public static class Books
    {
        public const string Index = "/admin/books";
        public const string Create = "/admin/books/create";
        public const string DetailTemplate = "/admin/books/{id:guid}";
        public const string EditTemplate = "/admin/books/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/books/{id}";

        public static string Edit(Guid id) => $"/admin/books/{id}/edit";
    }

    public static class Quizzes
    {
        public const string Index = "/admin/quizzes";
        public const string Create = "/admin/quizzes/create";
        public const string DetailTemplate = "/admin/quizzes/{id:guid}";
        public const string EditTemplate = "/admin/quizzes/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/quizzes/{id}";

        public static string Edit(Guid id) => $"/admin/quizzes/{id}/edit";
    }

    public static class Reports
    {
        public const string Index = "/admin/reports";
    }

    public static class Analytics
    {
        public const string Index = "/admin/analytics";
    }

    public static class SystemHealth
    {
        public const string Index = "/admin/system-health";
    }

    public static class Notifications
    {
        public const string Mine = "/learn/notifications";
        public const string Index = "/admin/notifications";
    }

    public static class EmailMessages
    {
        public const string Index = "/admin/email-messages";
    }

    public static class ContentQuality
    {
        public const string Index = "/admin/content-quality";
        public const string Checks = "/admin/content-quality/checks";
        public const string CheckDetailTemplate = "/admin/content-quality/checks/{id:guid}";
        public const string Rules = "/admin/content-quality/rules";
        public const string CreateRule = "/admin/content-quality/rules/create";
        public const string EditRuleTemplate = "/admin/content-quality/rules/{id:guid}/edit";

        public static string CheckDetail(Guid id) => $"/admin/content-quality/checks/{id}";

        public static string EditRule(Guid id) => $"/admin/content-quality/rules/{id}/edit";
    }

    public static class ContentRevisions
    {
        public const string Index = "/admin/content-revisions";
        public const string DetailTemplate = "/admin/content-revisions/{id:guid}";
        public const string TimelineTemplate = "/admin/content-revisions/{contentType}/{contentId:guid}";
        public const string Restores = "/admin/content-revision-restores";
        public const string RestoreDetailTemplate = "/admin/content-revision-restores/{id:guid}";

        public static string Detail(Guid id) => $"/admin/content-revisions/{id}";

        public static string Timeline(string contentType, Guid contentId) => $"/admin/content-revisions/{contentType}/{contentId}";

        public static string RestoreDetail(Guid id) => $"/admin/content-revision-restores/{id}";
    }

    public static class BulkOperations
    {
        public const string Index = "/admin/bulk-operations";
        public const string Create = "/admin/bulk-operations/create";
        public const string DetailTemplate = "/admin/bulk-operations/{id:guid}";

        public static string Detail(Guid id) => $"/admin/bulk-operations/{id}";
    }

    public static class ImportJobs
    {
        public const string Index = "/admin/import-jobs";
        public const string Upload = "/admin/import-jobs/upload";
        public const string DetailTemplate = "/admin/import-jobs/{id:guid}";
        public const string RowsTemplate = "/admin/import-jobs/{id:guid}/rows";
        public const string ErrorsTemplate = "/admin/import-jobs/{id:guid}/errors";

        public static string Detail(Guid id) => $"/admin/import-jobs/{id}";

        public static string Rows(Guid id) => $"/admin/import-jobs/{id}/rows";

        public static string Errors(Guid id) => $"/admin/import-jobs/{id}/errors";
    }

    public static class Publishing
    {
        public static class Jobs
        {
            public const string Index = "/admin/publishing/jobs";
            public const string Create = "/admin/publishing/jobs/create";
            public const string DetailTemplate = "/admin/publishing/jobs/{id:guid}";

            public static string Detail(Guid id) => $"/admin/publishing/jobs/{id}";
        }

        public static class Templates
        {
            public const string Index = "/admin/publishing/templates";
            public const string Create = "/admin/publishing/templates/create";
            public const string EditTemplate = "/admin/publishing/templates/{id:guid}/edit";

            public static string Edit(Guid id) => $"/admin/publishing/templates/{id}/edit";
        }

        public static class Artifacts
        {
            public const string Index = "/admin/publishing/artifacts";
        }
    }

    public static class ImportExport
    {
        public const string Import = "/admin/import";
        public const string Export = "/admin/export";
    }

    public static class Users
    {
        public const string Index = "/admin/users";
        public const string Create = "/admin/users/create";
        public const string DetailTemplate = "/admin/users/{id:guid}";
        public const string EditTemplate = "/admin/users/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/users/{id}";

        public static string Edit(Guid id) => $"/admin/users/{id}/edit";
    }

    public static class Roles
    {
        public const string Index = "/admin/roles";
        public const string Create = "/admin/roles/create";
        public const string DetailTemplate = "/admin/roles/{id:guid}";
        public const string EditTemplate = "/admin/roles/{id:guid}/edit";

        public static string Detail(Guid id) => $"/admin/roles/{id}";

        public static string Edit(Guid id) => $"/admin/roles/{id}/edit";
    }

    public static class Permissions
    {
        public const string Index = "/admin/permissions";
    }
}
