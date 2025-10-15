namespace BackendClassLib.Database.Models;

public class BugPermissionUser : Base
{
    public int BugId { get; set; }
    public int BugPermissionId { get; set; }
    public int UserId { get; set; }
}
