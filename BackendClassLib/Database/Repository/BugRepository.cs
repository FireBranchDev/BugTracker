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
        assignee.AssignedBugs.Add(bug);
        await Context.SaveChangesAsync();
    }

    public async Task CreateBugAsync(int projectId, int userId, string title, string? description)
    {
        Project foundProject = await Context.Projects.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new ProjectNotFoundException();
        if (!foundProject.Users.Any(c => c.Id == userId)) throw new UserNotProjectCollaboratorException();
        if (!await ProjectRepository.HasPermissionToPerformActionAsync(Context, projectId, userId, ProjectPermissionType.CreateBug)) throw new InsufficientPermissionToCreateBugException();
        Bug bug = new()
        {
            Title = title
        };
        foundProject.Bugs.Add(new Bug
        {
            Title = title,
            Description = description
        });
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
        return foundProject.Bugs.ToList();
    }

    public async Task MarkBugAsAssigned(int bugId, int projectId, int userId)
    {
        Bug? bug = await Context.Bugs.FindAsync(bugId) ?? throw new BugNotFoundException();
        Project? project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User? user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Users.Where(x => x.Id == user.Id && x.Projects.Contains(project)).AnyAsync()) throw new UserNotProjectCollaboratorException();
        if (!await Context.Bugs.Where(x => x.Id == bugId && x.ProjectId == projectId).AnyAsync()) throw new NotProjectBugException();
        
        bug.Status = BugStatusType.Assigned;
        await Context.SaveChangesAsync();
    }

    public async Task<List<User>> GetAssignedCollaborators(int bugId, int userId)
    {
        Bug bug = await Context.Bugs.FindAsync(bugId) ?? throw new BugNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        if (!await Context.Projects.AnyAsync(c => c.Bugs.Contains(bug) && c.Users.Contains(user))) throw new UserNotProjectCollaboratorException();

        return await Context.Entry(bug)
            .Collection(c => c.AssignedUsers)
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

        if (!await Context.Entry(bug).Collection(c => c.BugAssignees).Query().Where(c => c.BugId == bugId && c.UserId == assignedCollaboratorUserId).AnyAsync())
            throw new CollaboratorNotAssignedToBugException();

        await Context.Entry(bug).Collection(c => c.BugAssignees).Query()
            .Where(c => c.BugId == bugId && c.UserId == assignedCollaboratorUserId).ExecuteDeleteAsync();
    }

    public async Task UpdateStatusAsync(int bugId, int userId, BugStatusType bugStatus)
    {
        if (!await Context.Bugs.AnyAsync(x => x.Id == bugId)) throw new BugNotFoundException();
        if (!await Context.Users.AnyAsync(x => x.Id == userId)) throw new UserNotFoundException();
        if (!await Context.Projects.AnyAsync(x => x.Users.Any(c => c.Id == userId))) throw new UserNotProjectCollaboratorException();
        if (!await Context.Bugs.AnyAsync(x => x.Id == bugId
            && x.BugAssignees.Any(x => x.UserId == userId))) throw new UserNotAssignedToBugException();

        // Check if user has permissions to update the status
        int? updateStatusBugPermissionId = await Context.BugPermissions.Where(x => x.Type == BugPermissionType.UpdateStatus).Select(x => x.Id).SingleOrDefaultAsync();
        if (updateStatusBugPermissionId is null || updateStatusBugPermissionId == 0) throw new BugPermissionNotFoundException();

        bool hasPermission = await Context.BugPermissionUsers.Where(x => x.BugId == bugId)
            .Where(x => x.UserId == userId)
            .Where(x => x.BugPermissionId == updateStatusBugPermissionId)
            .AnyAsync();

        if (!hasPermission) throw new InsufficientPermissionToUpdateBugStatusException();

        await Context.Bugs.Where(x => x.Id == bugId).ExecuteUpdateAsync(setters => setters.SetProperty(b => b.Status, bugStatus));
    }
}
