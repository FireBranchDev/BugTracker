using BackendApi.Controllers;
using BackendApi.Models;
using BackendApi.Services;
using BackendClassLib.Database;
using BackendClassLib.Database.Repository;
using ClassLib;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApiTests;

[Collection(nameof(DatabaseCollection))]
public class ProjectsControllerTest(DatabaseFixture databaseFixture)
{
    readonly DatabaseFixture _databaseFixture = databaseFixture;

    public class MyFakeUserService : IUserService
    {
        public ClaimsPrincipal? User { get; set; } = null;

        public Claim? GetSubClaim()
        {
            return User?.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        }
    }

    [Fact]
    public async Task Post()
    {
        using ApplicationDbContext dbContext = DatabaseFixture.CreateContext();
        await dbContext.Database.BeginTransactionAsync();

        AuthRepository authRepo = new(dbContext);
        ProjectRepository projectRepo = new(dbContext);
        UserRepository userRepo = new(dbContext);
        MyFakeUserService userService = new();

        ProjectsController projectsController = new(authRepo, projectRepo, userRepo, userService);

        string auth0UserId = "auth0|h8g6antdzgykodou4s98t0xr";
        Claim subClaim = new(ClaimTypes.NameIdentifier, auth0UserId);
        ClaimsIdentity claimsIdentity = new([subClaim]);
        userService.User = new(claimsIdentity);

        Project newProject = new()
        {
            Name = "Project One",
            Description = "Lorem ipsum odor amet, consectetuer adipiscing elit. Accumsan senectus morbi habitant non torquent praesent. Posuere dapibus tortor sapien laoreet vestibulum quis eu. Sociosqu hac blandit luctus, eleifend consectetur euismod tortor. Habitant mauris felis donec sit; facilisis torquent at quam? Maximus auctor suscipit ante sed, semper nec habitant."
        };

        // There should not be a user account linked to the auth0 user id.
        IActionResult result = await projectsController.Post(newProject);
        BadRequestObjectResult badRequest = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, badRequest.Value);

        BackendClassLib.Database.Models.Auth auth = await dbContext.Auths.Where(c => c.UserIds.Contains(auth0UserId)).FirstAsync();
        auth.User = new BackendClassLib.Database.Models.User()
        {
            DisplayName = "Testing User 1"
        };
        await dbContext.SaveChangesAsync();

        result = await projectsController.Post(newProject);
        Assert.IsType<NoContentResult>(result);

        // Has the new project been inserted
        Assert.Equal(1, await dbContext.Projects.CountAsync());

        dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task GetAllProjects()
    {
        using ApplicationDbContext dbContext = DatabaseFixture.CreateContext();
        await dbContext.Database.BeginTransactionAsync();

        AuthRepository authRepo = new(dbContext);
        ProjectRepository projectRepo = new(dbContext);
        UserRepository userRepo = new(dbContext);
        MyFakeUserService userService = new();

        ProjectsController projectsController = new(authRepo, projectRepo, userRepo, userService);

        IActionResult result = await projectsController.GetAllProjects();
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequest = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.MissingSubClaim, badRequest.Value);

        string auth0UserId = "auth0|h8g6antdzgykodou4s98t0xr";
        Claim subClaim = new(ClaimTypes.NameIdentifier, auth0UserId);
        ClaimsIdentity claimsIdentity = new([subClaim]);
        userService.User = new(claimsIdentity);

        result = await projectsController.GetAllProjects();
        Assert.IsType<BadRequestObjectResult>(result);
        badRequest = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, badRequest.Value);

        BackendClassLib.Database.Models.Auth auth = await dbContext.Auths.Where(c => c.UserIds.Contains(auth0UserId)).FirstAsync();
        auth.User = new()
        {
            DisplayName = "Test User 1"
        };
        await dbContext.SaveChangesAsync();

        BackendClassLib.Database.Models.Project testProject1 = new()
        {
            Name = "Test Project 1"
        };
        BackendClassLib.Database.Models.Project testProject2 = new()
        {
            Name = "Test Project 2"
        };
        BackendClassLib.Database.Models.Project testProject3 = new()
        {
            Name = "Test Project 3"
        };
        BackendClassLib.Database.Models.Project testProject4 = new()
        {
            Name = "Test Project 4"
        };
        testProject1.Users.Add(auth.User);
        testProject2.Users.Add(auth.User);
        testProject3.Users.Add(auth.User);
        testProject4.Users.Add(auth.User);

        await dbContext.AddRangeAsync([testProject1, testProject2, testProject3, testProject4]);
        await dbContext.SaveChangesAsync();

        result = await projectsController.GetAllProjects();
        Assert.IsType<OkObjectResult>(result);
        OkObjectResult okResult = (OkObjectResult)result;

        Assert.NotNull(okResult.Value);
        List<BackendClassLib.Database.Models.Project>? projects = (List<BackendClassLib.Database.Models.Project>)okResult.Value;
        Assert.NotNull(projects);
        if (projects != null)
            Assert.Equal(4, projects.Count);
    }
}
