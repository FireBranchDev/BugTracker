using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class ProjectRepository(ApplicationDbContext context) : Repository(context), IProjectRepository
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

        project.Users.Add(user);
        await Context.Projects.AddAsync(project);
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

    public async Task<Project?> FindAsync(int projectId, int userId)
    {
        Project? project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User? user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Projects.Where(c => c.Users.Contains(user)).AnyAsync()) throw new UserNotProjectCollaboratorException();
        return project;
    }
}
