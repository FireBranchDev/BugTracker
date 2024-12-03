namespace BackendClassLib.Database.Models;

public abstract class Base
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime UpdatedOn { get; set; }
}
