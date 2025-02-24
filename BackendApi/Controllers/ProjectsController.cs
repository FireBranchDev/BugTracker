using BackendClassLib;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Project = BackendApi.Models.Project;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController(IAuthRepository authRepository, IProjectRepository projectRepository, IUserRepository userRepository, ILogger<ProjectsController> logger) : ControllerBase
{
    readonly IAuthRepository _authRepository = authRepository;
    readonly IProjectRepository _projectRepository = projectRepository;
    readonly IUserRepository _userRepository = userRepository;
    readonly ILogger<ProjectsController> _logger = logger;

    [HttpPost]
    public async Task<IActionResult> Post(Project project)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        try
        {
            await _projectRepository.AddAsync(project.Name, project.Description, auth.Id);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }
        catch (ProjectPermissionNotFoundException ex)
        {
            if (ex.Data.Contains(nameof(ProjectPermissionType) + "s"))
            {
                foreach (ProjectPermissionType projectPermissionType in (List<ProjectPermissionType>)ex.Data[nameof(ProjectPermissionType) + "s"]!)
                {
                    _logger.LogError(ex, "LogError: the {projectPermissionType} project permission type was not found in the database.", projectPermissionType);
                }
            }
            else
            {
                _logger.LogError(ex, "LogError: A project permission type was not found in the database.");
            }
            _logger.LogError("LogError: {ApiErrorMessages.CreatingNewProjectUnavailable}", ApiErrorMessages.CreatingNewProjectUnavailable);
            return StatusCode((int)HttpStatusCode.InternalServerError, ApiErrorMessages.CreatingNewProjectUnavailable);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }

        List<BackendClassLib.Database.Models.Project> projects = await _projectRepository.GetAllProjectsAsync(user.Id);
        return Ok(projects);
    }

    [Route("{projectId}")]
    [HttpGet]
    public async Task<IActionResult> FindAsync(int projectId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return StatusCode(403, ApiErrorMessages.NoRecordOfUserAccount);
        }

        BackendClassLib.Database.Models.Project project;
        try
        {
            project = await _projectRepository.FindAsync(projectId, user.Id);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound(ApiErrorMessages.ProjectNotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }

        return Ok(Convert(project));
    }

    [Route("{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        BackendClassLib.Database.Models.User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }

        try
        {
            await _projectRepository.DeleteAsync(id, user.Id);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound(ApiErrorMessages.ProjectNotFound);
        }
        catch (UserNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (InsufficientPermissionToDeleteProjectException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.InsufficientPermissionToDeleteProject);
        }

        return Ok(ApiSuccessMessages.SuccessfullyDeletedProject);
    }

    public static Project Convert(BackendClassLib.Database.Models.Project project)
    {
        return new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
        };
    }
}
