namespace BackendClassLib.Database.Models;

public class DefaultProjectRoleProjectUser
{
    public int Id { get; set; }
    public int DefaultProjectRoleId { get; set; }
    public int ProjectId { get; set; }
    public int UserId { get; set; }

    public DefaultProjectRole DefaultProjectRole { get; set; } = null!;
    public Project Project { get; set; } = null!;
    public User User { get; set; } = null!;
}
