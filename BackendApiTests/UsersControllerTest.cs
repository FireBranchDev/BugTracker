using BackendApi.Controllers;
using BackendClassLib.Database;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User = BackendApi.Models.User;

namespace BackendApiTests;

[Collection(nameof(DatabaseCollection))]
public class UsersControllerTest(DatabaseFixture databaseFixture)
{
    readonly DatabaseFixture _databaseFixture = databaseFixture;

    [Fact]
    public async Task Post()
    {
        using ApplicationDbContext dbContext = DatabaseFixture.CreateContext();
        await dbContext.Database.BeginTransactionAsync();

        AuthRepository authRepo = new(dbContext);
        UserRepository userRepo = new(dbContext);

        UsersController usersController = new(authRepo, userRepo);

        User user = new()
        {
            DisplayName = "John Doe"
        };

        IActionResult result = await usersController.Post(user);
        Assert.IsType<BadRequestObjectResult>(result);

        Claim subClaim = new(ClaimTypes.NameIdentifier, "auth0|h8g6antdzgykodou4s98t0xr");
        ClaimsIdentity claimsIdentity = new([subClaim]);
        usersController.HttpContext.User = new(claimsIdentity);

        result = await usersController.Post(user);

        Assert.IsType<NoContentResult>(result);

        Assert.True(dbContext.Auths.Any(c => c.UserIds.Contains(subClaim.Value)));
        Assert.True(dbContext.Users.Any(c => c.Auth.UserIds.Contains(subClaim.Value)));
        Assert.True(dbContext.Users.Any(c => c.DisplayName == user.DisplayName));

        result = await usersController.Post(user);
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult userAccountAlreadyCreated = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.UserAccountAlreadyCreated, userAccountAlreadyCreated.Value!.ToString());

        dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Get()
    {
        using ApplicationDbContext dbContext = DatabaseFixture.CreateContext();
        await dbContext.Database.BeginTransactionAsync();

        AuthRepository authRepo = new(dbContext);
        UserRepository userRepo = new(dbContext);
        UsersController usersController = new(authRepo, userRepo);

        IActionResult result = await usersController.Get();
        Assert.IsType<BadRequestObjectResult>(result);

        BadRequestObjectResult missingSubClaim = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.MissingSubClaim, missingSubClaim.Value!.ToString());

        Claim subClaim = new(ClaimTypes.NameIdentifier, "auth0|h8g6antdzgykodou4s98t0xr");
        ClaimsIdentity claimsIdentity = new([subClaim]);
        usersController.HttpContext.User = new(claimsIdentity);

        result = await usersController.Get();
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult noRecordOfAuth0UserId = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfAuth0UserId, noRecordOfAuth0UserId.Value!.ToString());

        Auth testAuth = new()
        {
            UserIds = [subClaim.Value]
        };
        await dbContext.Auths.AddAsync(testAuth);
        await dbContext.SaveChangesAsync();

        result = await usersController.Get();
        Assert.IsType<BadRequestObjectResult>(result);

        BadRequestObjectResult noRecordOfUserAccount = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, noRecordOfUserAccount.Value!.ToString());

        testAuth.User = new()
        {
            DisplayName = "John Doe"
        };
        await dbContext.SaveChangesAsync();

        result = await usersController.Get();
        Assert.IsType<OkObjectResult>(result);
        OkObjectResult okResult = (OkObjectResult)result;
        Assert.Equal(testAuth.User, okResult.Value);

        dbContext.ChangeTracker.Clear();
    }
}
