using BackendClassLib.Database.Models;
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
public class UsersController(AuthRepository authRepo, UserRepository userRepo) : ControllerBase
{
    readonly AuthRepository _authRepo = authRepo;
    readonly UserRepository _userRepo = userRepo;

    [HttpPost]
    public async Task<IActionResult> Post(Models.User user)
    {
        Claim? subClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

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
            return BadRequest(ApiErrorMessages.UserAccountAlreadyCreated);
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
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

        try
        {
            Auth auth = await _authRepo.FindAsync(subClaim.Value);
            User user = await _userRepo.FindAsync(auth.Id);
            return Ok(new Models.User() { DisplayName = user.DisplayName });
        }
        catch (AuthUserIdNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfAuth0UserId);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }
    }
}
