﻿using BackendApi.Controllers;
using BackendApi.DTOs;
using BackendApi.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace BackendClassLib.UnitTests;

public class BugsControllerTests
{
    [Fact]
    public async Task Post_CreateBug_ReturnsBadRequestMissingSubClaimErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        Mock<IUserRepository> stubUserRepository = new();
        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);
        DefaultHttpContext defaultHttpContext = new();
        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        Bug bug = new()
        {
            Title = "A"
        };

        // Act
        IActionResult result = await bugsController.CreateBug(0, bug);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.MissingSubClaim, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Post_CreateBug_ReturnsNoContent()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = 1 }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(1)).Returns(Task.FromResult<Database.Models.User>(new() { Id = 1 }));

        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(1))
            .Returns(Task.FromResult<Database.Models.Project?>(new() { Id = 1 }));

        Bug bug = new()
        {
            Title = "A",
            Description = null
        };

        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.CreateBugAsync(1, 1, bug.Title, bug.Description));

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);
        DefaultHttpContext defaultHttpContext = new();
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        defaultHttpContext.User = claimsPrincipal;

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        // Act
        IActionResult result = await bugsController.CreateBug(1, bug);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Post_CreateBug_ReturnsNotFoundProjectNotFoundErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = 1 }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(1))
            .Returns(Task.FromResult<Database.Models.User>(new() { Id = 1 }));

        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);
        DefaultHttpContext defaultHttpContext = new();
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, "auth0|4dbi6xalpm3t9zyumde7kyd3")
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        defaultHttpContext.User = claimsPrincipal;

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        Bug bug = new()
        {
            Title = "A"
        };

        // Act
        IActionResult result = await bugsController.CreateBug(0, bug);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)result;
        Assert.Equal(ApiErrorMessages.ProjectNotFound, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task Post_CreateBug_ReturnsBadRequestNoRecordOfUserAccountErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = 1 }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(1))
            .Throws<UserNotFoundException>();

        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);
        DefaultHttpContext defaultHttpContext = new();
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        defaultHttpContext.User = claimsPrincipal;

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        Bug bug = new()
        {
            Title = "A"
        };

        // Act
        IActionResult result = await bugsController.CreateBug(0, bug);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Post_CreateBug_ReturnsBadRequestWithUserNotProjectCollaboratorApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<Database.Models.User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Database.Models.Project?>(new() { Id = ProjectId }));

        Bug newBug = new()
        {
            Title = "A",
            Description = "B"
        };
        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.CreateBugAsync(ProjectId, UserId, newBug.Title, newBug.Description))
            .Throws<UserNotProjectCollaboratorException>();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);
        DefaultHttpContext defaultHttpContext = new();
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        defaultHttpContext.User = claimsPrincipal;

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        // Act
        IActionResult result = await bugsController.CreateBug(ProjectId, newBug);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.UserNotProjectCollaborator, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Get_Bugs_ReturnsBadRequestWithMissingSubClaimApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        Mock<IUserRepository> stubUserRepository = new();
        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);

        DefaultHttpContext defaultHttpContext = new();

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        // Act
        IActionResult result = await bugsController.GetBugs(0);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.MissingSubClaim, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Get_Bugs_ReturnsBadRequestWithProjectNotFoundApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<Database.Models.User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Database.Models.Project?>(null));

        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);

        DefaultHttpContext defaultHttpContext = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        defaultHttpContext.User = claimsPrincipal;

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        // Act
        IActionResult result = await bugsController.GetBugs(0);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.ProjectNotFound, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Get_Bugs_ReturnsBadRequestWithNoRecordOfUserAccountApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = AuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Throws<UserNotFoundException>();

        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object);

        DefaultHttpContext defaultHttpContext = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];

        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        defaultHttpContext.User = claimsPrincipal;

        bugsController.ControllerContext = new()
        {
            HttpContext = defaultHttpContext
        };

        // Act
        IActionResult result = await bugsController.GetBugs(0);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Get_Bugs_ReturnsBadRequestWithUserNotProjectCollaboratorApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<Database.Models.User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Database.Models.Project?>(new() { Id = ProjectId }));

        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.GetBugsAsync(ProjectId, UserId))
            .Throws<UserNotProjectCollaboratorException>();

        DefaultHttpContext defaultHttpContext = new();
        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);
        defaultHttpContext.User = claimsPrincipal;

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.GetBugs(ProjectId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.UserNotProjectCollaborator, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Get_Bugs_ReturnsOkWithListOfBugs()
    {
        // Arrange
        const string Auth0UserId = "auth0|4dbi6xalpm3t9zyumde7kyd3";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Database.Models.Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<Database.Models.User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Database.Models.Project?>(new() { Id = ProjectId }));

        const int Bug1Id = 1;
        const int Bug2Id = 2;
        const int Bug3Id = 3;

        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.GetBugsAsync(ProjectId, UserId))
            .Returns(Task.FromResult<List<Database.Models.Bug>>([
                new()
                {
                    Id = Bug1Id
                },
                new()
                {
                    Id = Bug2Id
                },
                new()
                {
                    Id = Bug3Id
                }
            ]));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        ClaimsPrincipal claimsPrincipal = new(claimsIdentity);

        DefaultHttpContext defaultHttpContext = new()
        {
            User = claimsPrincipal
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.GetBugs(ProjectId);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        OkObjectResult okObjectResult = (OkObjectResult)result;
        Assert.NotNull(okObjectResult.Value as List<BugDto>);
        Assert.Equal(3, (okObjectResult.Value as List<BugDto>)!.Count);
    }
}
