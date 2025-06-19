namespace BackendClassLib.Database.Models;

public class UserProjectPermission
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public int ProjectPermissionId { get; set; }
    public ProjectPermission ProjectPermission { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
