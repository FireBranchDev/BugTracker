namespace BackendClassLib.Database.Models;

public class BugUser : Base
{
    public int BugId { get; set; }
    public int UserId { get; set; }

    public Bug Bug { get; set; } = null!;
    public User User { get; set; } = null!;
}
