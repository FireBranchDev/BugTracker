namespace BackendClassLib.Database.Models;

public class BugAssignee
{
    public int BugId { get; set; }
    public int UserId { get; set; }

    public Bug Bug { get; set; } = null!;
    public User User { get; set; } = null!;

    public DateTime CreatedOn { get; set; }
}
