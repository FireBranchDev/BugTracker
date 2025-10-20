using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class BugRepository(ApplicationDbContext context) : Repository(context), IBugRepository
{
    public async Task AssignCollaboratorToBugAsync(int bugId, int userId, int assigneeUserId)
    {
        Bug bug = await Context.Bugs.FindAsync(bugId) ?? throw new BugNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        
        Project? project = await Context.Projects.Where(c => c.Bugs.Contains(bug)).FirstOrDefaultAsync() ?? throw new ProjectNotFoundException();
        if (!await Context.Projects.AnyAsync(c => c.Bugs.Contains(bug) && c.Users.Contains(user))) throw new UserNotProjectCollaboratorException();
        
        if (!await ProjectRepository.HasPermissionToPerformActionAsync(Context, project.Id, userId, ProjectPermissionType.AssignCollaboratorToBug)) throw new InsufficientPermissionToAssignCollaboratorToBug();
        
        User assignee = await Context.Users.FindAsync(assigneeUserId) ?? throw new UserNotFoundException();

        if (await Context.Entry(assignee).Collection(a => a.Bugs).Query().Where(x => x.Id == bug.Id).AnyAsync()) throw new CollaboratorAlreadyAssignedBug();

        assignee.Bugs.Add(bug);
        await Context.SaveChangesAsync();
    }

    public async Task CreateBugAsync(int projectId, int userId, string title, string? description)
    {
        Project foundProject = await Context.Projects.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new ProjectNotFoundException();
        if (!foundProject.Users.Any(c => c.Id == userId)) throw new UserNotProjectCollaboratorException();
        if (!await ProjectRepository.HasPermissionToPerformActionAsync(Context, projectId, userId, ProjectPermissionType.CreateBug)) throw new InsufficientPermissionToCreateBugException();
        Bug bug = new()
        {
            Title = title,
            Description = description
        };


        var user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();

        var bugPermissions = await Context.BugPermissions.ToListAsync();

        foreach (var permission in bugPermissions)
        {
            bug.BugPermissionUsers.Add(new BugPermissionUser()
            {
                Bug = bug,
                BugPermission = permission,
                User = user
            });
        }

        foundProject.Bugs.Add(bug);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteBugAsync(int projectId, int userId, int bugId)
    {
        Project foundProject = await Context.Projects.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new ProjectNotFoundException();
        if (!foundProject.Users.Any(c => c.Id == userId)) throw new UserNotProjectCollaboratorException();
        if (!await Context.Bugs.AnyAsync(x => x.Id == bugId)) throw new BugNotFoundException();
        if (!await Context.Bugs.AnyAsync(c => c.Id == bugId && c.ProjectId == projectId)) throw new NotProjectBugException();
        if (!await ProjectRepository.HasPermissionToPerformActionAsync(Context, projectId, userId, ProjectPermissionType.DeleteBug))
            throw new InsufficientPermissionToDeleteBugException();
        Bug bug = await Context.Bugs.Where(c => c.Id == bugId).FirstAsync();
        Context.Bugs.Remove(bug);
        await Context.SaveChangesAsync();
    }

    public async Task<List<Bug>> GetBugsAsync(int projectId, int userId)
    {
        Project foundProject = await Context.Projects.Include(c => c.Users).Include(c => c.Bugs).FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new ProjectNotFoundException();
        if (!foundProject.Users.Any(c => c.Id == userId)) throw new UserNotProjectCollaboratorException();
        return foundProject.Bugs;
    }

    public async Task<List<User>> GetAssignedCollaborators(int bugId, int userId)
    {
        Bug bug = await Context.Bugs.FindAsync(bugId) ?? throw new BugNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Projects.AnyAsync(c => c.Bugs.Contains(bug) && c.Users.Contains(user))) throw new UserNotProjectCollaboratorException();

        return await Context.Entry(bug)
            .Collection(c => c.Users)
            .Query()
            .ToListAsync();
    }

    public async Task UnassignCollaboratorAsync(int bugId, int userId, int assignedCollaboratorUserId)
    {
        Bug bug = await Context.Bugs.FindAsync(bugId) ?? throw new BugNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Projects.AnyAsync(c => c.Bugs.Contains(bug) && c.Users.Contains(user))) throw new UserNotProjectCollaboratorException();
        if (!await Context.Users.AnyAsync(c => c.Id == assignedCollaboratorUserId)) throw new AssignedCollaboratorUserIdNotFoundException();
        if (!await Context.Bugs.AnyAsync(c => c.Project.Users.Any(x => x.Id == assignedCollaboratorUserId))) throw new UserNotProjectCollaboratorException();
        Project? project = await Context.Projects.Where(c => c.Bugs.Contains(bug)).FirstOrDefaultAsync() ?? throw new ProjectNotFoundException();
        if (!await ProjectRepository.HasPermissionToPerformActionAsync(Context, project.Id, userId, ProjectPermissionType.UnassignCollaboratorFromBug))
            throw new InsufficientPermissionToUnassignCollaboratorFromBugException();

        if (!await Context.Entry(bug).Collection(c => c.BugUsers).Query().Where(c => c.BugId == bugId && c.UserId == assignedCollaboratorUserId).AnyAsync())
            throw new CollaboratorNotAssignedToBugException();

        await Context.Entry(bug).Collection(c => c.BugUsers).Query()
            .Where(c => c.BugId == bugId && c.UserId == assignedCollaboratorUserId).ExecuteDeleteAsync();
    }

    public async Task UpdateStatusAsync(int bugId, int userId, BugStatusType bugStatus)
    {
        Bug bug = await Context.Bugs.Include(b => b.Project).Where(b => b.Id == bugId).SingleOrDefaultAsync() ?? throw new BugNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Bugs.AnyAsync(x => x.Id == bugId)) throw new BugNotFoundException();
        if (!await Context.Users.AnyAsync(x => x.Id == userId)) throw new UserNotFoundException();
        if (!await Context.Projects.AnyAsync(c => c.Bugs.Contains(bug) && c.Users.Contains(user))) throw new UserNotProjectCollaboratorException();

        bool isProjectOwner = await Context.DefaultProjectRoles.Where(d => d.DefaultProjectRoleProjectUsers.Where(x => x.Project == bug.Project
            && x.User == user
            && x.DefaultProjectRole.Name == "Owner").Any()).AnyAsync();

        // Checking that the user updating the bug's status is also the project owner
        if (!isProjectOwner)
        {
            if (!await Context.Bugs.AnyAsync(x => x.Id == bugId
            && x.BugUsers.Any(x => x.UserId == userId))) throw new UserNotAssignedToBugException();
        }

        bool hasPermission = isProjectOwner;
        
        if (!hasPermission)
        {
            // Checking if the user has the update status bug permission
            int? updateStatusBugPermissionId = await Context.BugPermissions.Where(x => x.Type == BugPermissionType.UpdateStatus).Select(x => x.Id).SingleOrDefaultAsync();
            if (updateStatusBugPermissionId is null || updateStatusBugPermissionId == 0) throw new BugPermissionNotFoundException();

            hasPermission = await Context.BugPermissionUsers.Where(x => x.BugId == bugId)
                .Where(x => x.UserId == userId)
                .Where(x => x.BugPermissionId == updateStatusBugPermissionId)
                .AnyAsync();
        }

        if (!hasPermission) throw new InsufficientPermissionToUpdateBugStatusException();

        await Context.Bugs.Where(x => x.Id == bugId).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Status, bugStatus));
    }

    public async Task<int> GetBugProjectIdAsync(int bugId)
    {
        Bug? bug = await Context.Bugs.Include(c => c.Project).FirstOrDefaultAsync(x => x.Id == bugId) ?? throw new BugNotFoundException();
        return bug.ProjectId;
    }

    public async Task<Bug> FindBugAsync(int bugId, int userId)
    {
        Bug bug = await Context.Bugs.FindAsync(bugId) ?? throw new BugNotFoundException();
        if (!await ProjectRepository.IsCollaboratorAsync(Context, bug.ProjectId, userId)) throw new UserNotProjectCollaboratorException();
        return bug;
    }
}
