using BackendApi.DTOs;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using BackendClassLib.Models;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using Project = BackendApi.Models.Project;
using User = BackendClassLib.Database.Models.User;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectsController(IAuthRepository authRepository, IProjectRepository projectRepository, IUserRepository userRepository, IProjectRolesRepository projectRolesRepository) : ControllerBase
{
    readonly IAuthRepository _authRepository = authRepository;
    readonly IProjectRepository _projectRepository = projectRepository;
    readonly IUserRepository _userRepository = userRepository;
    readonly IProjectRolesRepository _projectRolesRepository = projectRolesRepository;

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

    [HttpGet("{projectId}/collaborators")]
    [ActionName(nameof(GetAllCollaboratorsAsync))]
    public async Task<ActionResult<List<Models.User>>> GetAllCollaboratorsAsync(int projectId, byte take = 10, int? lastRetrievedUserId = null)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier); ;
        if (subClaim is null) return Forbid();

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
        catch (AuthNotFoundException)
        {
            return Problem(
                title: "Auth User ID Not Found",
                detail: ApiErrorMessages.NoRecordOfAuth0UserId,
                statusCode: StatusCodes.Status403Forbidden);
        }
        catch (UserNotFoundException)
        {
            return Problem(
                title: "No Record of User Account",
                detail: ApiErrorMessages.NoRecordOfUserAccount,
                statusCode: StatusCodes.Status403Forbidden);
        }

        try
        {
            List<Collaborator> collaborators = await _projectRepository.RetrieveCollaboratorsAsync(projectId, user.Id, take, lastRetrievedUserId);
            List<CollaboratorDto> collaboratorsDtos = [];
            foreach (Collaborator collaborator in collaborators)
            {
                CollaboratorDto collaboratorDto = new()
                {
                    Id = collaborator.UserId,
                    DisplayName = collaborator.DisplayName,
                    IsOwner = await _projectRolesRepository.IsOwnerAsync(projectId, collaborator.UserId),
                    Joined = collaborator.Joined,
                };
                collaboratorsDtos.Add(collaboratorDto);
            }

            return Ok(new
            {
                Data = collaboratorsDtos
            });
        }
        catch (ProjectNotFoundException)
        {
            return Problem(
                title: "Project Not Found",
                detail: ApiErrorMessages.ProjectNotFound,
                statusCode: StatusCodes.Status404NotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return Problem(
                title: "User Not Project Collaborator",
                detail: ApiErrorMessages.UserNotProjectCollaborator,
                statusCode: StatusCodes.Status403Forbidden);
        }
    }

    [HttpPost("{projectId}/manage/collaborators/add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult> AddCollaboratorsAsync(int projectId, [FromBody] AddCollaboratorsDto addCollaboratorsDto)
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
        catch (AuthUserIdNotFoundException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }
        
        bool isSuccessfulAddingCollaborators = await _projectRepository.AddCollaboratorsAsync(projectId, user.Id, addCollaboratorsDto.UserIds);
        if (!isSuccessfulAddingCollaborators)
        {
            return Ok(new
            {
                error = new {
                    message = "Failed to add collaborators."
                }
            });
        }
       
        return CreatedAtAction(nameof(GetAllCollaboratorsAsync), new { projectId }, null);
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

    public static Models.User ConvertToModel(User user)
    {
        return new()
        {
            Id = user.Id,
            DisplayName = user.DisplayName
        };
    }
}
