using BackendClassLib.Database.AbstractModels;
using BackendClassLib.Database.Models.Types;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendClassLib.Database.Models;

[Table("ProjectRoles")]
public class ProjectRole : Role
{
    public ProjectRoleType Type { get; set; }

    public List<Project> Projects { get; } = [];
}
