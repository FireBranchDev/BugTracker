using BackendApi.DTOs;
using BackendClassLib.Database.Models;
using BackendClassLib.Database.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProjectRolesController(IProjectDefaultRolesRepository projectDefaultRolesRepository) : ControllerBase
{
    readonly IProjectDefaultRolesRepository _projectDefaultRolesRepository = projectDefaultRolesRepository;

    [HttpGet("/api/roles/project/default/names")]
    public async Task<IActionResult> GetDefaultRolesNamesAsync()
    {
        return Ok(await _projectDefaultRolesRepository.GetAllRolesNamesAsync());
    }

    [HttpGet("/api/roles/project/default")]
    public async Task<IActionResult> GetDefaultRolesAsync()
    {
        List<DefaultProjectRole> roles = await _projectDefaultRolesRepository.GetAllRolesAsync();
        return Ok(roles.Select(DtoHelper.ConvertToDto).ToList());
    }

    [HttpGet("/api/roles/project/default/{defaultProjectRoleId}/permissions")]
    public async Task<IActionResult> GetDefaultRolePermissionAsync(int defaultProjectRoleId)
    {
        List<ProjectPermission> projectPermissions = await _projectDefaultRolesRepository.GetRolePermissionsAsync(defaultProjectRoleId);
        return Ok(projectPermissions.Select(DtoHelper.ConvertToDto).ToList());
    }
}
