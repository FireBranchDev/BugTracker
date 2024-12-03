namespace BackendApiTests;

public class ValidationError
{
    public string Field { get; set; } = null!;
    public string[] Messages { get; set; } = [];
}
