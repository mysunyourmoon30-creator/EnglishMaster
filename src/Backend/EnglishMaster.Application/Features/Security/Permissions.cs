namespace EnglishMaster.Application.Features.Security;

public static class SecurityRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string ContentEditor = "ContentEditor";
    public const string Reviewer = "Reviewer";
    public const string Viewer = "Viewer";

    public static readonly string[] All =
    [
        SuperAdmin,
        Admin,
        ContentEditor,
        Reviewer,
        Viewer
    ];
}

public static class SecurityPermissionClaimTypes
{
    public const string Permission = "permission";
}

public static class Permissions
{
    public const string WordsRead = "words.read";
    public const string WordsCreate = "words.create";
    public const string WordsUpdate = "words.update";
    public const string WordsDelete = "words.delete";
    public const string CategoriesRead = "categories.read";
    public const string CategoriesCreate = "categories.create";
    public const string CategoriesUpdate = "categories.update";
    public const string CategoriesDelete = "categories.delete";
    public const string TagsRead = "tags.read";
    public const string TagsCreate = "tags.create";
    public const string TagsUpdate = "tags.update";
    public const string TagsDelete = "tags.delete";
    public const string MediaRead = "media.read";
    public const string MediaCreate = "media.create";
    public const string MediaUpdate = "media.update";
    public const string MediaDelete = "media.delete";
    public const string PronunciationRead = "pronunciation.read";
    public const string PronunciationCreate = "pronunciation.create";
    public const string PronunciationUpdate = "pronunciation.update";
    public const string PronunciationDelete = "pronunciation.delete";
    public const string GrammarRead = "grammar.read";
    public const string GrammarCreate = "grammar.create";
    public const string GrammarUpdate = "grammar.update";
    public const string GrammarDelete = "grammar.delete";
    public const string LessonsRead = "lessons.read";
    public const string LessonsCreate = "lessons.create";
    public const string LessonsUpdate = "lessons.update";
    public const string LessonsDelete = "lessons.delete";
    public const string CoursesRead = "courses.read";
    public const string CoursesCreate = "courses.create";
    public const string CoursesUpdate = "courses.update";
    public const string CoursesDelete = "courses.delete";
    public const string BooksRead = "books.read";
    public const string BooksCreate = "books.create";
    public const string BooksUpdate = "books.update";
    public const string BooksDelete = "books.delete";
    public const string QuizzesRead = "quizzes.read";
    public const string QuizzesCreate = "quizzes.create";
    public const string QuizzesUpdate = "quizzes.update";
    public const string QuizzesDelete = "quizzes.delete";
    public const string PublishingRead = "publishing.read";
    public const string PublishingCreate = "publishing.create";
    public const string PublishingUpdate = "publishing.update";
    public const string PublishingDelete = "publishing.delete";
    public const string PublishingRun = "publishing.run";
    public const string ReportsRead = "reports.read";
    public const string NotificationsRead = "notifications.read";
    public const string NotificationsManage = "notifications.manage";
    public const string EmailRead = "email.read";
    public const string EmailManage = "email.manage";
    public const string ContentQualityRead = "content-quality.read";
    public const string ContentQualityRun = "content-quality.run";
    public const string ContentQualityManage = "content-quality.manage";
    public const string ContentRevisionsRead = "content-revisions.read";
    public const string ContentRevisionsRestoreRequest = "content-revisions.restore.request";
    public const string ContentRevisionsRestoreApprove = "content-revisions.restore.approve";
    public const string ContentRevisionsManage = "content-revisions.manage";
    public const string BulkOperationsRead = "bulk-operations.read";
    public const string BulkOperationsRun = "bulk-operations.run";
    public const string BulkOperationsCancel = "bulk-operations.cancel";
    public const string ImportRead = "import.read";
    public const string ImportUpload = "import.upload";
    public const string ImportValidate = "import.validate";
    public const string ImportRun = "import.run";
    public const string ImportRollback = "import.rollback";
    public const string MotivationRead = "motivation.read";
    public const string AchievementsRead = "achievements.read";
    public const string AchievementsManage = "achievements.manage";
    public const string CertificatesRead = "certificates.read";
    public const string CertificatesManage = "certificates.manage";
    public const string UsersRead = "users.read";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";
    public const string RolesRead = "roles.read";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";
    public const string PermissionsRead = "permissions.read";
    public const string PermissionsUpdate = "permissions.update";
    public const string SystemHealthRead = "system-health.read";

