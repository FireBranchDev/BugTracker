using BackendClassLib.Database.Models;

namespace BackendApi.DTOs;

public static class DtoHelper
{
    public static DefaultProjectRoleDto ConvertToDto(DefaultProjectRole x)
    {
        return new DefaultProjectRoleDto()
        {
            Id = x.Id,
            Name = x.Name,
            CreatedAt = x.CreatedAt,
            UpdatedAt = x.UpdatedAt,
        };
    }

    public static ProjectPermissionDto ConvertToDto(ProjectPermission x)
    {
        return new ProjectPermissionDto()
        {
            Id = x.Id,
            CreatedOn = x.CreatedOn,
            UpdatedOn = x.UpdatedOn,
            Name = x.Name,
            Description = x.Description,
            Type = x.Type,
        };
    }
}
