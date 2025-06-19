namespace BackendClassLib.Database.Models;

public class BugPermissionUser
{
    public int BugId { get; set; }
    public int BugPermissionId { get; set; }
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