    public static readonly PermissionDefinition[] All =
    [
        P(WordsRead, "Words Read", "Words"), P(WordsCreate, "Words Create", "Words"), P(WordsUpdate, "Words Update", "Words"), P(WordsDelete, "Words Delete", "Words"),
        P(CategoriesRead, "Categories Read", "Categories"), P(CategoriesCreate, "Categories Create", "Categories"), P(CategoriesUpdate, "Categories Update", "Categories"), P(CategoriesDelete, "Categories Delete", "Categories"),
        P(TagsRead, "Tags Read", "Tags"), P(TagsCreate, "Tags Create", "Tags"), P(TagsUpdate, "Tags Update", "Tags"), P(TagsDelete, "Tags Delete", "Tags"),
        P(MediaRead, "Media Read", "Media"), P(MediaCreate, "Media Create", "Media"), P(MediaUpdate, "Media Update", "Media"), P(MediaDelete, "Media Delete", "Media"),
        P(PronunciationRead, "Pronunciation Read", "Pronunciation"), P(PronunciationCreate, "Pronunciation Create", "Pronunciation"), P(PronunciationUpdate, "Pronunciation Update", "Pronunciation"), P(PronunciationDelete, "Pronunciation Delete", "Pronunciation"),
        P(GrammarRead, "Grammar Read", "Grammar"), P(GrammarCreate, "Grammar Create", "Grammar"), P(GrammarUpdate, "Grammar Update", "Grammar"), P(GrammarDelete, "Grammar Delete", "Grammar"),
        P(LessonsRead, "Lessons Read", "Lessons"), P(LessonsCreate, "Lessons Create", "Lessons"), P(LessonsUpdate, "Lessons Update", "Lessons"), P(LessonsDelete, "Lessons Delete", "Lessons"),
        P(CoursesRead, "Courses Read", "Courses"), P(CoursesCreate, "Courses Create", "Courses"), P(CoursesUpdate, "Courses Update", "Courses"), P(CoursesDelete, "Courses Delete", "Courses"),
        P(BooksRead, "Books Read", "Books"), P(BooksCreate, "Books Create", "Books"), P(BooksUpdate, "Books Update", "Books"), P(BooksDelete, "Books Delete", "Books"),
        P(QuizzesRead, "Quizzes Read", "Quizzes"), P(QuizzesCreate, "Quizzes Create", "Quizzes"), P(QuizzesUpdate, "Quizzes Update", "Quizzes"), P(QuizzesDelete, "Quizzes Delete", "Quizzes"),
        P(PublishingRead, "Publishing Read", "Publishing"), P(PublishingCreate, "Publishing Create", "Publishing"), P(PublishingUpdate, "Publishing Update", "Publishing"), P(PublishingDelete, "Publishing Delete", "Publishing"), P(PublishingRun, "Publishing Run", "Publishing"),
        P(ReportsRead, "Reports Read", "Reporting"),
        P(NotificationsRead, "Notifications Read", "Notifications"), P(NotificationsManage, "Notifications Manage", "Notifications"),
        P(EmailRead, "Email Read", "Email"), P(EmailManage, "Email Manage", "Email"),
        P(ContentQualityRead, "Content Quality Read", "Content Quality"), P(ContentQualityRun, "Content Quality Run", "Content Quality"), P(ContentQualityManage, "Content Quality Manage", "Content Quality"),
        P(ContentRevisionsRead, "Content Revisions Read", "Content Revisions"), P(ContentRevisionsRestoreRequest, "Content Revisions Restore Request", "Content Revisions"), P(ContentRevisionsRestoreApprove, "Content Revisions Restore Approve", "Content Revisions"), P(ContentRevisionsManage, "Content Revisions Manage", "Content Revisions"),
        P(BulkOperationsRead, "Bulk Operations Read", "Bulk Operations"), P(BulkOperationsRun, "Bulk Operations Run", "Bulk Operations"), P(BulkOperationsCancel, "Bulk Operations Cancel", "Bulk Operations"),
        P(ImportRead, "Import Read", "Import"), P(ImportUpload, "Import Upload", "Import"), P(ImportValidate, "Import Validate", "Import"), P(ImportRun, "Import Run", "Import"), P(ImportRollback, "Import Rollback", "Import"),
        P(MotivationRead, "Motivation Read", "Motivation"), P(AchievementsRead, "Achievements Read", "Achievements"), P(AchievementsManage, "Achievements Manage", "Achievements"),
        P(CertificatesRead, "Certificates Read", "Certificates"), P(CertificatesManage, "Certificates Manage", "Certificates"),
        P(UsersRead, "Users Read", "Administration"), P(UsersCreate, "Users Create", "Administration"), P(UsersUpdate, "Users Update", "Administration"), P(UsersDelete, "Users Delete", "Administration"),
        P(RolesRead, "Roles Read", "Administration"), P(RolesCreate, "Roles Create", "Administration"), P(RolesUpdate, "Roles Update", "Administration"), P(RolesDelete, "Roles Delete", "Administration"),
        P(PermissionsRead, "Permissions Read", "Administration"), P(PermissionsUpdate, "Permissions Update", "Administration"),
        P(SystemHealthRead, "System Health Read", "System Health")
    ];

    public static readonly string[] ContentRead =
    [
        WordsRead, CategoriesRead, TagsRead, MediaRead, PronunciationRead, GrammarRead, LessonsRead, CoursesRead, BooksRead, QuizzesRead
    ];

    public static readonly string[] ContentCreateUpdate =
    [
        WordsCreate, WordsUpdate, CategoriesCreate, CategoriesUpdate, TagsCreate, TagsUpdate, MediaCreate, MediaUpdate,
        PronunciationCreate, PronunciationUpdate, GrammarCreate, GrammarUpdate, LessonsCreate, LessonsUpdate,
        CoursesCreate, CoursesUpdate, BooksCreate, BooksUpdate, QuizzesCreate, QuizzesUpdate
    ];

    public static readonly string[] ContentDelete =
    [
        WordsDelete, CategoriesDelete, TagsDelete, MediaDelete, PronunciationDelete, GrammarDelete, LessonsDelete, CoursesDelete, BooksDelete, QuizzesDelete
    ];

    private static PermissionDefinition P(string key, string name, string module) =>
        new(key, name, module, $"Allows {name.ToLowerInvariant()} operations.");
}

public sealed record PermissionDefinition(
    string Key,
    string Name,
    string Module,
    string Description);
