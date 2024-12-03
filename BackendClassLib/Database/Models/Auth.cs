namespace BackendClassLib.Database.Models;

public class Auth : Base
{
    public List<string> UserIds { get; set; } = [];
    public User? User { get; set; }
}
