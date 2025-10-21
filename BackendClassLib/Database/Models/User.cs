using System.Reflection.Metadata;

namespace BackendClassLib.Database.Models;

public class User : Base
{
    public string DisplayName { get; set; } = null!;

    public int? AuthId { get; set; }
    public Auth? Auth { get; set; }

    public List<Project> Projects { get; } = [];

    public List<ProjectUser> ProjectUsers { get; } = [];

    public List<UserProjectPermission> UserProjectPermissions { get; } = [];

    public List<Bug> Bugs { get; } = [];
    public List<BugUser> BugUsers { get; } = [];

    public List<DefaultProjectRoleProjectUser> DefaultProjectRoleProjectUsers { get; } = [];
}
