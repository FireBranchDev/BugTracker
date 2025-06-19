using BackendClassLib.Database.Models;
using ClassLib.Enums;
using ClassLib.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Repository;

public class ProjectRolesRepository(ApplicationDbContext context) : Repository(context), IProjectRolesRepository
{
    public async Task<bool> IsOwnerAsync(int projectId, int userId)
    {
        Project project = await Context.Projects.FindAsync(projectId) ?? throw new ProjectNotFoundException();
        User user = await Context.Users.FindAsync(userId) ?? throw new UserNotFoundException();
        DefaultProjectRole owner = await Context.DefaultProjectRoles.Where(c => c.Name == RoleType.Owner.ToString()).FirstAsync() ?? throw new ProjectDefaultRoleNotFoundException();
        return await Context.Entry(project).Collection(p => p.DefaultProjectRoleProjectUsers).Query().Where(d => d.User == user && d.DefaultProjectRole == owner).AnyAsync();
    }
}
