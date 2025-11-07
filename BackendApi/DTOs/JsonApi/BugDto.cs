namespace BackendApi.DTOs.JsonApi;

public class BugDto
{
    public string Type { get; } = "bugs";

    public string? Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
