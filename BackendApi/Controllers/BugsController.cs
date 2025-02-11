using BackendApi.DTOs;
using BackendApi.Models;
using BackendClassLib;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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

        if (!ModelState.IsValid) return ValidationProblem();
        
        try
        {
            await _bugRepository.CreateBugAsync(foundProject.Id, user.Id, bug.Title, bug.Description);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, ApiErrorMessages.UserNotProjectCollaborator);
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
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
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

    [Route("/api/projects/{projectId}/bugs/{bugId}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteBug(int projectId, int bugId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        BackendClassLib.Database.Models.Auth auth;
        try
        {
            auth = await _authRepository.FindAsync(subClaim.Value);
        }
        catch(AuthUserIdNotFoundException)
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

        BackendClassLib.Database.Models.Project? project = await _projectRepository.FindAsync(projectId);
        if (project is null) return NotFound(ApiErrorMessages.ProjectNotFound);

        try
        {
            await _bugRepository.DeleteBugAsync(project.Id, user.Id, bugId);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return BadRequest(ApiErrorMessages.UserNotProjectCollaborator);
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

    [Route("/api/projects/{projectId}/bugs/{bugId}/status/assigned")]
    [HttpPost]
    public async Task<IActionResult> MarkBugStatusAsAssigned(int projectId, int bugId)
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
            await _bugRepository.MarkBugAsAssigned(bugId, projectId, user.Id);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (ProjectNotFoundException)
        {
            return NotFound(ApiErrorMessages.ProjectNotFound);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return BadRequest(ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (NotProjectBugException)
        {
            return BadRequest(ApiErrorMessages.BugNotAssociatedWithProject);
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
            return StatusCode(403, ApiErrorMessages.NoRecordOfUserAccount);
        }

        try
        {
            await _bugRepository.AssignCollaboratorToBugAsync(bugId, user.Id, assigneeUserId);
        }
        catch (BugNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfBug);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return StatusCode(403, ApiErrorMessages.UserNotProjectCollaborator);
        }
        catch (InsufficientPermissionToAssignCollaboratorToBug)
        {
            return StatusCode(403, ApiErrorMessages.InsufficientPermissionAssignCollaboratorToBug);
        }
        catch (UserNotFoundException)
        {
            return NotFound(ApiErrorMessages.AssignCollaboratorToBugAssigneeNotFound);
        }

        return NoContent();
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

    static BugDto Convert(BackendClassLib.Database.Models.Bug bug)
    {
        return new()
        {
            Id = bug.Id,
            Title = bug.Title,
            Description = bug.Description,
        };
    }
}
