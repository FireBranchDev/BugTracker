using BackendClassLib.Database.AbstractModels;
using BackendClassLib.Database.Models.Types;

namespace BackendClassLib.Database.Models;

public class DefaultProjectRole : Role
{
    public DefaultProjectRoleType Type { get; set; }

    public List<ProjectPermission> ProjectPermissions { get; } = [];
}
