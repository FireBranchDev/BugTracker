using BackendClassLib.Database;
using ClassLib;

namespace BackendApiTests;

[Collection(nameof(LiveApiCollection))]
public class LiveProjectsTest(ApiFixture apiFixture)
{
    readonly ApiFixture _apiFixture = apiFixture;

    [Fact]
    public async Task GetAllProjects()
    {
        using ApplicationDbContext context = ApiFixture.CreateDbContext();

        HttpResponseMessage response = await _apiFixture.ApiClient.GetAsync("projects");
        Assert.True(response.StatusCode is System.Net.HttpStatusCode.BadRequest);
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, await response.Content.ReadAsStringAsync());

        // Todo: Create a new auth record in db with the testing client's auth0 user id.
    }
}
