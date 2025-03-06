using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

#pragma warning disable CS9113 // Parameter is unread.
public class ProjectRepository(ApplicationDbContext context, IProjectPermissionRepository projectPermissionRepository) : Repository(context), IProjectRepository
#pragma warning restore CS9113 // Parameter is unread.
{
    public async Task<int> AddAsync(string name, string? description, int authId)
    {
        if (!await Context.Auths.AnyAsync(x => x.Id == authId)) throw new AuthNotFoundException();
        if (!await Context.Users.AnyAsync(x => x.AuthId == authId)) throw new UserNotFoundException();

        User user = await Context.Users.FirstAsync(x => x.AuthId == authId);

        Project project = new()
        {
            Name = name,
            Description = description,
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        };
        await Context.AddAsync(project);

        project.Users.Add(user);
        
        foreach (DefaultProjectRole defaultProjectRole in await Context.DefaultProjectRoles.ToListAsync())
            project.DefaultProjectRoles.Add(defaultProjectRole);

        DefaultProjectRole? owner = await Context.DefaultProjectRoles.Where(c => c.Name == "Owner").FirstOrDefaultAsync();

        if (owner is not null)
        {
            project.DefaultProjectRoleProjectUsers.Add(new()
            {
                DefaultProjectRole = owner,
                Project = project,
                User = user
            });
        }

        await Context.SaveChangesAsync();

        return project.Id;
    }

    public async Task AddCollaboratorAsync(int userId, int userIdOfCollaboratorToAdd, int projectId)
    {
        if (await Context.Users.FindAsync(userId) is null) throw new UserNotFoundException();
        if (await Context.Users.FindAsync(userIdOfCollaboratorToAdd) is null) throw new UserNotFoundException();
        if (await Context.Projects.FindAsync(projectId) is null) throw new ProjectNotFoundException();

        Project project = await Context.Projects.Include(x => x.Users).FirstAsync(c => c.Id == projectId);
        User userAdding = await Context.Users.FirstAsync(c => c.Id == userId);
        if (!project.Users.Contains(userAdding)) throw new UserNotProjectCollaboratorException();

        bool hasPermission = await Context.UserProjectPermissions.AnyAsync(c => c.User == userAdding && c.Project == project && c.ProjectPermission.Type == ProjectPermissionType.AddCollaborator);
        if (!hasPermission) throw new InsufficientPermissionToAddCollaboratorException();

        User collaboratorToAdd = await Context.Users.FirstAsync(c => c.Id == userIdOfCollaboratorToAdd);
        project.Users.Add(collaboratorToAdd);
        await Context.SaveChangesAsync();
    }

    public async Task RemoveCollaboratorAsync(int userId, int userIdOfCollaboratorToRemove, int projectId)
    {
        if (await Context.Users.FindAsync(userId) is null) throw new UserNotFoundException();
        if (await Context.Users.FindAsync(userIdOfCollaboratorToRemove) is null) throw new UserNotFoundException();
        if (await Context.Projects.FindAsync(projectId) is null) throw new ProjectNotFoundException();

        if (!await Context.Projects.AnyAsync(y => y.Users.Any(x => x.Id == userId)
            && y.Users.Any(x => x.Id == userIdOfCollaboratorToRemove))) throw new UserNotProjectCollaboratorException();

        if (!await Context.UserProjectPermissions.Where(c => c.UserId == userId && c.ProjectId == projectId
            && c.ProjectPermissionId == Context.ProjectPermissions.Where(x => x.Type == ProjectPermissionType.RemoveCollaborator).Select(c => c.Id).First())
            .AnyAsync()) throw new InsufficientPermissionToRemoveCollaboratorException();

        await Context.Projects.Where(c => c.Id == projectId && c.Users.Any(x => x.Id == userIdOfCollaboratorToRemove)).ExecuteDeleteAsync();
        await Context.SaveChangesAsync();
    }

    public async Task<List<Project>> GetAllProjectsAsync(int userId)
    {
        User? user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        return await Context.Projects.Where(c => c.Users.Contains(user)).ToListAsync();
    }

    public async Task<Project> FindAsync(int projectId, int userId)
    {
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Entry(project).Collection(x => x.Users).Query().Where(c => c.Id == userId).AnyAsync()) throw new UserNotProjectCollaboratorException();
        return project;
    }

    public async Task<Project?> FindAsync(int projectId)
    {
        return await Task.FromResult<Project?>(null);
    }

    public async Task DeleteAsync(int projectId, int userId)
    {
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Entry(project).Collection(c => c.Users).Query().AnyAsync(x => x.Id == userId)) throw new UserNotProjectCollaboratorException();
        if (!await HasPermissionToPerformActionAsync(Context, projectId, userId, ProjectPermissionType.DeleteProject))
            throw new InsufficientPermissionToDeleteProjectException();
        await Context.Projects.Where(c => c.Id == projectId).ExecuteDeleteAsync();
    }

    public static async Task<bool> HasPermissionToPerformActionAsync(ApplicationDbContext context, int projectId, int userId, ProjectPermissionType projectPermissionType)
    {
        Project project = await context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await context.Entry(project).Collection(c => c.Users).Query().Where(x => x.Id == userId).AnyAsync()) throw new UserNotProjectCollaboratorException();
        if (!await context.ProjectPermissions.Where(c => c.Type == projectPermissionType).AnyAsync()) throw new ProjectPermissionNotFoundException();
        return await context.Entry(project).Collection(c => c.DefaultProjectRoleProjectUsers).Query()
            .Where(x => x.ProjectId == projectId && x.UserId == userId && x.DefaultProjectRole.ProjectPermissions.Where(c => c.Type == projectPermissionType).Any()).AnyAsync();
    }

    public async Task<bool> HasPermissionToPerformActionAsync(int projectId, int userId, ProjectPermissionType projectPermissionType)
    {
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Entry(project).Collection(c => c.Users).Query().Where(x => x.Id == userId).AnyAsync()) throw new UserNotProjectCollaboratorException();
        if (!await Context.ProjectPermissions.Where(c => c.Type == projectPermissionType).AnyAsync()) throw new ProjectPermissionNotFoundException();
        return await Context.Entry(project).Collection(c => c.DefaultProjectRoleProjectUsers).Query()
            .Where(x => x.ProjectId == projectId && x.UserId == userId && x.DefaultProjectRole.ProjectPermissions.Where(c => c.Type == projectPermissionType).Any()).AnyAsync();
    }
}
