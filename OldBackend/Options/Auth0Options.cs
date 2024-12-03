namespace BugTrackerBackend.Options;

public class Auth0Options
{
    public string Domain { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string ClientSecret { get; set; } = null!;
}
