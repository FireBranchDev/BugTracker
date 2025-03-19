
using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class ProjectDefaultRolesRepository(ApplicationDbContext context) : Repository(context), IProjectDefaultRolesRepository
{
    public async Task<List<DefaultProjectRole>> GetAllRolesAsync()
    {
        return await Context.DefaultProjectRoles.ToListAsync();
    }

    public async Task<List<string>> GetAllRolesNamesAsync()
    {
        return await Context.DefaultProjectRoles.Select(x => x.Name).ToListAsync();
    }

    public async Task<List<ProjectPermission>> GetRolePermissionsAsync(int defaultRoleId)
    {
        return await Context.ProjectPermissions.Where(c => c.DefaultProjectRoles.Where(x => x.Id == defaultRoleId).Any()).ToListAsync();
    }
}
