using BackendClassLib;

namespace BackendApi.DTOs;

public class ProjectPermissionDto
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public ProjectPermissionType Type { get; set; }
}
