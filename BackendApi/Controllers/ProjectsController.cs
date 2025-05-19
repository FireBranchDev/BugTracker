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
public class ProjectsController(IAuthRepository authRepository, IProjectRepository projectRepository, IUserRepository userRepository) : ControllerBase
{
    readonly IAuthRepository _authRepository = authRepository;
    readonly IProjectRepository _projectRepository = projectRepository;
    readonly IUserRepository _userRepository = userRepository;

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
            int id = await _projectRepository.AddAsync(project.Name, project.Description, auth.Id);

            CreatedResult createdResult = Created();
            createdResult.Location = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}/{id}";
            return createdResult;
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch
        {
            auth = await _authRepository.InsertAsync(subClaim.Value);
        }

        User user;
        try
        {
            user = await _userRepository.FindAsync(auth.Id);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }

        List<BackendClassLib.Database.Models.Project> projects = await _projectRepository.GetAllProjectsAsync(user.Id);
        return Ok(projects.Select(ConvertToModel).ToList());
    }

    [Route("{projectId}")]
    [HttpGet]
    public async Task<IActionResult> FindAsync(int projectId)
    {
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

        User user;
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

        return Ok(ConvertToModel(project));
    }

    [Route("{projectId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(int projectId)
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

        User user;
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
            await _projectRepository.DeleteAsync(projectId, user.Id);
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

    [Route("{projectId}/add-collaborator/{collaboratorToAddId}")]
    [HttpPost]
    public async Task<IActionResult> AddCollaboratorAsync(int projectId, int collaboratorToAddId)
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

        User user;
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
            await _projectRepository.AddCollaboratorAsync(user.Id, collaboratorToAddId, projectId);
        }
        catch (CollaboratorToAddNotFoundException)
        {
            return NotFound(ApiErrorMessages.CollaboratorToAddNotFound);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound(ApiErrorMessages.ProjectNotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (InsufficientPermissionToAddCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.InsufficientPermissionToAddProjectCollaborator);
        }
        
        return NoContent();
    }

    [Route("{projectId}/collaborators/remove/{collaboratorId}")]
    [HttpDelete]
    public async Task<IActionResult> RemoveCollaboratorAsync(int projectId, int collaboratorId)
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

        User user;
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
            await _projectRepository.RemoveCollaboratorAsync(user.Id, collaboratorId, projectId);
        }
        catch (CollaboratorToRemoveNotFoundException)
        {
            return NotFound(ApiErrorMessages.CollaboratorToRemoveNotFound);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound(ApiErrorMessages.ProjectNotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (InsufficientPermissionToRemoveCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.InsufficientPermissionToRemoveProjectCollaborator);
        }
        catch (AttemptingToRemoveNonProjectCollaboratorException)
        {
            return NotFound(ApiErrorMessages.AttemptingToRemoveNonProjectCollaborator);
        }

        return NoContent();
    }

    public static Project ConvertToModel(BackendClassLib.Database.Models.Project project)
    {
        return new()
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
        };
    }
}
