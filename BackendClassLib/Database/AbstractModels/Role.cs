using System.ComponentModel.DataAnnotations;

namespace BackendClassLib.Database.AbstractModels;

public abstract class Role
{
    public int Id { get; set; }

    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
