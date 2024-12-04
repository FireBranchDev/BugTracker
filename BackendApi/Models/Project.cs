using System.ComponentModel.DataAnnotations;

namespace BackendApi.Models;

public class Project
{
    [Required]
    [MinLength(1), MaxLength(120)]
    public string Name { get; set; } = null!;

    [MaxLength(512)]
    public string? Description { get; set; }
}
