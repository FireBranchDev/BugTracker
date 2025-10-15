using Azure;

namespace BackendClassLib.Database.Models;

public class ProjectUser : Base
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }
    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}
