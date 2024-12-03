namespace BackendClassLib.Database.Models;

public class User : Base
{
    public string DisplayName { get; set; } = null!;

    public int AuthId { get; set; }
    public Auth Auth { get; set; } = null!;

    public List<Project> Projects { get; } = [];

    public List<UserProjectPermission> UserProjectPermissions { get; } = [];
}
