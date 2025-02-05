namespace BackendClassLib.Database.Models;

public class BugPermission
{
    public int Id { get; set; }
    public BugPermissionType Type { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<Bug> Bugs { get; } = [];
    public List<User> Users { get; } = [];

    public List<BugPermissionUser> BugPermissionUsers { get; } = [];
}
