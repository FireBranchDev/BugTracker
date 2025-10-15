using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.Models;

public class Bug : Base
{
    [MinLength(1), MaxLength(90)]
    public string Title { get; set; } = null!;

    [MaxLength(255)]
    public string? Description { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public BugStatusType Status { get; set; }

    public List<User> Users { get; } = [];

    public List<BugUser> BugUsers { get; } = [];

    public List<BugPermission> BugPermissions { get; } = [];

    public List<BugPermissionUser> BugPermissionUsers { get; } = [];
}
