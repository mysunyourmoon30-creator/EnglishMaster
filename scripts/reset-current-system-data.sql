SET XACT_ABORT ON;

BEGIN TRANSACTION;

DECLARE @Now datetimeoffset = SYSDATETIMEOFFSET();

PRINT N'1) Upsert current content categories';

DECLARE @CurrentCategories TABLE
(
    [Name] nvarchar(120) NOT NULL,
    [Slug] nvarchar(140) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [SortOrder] int NOT NULL
);

INSERT INTO @CurrentCategories ([Name], [Slug], [Description], [SortOrder])
VALUES
    (N'Vocabulary', N'vocabulary', N'Page / หน้า: Words, Search, Dictionary | Use / ใช้สำหรับ: คำศัพท์หลัก ความหมาย ตัวอย่าง และคลังคำสำหรับผู้เรียน', 10),
    (N'Grammar', N'grammar', N'Page / หน้า: Grammar, Lessons, Search | Use / ใช้สำหรับ: หัวข้อไวยากรณ์ กฎ โครงสร้าง และตัวอย่างประโยค', 20),
    (N'Pronunciation', N'pronunciation', N'Page / หน้า: Pronunciation, Practice, Words | Use / ใช้สำหรับ: IPA เสียงอ่าน ตำแหน่งปาก และแบบฝึกออกเสียง', 30),
    (N'Lessons', N'lessons', N'Page / หน้า: Lessons, Study Plan | Use / ใช้สำหรับ: บทเรียนรายวัน เนื้อหาตามระดับ และกิจกรรมเรียนเป็นขั้นตอน', 40),
    (N'Courses', N'courses', N'Page / หน้า: Courses, Study Plan | Use / ใช้สำหรับ: ชุดบทเรียนตามเป้าหมาย ระดับ และเส้นทางการเรียน', 50),
    (N'Books', N'books', N'Page / หน้า: Books, Publishing | Use / ใช้สำหรับ: หนังสือ บทอ่าน บท/Chapter และสื่อเผยแพร่', 60),
    (N'Quizzes', N'quizzes', N'Page / หน้า: Quizzes, Practice, Reports | Use / ใช้สำหรับ: แบบทดสอบ คำถาม คะแนนผ่าน และผลการทำข้อสอบ', 70),
    (N'Practice', N'practice', N'Page / หน้า: Practice, Study Plan | Use / ใช้สำหรับ: ฝึกทบทวน คิวสิ่งที่ควรฝึก และ session สำหรับผู้เรียน', 80),
    (N'Study Plan', N'study-plan', N'Page / หน้า: Study Plan, Goals | Use / ใช้สำหรับ: แผนเรียนวันนี้ งานที่ต้องทำ เป้าหมาย และความคืบหน้า', 90),
    (N'Reports', N'reports', N'Page / หน้า: Reports, Analytics | Use / ใช้สำหรับ: รายงานผลเรียน ความคืบหน้า สถิติ และ insight ผู้เรียน', 100),
    (N'Media', N'media', N'Page / หน้า: Media, Words, Lessons | Use / ใช้สำหรับ: รูปภาพ เสียง วิดีโอ และไฟล์ประกอบเนื้อหา', 110),
    (N'Publishing', N'publishing', N'Page / หน้า: Publishing, Books | Use / ใช้สำหรับ: template งานเผยแพร่ export และ published artifacts', 120),
    (N'Content Quality', N'content-quality', N'Page / หน้า: Content Quality, Content Revisions | Use / ใช้สำหรับ: ตรวจคุณภาพเนื้อหา ประวัติแก้ไข และการอนุมัติ', 130),
    (N'Motivation', N'motivation', N'Page / หน้า: Motivation, Goals | Use / ใช้สำหรับ: streak achievement กำลังใจ และแรงจูงใจในการเรียน', 140);

MERGE dbo.Categories AS Target
USING @CurrentCategories AS Source
    ON Target.Slug = Source.Slug
WHEN MATCHED THEN
    UPDATE SET
        Target.[Name] = Source.[Name],
        Target.[Description] = Source.[Description],
        Target.[SortOrder] = Source.[SortOrder],
        Target.[IsActive] = 1,
        Target.[UpdatedAt] = @Now
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([Id], [Name], [Slug], [Description], [SortOrder], [IsActive], [CreatedAt], [UpdatedAt])
    VALUES (NEWID(), Source.[Name], Source.[Slug], Source.[Description], Source.[SortOrder], 1, @Now, @Now);

PRINT N'2) Disable old demo/development seed content';

UPDATE dbo.Courses
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug IN (N'a1-starter-english');

UPDATE dbo.Books
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug IN (N'englishmaster-mvp-starter-book');

UPDATE dbo.Quizzes
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug IN (N'a1-starter-quiz');

UPDATE dbo.PublishTemplates
SET IsActive = 0, UpdatedAt = @Now
WHERE Slug IN (N'basic-html-template', N'basic-markdown-template');

PRINT N'3) Disable unused non-current categories without breaking linked content';

UPDATE Category
SET
    Category.IsActive = 0,
    Category.UpdatedAt = @Now
FROM dbo.Categories AS Category
WHERE NOT EXISTS
(
    SELECT 1
    FROM @CurrentCategories AS CurrentCategory
    WHERE CurrentCategory.Slug = Category.Slug
)
AND NOT EXISTS (SELECT 1 FROM dbo.Words AS Word WHERE Word.CategoryId = Category.Id)
AND NOT EXISTS (SELECT 1 FROM dbo.Lessons AS Lesson WHERE Lesson.CategoryId = Category.Id)
AND NOT EXISTS (SELECT 1 FROM dbo.Courses AS Course WHERE Course.CategoryId = Category.Id)
AND NOT EXISTS (SELECT 1 FROM dbo.Books AS Book WHERE Book.CategoryId = Category.Id)
AND NOT EXISTS (SELECT 1 FROM dbo.Quizzes AS Quiz WHERE Quiz.CategoryId = Category.Id);

COMMIT TRANSACTION;

SELECT N'Categories' AS [Section], [Name], [Slug], [Description], [SortOrder], [IsActive]
FROM dbo.Categories
ORDER BY [SortOrder], [Name];

SELECT N'Courses' AS [Section], [Title], [Slug], [Summary], [IsPublished], [IsActive]
FROM dbo.Courses
WHERE Slug IN (N'a1-daily-english-foundations', N'a1-starter-english')
ORDER BY [IsActive] DESC, [Title];

SELECT N'Books' AS [Section], [Title], [Slug], [Summary], [IsPublished], [IsActive]
FROM dbo.Books
WHERE Slug IN (N'a1-daily-english-starter-handbook', N'englishmaster-mvp-starter-book')
ORDER BY [IsActive] DESC, [Title];

SELECT N'Quizzes' AS [Section], [Title], [Slug], [Summary], [IsPublished], [IsActive]
FROM dbo.Quizzes
WHERE Slug IN (N'a1-daily-english-check', N'a1-starter-quiz')
ORDER BY [IsActive] DESC, [Title];
