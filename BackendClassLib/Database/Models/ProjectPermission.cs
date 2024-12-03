using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.Models;

public class ProjectPermission : Base
{
    [MaxLength(60)]
    public string Name { get; set; } = null!;
    [MaxLength(128)]
    public string Description { get; set; } = null!;
    public ProjectPermissionType Type { get; set; }

    public List<UserProjectPermission> UserProjectPermissions { get; } = [];
}
