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

        var vocabulary = await GetOrCreateCategoryAsync(
            "Vocabulary",
            "Page / หน้า: Words, Search, Dictionary | Use / ใช้สำหรับ: คำศัพท์หลัก ความหมาย ตัวอย่าง และคลังคำสำหรับผู้เรียน",
            10,
            now,
            cancellationToken);
        var grammar = await GetOrCreateCategoryAsync(
            "Grammar",
            "Page / หน้า: Grammar, Lessons, Search | Use / ใช้สำหรับ: หัวข้อไวยากรณ์ กฎ โครงสร้าง และตัวอย่างประโยค",
            20,
            now,
            cancellationToken);
        var pronunciation = await GetOrCreateCategoryAsync(
            "Pronunciation",
            "Page / หน้า: Pronunciation, Practice, Words | Use / ใช้สำหรับ: IPA เสียงอ่าน ตำแหน่งปาก และแบบฝึกออกเสียง",
            30,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Lessons",
            "Page / หน้า: Lessons, Study Plan | Use / ใช้สำหรับ: บทเรียนรายวัน เนื้อหาตามระดับ และกิจกรรมเรียนเป็นขั้นตอน",
            40,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Courses",
            "Page / หน้า: Courses, Study Plan | Use / ใช้สำหรับ: ชุดบทเรียนตามเป้าหมาย ระดับ และเส้นทางการเรียน",
            50,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Books",
            "Page / หน้า: Books, Publishing | Use / ใช้สำหรับ: หนังสือ บทอ่าน บท/Chapter และสื่อเผยแพร่",
            60,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Quizzes",
            "Page / หน้า: Quizzes, Practice, Reports | Use / ใช้สำหรับ: แบบทดสอบ คำถาม คะแนนผ่าน และผลการทำข้อสอบ",
            70,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Practice",
            "Page / หน้า: Practice, Study Plan | Use / ใช้สำหรับ: ฝึกทบทวน คิวสิ่งที่ควรฝึก และ session สำหรับผู้เรียน",
            80,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Study Plan",
            "Page / หน้า: Study Plan, Goals | Use / ใช้สำหรับ: แผนเรียนวันนี้ งานที่ต้องทำ เป้าหมาย และความคืบหน้า",
            90,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Reports",
            "Page / หน้า: Reports, Analytics | Use / ใช้สำหรับ: รายงานผลเรียน ความคืบหน้า สถิติ และ insight ผู้เรียน",
            100,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Media",
            "Page / หน้า: Media, Words, Lessons | Use / ใช้สำหรับ: รูปภาพ เสียง วิดีโอ และไฟล์ประกอบเนื้อหา",
            110,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Publishing",
            "Page / หน้า: Publishing, Books | Use / ใช้สำหรับ: template งานเผยแพร่ export และ published artifacts",
            120,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Content Quality",
            "Page / หน้า: Content Quality, Content Revisions | Use / ใช้สำหรับ: ตรวจคุณภาพเนื้อหา ประวัติแก้ไข และการอนุมัติ",
            130,
            now,
            cancellationToken);
        await GetOrCreateCategoryAsync(
            "Motivation",
            "Page / หน้า: Motivation, Goals | Use / ใช้สำหรับ: streak achievement กำลังใจ และแรงจูงใจในการเรียน",
            140,
            now,
            cancellationToken);

        var beginner = await GetOrCreateTagAsync("Beginner", "For new learners who need clear, simple English foundations.", now, cancellationToken);
        var dailyEnglish = await GetOrCreateTagAsync("Daily English", "Useful English for greetings, study, class, home, and daily life.", now, cancellationToken);
        var a1 = await GetOrCreateTagAsync("A1", "CEFR A1 learner content with short words, simple grammar, and familiar situations.", now, cancellationToken);

        var hello = await GetOrCreateWordAsync(
            "hello",
            "heh-LOH",
            "heh-LOH",
            "เฮล-โล",
            "คำทักทาย",
            "A greeting used when meeting someone.",
            PartOfSpeech.Interjection,
            CefrLevel.A1,
            "Hello, my name is Mina.",
            "ใช้ทักทายและเริ่มแนะนำตัว",
            vocabulary.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        var book = await GetOrCreateWordAsync(
            "book",
            "buk",
            "buk",
            "บุค",
            "หนังสือ",
            "A set of printed or digital pages.",
            PartOfSpeech.Noun,
            CefrLevel.A1,
            "This is my English book.",
            "นี่คือหนังสือภาษาอังกฤษของฉัน",
            vocabulary.Id,
            [beginner.Id, a1.Id],
            now,
            cancellationToken);
        var learn = await GetOrCreateWordAsync(
            "learn",
            "lern",
            "lern",
            "เลิร์น",
            "เรียนรู้",
            "To gain knowledge or skill.",
            PartOfSpeech.Verb,
            CefrLevel.A1,
            "I learn English every day.",
            "ฉันเรียนภาษาอังกฤษทุกวัน",
            vocabulary.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        var speak = await GetOrCreateWordAsync(
            "speak",
            "speek",
            "speek",
            "สพีค",
            "พูด",
            "To say words using your voice.",
            PartOfSpeech.Verb,
            CefrLevel.A1,
            "We speak English in class.",
            "พวกเราพูดภาษาอังกฤษในห้องเรียน",
            pronunciation.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        var daily = await GetOrCreateWordAsync(
            "daily",
            "DAY-lee",
            "DAY-lee",
            "เดย์-ลี",
            "ประจำวัน",
            "Happening every day.",
            PartOfSpeech.Adjective,
            CefrLevel.A1,
            "Daily practice helps you improve.",
            "การฝึกทุกวันช่วยให้พัฒนาได้ดีขึ้น",
            vocabulary.Id,
            [beginner.Id, dailyEnglish.Id, a1.Id],
            now,
            cancellationToken);
        await SeedCoreLearnerWordsAsync(vocabulary.Id, [beginner.Id, dailyEnglish.Id, a1.Id], now, cancellationToken);

        var helloPronunciation = await GetOrCreatePronunciationAsync(
            hello.Id,
            "heh-LOH",
            "heh-LOH",
            "เฮล-โล",
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
            "บุค",
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
            "สพีค",
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
            "ใช้กับกิจวัตร นิสัย และความจริงทั่วไป",
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
            "ใช้ a/an กับคำนามเอกพจน์ตามเสียงขึ้นต้น",
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
            "A practical first lesson for greeting people, introducing yourself, and speaking politely.",
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
            "Learn how to choose a or an before singular nouns.",
            "Practice choosing a or an by sound, with simple everyday examples.",
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
            "A1 Daily English Foundations",
            "A practical beginner course for everyday English.",
            "Builds a small foundation with greetings, classroom words, simple article grammar, and pronunciation practice.",
            CefrLevel.A1,
            vocabulary.Id,
            25,
            10,
            [greetingsLesson.Id, articlesLesson.Id],
            now,
            cancellationToken);

        var starterBook = await GetOrCreateBookAsync(
            "A1 Daily English Starter Handbook",
            "Foundation edition",
            "A short handbook that supports the A1 Daily English Foundations course.",
            "A learner-friendly guide with vocabulary, grammar notes, examples, and linked starter lessons.",
            CefrLevel.A1,
            vocabulary.Id,
            starterCourse.Id,
            "EnglishMaster Team",
            "Foundation",
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
            "A1 Daily English Check",
            "A short quiz for checking beginner vocabulary and grammar.",
            "Includes greeting vocabulary, article grammar, and pronunciation-aware questions.",
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
            "Standard HTML Lesson Template",
            "Clean HTML template for publishing learner-facing English content.",
            PublishFormat.Html,
            "<!doctype html><html><head><meta charset=\"utf-8\"><title>{{title}}</title></head><body><main>{{content}}</main></body></html>",
            true,
            now,
            cancellationToken);
        await GetOrCreatePublishTemplateAsync(
            "Standard Markdown Lesson Template",
            "Clean Markdown template for exporting learner-facing English content.",
            PublishFormat.Markdown,
            "# {{title}}\n\n{{content}}\n",
            true,
            now,
            cancellationToken);

        await DeactivateLegacySeedContentAsync(now, cancellationToken);
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
            if (category.Name != name ||
                category.Description != description ||
                category.SortOrder != sortOrder ||
                !category.IsActive)
            {
                category.Update(name, description, sortOrder, isActive: true, now);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

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
            if (tag.Name != name || tag.Description != description || !tag.IsActive)
            {
                tag.Update(name, description, isActive: true, now);
                await dbContext.SaveChangesAsync(cancellationToken);
            }

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
            word.Update(
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
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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

    private async Task SeedCoreLearnerWordsAsync(
        Guid categoryId,
        IReadOnlyCollection<Guid> tagIds,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        CoreLearnerWord[] words =
        [
            new("apple", "AP-ul", "AP-ul", "ap-ple", "apple; a fruit", "A round fruit that is often red or green.", PartOfSpeech.Noun, CefrLevel.A1, "I eat an apple every morning.", "I eat an apple every morning."),
            new("banana", "buh-NAN-uh", "buh-NAN-uh", "ba-na-na", "banana; a yellow fruit", "A long yellow fruit.", PartOfSpeech.Noun, CefrLevel.A1, "She has a banana for breakfast.", "She has a banana for breakfast."),
            new("water", "WAW-tuh", "WAH-ter", "wa-ter", "water; drinking liquid", "A clear liquid that people drink.", PartOfSpeech.Noun, CefrLevel.A1, "Please drink some water.", "Please drink some water."),
            new("food", "food", "food", "food", "food; something we eat", "Things that people or animals eat.", PartOfSpeech.Noun, CefrLevel.A1, "This food is delicious.", "This food is delicious."),
            new("house", "hows", "hows", "house", "house; home building", "A building where people live.", PartOfSpeech.Noun, CefrLevel.A1, "My house is near the school.", "My house is near the school."),
            new("room", "room", "room", "room", "room; part of a building", "A part of a house or building.", PartOfSpeech.Noun, CefrLevel.A1, "This room is clean.", "This room is clean."),
            new("school", "skool", "skool", "school", "school; place to learn", "A place where students learn.", PartOfSpeech.Noun, CefrLevel.A1, "The children go to school.", "The children go to school."),
            new("teacher", "TEE-chuh", "TEE-cher", "tea-cher", "teacher; person who teaches", "A person who helps students learn.", PartOfSpeech.Noun, CefrLevel.A1, "Our teacher speaks slowly.", "Our teacher speaks slowly."),
            new("student", "STYOO-dunt", "STOO-dent", "stu-dent", "student; learner", "A person who studies.", PartOfSpeech.Noun, CefrLevel.A1, "Every student needs practice.", "Every student needs practice."),
            new("friend", "frend", "frend", "friend", "friend; person you like", "A person you know and like.", PartOfSpeech.Noun, CefrLevel.A1, "My friend studies English with me.", "My friend studies English with me."),
            new("family", "FAM-uh-lee", "FAM-uh-lee", "fam-i-ly", "family; relatives", "People related to you, such as parents and children.", PartOfSpeech.Noun, CefrLevel.A1, "My family eats dinner together.", "My family eats dinner together."),
            new("mother", "MUTH-uh", "MUTH-er", "moth-er", "mother; female parent", "A female parent.", PartOfSpeech.Noun, CefrLevel.A1, "My mother likes tea.", "My mother likes tea."),
            new("father", "FAH-thuh", "FAH-ther", "fa-ther", "father; male parent", "A male parent.", PartOfSpeech.Noun, CefrLevel.A1, "His father works at home.", "His father works at home."),
            new("morning", "MAW-ning", "MOR-ning", "mor-ning", "morning; early part of day", "The first part of the day.", PartOfSpeech.Noun, CefrLevel.A1, "I study in the morning.", "I study in the morning."),
            new("night", "nyt", "nyt", "night", "night; dark time", "The dark part of the day.", PartOfSpeech.Noun, CefrLevel.A1, "We sleep at night.", "We sleep at night."),
            new("today", "tuh-DAY", "tuh-DAY", "to-day", "today; this day", "This day.", PartOfSpeech.Adverb, CefrLevel.A1, "Today I will learn five words.", "Today I will learn five words."),
            new("tomorrow", "tuh-MOR-oh", "tuh-MAR-oh", "to-mor-row", "tomorrow; next day", "The day after today.", PartOfSpeech.Adverb, CefrLevel.A1, "Tomorrow we have a quiz.", "Tomorrow we have a quiz."),
            new("yesterday", "YES-tuh-day", "YES-ter-day", "yes-ter-day", "yesterday; previous day", "The day before today.", PartOfSpeech.Adverb, CefrLevel.A1, "Yesterday I reviewed grammar.", "Yesterday I reviewed grammar."),
            new("read", "reed", "reed", "read", "read; look at words", "To look at and understand written words.", PartOfSpeech.Verb, CefrLevel.A1, "Please read the sentence.", "Please read the sentence."),
            new("write", "ryt", "ryt", "write", "write; make words", "To make words on paper or a screen.", PartOfSpeech.Verb, CefrLevel.A1, "Write your name here.", "Write your name here."),
            new("listen", "LIS-un", "LIS-un", "lis-ten", "listen; hear with attention", "To pay attention to sound.", PartOfSpeech.Verb, CefrLevel.A1, "Listen to the word again.", "Listen to the word again."),
            new("repeat", "ri-PEET", "ri-PEET", "re-peat", "repeat; say again", "To say or do something again.", PartOfSpeech.Verb, CefrLevel.A1, "Repeat after the teacher.", "Repeat after the teacher."),
            new("open", "OH-pun", "OH-pen", "o-pen", "open; not closed", "To move something so it is not closed.", PartOfSpeech.Verb, CefrLevel.A1, "Open your book.", "Open your book."),
            new("close", "klohz", "klohz", "close", "close; shut", "To shut something.", PartOfSpeech.Verb, CefrLevel.A1, "Close the door, please.", "Close the door, please."),
            new("start", "staht", "start", "start", "start; begin", "To begin doing something.", PartOfSpeech.Verb, CefrLevel.A1, "Start the lesson now.", "Start the lesson now."),
            new("finish", "FIN-ish", "FIN-ish", "fin-ish", "finish; complete", "To complete something.", PartOfSpeech.Verb, CefrLevel.A1, "Finish the practice today.", "Finish the practice today."),
            new("like", "lyk", "lyk", "like", "like; enjoy", "To enjoy something or someone.", PartOfSpeech.Verb, CefrLevel.A1, "I like English songs.", "I like English songs."),
            new("want", "wont", "wahnt", "want", "want; wish to have", "To wish for something.", PartOfSpeech.Verb, CefrLevel.A1, "I want to speak English.", "I want to speak English."),
            new("need", "need", "need", "need", "need; must have", "To require something.", PartOfSpeech.Verb, CefrLevel.A1, "You need more practice.", "You need more practice."),
            new("help", "help", "help", "help", "help; assist", "To make it easier for someone to do something.", PartOfSpeech.Verb, CefrLevel.A1, "Can you help me?", "Can you help me?"),
            new("ask", "ahsk", "ask", "ask", "ask; request information", "To say a question.", PartOfSpeech.Verb, CefrLevel.A1, "Ask one question in English.", "Ask one question in English."),
            new("answer", "AN-suh", "AN-ser", "an-swer", "answer; reply", "To reply to a question.", PartOfSpeech.Verb, CefrLevel.A1, "Answer the question aloud.", "Answer the question aloud."),
            new("good", "gud", "gud", "good", "good; nice or correct", "Pleasant, useful, or right.", PartOfSpeech.Adjective, CefrLevel.A1, "That is a good answer.", "That is a good answer."),
            new("bad", "bad", "bad", "bad", "bad; not good", "Not good or not pleasant.", PartOfSpeech.Adjective, CefrLevel.A1, "This is a bad habit.", "This is a bad habit."),
            new("new", "nyoo", "noo", "new", "new; not old", "Recently made, found, or learned.", PartOfSpeech.Adjective, CefrLevel.A1, "Learn one new word.", "Learn one new word."),
            new("old", "ohld", "ohld", "old", "old; not new", "Having existed for a long time.", PartOfSpeech.Adjective, CefrLevel.A1, "Review old words first.", "Review old words first."),
            new("easy", "EE-zee", "EE-zee", "ea-sy", "easy; not difficult", "Not difficult.", PartOfSpeech.Adjective, CefrLevel.A1, "This question is easy.", "This question is easy."),
            new("difficult", "DIF-uh-kult", "DIF-uh-kult", "dif-fi-cult", "difficult; hard", "Not easy.", PartOfSpeech.Adjective, CefrLevel.A2, "This grammar rule is difficult.", "This grammar rule is difficult."),
            new("important", "im-PAW-tunt", "im-POR-tant", "im-por-tant", "important; valuable", "Something that matters a lot.", PartOfSpeech.Adjective, CefrLevel.A2, "Practice is important.", "Practice is important."),
            new("different", "DIF-uh-runt", "DIF-er-ent", "dif-fer-ent", "different; not the same", "Not the same.", PartOfSpeech.Adjective, CefrLevel.A1, "These two words are different.", "These two words are different."),
            new("because", "bi-KOZ", "bi-KAWZ", "be-cause", "because; for the reason that", "Used to give a reason.", PartOfSpeech.Conjunction, CefrLevel.A2, "I study because I want to improve.", "I study because I want to improve."),
            new("but", "but", "but", "but", "but; however", "Used to connect contrasting ideas.", PartOfSpeech.Conjunction, CefrLevel.A1, "The word is short but useful.", "The word is short but useful."),
            new("and", "and", "and", "and", "and; also", "Used to join words or ideas.", PartOfSpeech.Conjunction, CefrLevel.A1, "Read and repeat.", "Read and repeat."),
            new("before", "bi-FAW", "bi-FOR", "be-fore", "before; earlier than", "Earlier than something.", PartOfSpeech.Preposition, CefrLevel.A2, "Review before the quiz.", "Review before the quiz."),
            new("after", "AF-tuh", "AF-ter", "af-ter", "after; later than", "Later than something.", PartOfSpeech.Preposition, CefrLevel.A2, "Practice after lunch.", "Practice after lunch."),
            new("with", "with", "with", "with", "with; together", "Together with someone or something.", PartOfSpeech.Preposition, CefrLevel.A1, "Study with a friend.", "Study with a friend."),
            new("without", "with-OWT", "with-OWT", "with-out", "without; not having", "Not having someone or something.", PartOfSpeech.Preposition, CefrLevel.A2, "Try again without help.", "Try again without help."),
            new("about", "uh-BOWT", "uh-BOWT", "a-bout", "about; connected with", "Connected with a topic.", PartOfSpeech.Preposition, CefrLevel.A1, "This lesson is about food.", "This lesson is about food."),
            new("always", "AWL-wayz", "AWL-wayz", "al-ways", "always; every time", "Every time.", PartOfSpeech.Adverb, CefrLevel.A1, "Always check your answer.", "Always check your answer."),
            new("usually", "YOO-zhoo-uh-lee", "YOO-zhoo-uh-lee", "u-su-al-ly", "usually; often", "In most situations.", PartOfSpeech.Adverb, CefrLevel.A1, "I usually study at night.", "I usually study at night."),
            new("sometimes", "SUM-tymz", "SUM-tymz", "some-times", "sometimes; not always", "On some occasions.", PartOfSpeech.Adverb, CefrLevel.A1, "Sometimes I listen to podcasts.", "Sometimes I listen to podcasts."),
            new("never", "NEV-uh", "NEV-er", "nev-er", "never; not ever", "Not at any time.", PartOfSpeech.Adverb, CefrLevel.A1, "Never stop learning.", "Never stop learning."),
            new("quickly", "KWIK-lee", "KWIK-lee", "quick-ly", "quickly; fast", "In a fast way.", PartOfSpeech.Adverb, CefrLevel.A2, "Read the sentence quickly.", "Read the sentence quickly."),
            new("slowly", "SLOH-lee", "SLOH-lee", "slow-ly", "slowly; not fast", "In a slow way.", PartOfSpeech.Adverb, CefrLevel.A2, "Speak slowly and clearly.", "Speak slowly and clearly.")
        ];

        foreach (var word in words)
        {
            await GetOrCreateWordAsync(
                word.Text,
                word.IpaUk,
                word.IpaUs,
                word.ThaiReading,
                word.MeaningTh,
                word.MeaningEn,
                word.PartOfSpeech,
                word.CefrLevel,
                word.ExampleEn,
                word.ExampleTh,
                categoryId,
                tagIds,
                now,
                cancellationToken);
        }
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
            pronunciation.Update(
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
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
            topic.Update(title, summary, cefrLevel, sortOrder, isActive: true, now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
            rule.Update(
                grammarTopicId,
                title,
                ruleText,
                explanationTh,
                explanationEn,
                structurePattern,
                commonMistake,
                correctUsageNote,
                sortOrder,
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
            lesson.Update(
                title,
                summary,
                description,
                cefrLevel,
                categoryId,
                thumbnailMediaId: null,
                estimatedMinutes,
                sortOrder,
                isPublished: true,
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
            course.Update(
                title,
                summary,
                description,
                cefrLevel,
                categoryId,
                thumbnailMediaId: null,
                estimatedMinutes,
                sortOrder,
                isPublished: true,
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
            book.Update(
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
                isPublished: true,
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
        var existingQuiz = await dbContext.Quizzes.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (existingQuiz is not null)
        {
            existingQuiz.Update(
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
                isPublished: true,
                isActive: true,
                now);
            await dbContext.SaveChangesAsync(cancellationToken);
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
        var template = await dbContext.PublishTemplates.SingleOrDefaultAsync(item => item.Slug == slug, cancellationToken);
        if (template is not null)
        {
            template.Update(name, description, format, templateContent, isDefault, isActive: true, now);
            await dbContext.SaveChangesAsync(cancellationToken);
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

    private async Task DeactivateLegacySeedContentAsync(DateTimeOffset now, CancellationToken cancellationToken)
    {
        await DeactivateCoursesAsync(["a1-starter-english"], now, cancellationToken);
        await DeactivateBooksAsync(["englishmaster-mvp-starter-book"], now, cancellationToken);
        await DeactivateQuizzesAsync(["a1-starter-quiz"], now, cancellationToken);
        await DeactivatePublishTemplatesAsync(["basic-html-template", "basic-markdown-template"], now, cancellationToken);
    }

    private async Task DeactivateCoursesAsync(string[] slugs, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var courses = await dbContext.Courses.Where(item => slugs.Contains(item.Slug)).ToArrayAsync(cancellationToken);
        foreach (var course in courses)
        {
            course.Deactivate(now);
        }

        if (courses.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task DeactivateBooksAsync(string[] slugs, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var books = await dbContext.Books.Where(item => slugs.Contains(item.Slug)).ToArrayAsync(cancellationToken);
        foreach (var book in books)
        {
            book.Deactivate(now);
        }

        if (books.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task DeactivateQuizzesAsync(string[] slugs, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var quizzes = await dbContext.Quizzes.Where(item => slugs.Contains(item.Slug)).ToArrayAsync(cancellationToken);
        foreach (var quiz in quizzes)
        {
            quiz.Deactivate(now);
        }

        if (quizzes.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task DeactivatePublishTemplatesAsync(string[] slugs, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var templates = await dbContext.PublishTemplates.Where(item => slugs.Contains(item.Slug)).ToArrayAsync(cancellationToken);
        foreach (var template in templates)
        {
            template.Deactivate(now);
        }

        if (templates.Length > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private sealed record CoreLearnerWord(
        string Text,
        string IpaUk,
        string IpaUs,
        string ThaiReading,
        string MeaningTh,
        string MeaningEn,
        PartOfSpeech PartOfSpeech,
        CefrLevel CefrLevel,
        string ExampleEn,
        string ExampleTh);
}
