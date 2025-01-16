﻿using BackendApi.Controllers;
using BackendApi.DTOs;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Bug = BackendClassLib.Database.Models.Bug;
using Project = BackendClassLib.Database.Models.Project;
using User = BackendClassLib.Database.Models.User;

namespace BackendApi.UnitTests;

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

        Models.Bug bug = new()
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
            .Returns(Task.FromResult<Auth>(new() { Id = 1 }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(1)).Returns(Task.FromResult<User>(new() { Id = 1 }));

        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(1))
            .Returns(Task.FromResult<Project?>(new() { Id = 1 }));

        Models.Bug bug = new()
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
            .Returns(Task.FromResult<Auth>(new() { Id = 1 }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(1))
            .Returns(Task.FromResult<User>(new() { Id = 1 }));

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

        Models.Bug bug = new()
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
            .Returns(Task.FromResult<Auth>(new() { Id = 1 }));

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

        Models.Bug bug = new()
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
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

        Models.Bug newBug = new()
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
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(null));

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
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

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
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

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
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

        const int Bug1Id = 1;
        const int Bug2Id = 2;
        const int Bug3Id = 3;

        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.GetBugsAsync(ProjectId, UserId))
            .Returns(Task.FromResult<List<Bug>>([
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

    [Fact]
    public async Task Delete_Bug_ReturnsBadRequestWithMissingSubClaimApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        Mock<IUserRepository> stubUserRepository = new();
        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        const int ProjectId = 0;
        const int BugId = 0;

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);
        
        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.MissingSubClaim, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Delete_Bug_ReturnsBadRequestWithNoRecordOfUserAccountApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|6c76g4gw3pqvoojspve8jbix";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Throws<UserNotFoundException>();

        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        const int ProjectId = 0;
        const int BugId = 0;

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
        BadRequestObjectResult badRequestObjectResult = (BadRequestObjectResult)result;
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Delete_Bug_ReturnsNotFoundWithProjectNotFoundApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|6c76g4gw3pqvoojspve8jbix";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(null));

        Mock<IBugRepository> stubBugRepository = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };
        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        const int BugId = 0;

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        NotFoundObjectResult notFoundObjectResult = (NotFoundObjectResult)result;
        Assert.Equal(ApiErrorMessages.ProjectNotFound, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task Delete_Bug_ReturnsBadRequestWithUserNotProjectCollaboratorApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|6c76g4gw3pqvoojspve8jbix";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

        const int BugId = 1;
        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.DeleteBugAsync(ProjectId, UserId, BugId))
            .Throws<UserNotProjectCollaboratorException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);

        // Assert
        BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(ApiErrorMessages.UserNotProjectCollaborator, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Delete_Bug_ReturnsNotFoundWithNoRecordOfBugApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|6c76g4gw3pqvoojspve8jbix";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

        const int BugId = 1;
        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.DeleteBugAsync(ProjectId, UserId, BugId))
            .Throws<BugNotFoundException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);

        // Assert
        NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ApiErrorMessages.NoRecordOfBug, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task Delete_Bug_ReturnsBadRequestWithInsufficientPermissionToDeleteBugApiErrorMessage()
    {
        // Arrange
        const string Auth0UserId = "auth0|6c76g4gw3pqvoojspve8jbix";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

        const int BugId = 1;
        Mock<IBugRepository> stubBugRepository = new();
        stubBugRepository.Setup(x => x.DeleteBugAsync(ProjectId, UserId, BugId))
            .Throws<InsufficientPermissionToDeleteBugException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);

        // Assert
        BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(ApiErrorMessages.InsufficientPermissionToDeleteBug, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task Delete_Bug_ReturnsNoContent()
    {
        // Arrange
        const string Auth0UserId = "auth0|6c76g4gw3pqvoojspve8jbix";
        const int AuthId = 1;
        Mock<IAuthRepository> stubAuthRepository = new();
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        const int UserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        const int ProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.FindAsync(ProjectId))
            .Returns(Task.FromResult<Project?>(new() { Id = ProjectId }));

        const int BugId = 1;
        Mock<IBugRepository> stubBugRepository = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.DeleteBug(ProjectId, BugId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_MissingSubClaim_ReturnsBadRequestWithMissingSubClaimApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        Mock<IUserRepository> stubUserRepository = new();
        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        const int ProjectId = 0;
        const int BugId = 0;
        
        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(ApiErrorMessages.MissingSubClaim, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_NoUserAccount_ReturnsForbiddenWithNoRecordOfUserAccountApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|vrfs3uukji0jp7855ibm2835";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new()
            {
                Id = AuthId
            }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Throws<UserNotFoundException>();

        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IBugRepository> stubBugRepository = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        const int ProjectId = 0;
        const int BugId = 0;

        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(403, objectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, objectResult.Value);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_ProjectNotFound_ReturnsNotFoundWithProjectNotFoundApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|a94bxpgvpkjq5o2uq8deljv1";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new()
            {
                Id = AuthId
            }));

        Mock<IUserRepository> stubUserRepository = new();
        const int UserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int ProjectId = 1;

        Mock<IBugRepository> stubBugRepository = new();
        const int BugId = 1;
        stubBugRepository.Setup(x => x.MarkBugAsAssigned(BugId, ProjectId, UserId))
            .Throws<ProjectNotFoundException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ApiErrorMessages.ProjectNotFound, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_BugNotFound_ReturnsNotFoundWithNoRecordOfBugApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|ed0rtgo71nnujdxrkh4qllrf";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int UserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int ProjectId = 1;

        Mock<IBugRepository> stubBugRepository = new();
        const int BugId = 1;
        stubBugRepository.Setup(x => x.MarkBugAsAssigned(BugId, ProjectId, UserId))
            .Throws<BugNotFoundException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        
        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ApiErrorMessages.NoRecordOfBug, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_UserNotProjectCollaborator_ReturnsBadRequestWithUserNotProjectCollaboratorApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|ed0rtgo71nnujdxrkh4qllrf";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int UserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int ProjectId = 1;

        Mock<IBugRepository> stubBugRepository = new();
        const int BugId = 1;
        stubBugRepository.Setup(x => x.MarkBugAsAssigned(BugId, ProjectId, UserId))
            .Throws<UserNotProjectCollaboratorException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(ApiErrorMessages.UserNotProjectCollaborator, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_NotProjectsBug_ReturnsBadRequestWithBugNotAssociatedWithProjectApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|ed0rtgo71nnujdxrkh4qllrf";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int UserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int ProjectId = 1;

        Mock<IBugRepository> stubBugRepository = new();
        const int BugId = 1;
        stubBugRepository.Setup(x => x.MarkBugAsAssigned(BugId, ProjectId, UserId))
            .Throws<NotProjectBugException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        BadRequestObjectResult badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(ApiErrorMessages.BugNotAssociatedWithProject, badRequestObjectResult.Value);
    }

    [Fact]
    public async Task MarkBugStatusAsAssigned_NoErrors_ReturnsNoContent()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|ed0rtgo71nnujdxrkh4qllrf";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int UserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new() { Id = UserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int ProjectId = 1;

        Mock<IBugRepository> stubBugRepository = new();
        const int BugId = 1;

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        DefaultHttpContext defaultHttpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        BugsController bugsController = new(stubAuthRepository.Object, stubUserRepository.Object, stubProjectRepository.Object, stubBugRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = defaultHttpContext
            }
        };

        // Act
        IActionResult result = await bugsController.MarkBugStatusAsAssigned(ProjectId, BugId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
