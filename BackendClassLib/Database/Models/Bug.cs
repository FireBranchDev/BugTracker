using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.Models;

public class Bug : Base
{
    [MinLength(1), MaxLength(90)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public BugStatusType Status { get; set; }
}
