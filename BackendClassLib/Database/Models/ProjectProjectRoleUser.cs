using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendClassLib.Database.Models;

[Table("ProjectProjectRoleUsers")]
[PrimaryKey(nameof(ProjectsId), nameof(ProjectRolesId), nameof(UsersId))]
public class ProjectProjectRoleUser
{
    public int ProjectsId { get; set; }
    public Project Project { get; set; } = null!;

    public int ProjectRolesId { get; set; }
    public ProjectRole ProjectRole { get; set; } = null!;

    public int UsersId { get; set; }
    public User User { get; set; } = null!;
}
