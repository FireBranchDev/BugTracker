using BackendClassLib.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BackendClassLib.Database.DataSeeding;

public class DatabaseSeeding(ILogger<DatabaseSeeding> logger, ApplicationDbContext context)
{
    readonly ILogger<DatabaseSeeding> _logger = logger;
    readonly ApplicationDbContext _context = context;

    public async Task InitialiseOwnerDefaultProjectRoleProjectPermissions()
    {
        DefaultProjectRole? ownerRole = await _context.DefaultProjectRoles.Where(c => c.Type == Models.Types.DefaultProjectRoleType.Owner).FirstOrDefaultAsync();
        if (ownerRole is null)
        {
            // Do some logging
            _logger.LogCritical("The owner default project role is missing from the database.");
            throw new Exception();
        }

        ProjectPermission? addCollaboratorPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.AddCollaborator);
        if (addCollaboratorPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The add collaborator permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, addCollaboratorPermission))
            {
                ownerRole.ProjectPermissions.Add(addCollaboratorPermission);
            }
        }

        ProjectPermission? removeCollaboratorPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.RemoveCollaborator);
        if (removeCollaboratorPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The remove collaborator permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, removeCollaboratorPermission))
            {
                ownerRole.ProjectPermissions.Add(removeCollaboratorPermission);
            }
        }

        ProjectPermission? deleteBugPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.DeleteBug);
        if (deleteBugPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The delete bug permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, deleteBugPermission))
            {
                ownerRole.ProjectPermissions.Add(deleteBugPermission);
            }
        }

        ProjectPermission? assignCollaboratorToBugPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.AssignCollaboratorToBug);
        if (assignCollaboratorToBugPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The assign collaborator to bug permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, assignCollaboratorToBugPermission))
            {
                ownerRole.ProjectPermissions.Add(assignCollaboratorToBugPermission);
            }
        }

        ProjectPermission? unassignCollaboratorFromBugPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.UnassignCollaboratorFromBug);
        if (unassignCollaboratorFromBugPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The unassign collaborator from bug permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, unassignCollaboratorFromBugPermission))
            {
                ownerRole.ProjectPermissions.Add(unassignCollaboratorFromBugPermission);
            }
        }

        ProjectPermission? deleteProjectPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.DeleteProject);
        if (deleteProjectPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The delete project permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, deleteProjectPermission))
            {
                ownerRole.ProjectPermissions.Add(deleteProjectPermission);
            }
        }

        ProjectPermission? createBugPermission = await FindProjectPermissionByPermissionType(_context, ProjectPermissionType.CreateBug);
        if (createBugPermission is null)
        {
            // Do some logging
            _logger.LogCritical("The create bug permission is missing from the database.");
            throw new Exception();
        }
        else
        {
            if (!await IsProjectPermissionAssignedToDefaultProjectRoleAsync(_context, ownerRole, createBugPermission))
            {
                ownerRole.ProjectPermissions.Add(createBugPermission);
            }
        }

        await _context.SaveChangesAsync();
    }

    static async Task<ProjectPermission?> FindProjectPermissionByPermissionType(ApplicationDbContext context, ProjectPermissionType projectPermissionType)
    {
        return await context.ProjectPermissions.Where(c => c.Type == projectPermissionType).FirstOrDefaultAsync();
    }

    static async Task<bool> IsProjectPermissionAssignedToDefaultProjectRoleAsync(ApplicationDbContext context, DefaultProjectRole defaultProjectRole, ProjectPermission projectPermission)
    {
        return await context.Entry(defaultProjectRole).Collection(x => x.ProjectPermissions).Query().Where(c => c.Id == projectPermission.Id).AnyAsync();
    }
}
