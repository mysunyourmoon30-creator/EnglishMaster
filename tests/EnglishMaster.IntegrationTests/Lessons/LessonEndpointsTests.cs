using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.GrammarRules;
using EnglishMaster.Contracts.GrammarTopics;
using EnglishMaster.Contracts.LessonSections;
using EnglishMaster.Contracts.Lessons;
using EnglishMaster.Contracts.Words;

namespace EnglishMaster.IntegrationTests.Lessons;

public sealed class LessonEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public LessonEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task LessonEndpointsSupportSectionsRelatedContentPublishAndSoftDelete()
    {
        var lessonResponse = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest(
                "Daily Routines Integration",
                "Learn to talk about your day",
                "A longer description.",
                "A1",
                null,
                null,
                15,
                0));
        Assert.Equal(HttpStatusCode.Created, lessonResponse.StatusCode);
        var lesson = await lessonResponse.Content.ReadFromJsonAsync<LessonDto>();
        Assert.NotNull(lesson);
        Assert.Equal("daily-routines-integration", lesson.Slug);
        Assert.False(lesson.IsPublished);

        var lessonSearch = await client.GetFromJsonAsync<LessonSearchResponse>(
            "/api/v1/lessons?cefrLevel=A1&search=Daily");
        Assert.Contains(lessonSearch!.Items, item => item.Id == lesson.Id);

        var word = await CreateWordAsync("lesson wake up integration");
        var addWordResponse = await client.PostAsync(
            $"/api/v1/lessons/{lesson.Id}/words/{word.Id}?sortOrder=0",
            content: null);
        Assert.Equal(HttpStatusCode.OK, addWordResponse.StatusCode);
        var lessonWithWord = await addWordResponse.Content.ReadFromJsonAsync<LessonDto>();
        Assert.Contains(lessonWithWord!.Words, item => item.Id == word.Id);

        var topicResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-topics",
            new CreateGrammarTopicRequest("Lesson Integration Topic", null, "A1", 0));
        var topic = await topicResponse.Content.ReadFromJsonAsync<GrammarTopicDto>();
        var ruleResponse = await client.PostAsJsonAsync(
            "/api/v1/grammar-rules",
            new CreateGrammarRuleRequest(
                topic!.Id,
                "Lesson Integration Rule",
                "Subject + base verb",
                null,
                null,
                null,
                null,
                null,
                0));
        var rule = await ruleResponse.Content.ReadFromJsonAsync<GrammarRuleDto>();
        var addRuleResponse = await client.PostAsync(
            $"/api/v1/lessons/{lesson.Id}/grammar-rules/{rule!.Id}?sortOrder=0",
            content: null);
        Assert.Equal(HttpStatusCode.OK, addRuleResponse.StatusCode);
        var lessonWithRule = await addRuleResponse.Content.ReadFromJsonAsync<LessonDto>();
        Assert.Contains(lessonWithRule!.GrammarRules, item => item.Id == rule.Id);

        var sectionOneResponse = await client.PostAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}/sections",
            new CreateLessonSectionRequest("Vocabulary Warm-Up", "## Words", "Vocabulary", null, 0));
        Assert.Equal(HttpStatusCode.Created, sectionOneResponse.StatusCode);
        var sectionOne = await sectionOneResponse.Content.ReadFromJsonAsync<LessonSectionDto>();

        var sectionTwoResponse = await client.PostAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}/sections",
            new CreateLessonSectionRequest("Grammar Focus", "## Grammar", "Grammar", null, 1));
        Assert.Equal(HttpStatusCode.Created, sectionTwoResponse.StatusCode);
        var sectionTwo = await sectionTwoResponse.Content.ReadFromJsonAsync<LessonSectionDto>();

        var sections = await client.GetFromJsonAsync<IReadOnlyCollection<LessonSectionDto>>(
            $"/api/v1/lessons/{lesson.Id}/sections");
        Assert.Equal(2, sections!.Count);

        var reorderResponse = await client.PostAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}/sections/reorder",
            new ReorderLessonSectionsRequest([sectionTwo!.Id, sectionOne!.Id]));
        Assert.Equal(HttpStatusCode.OK, reorderResponse.StatusCode);
        var reordered = await reorderResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<LessonSectionDto>>();
        Assert.Equal(sectionTwo.Id, reordered!.First().Id);

        var updateSectionResponse = await client.PutAsJsonAsync(
            $"/api/v1/lesson-sections/{sectionOne.Id}",
            new UpdateLessonSectionRequest("Vocabulary Warm-Up Updated", "## Words", "Vocabulary", null, 1, true));
        Assert.Equal(HttpStatusCode.OK, updateSectionResponse.StatusCode);
        var updatedSection = await updateSectionResponse.Content.ReadFromJsonAsync<LessonSectionDto>();
        Assert.Equal("Vocabulary Warm-Up Updated", updatedSection!.Title);

        var publishResponse = await client.PostAsync($"/api/v1/lessons/{lesson.Id}/publish", content: null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        var publishedLesson = await publishResponse.Content.ReadFromJsonAsync<LessonDto>();
        Assert.True(publishedLesson!.IsPublished);

        var unpublishResponse = await client.PostAsync($"/api/v1/lessons/{lesson.Id}/unpublish", content: null);
        Assert.Equal(HttpStatusCode.OK, unpublishResponse.StatusCode);
        var unpublishedLesson = await unpublishResponse.Content.ReadFromJsonAsync<LessonDto>();
        Assert.False(unpublishedLesson!.IsPublished);

        var updateLessonResponse = await client.PutAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}",
            new UpdateLessonRequest(
                "Daily Routines Integration Updated",
                "Learn to talk about your day",
                "A longer description.",
                "A1",
                null,
                null,
                20,
                1,
                false,
                true));
        Assert.Equal(HttpStatusCode.OK, updateLessonResponse.StatusCode);
        var updatedLesson = await updateLessonResponse.Content.ReadFromJsonAsync<LessonDto>();
        Assert.Equal("Daily Routines Integration Updated", updatedLesson!.Title);

        var removeWordResponse = await client.DeleteAsync($"/api/v1/lessons/{lesson.Id}/words/{word.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeWordResponse.StatusCode);

        var removeRuleResponse = await client.DeleteAsync($"/api/v1/lessons/{lesson.Id}/grammar-rules/{rule.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeRuleResponse.StatusCode);

        var deleteSectionResponse = await client.DeleteAsync($"/api/v1/lesson-sections/{sectionTwo.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteSectionResponse.StatusCode);

        var deleteLessonResponse = await client.DeleteAsync($"/api/v1/lessons/{lesson.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteLessonResponse.StatusCode);

        var activeLessonSearch = await client.GetFromJsonAsync<LessonSearchResponse>(
            "/api/v1/lessons?search=Updated");
        Assert.DoesNotContain(activeLessonSearch!.Items, item => item.Id == lesson.Id);

        var inactiveLessonSearch = await client.GetFromJsonAsync<LessonSearchResponse>(
            "/api/v1/lessons?search=Updated&isActive=false");
        Assert.Contains(inactiveLessonSearch!.Items, item => item.Id == lesson.Id);
    }

    [Fact]
    public async Task CreateLessonReturnsValidationProblemWhenTitleIsMissing()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest(" ", null, null, null, null, null, 0, 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateLessonReturnsValidationProblemWhenTitleAlreadyExists()
    {
        var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest("Duplicate Lesson Title", null, null, null, null, null, 0, 0));
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest("Duplicate Lesson Title", null, null, null, null, null, 0, 1));

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    [Fact]
    public async Task LessonRelationshipEndpointsReturnValidationProblemsForInvalidInput()
    {
        var lesson = await CreateLessonAsync($"Invalid Relationship Lesson {Guid.NewGuid():N}");
        var word = await CreateWordAsync($"invalid relationship word {Guid.NewGuid():N}");

        var addWordResponse = await client.PostAsync(
            $"/api/v1/lessons/{lesson.Id}/words/{word.Id}?sortOrder=-1",
            content: null);

        Assert.Equal(HttpStatusCode.BadRequest, addWordResponse.StatusCode);

        var firstSectionResponse = await client.PostAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}/sections",
            new CreateLessonSectionRequest("First Section", null, "Text", null, 0));
        var firstSection = await firstSectionResponse.Content.ReadFromJsonAsync<LessonSectionDto>();

        var secondSectionResponse = await client.PostAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}/sections",
            new CreateLessonSectionRequest("Second Section", null, "Text", null, 1));
        var secondSection = await secondSectionResponse.Content.ReadFromJsonAsync<LessonSectionDto>();

        var reorderResponse = await client.PostAsJsonAsync(
            $"/api/v1/lessons/{lesson.Id}/sections/reorder",
            new ReorderLessonSectionsRequest([firstSection!.Id, firstSection.Id, secondSection!.Id]));

        Assert.Equal(HttpStatusCode.BadRequest, reorderResponse.StatusCode);
    }

    private async Task<LessonDto> CreateLessonAsync(string title)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest(title, null, null, null, null, null, 0, 0));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<LessonDto>())!;
    }

    private async Task<WordDto> CreateWordAsync(string text)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/words",
            new CreateWordRequest(
                text,
                null,
                null,
                null,
                "Thai meaning",
                text,
                "Verb",
                "A1",
                null,
                null));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<WordDto>())!;
    }
}
