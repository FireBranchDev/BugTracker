using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Bug
{
    [Required]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}
