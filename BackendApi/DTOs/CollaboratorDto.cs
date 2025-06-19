namespace BackendApi.DTOs;

public class CollaboratorDto
{
    public int Id { get; set; }
    public required string DisplayName { get; set; }
    public bool IsOwner { get; set; }
    public DateTime Joined { get; set; }
}
