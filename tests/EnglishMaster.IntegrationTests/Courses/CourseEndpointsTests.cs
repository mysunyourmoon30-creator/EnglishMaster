using System.Net;
using System.Net.Http.Json;
using EnglishMaster.Contracts.Courses;
using EnglishMaster.Contracts.Lessons;

namespace EnglishMaster.IntegrationTests.Courses;

public sealed class CourseEndpointsTests : IClassFixture<EnglishMasterApiFactory>
{
    private readonly HttpClient client;

    public CourseEndpointsTests(EnglishMasterApiFactory factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CourseEndpointsSupportLessonsPublishAndSoftDelete()
    {
        var courseResponse = await client.PostAsJsonAsync(
            "/api/v1/courses",
            new CreateCourseRequest(
                "Beginner English Course Integration",
                "A structured beginner path",
                "A longer course description.",
                "A1",
                null,
                null,
                120,
                0));
        Assert.Equal(HttpStatusCode.Created, courseResponse.StatusCode);
        var course = await courseResponse.Content.ReadFromJsonAsync<CourseDto>();
        Assert.NotNull(course);
        Assert.Equal("beginner-english-course-integration", course.Slug);
        Assert.False(course.IsPublished);

        var courseSearch = await client.GetFromJsonAsync<CourseSearchResponse>(
            "/api/v1/courses?cefrLevel=A1&search=Beginner");
        Assert.Contains(courseSearch!.Items, item => item.Id == course.Id);

        var firstLesson = await CreateLessonAsync("Course First Lesson Integration");
        var secondLesson = await CreateLessonAsync("Course Second Lesson Integration");

        var addFirstLessonResponse = await client.PostAsync(
            $"/api/v1/courses/{course.Id}/lessons/{firstLesson.Id}?sortOrder=0&isRequired=true",
            content: null);
        Assert.Equal(HttpStatusCode.OK, addFirstLessonResponse.StatusCode);
        var courseWithFirstLesson = await addFirstLessonResponse.Content.ReadFromJsonAsync<CourseDto>();
        Assert.Contains(courseWithFirstLesson!.Lessons, item => item.LessonId == firstLesson.Id);

        var duplicateLessonResponse = await client.PostAsync(
            $"/api/v1/courses/{course.Id}/lessons/{firstLesson.Id}?sortOrder=1&isRequired=false",
            content: null);
        Assert.Equal(HttpStatusCode.OK, duplicateLessonResponse.StatusCode);
        var courseAfterDuplicateAdd = await duplicateLessonResponse.Content.ReadFromJsonAsync<CourseDto>();
        Assert.Single(courseAfterDuplicateAdd!.Lessons, item => item.LessonId == firstLesson.Id);
        Assert.False(courseAfterDuplicateAdd.Lessons.Single(item => item.LessonId == firstLesson.Id).IsRequired);

        var addSecondLessonResponse = await client.PostAsync(
            $"/api/v1/courses/{course.Id}/lessons/{secondLesson.Id}?sortOrder=1&isRequired=true",
            content: null);
        Assert.Equal(HttpStatusCode.OK, addSecondLessonResponse.StatusCode);
        var courseWithLessons = await addSecondLessonResponse.Content.ReadFromJsonAsync<CourseDto>();

        var courseLessons = await client.GetFromJsonAsync<IReadOnlyCollection<CourseLessonDto>>(
            $"/api/v1/courses/{course.Id}/lessons");
        Assert.Equal(2, courseLessons!.Count);

        var firstRelation = courseWithLessons!.Lessons.Single(item => item.LessonId == firstLesson.Id);
        var secondRelation = courseWithLessons.Lessons.Single(item => item.LessonId == secondLesson.Id);
        var reorderResponse = await client.PostAsJsonAsync(
            $"/api/v1/courses/{course.Id}/lessons/reorder",
            new ReorderCourseLessonsRequest([secondRelation.Id, firstRelation.Id]));
        Assert.Equal(HttpStatusCode.OK, reorderResponse.StatusCode);
        var reordered = await reorderResponse.Content.ReadFromJsonAsync<IReadOnlyCollection<CourseLessonDto>>();
        Assert.Equal(secondRelation.Id, reordered!.First().Id);

        var publishResponse = await client.PostAsync($"/api/v1/courses/{course.Id}/publish", content: null);
        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        var publishedCourse = await publishResponse.Content.ReadFromJsonAsync<CourseDto>();
        Assert.True(publishedCourse!.IsPublished);

        var unpublishResponse = await client.PostAsync($"/api/v1/courses/{course.Id}/unpublish", content: null);
        Assert.Equal(HttpStatusCode.OK, unpublishResponse.StatusCode);
        var unpublishedCourse = await unpublishResponse.Content.ReadFromJsonAsync<CourseDto>();
        Assert.False(unpublishedCourse!.IsPublished);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/courses/{course.Id}",
            new UpdateCourseRequest(
                "Beginner English Course Integration Updated",
                "A structured beginner path",
                "A longer course description.",
                "A1",
                null,
                null,
                150,
                1,
                false,
                true));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updatedCourse = await updateResponse.Content.ReadFromJsonAsync<CourseDto>();
        Assert.Equal("Beginner English Course Integration Updated", updatedCourse!.Title);

        var removeLessonResponse = await client.DeleteAsync($"/api/v1/courses/{course.Id}/lessons/{firstLesson.Id}");
        Assert.Equal(HttpStatusCode.NoContent, removeLessonResponse.StatusCode);

        var deleteCourseResponse = await client.DeleteAsync($"/api/v1/courses/{course.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteCourseResponse.StatusCode);

        var activeCourseSearch = await client.GetFromJsonAsync<CourseSearchResponse>(
            "/api/v1/courses?search=Updated");
        Assert.DoesNotContain(activeCourseSearch!.Items, item => item.Id == course.Id);

        var inactiveCourseSearch = await client.GetFromJsonAsync<CourseSearchResponse>(
            "/api/v1/courses?search=Updated&isActive=false");
        Assert.Contains(inactiveCourseSearch!.Items, item => item.Id == course.Id);
    }

    [Fact]
    public async Task CreateCourseReturnsValidationProblemWhenTitleIsMissing()
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/courses",
            new CreateCourseRequest(" ", null, null, null, null, null, 0, 0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateCourseReturnsValidationProblemWhenTitleAlreadyExists()
    {
        var title = $"Duplicate Course Title {Guid.NewGuid():N}";
        var firstResponse = await client.PostAsJsonAsync(
            "/api/v1/courses",
            new CreateCourseRequest(title, null, null, null, null, null, 0, 0));
        Assert.Equal(HttpStatusCode.Created, firstResponse.StatusCode);

        var secondResponse = await client.PostAsJsonAsync(
            "/api/v1/courses",
            new CreateCourseRequest(title, null, null, null, null, null, 0, 1));

        Assert.Equal(HttpStatusCode.BadRequest, secondResponse.StatusCode);
    }

    private async Task<LessonDto> CreateLessonAsync(string title)
    {
        var response = await client.PostAsJsonAsync(
            "/api/v1/lessons",
            new CreateLessonRequest(
                title,
                "Lesson summary",
                null,
                "A1",
                null,
                null,
                15,
                0));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        return (await response.Content.ReadFromJsonAsync<LessonDto>())!;
    }
}
