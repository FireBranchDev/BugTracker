namespace BackendClassLib.Database.Models;

public class BugPermission : Base
{
    public BugPermissionType Type { get; set; }

    public List<Bug> Bugs { get; } = [];
    public List<User> Users { get; } = [];

    public List<BugPermissionUser> BugPermissionUsers { get; } = [];
}
