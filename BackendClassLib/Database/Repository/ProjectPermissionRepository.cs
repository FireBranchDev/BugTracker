using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class ProjectPermissionRepository(ApplicationDbContext context) : Repository(context), IProjectPermissionRepository
{
    public async Task<List<ProjectPermission>> GetAllAsync()
    {
        List<ProjectPermission> projectPermissions = [];
        List<ProjectPermissionType> projectPermissionTypesMissingInDb = [];
        foreach (ProjectPermissionType projectPermissionType in Enum.GetValues<ProjectPermissionType>())
        {
            ProjectPermission? projectPermission = await Context.ProjectPermissions
                .Where(c => c.Type == projectPermissionType).FirstOrDefaultAsync();
            if (projectPermission is null)
            {
                projectPermissionTypesMissingInDb.Add(projectPermissionType);
                continue;
            }

            projectPermissions.Add(projectPermission);
        }
        
        if (projectPermissionTypesMissingInDb.Count != 0)
        {
            ProjectPermissionNotFoundException projectPermissionNotFoundException = new();
            projectPermissionNotFoundException.Data.Add(nameof(ProjectPermissionType) + "s", projectPermissionTypesMissingInDb);
            throw projectPermissionNotFoundException;
        }

        return projectPermissions;
    }
}
