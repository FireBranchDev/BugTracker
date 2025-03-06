namespace BackendClassLib.Database.Models;

public abstract class BaseV2
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
