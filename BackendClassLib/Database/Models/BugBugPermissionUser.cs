namespace BackendClassLib.Database.Models;

public class BugBugPermissionUser : Base
{
    public int BugId { get; set; }
    public Bug Bug { get; set; } = null!;

    public int BugPermissionId { get; set; }
    public BugPermission BugPermission { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
