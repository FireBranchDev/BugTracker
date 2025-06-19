namespace BackendClassLib.Database.Models;

public class ProjectUser
{
    public int ProjectId { get; set; }
    public int UserId { get; set; }

    public DateTime Joined { get; set; }
}
