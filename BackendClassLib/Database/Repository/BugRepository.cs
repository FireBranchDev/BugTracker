using BackendClassLib.Database.Models;
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

    public async Task<List<Bug>> GetBugsAsync(int projectId, int userId)
    {
        Project foundProject = await Context.Projects.Include(c => c.Users).Include(c => c.Bugs).FirstOrDefaultAsync(x => x.Id == projectId) ?? throw new ProjectNotFoundException();
        if (!foundProject.Users.Any(c => c.Id == userId)) throw new UserNotProjectCollaboratorException();
        return foundProject.Bugs;
    }
}
