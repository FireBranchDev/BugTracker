using BackendClassLib.Database;
using ClassLib;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Net;
using System.Net.Http.Json;
using System.Reflection;

namespace BackendAPI.IntegrationTests;

public class ProjectsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    readonly HttpClient _client;
    readonly CustomWebApplicationFactory<Program> _factory;
    readonly IConfiguration _configuration;

    public ProjectsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions()
        {
            AllowAutoRedirect = false
        });

        Assembly executingAssembly = Assembly.GetExecutingAssembly();

        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddUserSecrets(executingAssembly);

        _configuration = configurationBuilder.Build();

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _configuration.GetValue("BackendAPI:AccessToken", string.Empty));
    }

    [Fact]
    public async Task Get_AllProjects_ReturnsProjects()
    {
        // Arrange
        JsonWebTokenHandler jsonWebTokenHandler = new();
        string? accessToken = _configuration.GetValue<string>("BackendAPI:AccessToken");
        if (accessToken is null)
            return;

        JsonWebToken jsonWebToken = jsonWebTokenHandler.ReadJsonWebToken(accessToken);

        using ApplicationDbContext applicationDbContext = CreateContext();
        await applicationDbContext.Database.EnsureCreatedAsync();
        await applicationDbContext.Auths.ExecuteDeleteAsync();

        BackendClassLib.Database.Models.Auth auth1 = new()
        {
            UserIds = [jsonWebToken.Subject]
        };
        await applicationDbContext.AddAsync(auth1);
        await applicationDbContext.SaveChangesAsync();

        await applicationDbContext.Users.ExecuteDeleteAsync();
        BackendClassLib.Database.Models.User testUser1 = new()
        {
            DisplayName = "Test User 1",
            Auth = auth1
        };
        await applicationDbContext.AddAsync(testUser1);
        await applicationDbContext.SaveChangesAsync();

        BackendClassLib.Database.Models.Project project1 = new()
        {
            Name = "Project One"
        };
        project1.Users.Add(testUser1);

        BackendClassLib.Database.Models.Project project2 = new()
        {
            Name = "Project Two"
        };
        project2.Users.Add(testUser1);

        BackendClassLib.Database.Models.Project project3 = new()
        {
            Name = "Project Three"
        };
        project3.Users.Add(testUser1);

        await applicationDbContext.AddRangeAsync([project1, project2, project3]);
        await applicationDbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync("api/projects");
        object[]? content = await response.Content.ReadFromJsonAsync<object[]>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(3, content.Length);
    }

    [Fact]
    public async Task Get_AllProjects_ReturnsNoRecordOfUserAccount()
    {
        // Arrange
        JsonWebTokenHandler jsonWebTokenHandler = new();
        string? accessToken = _configuration.GetValue<string>("BackendAPI:AccessToken");
        if (accessToken is null)
            return;

        JsonWebToken jsonWebToken = jsonWebTokenHandler.ReadJsonWebToken(accessToken);

        using ApplicationDbContext applicationDbContext = CreateContext();
        await applicationDbContext.Database.EnsureCreatedAsync();
        await applicationDbContext.Auths.ExecuteDeleteAsync();

        BackendClassLib.Database.Models.Auth auth1 = new()
        {
            UserIds = [jsonWebToken.Subject]
        };
        await applicationDbContext.AddAsync(auth1);
        await applicationDbContext.SaveChangesAsync();

        // Act
        HttpResponseMessage response = await _client.GetAsync("api/projects");
        string contentAsString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, contentAsString);
    }

    public ApplicationDbContext CreateContext()
    => new(
        new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlServer(_configuration.GetConnectionString("DefaultConnection")).Options);
}
