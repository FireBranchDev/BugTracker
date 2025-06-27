using BackendClassLib.Database.Models;
using BackendClassLib.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;
using User = BackendClassLib.Database.Models.User;

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
        if (await Context.Users.FindAsync(userIdOfCollaboratorToAdd) is null) throw new CollaboratorToAddNotFoundException();
        if (await Context.Projects.FindAsync(projectId) is null) throw new ProjectNotFoundException();

        Project project = await Context.Projects.Include(x => x.Users).FirstAsync(c => c.Id == projectId);
        User userAdding = await Context.Users.FirstAsync(c => c.Id == userId);
        if (!project.Users.Contains(userAdding)) throw new UserNotProjectCollaboratorException();

        bool hasPermission = await HasPermissionToPerformActionAsync(projectId, userId, ProjectPermissionType.AddCollaborator);
        if (!hasPermission) throw new InsufficientPermissionToAddCollaboratorException();

        User collaboratorToAdd = await Context.Users.FirstAsync(c => c.Id == userIdOfCollaboratorToAdd);
        project.Users.Add(collaboratorToAdd);
        await Context.SaveChangesAsync();
    }

    public async Task RemoveCollaboratorAsync(int userId, int userIdOfCollaboratorToRemove, int projectId)
    {
        if (await Context.Users.FindAsync(userId) is null) throw new UserNotFoundException();
        if (await Context.Users.FindAsync(userIdOfCollaboratorToRemove) is null) throw new CollaboratorToRemoveNotFoundException();
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();

        if (!await Context.Projects.Where(c => c.Id == projectId && c.Users.Where(x => x.Id == userId).Any()).AnyAsync())
            throw new UserNotProjectCollaboratorException();

        if (!await HasPermissionToPerformActionAsync(projectId, userId, ProjectPermissionType.RemoveCollaborator)) throw new InsufficientPermissionToRemoveCollaboratorException();

        if (!await Context.Projects.Where(c => c.Id == projectId && c.Users.Where(x => x.Id == userIdOfCollaboratorToRemove).Any()).AnyAsync())
            throw new AttemptingToRemoveNonProjectCollaboratorException();

        User userToRemoveFromProject = await Context.Users.FindAsync(userIdOfCollaboratorToRemove) ?? throw new UserNotFoundException();

        await Context.Entry(project).Collection(x => x.ProjectUsers).Query().Where(c => c.ProjectId == projectId && c.UserId == userIdOfCollaboratorToRemove).ExecuteDeleteAsync();
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

    public async Task<List<Collaborator>> RetrieveCollaboratorsAsync(int projectId, int userId, byte take = 10, int? lastRetrievedUserId = null)
    {
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Entry(project).Collection(c => c.Users).Query().Where(x => x.Id == userId).AnyAsync()) throw new UserNotProjectCollaboratorException();

        DefaultProjectRole owner = await Context.DefaultProjectRoles.Where(c => c.Name == "Owner").FirstAsync();

        IQueryable<User> retrieveCollaboratorsQuery = Context.Entry(project).Collection(c => c.Users).Query().Include(c => c.ProjectUsers)
            .Where(x => !x.DefaultProjectRoleProjectUsers.Any(c => c.DefaultProjectRole == owner && c.UserId == x.Id && c.ProjectId == project.Id));

        if (lastRetrievedUserId != null)
        {
            retrieveCollaboratorsQuery = retrieveCollaboratorsQuery.Where(c => c.Id > lastRetrievedUserId);
        }

        return await retrieveCollaboratorsQuery.Select(x => new
        {
            x.Id,
            x.DisplayName,
            Joined = x.ProjectUsers.Select(x => x.Joined).Single()
        }).Select(j => new Collaborator
        {
            UserId = j.Id,
            DisplayName = j.DisplayName,
            Joined = j.Joined
        }).Take(take).OrderBy(c => c.Joined).ToListAsync();
    }

    public async Task<bool> AddCollaboratorsAsync(int projectId, int userId, List<int> userIdsToAdd)
    {
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Entry(project).Collection(c => c.Users).Query().Where(x => x.Id == userId).AnyAsync()) throw new UserNotProjectCollaboratorException();

        bool hasPermission = await projectPermissionRepository.HasPermissionAsync(projectId, userId, ProjectPermissionType.AddCollaborator);
        if (!hasPermission) return false;

        foreach (var i in userIdsToAdd)
        {
            User? userToAdd = await Context.Users.FindAsync(i);
            if (userToAdd == null) return false;
            project.ProjectUsers.Add(new() { ProjectId = projectId, UserId = userToAdd.Id });
            DefaultProjectRole? collaborator = await Context.DefaultProjectRoles.Where(c => c.Name == "Collaborator").Take(1).FirstOrDefaultAsync();
            if (collaborator != null)
            {
                project.DefaultProjectRoleProjectUsers.Add(new() { Project = project, User = userToAdd, DefaultProjectRole = collaborator });
            }
        }

        await Context.SaveChangesAsync();

        return true;
    }
}
