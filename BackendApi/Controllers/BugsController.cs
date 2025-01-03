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
}
