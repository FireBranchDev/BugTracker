using Microsoft.EntityFrameworkCore;

namespace BackendClassLib.Database.Models;

[PrimaryKey(nameof(BugId), nameof(BugPermissionId), nameof(UserId))]
public class BugPermissionUser
{
    public int BugId { get; set; }
    public int BugPermissionId { get; set; }
    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }
}
