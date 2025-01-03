﻿using BackendApi.Models;
using BackendApi.Services;
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
public class ProjectsController(AuthRepository authRepository, ProjectRepository projectRepository, UserRepository userRepository, IUserService userService) : ControllerBase
{
    readonly AuthRepository _authRepository = authRepository;
    readonly ProjectRepository _projectRepository = projectRepository;
    readonly UserRepository _userRepository = userRepository;
    readonly IUserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Post(Project project)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem();
        }

        if (HttpContext is not null)
            _userService.User = HttpContext.User;

        Claim? subClaim = _userService.GetSubClaim();
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

        try
        {
            await _projectRepository.AddAsync(project.Name, project.Description, auth.Id);
        }
        catch (UserNotFoundException)
        {
            return BadRequest(ApiErrorMessages.NoRecordOfUserAccount);
        }

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProjects()
    {
        if (HttpContext is not null)
            _userService.User = HttpContext.User;

        Claim? subClaim = _userService.GetSubClaim();
        if (subClaim is null) return BadRequest(ApiErrorMessages.MissingSubClaim);

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
}
