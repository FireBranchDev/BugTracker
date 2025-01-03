﻿using BackendClassLib.Database.Models;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class BugRepository(ApplicationDbContext context) : Repository(context), IBugRepository
{
    public async Task CreateBugAsync(int projectId, int userId, string title, string? description)
    {
        Project foundProject = await Context.Projects.Include(c => c.Users).FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new ProjectNotFoundException();
        if (!foundProject.Users.Any(c => c.Id == userId)) throw new UserNotProjectCollaboratorException();
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
        if (!await Context.UserProjectPermissions.AnyAsync(x => x.ProjectId == projectId && x.UserId == userId && x.ProjectPermission.Type == ProjectPermissionType.DeleteBug))
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
}
