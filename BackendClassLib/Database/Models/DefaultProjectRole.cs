using BackendClassLib.Database.Models.Types;

namespace BackendClassLib.Database.Models;

public class DefaultProjectRole : ProjectRole
{
    public DefaultProjectRoleType Type { get; set; }

    public List<ProjectPermission> ProjectPermissions { get; } = [];
}
