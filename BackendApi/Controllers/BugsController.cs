using BackendApi.DTOs;
using BackendApi.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    [HttpPost("id")]
    public async Task<IActionResult> CreateBug(int projectId, Bug bug)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

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

        BackendClassLib.Database.Models.Project? foundProject = await _projectRepository.FindAsync(projectId);
        if (foundProject is null) return NotFound(ApiErrorMessages.ProjectNotFound);

        if (!ModelState.IsValid) return ValidationProblem();

        try
        {
            await _bugRepository.CreateBugAsync(foundProject.Id, user.Id, bug.Title, bug.Description);
        }
        catch (UserNotProjectCollaboratorException)
        {
            return BadRequest(ApiErrorMessages.UserNotProjectCollaborator);
        }

        return NoContent();
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetBugs(int projectId)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

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

        BackendClassLib.Database.Models.Project? project = await _projectRepository.FindAsync(projectId);
        if (project is null) return BadRequest(ApiErrorMessages.ProjectNotFound);

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
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

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
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

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
