namespace BackendClassLib.Database.Models;

public class User : Base
{
    public string DisplayName { get; set; } = null!;

    public int AuthId { get; set; }
    public Auth Auth { get; set; } = null!;

    public List<Project> Projects { get; } = [];

    public List<UserProjectPermission> UserProjectPermissions { get; } = [];

    public List<Bug> AssignedBugs { get; } = [];
    public List<BugAssignee> BugAssignees { get; } = [];

    public List<BugPermission> BugPermissions { get; } = [];

    public List<BugPermissionUser> BugPermissionUsers { get; } = [];

    public List<ProjectProjectRoleUser> ProjectProjectRoleUsers { get; } = [];
}
