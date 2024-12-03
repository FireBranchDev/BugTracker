using BackendApi.Models;
using System.Net.Http.Json;

namespace BackendApiTests;

public class LiveUsersTest(ApiFixture fixture) : IClassFixture<ApiFixture>
{
    readonly ApiFixture _fixture = fixture;

    [Fact]
    public async Task DoesPostRequestValidationWork()
    {
        HttpResponseMessage response = await _fixture.ApiClient.PostAsJsonAsync("users", new User() { });
        Assert.True(response.StatusCode is System.Net.HttpStatusCode.BadRequest);
        ValidationErrorResponse? validationError = await response.Content.ReadFromJsonAsync<ValidationErrorResponse>();
        Assert.NotNull(validationError);
        Assert.NotEmpty(validationError.Errors);
        Assert.True(validationError.Errors.ContainsKey("DisplayName"));
        if (validationError.Errors.TryGetValue("DisplayName", out string[]? displayNameErrors))
        {
            Assert.NotNull(displayNameErrors);
            if (displayNameErrors is not null)
            {
                Assert.Contains("The DisplayName field is required.", displayNameErrors);
            }
        }
    }
}
