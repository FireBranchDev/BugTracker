using BackendApi.DTOs;
using BackendApi.Models;
using BackendClassLib;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BugsController(IAuthRepository authRepository, IUserRepository userRepository, IProjectRepository projectRepository, IBugRepository bugRepository) : ControllerBase
{
    readonly IAuthRepository _authRepository = authRepository;
    readonly IUserRepository _userRepository = userRepository;
    readonly IProjectRepository _projectRepository = projectRepository;
    readonly IBugRepository _bugRepository = bugRepository;

    [HttpPost("{projectId}")]
    public async Task<IActionResult> CreateBug(int projectId, Bug bug)
    {
        if (!ModelState.IsValid) return ValidationProblem();

        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch (AuthNotFoundException)
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

        BackendClassLib.Database.Models.Project foundProject;
        try
        {
            foundProject = await _projectRepository.FindAsync(projectId, user.Id);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound(ApiErrorMessages.ProjectNotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }

        try
        {
            await _bugRepository.CreateBugAsync(foundProject.Id, user.Id, bug.Title, bug.Description);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (InsufficientPermissionToCreateBugException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.InsufficientPermissionToCreateBug);
        }

        return NoContent();
    }

    [HttpGet("{projectId}")]
    public async Task<IActionResult> GetBugs(int projectId)
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
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
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

        try
        {
            List<BackendClassLib.Database.Models.Bug> bugs = await _bugRepository.GetBugsAsync(projectId, user.Id);
            return Ok(bugs.Select(Convert).ToList());
        }
        catch (UserNotProjectCollaboratorException)
        {
            return BadRequest(ApiErrorMessages.UserNotProjectCollaborator);
        }
    }

    [Route("/api/bugs/{bugId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteBug(int bugId)
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
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }

        int projectId;
        try
        {
            projectId = await _bugRepository.GetBugProjectIdAsync(bugId);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }

        try
        {
            await _bugRepository.DeleteBugAsync(projectId, user.Id, bugId);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (InsufficientPermissionToDeleteBugException)
        {
            return BadRequest(ApiErrorMessages.InsufficientPermissionToDeleteBug);
        }

        return NoContent();
    }

    [Route("/api/bugs/{bugId}/assign/{assigneeUserId}")]
    [HttpPost]
    public async Task<IActionResult> AssignCollaboratorToBugAsync(int bugId, int assigneeUserId)
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
            return Problem(
               type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
               title: "No Record of User Account",
               detail: ApiErrorMessages.NoRecordOfUserAccount,
               statusCode: StatusCodes.Status404NotFound);
        }

        try
        {
            await _bugRepository.AssignCollaboratorToBugAsync(bugId, user.Id, assigneeUserId);
        }
        catch (BugNotFoundException)
        {
            return Problem(
               type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
               title: "No Record of Bug",
               detail: ApiErrorMessages.NoRecordOfBug,
               statusCode: StatusCodes.Status404NotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return Problem(
               type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
               title: "User Not Project Collaborator",
               detail: ApiErrorMessages.UserNotProjectCollaborator,
               statusCode: StatusCodes.Status403Forbidden);
        }
        catch (InsufficientPermissionToAssignCollaboratorToBug)
        {
            return Problem(
              type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.4",
              title: "Insufficient Permission to Assign Collaborator to Bug",
              detail: ApiErrorMessages.InsufficientPermissionAssignCollaboratorToBug,
              statusCode: StatusCodes.Status403Forbidden);
        }
        catch (UserNotFoundException)
        {
            return Problem(
             type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5",
             title: "Assign Collaborator to Bug Assignee Not Found",
             detail: ApiErrorMessages.AssignCollaboratorToBugAssigneeNotFound,
             statusCode: StatusCodes.Status404NotFound);
        }
        catch (CollaboratorAlreadyAssignedBug)
        {
            return Problem(
                type: "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10",
                title: "Already Assigned Bug Collaborator",
                detail: "The collaborator is already assigned to the bug.",
                statusCode: StatusCodes.Status409Conflict);
        }

        return Ok(new {
          title = "Successfully assigned collaborator"
        });
    }

    [Route("/api/bugs/{bugId}/unassign/{collaboratorId}")]
    [HttpPost]
    public async Task<IActionResult> UnassignCollaboratorFromBugAsync(int bugId, int collaboratorId)
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

        try
        {
            await _bugRepository.UnassignCollaboratorAsync(bugId, user.Id, collaboratorId);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(403, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (AssignedCollaboratorUserIdNotFoundException)
        {
            return NotFound(ApiErrorMessages.AssignedCollaboratorUserIdNotFound);
        }
        catch (InsufficientPermissionToUnassignCollaboratorFromBugException)
        {
            return StatusCode(403, ApiErrorMessages.InsufficientPermissionUnassignCollaboratorFromBug);
        }
        catch (CollaboratorNotAssignedToBugException)
        {
            return Ok(ApiErrorMessages.CollaboratorNotAssignedToBug);
        }

        return Ok(ApiSuccessMessages.CollaboratorUnassignedFromBug);
    }

    [Route("/api/bugs/{bugId:int}/status/{bugStatus:int}")]
    [HttpPost]
    public async Task<IActionResult> UpdateBugStatus(int bugId, int bugStatus)
    {
        Claim? subClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
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
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }

        if (!Enum.IsDefined(typeof(BugStatusType), bugStatus))
        {
            return BadRequest(ApiErrorMessages.NotDefinedInBugStatusType);
        }

        try
        {
            await _bugRepository.UpdateStatusAsync(bugId, user.Id, (BugStatusType)bugStatus);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (UserNotAssignedToBugException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.UserNotAssignedToBug);
        }
        catch (BugPermissionNotFoundException)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ApiErrorMessages.BugPermissionNotFound);
        }
        catch (InsufficientPermissionToUpdateBugStatusException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.InsufficientPermissionToUpdateBugStatus);
        }

        return NoContent();
    }

    [Route("{bugId}/find")]
    [HttpGet]
    public async Task<IActionResult> FindAsync(int bugId)
    {
        Claim? subClaim = HttpContext.User.Claims.SingleOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
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
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.NoRecordOfUserAccount);
        }

        try
        {
            BackendClassLib.Database.Models.Bug bug = await _bugRepository.FindBugAsync(bugId, user.Id);
            return Ok(Convert(bug));
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(StatusCodes.Status403Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
        }
    }

    static BugDto Convert(BackendClassLib.Database.Models.Bug bug)
    {
        return new()
        {
            Id = bug.Id,
            Title = bug.Title,
            Description = bug.Description,
            Status = bug.Status,
            CreatedAt = bug.Created,
            UpdatedAt = bug.Updated,
        };
    }
}
