using EnglishMaster.Domain.Books;
using EnglishMaster.Domain.Categories;
using EnglishMaster.Domain.Courses;
using EnglishMaster.Domain.Grammar;
using EnglishMaster.Domain.Lessons;
using EnglishMaster.Domain.Pronunciations;
using EnglishMaster.Domain.Publishing;
using EnglishMaster.Domain.Quizzes;
using EnglishMaster.Domain.Tags;
using EnglishMaster.Domain.Words;
using EnglishMaster.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EnglishMaster.Infrastructure.Security;

internal sealed class DevelopmentSeedDataSeeder(EnglishMasterDbContext dbContext, TimeProvider timeProvider)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var now = timeProvider.GetUtcNow();

        var vocabulary = await GetOrCreateCategoryAsync("Vocabulary", "Core word learning content.", 10, now, cancellationToken);
        var grammar = await GetOrCreateCategoryAsync("Grammar", "Grammar topics, rules, and examples.", 20, now, cancellationToken);
        var pronunciation = await GetOrCreateCategoryAsync("Pronunciation", "Pronunciation practice content.", 30, now, cancellationToken);

        var beginner = await GetOrCreateTagAsync("Beginner", "Starter-friendly content.", now, cancellationToken);
        var dailyEnglish = await GetOrCreateTagAsync("Daily English", "Common everyday English.", now, cancellationToken);
        var a1 = await GetOrCreateTagAsync("A1", "CEFR A1 level content.", now, cancellationToken);

        var hello = await GetOrCreateWordAsync(
            "hello",
            "heh-LOH",
            "heh-LOH",
            "heh-lo",
            "kham thakthai",
            "A greeting used when meeting someone.",
            PartOfSpeech.Interjection,
            CefrLevel.A1,
            "Hello, my name is Mina.",
            "thakthai lae bok chue",
            vocabulary.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        var book = await GetOrCreateWordAsync(
            "book",
            "buk",
            "buk",
            "buk",
            "nang sue",
            "A set of printed or digital pages.",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            "This is my English book.",
            "nang sue phasa angkrit khong chan",
            vocabulary.Id,
            [beginner.Id, a1.Id],
            now,
            cancellationToken);
        var learn = await GetOrCreateWordAsync(
            "learn",
            "lern",
            "lern",
            "lern",
            "rian ru",
            "To gain knowledge or skill.",
            PartOfSpeech.Verb,
            CefrLevel.A1,
            "I learn English every day.",
            "chan rian phasa angkrit thuk wan",
            vocabulary.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        var speak = await GetOrCreateWordAsync(
            "speak",
            "speek",
            "speek",
            "speak",
            "phut",
            "To say words using your voice.",
            PartOfSpeech.Verb,
            CefrLevel.A1,
            "We speak English in class.",
            "rao phut phasa angkrit nai chan rian",
            pronunciation.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        var daily = await GetOrCreateWordAsync(
            "daily",
            "DAY-lee",
            "DAY-lee",
            "day-lee",
            "pra cham wan",
            "Happening every day.",
            PartOfSpeech.Adjective,
            CefrLevel.A1,
            "Daily practice helps you improve.",
            "fan thuk wan chuai hai keng khuen",
            vocabulary.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);

        var helloPronunciation = await GetOrCreatePronunciationAsync(
            hello.Id,
            "heh-LOH",
            "heh-LOH",
            "heh-lo",
            "hel-lo",
            "second syllable",
            "Relax the mouth and open on the final vowel.",
            "Keep the tongue low for the final sound.",
            "Do not drop the final vowel.",
            "Repeat slowly, then at natural speed.",
            now,
            cancellationToken);
        await GetOrCreatePronunciationAsync(
            book.Id,
            "buk",
            "buk",
            "buk",
            "book",
            "one syllable",
            "Round the lips slightly.",
            "Keep the tongue relaxed.",
            "Avoid saying a long oo sound.",
            "Listen for the short vowel.",
            now,
            cancellationToken);
        await GetOrCreatePronunciationAsync(
            speak.Id,
            "speek",
            "speek",
            "speak",
            "speak",
            "one syllable",
            "Start with a clear s sound.",
            "Hold the long vowel.",
            "Avoid adding a vowel before s.",
            "Practice speak, speaking, speaker.",
            now,
            cancellationToken);

        var presentSimple = await GetOrCreateGrammarTopicAsync(
            "Present Simple",
            "Use present simple for routines, facts, and habits.",
            CefrLevel.A1,
            10,
            now,
            cancellationToken);
        var articles = await GetOrCreateGrammarTopicAsync(
            "Articles",
            "Use a, an, and the with nouns.",
            CefrLevel.A1,
            20,
            now,
            cancellationToken);

        var habitRule = await GetOrCreateGrammarRuleAsync(
            presentSimple.Id,
            "Present simple for habits",
            "Use the base verb for I, you, we, and they. Add s or es for he, she, and it.",
            "chai kap kitjawat rue khwam jing thua pai",
            "Use present simple for routines and facts.",
            "Subject + base verb",
            "Do not add s after I, you, we, or they.",
            "Use does with he, she, and it in questions.",
            10,
            [learn.Id, daily.Id],
            now,
            cancellationToken);
        var articleRule = await GetOrCreateGrammarRuleAsync(
            articles.Id,
            "A and an with singular nouns",
            "Use a before a consonant sound and an before a vowel sound.",
            "chai a/an kap kham nam ekaphot",
            "Choose a or an by sound, not only spelling.",
            "a/an + singular noun",
            "Do not use a or an with plural nouns.",
            "Say an apple, but a book.",
            10,
            [book.Id],
            now,
            cancellationToken);

        var greetingsLesson = await GetOrCreateLessonAsync(
            "Daily Greetings",
            "Practice simple greetings and introductions.",
            "A short starter lesson for saying hello and speaking politely.",
            CefrLevel.A1,
            vocabulary.Id,
            10,
            10,
            [hello.Id, speak.Id],
            [habitRule.Id],
            now,
            cancellationToken);
        await GetOrCreateLessonSectionAsync(
            greetingsLesson.Id,
            "Greeting practice",
            "Say hello, introduce yourself, and answer simple questions.",
            SectionType.Speaking,
            10,
            now,
            cancellationToken);

        var articlesLesson = await GetOrCreateLessonAsync(
            "Using A and An",
            "Learn a simple article rule.",
            "Practice choosing a or an before common nouns.",
            CefrLevel.A1,
            grammar.Id,
            12,
            20,
            [book.Id, learn.Id],
            [articleRule.Id],
            now,
            cancellationToken);
        await GetOrCreateLessonSectionAsync(
            articlesLesson.Id,
            "Article examples",
            "Use a before book and an before apple.",
            SectionType.Grammar,
            10,
            now,
            cancellationToken);

        var starterCourse = await GetOrCreateCourseAsync(
            "A1 Starter English",
            "A tiny demo course for MVP testing.",
            "Combines vocabulary, grammar, and pronunciation starter content.",
            CefrLevel.A1,
            vocabulary.Id,
            25,
            10,
            [greetingsLesson.Id, articlesLesson.Id],
            now,
            cancellationToken);

        var starterBook = await GetOrCreateBookAsync(
            "EnglishMaster MVP Starter Book",
            "Demo edition",
            "A short book that links the starter lessons.",
            "A development seed book for validating book, chapter, lesson, and course relationships.",
            CefrLevel.A1,
            vocabulary.Id,
            starterCourse.Id,
            "EnglishMaster Team",
            "Development",
            "1.0",
            8,
            10,
            now,
            cancellationToken);
        await GetOrCreateBookChapterAsync(
            starterBook.Id,
            "Starter Chapter",
            "A small chapter for MVP smoke testing.",
            "This chapter links the greeting and article lessons.",
            10,
            [greetingsLesson.Id, articlesLesson.Id],
            now,
            cancellationToken);

        await GetOrCreateQuizAsync(
            "A1 Starter Quiz",
            "A small quiz for checking the MVP quiz flow.",
            "Includes vocabulary and grammar questions.",
            CefrLevel.A1,
            vocabulary.Id,
            greetingsLesson.Id,
            starterCourse.Id,
            starterBook.Id,
            10,
            70,
            10,
            hello.Id,
            articleRule.Id,
            helloPronunciation.Id,
            now,
            cancellationToken);

        await GetOrCreatePublishTemplateAsync(
            "Basic HTML Template",
            "Development HTML template for smoke testing publishing.",
            PublishFormat.Html,
            "<!doctype html><html><head><meta charset=\"utf-8\"><title>{{title}}</title></head><body><main>{{content}}</main></body></html>",
            true,
            now,
            cancellationToken);
        await GetOrCreatePublishTemplateAsync(
            "Basic Markdown Template",
            "Development Markdown template for smoke testing publishing.",
            PublishFormat.Markdown,
            "# {{title}}\n\n{{content}}\n",
            true,
            now,
            cancellationToken);
    }

    private async Task<Category> GetOrCreateCategoryAsync(
        string name,
        string description,
        int sortOrder,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Category.GenerateSlug(name);
        var category = await dbContext.Categories.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (category is not null)
        {
            return category;
        }

        category = Category.Create(name, description, sortOrder, now);
        dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return category;
    }

    private async Task<Tag> GetOrCreateTagAsync(
        string name,
        string description,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Tag.GenerateSlug(name);
        var tag = await dbContext.Tags.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (tag is not null)
        {
            return tag;
        }

        tag = Tag.Create(name, description, now);
        dbContext.Tags.Add(tag);
        await dbContext.SaveChangesAsync(cancellationToken);
        return tag;
    }

    private async Task<Word> GetOrCreateWordAsync(
        string text,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string meaningTh,
        string meaningEn,
        PartOfSpeech partOfSpeech,
        CefrLevel cefrLevel,
        string exampleEn,
        string exampleTh,
        Guid categoryId,
        IReadOnlyCollection<Guid> tagIds,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Word.GenerateSlug(text);
        var word = await dbContext.Words.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (word is not null)
        {
            return word;
        }

        word = Word.Create(
            text,
            ipaUk,
            ipaUs,
            thaiReading,
            meaningTh,
            meaningEn,
            partOfSpeech,
            cefrLevel,
            exampleEn,
            exampleTh,
            categoryId,
            tagIds,
            imageMediaId: null,
            audioMediaId: null,
            now);
        dbContext.Words.Add(word);
        await dbContext.SaveChangesAsync(cancellationToken);
        return word;
    }

    private async Task<Pronunciation> GetOrCreatePronunciationAsync(
        Guid wordId,
        string ipaUk,
        string ipaUs,
        string thaiReading,
        string syllables,
        string stressPattern,
        string mouthPosition,
        string tonguePosition,
        string commonMistake,
        string practiceNote,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var pronunciation = await dbContext.Pronunciations.SingleOrDefaultAsync(item => item.WordId == wordId, cancellationToken);
        if (pronunciation is not null)
        {
            return pronunciation;
        }

        pronunciation = Pronunciation.Create(
            wordId,
            ipaUk,
            ipaUs,
            thaiReading,
            syllables,
            stressPattern,
            mouthPosition,
            tonguePosition,
            commonMistake,
            practiceNote,
            audioSlowMediaId: null,
            audioNormalMediaId: null,
            mouthImageMediaId: null,
            now);
        dbContext.Pronunciations.Add(pronunciation);
        await dbContext.SaveChangesAsync(cancellationToken);
        return pronunciation;
    }

    private async Task<GrammarTopic> GetOrCreateGrammarTopicAsync(
        string title,
        string summary,
        CefrLevel cefrLevel,
        int sortOrder,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = GrammarTopic.GenerateSlug(title);
        var topic = await dbContext.GrammarTopics.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (topic is not null)
        {
            return topic;
        }

        topic = GrammarTopic.Create(title, summary, cefrLevel, sortOrder, now);
        dbContext.GrammarTopics.Add(topic);
        await dbContext.SaveChangesAsync(cancellationToken);
        return topic;
    }

    private async Task<GrammarRule> GetOrCreateGrammarRuleAsync(
        Guid grammarTopicId,
        string title,
        string ruleText,
        string explanationTh,
        string explanationEn,
        string structurePattern,
        string commonMistake,
        string correctUsageNote,
        int sortOrder,
        IReadOnlyCollection<Guid> relatedWordIds,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = GrammarRule.GenerateSlug(title);
        var rule = await dbContext.GrammarRules.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (rule is not null)
        {
            return rule;
        }

        rule = GrammarRule.Create(
            grammarTopicId,
            title,
            ruleText,
            explanationTh,
            explanationEn,
            structurePattern,
            commonMistake,
            correctUsageNote,
            sortOrder,
            now);
        foreach (var wordId in relatedWordIds)
        {
            rule.AddRelatedWord(wordId, now);
        }

        dbContext.GrammarRules.Add(rule);
        await dbContext.SaveChangesAsync(cancellationToken);
        return rule;
    }

    private async Task<Lesson> GetOrCreateLessonAsync(
        string title,
        string summary,
        string description,
        CefrLevel cefrLevel,
        Guid categoryId,
        int estimatedMinutes,
        int sortOrder,
        IReadOnlyCollection<Guid> wordIds,
        IReadOnlyCollection<Guid> grammarRuleIds,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Lesson.GenerateSlug(title);
        var lesson = await dbContext.Lessons.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (lesson is not null)
        {
            return lesson;
        }

        lesson = Lesson.Create(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId: null,
            estimatedMinutes,
            sortOrder,
            now);
        foreach (var wordId in wordIds)
        {
            lesson.AddWord(wordId, sortOrder, now);
        }

        foreach (var grammarRuleId in grammarRuleIds)
        {
            lesson.AddGrammarRule(grammarRuleId, sortOrder, now);
        }

        lesson.Publish(now);
        dbContext.Lessons.Add(lesson);
        await dbContext.SaveChangesAsync(cancellationToken);
        return lesson;
    }

    private async Task GetOrCreateLessonSectionAsync(
        Guid lessonId,
        string title,
        string contentMarkdown,
        SectionType sectionType,
        int sortOrder,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var exists = await dbContext.LessonSections.AnyAsync(
            item => item.LessonId == lessonId && item.Title == title,
            cancellationToken);
        if (exists)
        {
            return;
        }

        dbContext.LessonSections.Add(LessonSection.Create(
            lessonId,
            title,
            contentMarkdown,
            sectionType,
            mediaId: null,
            sortOrder,
            now));
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<Course> GetOrCreateCourseAsync(
        string title,
        string summary,
        string description,
        CefrLevel cefrLevel,
        Guid categoryId,
        int estimatedMinutes,
        int sortOrder,
        IReadOnlyCollection<Guid> lessonIds,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Course.GenerateSlug(title);
        var course = await dbContext.Courses.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (course is not null)
        {
            return course;
        }

        course = Course.Create(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            thumbnailMediaId: null,
            estimatedMinutes,
            sortOrder,
            now);
        var relationSortOrder = 10;
        foreach (var lessonId in lessonIds)
        {
            course.AddLesson(lessonId, relationSortOrder, isRequired: true, now);
            relationSortOrder += 10;
        }

        course.Publish(now);
        dbContext.Courses.Add(course);
        await dbContext.SaveChangesAsync(cancellationToken);
        return course;
    }

    private async Task<Book> GetOrCreateBookAsync(
        string title,
        string subtitle,
        string summary,
        string description,
        CefrLevel cefrLevel,
        Guid categoryId,
        Guid courseId,
        string authorName,
        string edition,
        string version,
        int estimatedPages,
        int sortOrder,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Book.GenerateSlug(title);
        var book = await dbContext.Books.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (book is not null)
        {
            return book;
        }

        book = Book.Create(
            title,
            subtitle,
            summary,
            description,
            cefrLevel,
            categoryId,
            coverMediaId: null,
            courseId,
            authorName,
            edition,
            version,
            estimatedPages,
            sortOrder,
            now);
        book.Publish(now);
        dbContext.Books.Add(book);
        await dbContext.SaveChangesAsync(cancellationToken);
        return book;
    }

    private async Task GetOrCreateBookChapterAsync(
        Guid bookId,
        string title,
        string summary,
        string contentMarkdown,
        int sortOrder,
        IReadOnlyCollection<Guid> lessonIds,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = BookChapter.GenerateSlug(title);
        var chapter = await dbContext.BookChapters.SingleOrDefaultAsync(
            item => item.BookId == bookId && item.Slug == slug,
            cancellationToken);
        if (chapter is not null)
        {
            return;
        }

        chapter = BookChapter.Create(bookId, title, summary, contentMarkdown, sortOrder, now);
        var relationSortOrder = 10;
        foreach (var lessonId in lessonIds)
        {
            chapter.AddLesson(lessonId, relationSortOrder, now);
            relationSortOrder += 10;
        }

        dbContext.BookChapters.Add(chapter);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task GetOrCreateQuizAsync(
        string title,
        string summary,
        string description,
        CefrLevel cefrLevel,
        Guid categoryId,
        Guid lessonId,
        Guid courseId,
        Guid bookId,
        int timeLimitMinutes,
        int passingScore,
        int sortOrder,
        Guid helloWordId,
        Guid articleRuleId,
        Guid pronunciationId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = Quiz.GenerateSlug(title);
        var exists = await dbContext.Quizzes.AnyAsync(item => item.Slug == slug, cancellationToken);
        if (exists)
        {
            return;
        }

        var quiz = Quiz.Create(
            title,
            summary,
            description,
            cefrLevel,
            categoryId,
            lessonId,
            courseId,
            bookId,
            timeLimitMinutes,
            passingScore,
            sortOrder,
            now);
        quiz.Publish(now);
        dbContext.Quizzes.Add(quiz);

        var greetingQuestion = QuizQuestion.Create(
            quiz.Id,
            "Which word is a greeting?",
            QuizQuestionType.SingleChoice,
            "hello pen kham thakthai",
            "Hello is a greeting.",
            1,
            10,
            helloWordId,
            grammarRuleId: null,
            pronunciationId,
            now);
        greetingQuestion.AddChoice("hello", isCorrect: true, "kham thakthai", "Correct.", 10, now);
        greetingQuestion.AddChoice("book", isCorrect: false, "mai chai kham thakthai", "Book is a noun.", 20, now);
        greetingQuestion.AddChoice("daily", isCorrect: false, "mai chai kham thakthai", "Daily describes frequency.", 30, now);
        dbContext.QuizQuestions.Add(greetingQuestion);

        var articleQuestion = QuizQuestion.Create(
            quiz.Id,
            "Choose the correct phrase.",
            QuizQuestionType.SingleChoice,
            "a book thuk tong",
            "Use a before the consonant sound in book.",
            1,
            20,
            wordId: null,
            grammarRuleId: articleRuleId,
            pronunciationId: null,
            now);
        articleQuestion.AddChoice("a book", isCorrect: true, "thuk tong", "Correct.", 10, now);
        articleQuestion.AddChoice("an book", isCorrect: false, "mai thuk", "Use an before a vowel sound.", 20, now);
        dbContext.QuizQuestions.Add(articleQuestion);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task GetOrCreatePublishTemplateAsync(
        string name,
        string description,
        PublishFormat format,
        string templateContent,
        bool isDefault,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var slug = PublishTemplate.GenerateSlug(name);
        var exists = await dbContext.PublishTemplates.AnyAsync(item => item.Slug == slug, cancellationToken);
        if (exists)
        {
            return;
        }

        dbContext.PublishTemplates.Add(PublishTemplate.Create(
            name,
            description,
            format,
            templateContent,
            isDefault,
            now));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
