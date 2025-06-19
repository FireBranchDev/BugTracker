namespace BackendClassLib.Models;

public class Collaborator
{
    public int UserId { get; set; }
    public required string DisplayName { get; set; }
    public bool IsOwner { get; set; }
    public DateTime Joined { get; set; }
}
