using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.Models;

public class Project : Base
{
    [MinLength(1), MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(512)]
    public string? Description { get; set; }

    public List<User> Users { get; } = [];

    public List<Bug> Bugs { get; } = [];

    public List<UserProjectPermission> UserProjectPermissions { get; } = [];

    public List<ProjectRole> ProjectRoles { get; } = [];

    public List<ProjectProjectRoleUser> ProjectProjectRoleUsers { get; } = [];
}
