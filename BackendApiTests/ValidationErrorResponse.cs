namespace BackendApiTests;

public class ValidationErrorResponse
{
    public string Type { get; set; } = null!;
    public string Title { get; set; } = null!;
    public int Status { get; set; }
    public Dictionary<string, string[]> Errors { get; set; } = null!;
}
