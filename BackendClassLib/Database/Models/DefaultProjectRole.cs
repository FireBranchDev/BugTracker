using System.ComponentModel.DataAnnotations.Schema;

namespace BackendClassLib.Database.Models;

[Table("DefaultProjectRoles")]
public class DefaultProjectRole : RoleBase
{
    public List<ProjectPermission> ProjectPermissions { get; } = [];
    public List<Project> Projects { get; } = [];
    public List<DefaultProjectRoleProjectUser> DefaultProjectRoleProjectUsers { get; } = [];
}
