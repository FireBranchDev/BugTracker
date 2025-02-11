using BackendApi.Controllers;
using BackendApi.Services;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using System.Security.Claims;

namespace BackendApi.UnitTests;

public class ProjectsControllerTests
{
    [Fact]
    public async Task FindAsync_MissingSubClaim_ReturnsUnauthorizedWithMissingSubClaimApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        Mock<IUserRepository> stubUserRepository = new();
        Mock<IProjectRepository> stubProjectRepository = new();

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        const int ProjectId = 0;

        // Act
        IActionResult result = await projectsController.FindAsync(ProjectId);

        // Assert
        UnauthorizedObjectResult unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal(ApiErrorMessages.MissingSubClaim, unauthorizedObjectResult.Value);
    }

    [Fact]
    public async Task FindAsync_UserNotFound_ReturnsForbiddenWithNoRecordOfUserAccountApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestUserAuth0UserId = "auth0|ldoos2u5kdol7zgfof3aif7d";
        const int TestUserAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestUserAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestUserAuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(TestUserAuthId))
            .Throws<UserNotFoundException>();

        Mock<IProjectRepository> stubProjectRepository = new();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestUserAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);
        DefaultHttpContext httpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = httpContext
            }
        };

        const int ProjectId = 0;

        // Act
        IActionResult result = await projectsController.FindAsync(ProjectId);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, objectResult.Value);
    }

    [Fact]
    public async Task FindAsync_ProjectNotFound_ReturnsNotFoundWithProjectNotFoundApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestUserAuth0UserId = "auth0|ldoos2u5kdol7zgfof3aif7d";
        const int TestUserAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestUserAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestUserAuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int TestUserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(TestUserAuthId))
            .Returns(Task.FromResult<User>(new() { Id = TestUserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int TestProjectId = 1;
        stubProjectRepository.Setup(x => x.FindAsync(TestProjectId, TestUserId))
            .Throws<ProjectNotFoundException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestUserAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        DefaultHttpContext httpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = httpContext
            }
        };

        // Act
        IActionResult result = await projectsController.FindAsync(TestProjectId);

        // Assert
        NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ApiErrorMessages.ProjectNotFound, notFoundObjectResult.Value);
    }

    [Fact]
    public async Task FindAsync_UserNotProjectCollaborator_ReturnsForbiddenWithUserNotProjectCollaboratorApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestUserAuth0UserId = "auth0|ldoos2u5kdol7zgfof3aif7d";
        const int TestUserAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestUserAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestUserAuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int TestUserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(TestUserAuthId))
            .Returns(Task.FromResult<User>(new() { Id = TestUserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int TestProjectId = 1;
        stubProjectRepository.Setup(x => x.FindAsync(TestProjectId, TestUserId))
            .Throws<UserNotProjectCollaboratorException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestUserAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        DefaultHttpContext httpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };
        
        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = httpContext
            }
        };

        // Act
        IActionResult result = await projectsController.FindAsync(TestProjectId);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.UserNotProjectCollaborator, objectResult.Value);
    }

    [Fact]
    public async Task FindAsync_ReturnsOkWithProject()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestUserAuth0UserId = "auth0|ldoos2u5kdol7zgfof3aif7d";
        const int TestUserAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestUserAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestUserAuthId }));

        Mock<IUserRepository> stubUserRepository = new();
        const int TestUserId = 1;
        stubUserRepository.Setup(x => x.FindAsync(TestUserAuthId))
            .Returns(Task.FromResult<User>(new() { Id = TestUserId }));

        Mock<IProjectRepository> stubProjectRepository = new();
        const int TestProjectId = 1;
        Project testProject = new()
        {
            Id = TestProjectId,
            Name = "Test Project"
        };
        stubProjectRepository.Setup(x => x.FindAsync(TestProjectId, TestUserId))
            .Returns(Task.FromResult(testProject));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestUserAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        DefaultHttpContext httpContext = new()
        {
            User = new ClaimsPrincipal(claimsIdentity)
        };

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = httpContext
            }
        };

        // Act
        IActionResult result = await projectsController.FindAsync(TestProjectId);

        // Arrange
        OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
        Models.Project? returnedProject = okObjectResult.Value as Models.Project;
        Assert.NotNull(returnedProject);
        Assert.Equal(testProject.Id, returnedProject.Id);
        Assert.Equal(testProject.Name, returnedProject.Name);
        Assert.Equal(testProject.Description, returnedProject.Description);
    }

    [Fact]
    public async Task DeleteAsync_MissingSubClaim_ReturnsUnauthorizedWithMissingSubClaimApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IUserRepository> stubUserRepository = new();

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
            }
        };

        const int ProjectId = 0;

        // Act
        IActionResult result = await projectsController.DeleteAsync(ProjectId);

        // Assert
        UnauthorizedObjectResult unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Unauthorized, unauthorizedObjectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.MissingSubClaim, unauthorizedObjectResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_UserNotFound_ReturnsForbiddenWithNoRecordOfUserAccountApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|wgnvr5vruq1h97xqv0pnpjqi";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId, UserIds = [Auth0UserId] }));

        Mock<IProjectRepository> stubProjectRepository = new();
        Mock<IUserRepository> stubUserRepository = new();
        
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Throws<UserNotFoundException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                { 
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            }
        };

        const int ProjectId = 0;

        // Act
        IActionResult result = await projectsController.DeleteAsync(ProjectId);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.NoRecordOfUserAccount, objectResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_ProjectNotFound_ReturnsNotFoundWithProjectNotFoundApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string Auth0UserId = "auth0|wgnvr5vruq1h97xqv0pnpjqi";
        const int AuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(Auth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = AuthId, UserIds = [Auth0UserId] }));

        const int UserId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        const int ProjectId = 1;
        stubProjectRepository.Setup(x => x.DeleteAsync(ProjectId, UserId))
            .Throws<ProjectNotFoundException>();

        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(AuthId))
            .Returns(Task.FromResult<User>(new()
            {
                Id = UserId,
                DisplayName = "Test User",
                AuthId = AuthId,
            }));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, Auth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            }
        };

        // Act
        IActionResult result = await projectsController.DeleteAsync(ProjectId);

        // Assert
        NotFoundObjectResult notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal(ApiErrorMessages.ProjectNotFound, notFoundObjectResult.Value);
    }


    [Fact]
    public async Task DeleteAsync_UserNotProjectCollaborator_ReturnsForbiddenWithUserNotProjectCollaboratorApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestAuth0UserId = "auth0|wgnvr5vruq1h97xqv0pnpjqi";
        const int TestAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestAuthId, UserIds = [TestAuth0UserId] }));

        const int TestUserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(TestAuthId))
            .Returns(Task.FromResult<User>(new()
            {
                Id = TestUserId,
                DisplayName = "Test User"
            }));

        const int TestProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.DeleteAsync(TestProjectId, TestUserId))
            .Throws<UserNotProjectCollaboratorException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            }
        };
        
        // Act
        IActionResult result = await projectsController.DeleteAsync(TestProjectId);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.UserNotProjectCollaborator, objectResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_InsufficientPermissionToDeleteProject_ReturnsForbiddenWithInsufficientPermissionToDeleteProjectApiErrorMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestAuth0UserId = "auth0|wgnvr5vruq1h97xqv0pnpjqi";
        const int TestAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestAuthId, UserIds = [TestAuth0UserId] }));

        const int TestUserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(TestAuthId))
            .Returns(Task.FromResult<User>(new()
            {
                Id = TestUserId,
                DisplayName = "Test User"
            }));

        const int TestProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.DeleteAsync(TestProjectId, TestUserId))
            .Throws<InsufficientPermissionToDeleteProjectException>();

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            }
        };

        // Act
        IActionResult result = await projectsController.DeleteAsync(TestProjectId);

        // Assert
        ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
        Assert.Equal(ApiErrorMessages.InsufficientPermissionToDeleteProject, objectResult.Value);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsOkWithSuccessfullyDeletedProjectApiSuccessMessage()
    {
        // Arrange
        Mock<IAuthRepository> stubAuthRepository = new();
        const string TestAuth0UserId = "auth0|wgnvr5vruq1h97xqv0pnpjqi";
        const int TestAuthId = 1;
        stubAuthRepository.Setup(x => x.FindAsync(TestAuth0UserId))
            .Returns(Task.FromResult<Auth>(new() { Id = TestAuthId, UserIds = [TestAuth0UserId] }));

        const int TestUserId = 1;
        Mock<IUserRepository> stubUserRepository = new();
        stubUserRepository.Setup(x => x.FindAsync(TestAuthId))
            .Returns(Task.FromResult<User>(new()
            {
                Id = TestUserId,
                DisplayName = "Test User"
            }));

        const int TestProjectId = 1;
        Mock<IProjectRepository> stubProjectRepository = new();
        stubProjectRepository.Setup(x => x.DeleteAsync(TestProjectId, TestUserId));

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, TestAuth0UserId)
        ];
        ClaimsIdentity claimsIdentity = new(claims);

        ProjectsController projectsController = new(stubAuthRepository.Object, stubProjectRepository.Object, stubUserRepository.Object)
        {
            ControllerContext = new()
            {
                HttpContext = new DefaultHttpContext()
                {
                    User = new ClaimsPrincipal(claimsIdentity)
                }
            }
        };

        // Act
        IActionResult result = await projectsController.DeleteAsync(TestProjectId);

        // Assert
        OkObjectResult okObjectResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(ApiSuccessMessages.SuccessfullyDeletedProject, okObjectResult.Value);
    }
}
