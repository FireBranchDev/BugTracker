namespace BackendApi.DTOs;

public class DefaultProjectRoleDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
