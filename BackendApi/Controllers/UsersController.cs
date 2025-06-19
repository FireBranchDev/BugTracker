using BackendApi.DTOs;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using ClassLib;
using ClassLib.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Services;
using System.Security.Claims;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController(AuthRepository authRepo, UserRepository userRepo) : ControllerBase
{
    readonly AuthRepository _authRepo = authRepo;
    readonly UserRepository _userRepo = userRepo;

    [HttpPost]
    public async Task<IActionResult> Post(Models.User user)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        Auth auth;
        try
        {
            auth = await _authRepo.FindAsync(subClaim.Value);
        }
        catch (AuthUserIdNotFoundException)
        {
            auth = await _authRepo.InsertAsync(subClaim.Value);
        }

        try
        {
            await _userRepo.FindAsync(auth.Id);
            return Conflict(ApiErrorMessages.UserAccountAlreadyCreated);
        }
        catch
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem();
            }
            await _userRepo.AddAsync(user.DisplayName, auth.Id);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type is ClaimTypes.NameIdentifier);
        if (subClaim is null) return Unauthorized(ApiErrorMessages.MissingSubClaim);

        try
        {
            Auth auth = await _authRepo.FindAsync(subClaim.Value);
            User user = await _userRepo.FindAsync(auth.Id);
            return Ok(new Models.User() { DisplayName = user.DisplayName });
        }
        catch (AuthUserIdNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfAuth0UserId);
        }
        catch (UserNotFoundException)
        {
            return NotFound(ApiErrorMessages.NoRecordOfUserAccount);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult> SearchByDisplayName(string displayName, byte limit = 10, uint? lastSeenId = null, int? excludeFromProjectId = null)
    {
        if (excludeFromProjectId != null)
        {
            return Ok(await _userRepo.SearchByDisplayNameAsync(displayName, limit, lastSeenId, (int)excludeFromProjectId));
        }

        return Ok(await _userRepo.SearchByDisplayNameAsync(displayName, limit, lastSeenId));
    }

    [HttpGet("v2/search")]
    public ActionResult<IAsyncEnumerable<UserDto>> Search([FromQuery] string? displayName, [FromQuery] int? excludeFromProjectId, int limit = 10, int lastRetrievedId = 0)
    {
        var searchResult = _userRepo.Search(displayName, excludeFromProjectId, limit, lastRetrievedId);
        return new ActionResult<IAsyncEnumerable<UserDto>>(ConvertToDto(searchResult));
    }

    static async IAsyncEnumerable<UserDto> ConvertToDto(IAsyncEnumerable<User> users)
    {
        await foreach (var user in users)
        {
            yield return new UserDto { Id = user.Id, DisplayName = user.DisplayName };
        }
    }
}
