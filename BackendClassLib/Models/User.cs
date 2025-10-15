namespace BackendClassLib.Models;

public class User
{
    public int Id { get; set; }
    public required string DisplayName { get; set; }
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
